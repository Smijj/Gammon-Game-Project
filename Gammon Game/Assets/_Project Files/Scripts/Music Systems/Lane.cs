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
        public bool debug = false;
        public List<double> timeStamps = new List<double>();        // A list of all the timestamps needed for this lane of notes, gets set it [SongManager].
        private List<RhythmNote> notes = new List<RhythmNote>();    // a list of all the notes in used in this lane

        private int spawnIndex = 0;     // The index that tracks the order notes should be spawned
        private int inputIndex = 0;     // Tracks the players input and increments when they hit a note or it goes to far and counts as a miss

        #region Unity Functions

        private void Update() {
            // if not all the notes for this lane have been spawned:
            if (spawnIndex < timeStamps.Count) {
                // checks if the current point in the song is >= the next note's timestamp minus the time the note needs to travel from its spawn location to the tap location.
                // if it is, it's time for that note to spawn.
                if (SongManager.GetAudioSourceTime() >= timeStamps[spawnIndex] - SongManager.instance.noteTime) {
                    // a note prefab is instantiated then added to the notes list
                    var note = Instantiate(notePrefab, transform);  
                    notes.Add(note.GetComponent<RhythmNote>());
                    note.GetComponent<RhythmNote>().assignedTime = (float)timeStamps[spawnIndex];
                    spawnIndex++;
                }
            }

            // if not all the notes for this lane have been hit or missed:
            if (inputIndex < timeStamps.Count) {
                // Setting some variables so they are easier to work with
                double perfectMargin = SongManager.instance.perfectMargin;
                double goodMargin = SongManager.instance.goodMargin;
                double badMargin = SongManager.instance.badMargin;
                
                double timeStamp = timeStamps[inputIndex];
                double audioTime = SongManager.GetAudioSourceTime() - (SongManager.instance.inputDelayInMiliseconds / 1000.0f);

                if (Input.GetKey(input) || Input.GetKeyDown(secondaryInput)) {
                    
                    // Manage Perfect/Good/Bad notes in here
                    if (Math.Abs(audioTime - timeStamp) < perfectMargin) {
                        PerfectHit();
                        Log($"Perfect Hit on {inputIndex} note.");
                        Destroy(notes[inputIndex].gameObject);
                        inputIndex++;
                    } else if (Math.Abs(audioTime - timeStamp) < goodMargin) {
                        GoodHit();
                        Log($"Good Hit on {inputIndex} note.");
                        Destroy(notes[inputIndex].gameObject);
                        inputIndex++;
                    } else if (Math.Abs(audioTime - timeStamp) < badMargin) {
                        BadHit();
                        Log($"Bad Hit on {inputIndex} note.");
                        Destroy(notes[inputIndex].gameObject);
                        inputIndex++;
                    } else {
                        Log($"Hit inaccurate on {inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay.");
                    }
                }

                // If the note goes past the area where it can be hit then it counts as a miss, the note itself handles despawning so this script just leaves it to do that by itself.
                if (timeStamp + badMargin <= audioTime) {
                    Miss();
                    Log($"Missed {inputIndex} note.");
                    inputIndex++;
                }
            }
        }

        #endregion


        #region Public Functions

        /// <summary>
        /// Sets the timestamps for all the notes that will be played in this array.
        /// </summary>
        /// <param name="_array">Array that holds all the midi notes for the song.</param>
        public void SetTimeStamps(Melanchall.DryWetMidi.Interaction.Note[] _array) {
            
            // An attempt to create reset functionality, DOESNT WORK rn
            //spawnIndex = 0;
            //inputIndex = 0;
            //if (notes.Count > 0) {
            //    foreach (var note in notes) {
            //        Destroy(note.gameObject);
            //    }
            //    notes.Clear();
            //}

            foreach (var _note in _array) {
                // if the note in the midi matches the noteRestriction it will add it to the timestamps
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

        private void PerfectHit() {
            ScoreManager.PerfectHit();
        }
        private void GoodHit() {
            ScoreManager.GoodHit();
        }
        private void BadHit() {
            ScoreManager.BadHit();
        }
        private void Miss() {
            ScoreManager.Miss();
        }

        private void Log(string _msg) {
            if (debug) {
                Debug.Log("[Lane] " + _msg);
            }
        }

        #endregion
    }
}
