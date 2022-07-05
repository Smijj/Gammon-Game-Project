using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;
using TMPro;

namespace MusicSystem {
    public class Lane : MonoBehaviour
    {
        [Header("Lane Settings: ")]
        public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestriction;    // restricts notes to a certain key
        [Header("Input Settings: ")]
        public KeyCode input;
        public KeyCode secondaryInput;
        [Header("Refs: ")]
        public GameObject notePrefab;
        public Transform hitTextPos;    // the transform of where the hit texts prefabs will get instantiated
        public bool debug = false;
        
        
        public List<NoteData> notesData = new List<NoteData>();
        //public List<double> timeStamps = new List<double>();        // A list of all the timestamps needed for this lane of notes, gets set in [SongManager].        
        private List<RhythmNote> notes = new List<RhythmNote>();    // a list of all the notes in used in this lane


        private int spawnIndex = 0;     // The index that tracks the order notes should be spawned
        //private int inputIndex = 0;     // Tracks the players input and increments when they hit a note or it goes to far and counts as a miss


        #region Unity Functions

        private void Update() {
            // if not all the notes for this lane have been spawned:
            if (spawnIndex < notesData.Count) {
                // checks if the current point in the song is >= the next note's timestamp minus the time the note needs to travel from its spawn location to the tap location.
                // if it is, it's time for that note to spawn.
                if (RhythmManager.GetAudioSourceTime() >= notesData[spawnIndex].timeStamp - RhythmManager.instance.noteFallTime) {
                    // a note prefab is instantiated then added to the notes list
                    var note = Instantiate(notePrefab, transform);
                    note.GetComponent<RhythmNote>().noteData = notesData[spawnIndex];
                    notes.Add(note.GetComponent<RhythmNote>());
                    spawnIndex++;
                }
            }

            // if not all the notes for this lane have been hit or missed:
            if (notes.Count > 0) {

                RhythmNote nextNote = notes[0];

                if (Input.GetKeyDown(input) || Input.GetKeyDown(secondaryInput)) {
                    nextNote.Tapped();
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
            foreach (var _note in _array) {
                // if the note in the midi matches the noteRestriction it will add it to the timestamps
                if (_note.NoteName == noteRestriction) {
                    // _note.Time does not return a normal timestamp, so it is necessary to convert it into metric time
                    var metricTimeStamp = TimeConverter.ConvertTo<MetricTimeSpan>(_note.Time, RhythmManager.midiFile.GetTempoMap());
                    // Converting all the units of time (min, sec, millisec) into seconds
                    double timeStampInSeconds = (double)metricTimeStamp.Minutes * 60f + metricTimeStamp.Seconds + (double)metricTimeStamp.Milliseconds / 1000f;
                    
                    // Converting the _note.Length timescale into into metric time
                    var metricNoteLength = TimeConverter.ConvertTo<MetricTimeSpan>(_note.Length, RhythmManager.midiFile.GetTempoMap());
                    // Converting all the units of time (min, sec, millisec) into seconds
                    double lengthInSeconds = (double)metricNoteLength.Minutes * 60f + metricNoteLength.Seconds + (double)metricNoteLength.Milliseconds / 1000f;

                    notesData.Add(new NoteData(timeStampInSeconds, _note.Length, lengthInSeconds, this));
                }
            }
        }

        public void removeNote(RhythmNote _note) {
            //clear from notes, etc.
            notes.Remove(_note);
        }

        #endregion
    }
}
