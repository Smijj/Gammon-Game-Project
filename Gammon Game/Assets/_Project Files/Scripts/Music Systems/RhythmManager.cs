using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using TMPro;

namespace MusicSystem {
    using GameManagement;
    using InventorySystem;
    using MenuSystem;

    public class RhythmManager : MonoBehaviour
    {
        // a static ref to the midifile that the current song is using
        public static MidiFile midiFile;
        public static AudioClip audioClip;
        public static RhythmManager instance;
        public static bool songIsPlaying = false;
        public static bool isPaused = false;
        
        [Header("Song Mangager Stuff: ")]
        public AudioSource audioSource;     // The audiosource where the audio will be played through in the game
        public Lane[] lanes;                // The lanes that the notes drop from
        public Image songCharacterImageObj;
        public Sprite songCharacterSprite;  // IDEA: Can have the character defined in the Song obj and just find the correct char through that
        public bool debug = false;


        [Header("Song Settings: ")]
        public Song song;
        public float songDelayInSeconds;    // The delay before the song starts
        public GameObject startCountdown;

        [Header("Tap Settings: ")]
        public int inputDelayInMiliseconds; // To account for input delay
        public double perfectMargin;
        public double goodMargin;       
        public double badMargin;

        public float tapIndicatorWidth = 20f;
        public Color perfectTapZone;
        public Transform gameArea;


        [Header("Note Settings: ")]
        [Tooltip("The time it takes for the note to reach the noteTapY position.")]
        public float noteFallTime;
        private float lastNoteTime;
        [Tooltip("The position the notes will spawn in the world.")]
        public float noteSpawnY;
        [Tooltip("The position the player has to tap the notes.")]
        public float noteTapY;
        public float noteDespawnY {
            get {
                return noteTapY - (noteSpawnY - noteTapY);
            }
        }

        public GameObject perfectHitEffect;
        public GameObject badHitEffect;
        public GameObject holdEffect;
        public Color noteBGColourStart;
        public Color noteBGColourTap;
        public Color noteBGColourMiss;
        public Color hitIndicatorStartColour;
        public Color hitIndicatorEndColour;
        public Color holdNoteTailColour;


        private Camera mainCamera;
        private List<GameObject> hitIndicatorObjects = new List<GameObject>();


        #region Unity Functions

        private void OnEnable() {
            instance = this;
            if (!GameManager.isPaused) GameManager.PauseGame();
            mainCamera = Camera.main;
            //mainCamera.enabled = false;
            mainCamera.gameObject.SetActive(false);

            songCharacterImageObj.sprite = songCharacterSprite;

            isPaused = true;
        }

        private void OnDisable() {
            mainCamera.gameObject.SetActive(true);
            //mainCamera.enabled = true;
            songIsPlaying = false;
            isPaused = false;
        }

        private void Start() {
            // Checks if the mififile that is trying to be read is from an online link or on file, then calls the necessary function to handle it
            if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://")) {
                StartCoroutine(ReadMidiFromWebsite(song.midiFileName));
            } else {
                ReadMidiFromFile(song.midiFileName);
            }
            
            StartCoroutine(LoadAudio(song.songFileName));
            
        }

        private void Update() {
            if (lastNoteTime != noteFallTime) {
                DrawTapLineIndicators();
                lastNoteTime = noteFallTime;
            }
            if (audioSource.clip != null && GetAudioSourceTime() > audioSource.clip.length) {
                Debug.Log("Song is finished");
                FinishSong();
            }
        }

        #endregion


        #region Public Functions

        public void InitSong(Song _song) {
            song = null;
            ScoreManager.ResetStatistics();

            song = _song;
        }

        public void PauseSong(bool _pause) {
            if (_pause) {
                audioSource.pitch = 0; 
                isPaused = true;
                if (!GameManager.isPaused) GameManager.PauseGame();
            } else {
                audioSource.pitch = 1; // Can set this to equal some pitch speed var later
                isPaused = false;
                if (!GameManager.isPaused) GameManager.PauseGame();
            } 
        }

        public void FinishSong() {
            songIsPlaying = false;
            PauseSong(true);

            ScoreManager.instance.CalculateAndSaveSongStats();

            PageManager.singleton.TurnPageOn(PageType.SongOver);
        }


        /// <summary>
        /// Gets the current timestamp of the audio that is playing
        /// </summary>
        /// <returns></returns>
        public static double GetAudioSourceTime() {
            if (instance.audioSource.clip != null) {
                return (double)instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
            }
            return 0;
        }

        #endregion


        #region Private Functions

        private void DrawTapLineIndicators() {
            // Cleans up any old hit indicators in the case this function is called more than once.
            if (hitIndicatorObjects.Count > 0) {
                foreach (var _object in hitIndicatorObjects) {
                    Destroy(_object);
                }
                hitIndicatorObjects.Clear();
            }

            // Spawns the tap line indicators.
            hitIndicatorObjects.Add(DrawRhythmUI.instance.DrawBlockAroundTapPos(0.01, perfectTapZone));
            //hitIndicatorObjects.Add(DrawBlockAroundTapPos(perfectMargin, perfectTapZone));
            //hitIndicatorObjects.Add(DrawAroundTapPos(-goodMargin, goodTapZoneLines));   // Draws line before the tapPos
            //hitIndicatorObjects.Add(DrawAroundTapPos(goodMargin, goodTapZoneLines));    // Draws line after the tapPos
        }

        /// <summary>
        /// Loads the Audio file that accompanies the Midi file (the filetype is required to be WAV right now)
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        private IEnumerator LoadAudio(string _name) {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(Application.streamingAssetsPath + "/" + _name, AudioType.WAV)) {
                yield return www.SendWebRequest();  // Waits for websites response

                if (www.result == UnityWebRequest.Result.DataProcessingError) {
                    Debug.Log(www.error);
                }

                audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip.name = _name;
                audioSource.clip = audioClip;
            }
        }

        private IEnumerator ReadMidiFromWebsite(string _name) {
            // Sends a request to the streamingAssetsPath website looking for the midiFileLocation
            using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + _name)) {
                yield return www.SendWebRequest();  // Waits for websites response

                // Checks for errors with the response
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
                    Debug.Log(www.error);
                } else {
                    byte[] results = www.downloadHandler.data;          // Downloads the data from the web request
                    using (var stream = new MemoryStream(results)) {    // Inputs the data into a memory stream
                        midiFile = MidiFile.Read(stream);               // stores that memory stream into the midiFile
                        GetDataFromMidi();
                    }
                }
            }
        }

        private void ReadMidiFromFile(string _name) {
            midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + _name);
            GetDataFromMidi();
        }

        private void GetDataFromMidi() {
            var notes = midiFile.GetNotes();    // gets the notes from the midi file
            var array = new Note[notes.Count];  // creates a new array of type Note
            notes.CopyTo(array, 0);             // stores the notes from the midi file into the new array

            // sets the timestamps for all the notes in each lane.
            foreach (var _lane in lanes) _lane.SetTimeStamps(array);

            StartCoroutine(StartSong(songDelayInSeconds));
        }

        private IEnumerator StartSong(float _startDelay) {
            int i = (int)_startDelay;
            while(i > 0) {
                startCountdown.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString("0");
                yield return new WaitForSecondsRealtime(1);
                i--;
            }
            startCountdown.SetActive(false);
            audioSource.Play();
            songIsPlaying = true;
            isPaused = false;

            if (!GameManager.isPaused) GameManager.PauseGame();

            Debug.Log("Start");
        }

        #endregion
    }
}
