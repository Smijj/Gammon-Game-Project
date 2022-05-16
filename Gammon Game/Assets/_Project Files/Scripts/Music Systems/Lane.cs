using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;

namespace MusicSystem {
    public class Lane : MonoBehaviour
    {
        public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;    // restricts notes to a certain key
        public KeyCode input;
        public KeyCode secondaryInput;
        public GameObject notePrefab;
        public List<double> timeStamps = new List<double>();
        private List<RhythmNote> notes = new List<RhythmNote>();

        private int spawnIndex = 0;
        private int inputIndex = 0;

        #region Unity Functions

        private void Update() {
            if (spawnIndex < timeStamps.Count) {
                // checks if the current point in the song is >= the next note's timestamp minus the time the note needs to travel from its spawn location to the tap location.
                // if it is, it's time for that note to spawn.
                if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.instance.noteTime) {
                    var note = Instantiate(notePrefab, transform);
                    notes.Add(note.GetComponent<RhythmNote>());
                    note.GetComponent<RhythmNote>().assignedTime = (float)timeStamps[spawnIndex];
                    spawnIndex++;
                }
            }

            if (inputIndex < timeStamps.Count) {
                double timeStamp = timeStamps[inputIndex];
                double marginOfError = SongManager.instance.marginOfError;
                double audioTime = SongManager.GetAudioSourceTime() - (SongManager.instance.inputDelayInMiliseconds / 1000.0f);

                if (Input.GetKeyDown(input) || Input.GetKeyDown(secondaryInput)) {
                    if (Math.Abs(audioTime - timeStamp) < marginOfError) {
                        Hit();
                        Debug.Log($"Hit on {inputIndex} note.");
                        Destroy(notes[inputIndex].gameObject);
                        inputIndex++;
                    } else {
                        Debug.Log($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay.");
                    }
                }
                if (timeStamp + marginOfError <= audioTime) {
                    Miss();
                    Debug.Log($"Missed {inputIndex} note.");
                    inputIndex++;
                }
            }
        }

        #endregion


        #region Public Functions

        public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] _array) {
            foreach (var _note in _array) {
                if (_note.NoteName == noteRestriction) {
                    // _note.Time does not return a normal timestamp, so it is necessary to convert it into metric time
                    var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(_note.Time, SongManager.midiFile.GetTempoMap());
                    // Converting all the units of time (min, sec, millisec) into seconds and adding that to the timeStamps list
                    timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);    
                }
            }
        }
        
        #endregion
        
        
        #region Private Functions

        private void Hit() {
            ScoreManager.Hit();
        }

        private void Miss() {
            ScoreManager.Miss();
        }

        #endregion
    }
}
