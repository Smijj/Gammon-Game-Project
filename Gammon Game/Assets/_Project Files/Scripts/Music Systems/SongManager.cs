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
        public static SongManager instance;
        public AudioSource audioSource;
        public Lane[] lanes;
        public float songDelayInSeconds;
        public double marginOfError;    // in seconds
        public int inputDelayInMiliseconds;

        public string fileLocation;
        public float noteTime;
        public float noteSpawnY;
        public float noteTapY;
        public float noteDespawnY {
            get {
                return noteTapY - (noteSpawnY - noteTapY);
            }
        }

        public GameObject indicatorLine;

        public static MidiFile midiFile;

        #region Unity Functions

        private void Start() {
            instance = this;
            if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://")) {
                StartCoroutine(ReadFromWebsite());
            } else {
                ReadFromFile();
            }

            Vector3 spawnYPos = new Vector3(0, noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, noteDespawnY, 0);

            double timeToGetToFirstMargin = noteTime - (inputDelayInMiliseconds / 1000.0f) - marginOfError;
            float firstMarginPercentage = (float)timeToGetToFirstMargin / (noteTime * 2);
            Vector3 firstMarginYPos = Vector3.Lerp(spawnYPos, despawnYPos, firstMarginPercentage);

            double timeToGetToSecondMargin = noteTime - (inputDelayInMiliseconds / 1000.0f) + marginOfError;
            float secondMarginPercentage = (float)timeToGetToSecondMargin / (noteTime * 2);
            Vector3 secondMarginYPos = Vector3.Lerp(spawnYPos, despawnYPos, secondMarginPercentage);


            if (indicatorLine) {
                Instantiate(indicatorLine, firstMarginYPos, Quaternion.identity);
                Instantiate(indicatorLine, secondMarginYPos, Quaternion.identity);
            }

        }

        //private void OnDrawGizmos() {

        //    Vector3 spawnYPos = new Vector3(0, noteSpawnY, 0);
        //    Vector3 despawnYPos = new Vector3(0, noteDespawnY, 0);

        //    double timeToGetToFirstMargin = noteTime - (inputDelayInMiliseconds / 1000.0f) - marginOfError;
        //    float firstMarginPercentage = (float)timeToGetToFirstMargin / (noteTime * 2);
        //    Vector3 firstMarginYPos = Vector3.Lerp(spawnYPos, despawnYPos, firstMarginPercentage);
        //    Gizmos.DrawLine(new Vector3(-20, firstMarginYPos.y, 0), new Vector3(20, firstMarginYPos.y, 0));

        //    double timeToGetToSecondMargin = noteTime - (inputDelayInMiliseconds / 1000.0f) + marginOfError;
        //    float secondMarginPercentage = (float)timeToGetToSecondMargin / (noteTime * 2);
        //    Vector3 secondMarginYPos = Vector3.Lerp(spawnYPos, despawnYPos, secondMarginPercentage);
        //    Gizmos.DrawLine(new Vector3(-20, secondMarginYPos.y, 0), new Vector3(20, secondMarginYPos.y, 0));
        //}

        #endregion


        #region Public Functions

        /// <summary>
        /// Gets the current timestamp of the audio that is playing
        /// </summary>
        /// <returns></returns>
        public static double GetAudioSourceTime() {
            return (double)instance.audioSource.timeSamples / instance.audioSource.clip.frequency;
        }

        // Doesnt Work
        //public void ResetSong() {
        //    if (audioSource.isPlaying) audioSource.Stop();
        //    GetDataFromMidi();
        //}

        #endregion


        #region Private Functions

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
