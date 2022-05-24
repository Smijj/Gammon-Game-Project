using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace MusicSystem {
    public class SongManager : MonoBehaviour
    {
        // a static ref to the midifile that the current song is using
        public static MidiFile midiFile;
        public GameObject indicatorLine;    // A simple gameobject that will be spawned to indicate where the player needs to tap
        
        public static SongManager instance; // Might need to turn this into a singleton, but dont need to rn
        public AudioSource audioSource;     // The audiosource where the audio will be played through in the game
        public Lane[] lanes;                // The lanes that the notes drop from
        public float songDelayInSeconds;    // The ddelay before the song starts
        public int inputDelayInMiliseconds; // To account for input delay

        public double perfectMargin;
        public double goodMargin;       
        public double badMargin;


        [Tooltip("The location of the midi file that is to be played alongside the music.")]
        public string fileLocation;
        [Tooltip("The time it takes for the note to reach the noteTapY position.")]
        public float noteTime;
        [Tooltip("The position the notes will spawn in the world.")]
        public float noteSpawnY;
        [Tooltip("The position the player has to tap the notes.")]
        public float noteTapY;
        public float noteDespawnY {
            get {
                return noteTapY - (noteSpawnY - noteTapY);
            }
        }




        #region Unity Functions

        private void Start() {
            instance = this;

            // Checks if the mififile that is trying to be read is from an online link or on file, then calls the necessary function to handle it
            if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://")) {
                StartCoroutine(ReadFromWebsite());
            } else {
                ReadFromFile();
            }


            // Spawns the tap line indicators.
            DrawAroundTapPos(-perfectMargin, Color.grey + Color.red + Color.yellow);   // Draws line before the tapPos
            DrawAroundTapPos(perfectMargin, Color.grey + Color.red + Color.yellow);    // Draws line after the tapPos
            DrawAroundTapPos(-goodMargin, Color.white);   // Draws line before the tapPos
            DrawAroundTapPos(goodMargin, Color.white);    // Draws line after the tapPos

        }

        // Just using this to visulize some lines
        private void OnDrawGizmos() {

            Vector3 spawnYPos = new Vector3(0, noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, noteDespawnY, 0);

            double timeToGetToFirstMargin = noteTime - (inputDelayInMiliseconds / 1000.0f) - badMargin;
            float firstMarginPercentage = (float)timeToGetToFirstMargin / (noteTime * 2);
            Vector3 firstMarginYPos = Vector3.Lerp(spawnYPos, despawnYPos, firstMarginPercentage);
            Gizmos.DrawLine(new Vector3(-20, firstMarginYPos.y, 0), new Vector3(20, firstMarginYPos.y, 0));

            double timeToGetToSecondMargin = noteTime - (inputDelayInMiliseconds / 1000.0f) + badMargin;
            float secondMarginPercentage = (float)timeToGetToSecondMargin / (noteTime * 2);
            Vector3 secondMarginYPos = Vector3.Lerp(spawnYPos, despawnYPos, secondMarginPercentage);
            Gizmos.DrawLine(new Vector3(-20, secondMarginYPos.y, 0), new Vector3(20, secondMarginYPos.y, 0));
        }

        #endregion


        #region Public Functions

        /// <summary>
        /// Gets the current timestamp of the audio that is playing
        /// </summary>
        /// <returns></returns>
        public static double GetAudioSourceTime() {
            return (double)instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
        }

        // DOESNT WORK
        //public void ResetSong() {
        //    if (audioSource.isPlaying) audioSource.Stop();
        //    GetDataFromMidi();
        //}

        #endregion


        #region Private Functions

        /// <summary>
        /// Used for drawning a horizontal line across the screen in relation to the tapPos.
        /// </summary>
        /// <param name="_timeMargin">Negative number to place before the tapPos, positive for after. This is the time in seconds before or after the tapPos you want to draw something.</param>
        /// <param name="_accountForInputDelay">Controls whether or not the drawn objects pos will account for input delay or not (i.e. will it visually move to match the input delay).</param>
        private void DrawAroundTapPos(double _timeMargin, Color _lineColour, bool _accountForInputDelay = false) {
            Vector3 spawnYPos = new Vector3(0, noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, noteDespawnY, 0);

            double timeToGetToMargin = !_accountForInputDelay ? noteTime + _timeMargin : noteTime - (inputDelayInMiliseconds / 1000.0f) + _timeMargin;
            float marginPercentage = (float)timeToGetToMargin / (noteTime * 2);
            Vector3 marginYPos = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage);

            if (indicatorLine) {
                Instantiate(indicatorLine, marginYPos, Quaternion.identity).GetComponent<SpriteRenderer>().color = _lineColour;
            }
        }

        private IEnumerator ReadFromWebsite() {
            // Sends a request to the streamingAssetsPath website looking for the fileLocation
            using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileLocation)) {
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

        private void ReadFromFile() {
            midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
            GetDataFromMidi();
        }

        private void GetDataFromMidi() {
            var notes = midiFile.GetNotes();    // gets the notes from the midi file
            var array = new Note[notes.Count];  // creates a new array of type Note
            notes.CopyTo(array, 0);             // stores the notes from the midi file into the new array

            // sets the timestamps for all the notes in each lane.
            foreach (var _lane in lanes) _lane.SetTimeStamps(array);

            Invoke(nameof(StartSong), songDelayInSeconds);
        }

        private void StartSong() {
            audioSource.Play();
            Debug.Log("Start");
        }

        #endregion
    }
}
