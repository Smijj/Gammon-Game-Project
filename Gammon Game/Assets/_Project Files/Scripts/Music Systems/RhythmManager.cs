using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
        [Header("Song Mangager Stuff: ")]
        public AudioSource audioSource;     // The audiosource where the audio will be played through in the game
        public Lane[] lanes;                // The lanes that the notes drop from
        
        [Header("Song Settings: ")]
        public Song song;
        public float songDelayInSeconds;    // The ddelay before the song starts
        public GameObject startCountdown;

        [Header("Tap Settings: ")]
        public int inputDelayInMiliseconds; // To account for input delay
        public double perfectMargin;
        public double goodMargin;       
        public double badMargin;

        public float tapIndicatorWidth = 20f;
        public Color perfectTapZone;
        public Color goodTapZoneLines;
        public Transform gameArea;
        public GameObject indicatorPrefab;    // A simple gameobject that will be spawned to indicate where the player needs to tap


        [Header("Note Settings: ")]
        [Tooltip("The time it takes for the note to reach the noteTapY position.")]
        public float noteTime;
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


        private Camera mainCamera;
        private List<GameObject> hitIndicatorObjects = new List<GameObject>();


        #region Unity Functions

        private void OnEnable() {
            instance = this;
            GameManager.PauseGame();
            mainCamera = Camera.main;
            mainCamera.gameObject.SetActive(false);
        }

        private void OnDisable() {
            mainCamera.gameObject.SetActive(true);
            songIsPlaying = false;
        }

        private void Start() {
            //audioSource.pitch = noteTime;

            // Checks if the mififile that is trying to be read is from an online link or on file, then calls the necessary function to handle it
            if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://")) {
                StartCoroutine(ReadMidiFromWebsite(song.midiFileName));
            } else {
                ReadMidiFromFile(song.midiFileName);
            }
            
            StartCoroutine(LoadAudio(song.songFileName));
            
        }

        private void Update() {
            if (lastNoteTime != noteTime) {
                DrawTapLineIndicators();
                lastNoteTime = noteTime;
            }
            if (audioSource.clip != null && GetAudioSourceTime() > audioSource.clip.length) {
                Debug.Log("Song is finished");
                FinishSong();
            }
        }

        #endregion


        #region Public Functions

        public void InitSong(Song _song) {
            song = _song;
        }

        public void PauseSong(bool _pause) {
            if (_pause) audioSource.pitch = 0;
            else audioSource.pitch = 1; // Can set this to equal some pitch speed var later
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

            Debug.Log("Start");
        }

        private void FinishSong() {
            songIsPlaying = false;
            PauseSong(true);
            PageManager.singleton.TurnPageOn(PageType.SongOver);
        }





        private void DrawTapLineIndicators() {
            // Cleans up any old hit indicators in the case this function is called more than once.
            if (hitIndicatorObjects.Count > 0) {
                foreach (var _object in hitIndicatorObjects) {
                    Destroy(_object);
                }
                hitIndicatorObjects.Clear();
            }

            // Spawns the tap line indicators.
            hitIndicatorObjects.Add(DrawBlockAroundTapPos(perfectMargin, perfectTapZone));
            hitIndicatorObjects.Add(DrawAroundTapPos(-goodMargin, goodTapZoneLines));   // Draws line before the tapPos
            hitIndicatorObjects.Add(DrawAroundTapPos(goodMargin, goodTapZoneLines));    // Draws line after the tapPos
        }

        /// <summary>
        /// Used for drawning a horizontal line across the screen in relation to the tapPos.
        /// </summary>
        /// <param name="_timeMargin">Negative number to place before the tapPos, positive for after. This is the time in seconds before or after the tapPos you want to draw something.</param>
        /// <param name="_lineColour"></param>
        /// <param name="_accountForInputDelay">Controls whether or not the drawn objects pos will account for input delay or not (i.e. will it visually move to match the input delay).</param>
        private GameObject DrawAroundTapPos(double _timeMargin, Color _lineColour, bool _accountForInputDelay = false) {
            Vector3 spawnYPos = new Vector3(0, noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, noteDespawnY, 0);

            double timeToGetToMargin = !_accountForInputDelay ? noteTime + _timeMargin : noteTime - (inputDelayInMiliseconds / 1000.0f) + _timeMargin;
            float marginPercentage = (float)timeToGetToMargin / (noteTime * 2);
            Vector3 marginYPos = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage);

            if (indicatorPrefab && gameArea) {
                GameObject prefabInst = Instantiate(indicatorPrefab, gameArea);

                Vector3 yPosVec = marginYPos;
                prefabInst.transform.localPosition = new Vector3(0, yPosVec.y, 0);
                prefabInst.transform.localScale = new Vector3(tapIndicatorWidth, 0.1f, 1);
                prefabInst.GetComponent<SpriteRenderer>().color = _lineColour;

                return prefabInst;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_timeMargin"></param>
        /// <param name="_blockColour"></param>
        /// <param name="_accountForInputDelay"></param>
        private GameObject DrawBlockAroundTapPos(double _timeMargin, Color _blockColour, bool _accountForInputDelay = false) {
            Vector3 spawnYPos = new Vector3(0, noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, noteDespawnY, 0);

            double timeToGetToMargin1 = !_accountForInputDelay ? noteTime - _timeMargin : noteTime - (inputDelayInMiliseconds / 1000.0f) - _timeMargin;
            float marginPercentage1 = (float)timeToGetToMargin1 / (noteTime * 2);
            Vector3 marginYPos1 = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage1);

            double timeToGetToMargin2 = !_accountForInputDelay ? noteTime + _timeMargin : noteTime - (inputDelayInMiliseconds / 1000.0f) + _timeMargin;
            float marginPercentage2 = (float)timeToGetToMargin2 / (noteTime * 2);
            Vector3 marginYPos2 = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage2);

            if (indicatorPrefab && gameArea) {
                GameObject prefabInst = Instantiate(indicatorPrefab, gameArea);

                Vector3 yPosVec = (marginYPos1 + marginYPos2) / 2;
                prefabInst.transform.localPosition = new Vector3(0, yPosVec.y, 0);
                float disY = Vector3.Distance(marginYPos1, marginYPos2);
                prefabInst.transform.localScale = new Vector3(tapIndicatorWidth, disY, 1);

                prefabInst.GetComponent<SpriteRenderer>().color = _blockColour;

                return prefabInst;
            }
            return null;
        }

        #endregion
    }
}
