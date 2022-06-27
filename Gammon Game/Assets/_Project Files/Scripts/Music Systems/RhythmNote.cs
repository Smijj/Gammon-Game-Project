using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {

    public class RhythmNote : MonoBehaviour
    {
        public static int tapNoteThreshold = 64;

        [Header("Note Info: ")]
        public NoteData noteData;

        [Header("Refs: ")]
        public SpriteRenderer foodSprite;
        public SpriteRenderer noteBG;
        public Color noteBGColourStart;
        public Color noteBGColourTap;
        public Color noteBGColourMiss;
        
        public GameObject hitIndicator;
        public Color hitIndicatorStartColour;
        public Color hitIndicatorEndColour;

        private Vector3 hitIndicatorStartingScale;
        private SpriteRenderer hitIndicatorSprite;

        private void Start() {
            hitIndicatorSprite = hitIndicator.GetComponentInChildren<SpriteRenderer>();
            hitIndicatorStartingScale = hitIndicator.transform.localScale;
            hitIndicatorSprite.color = hitIndicatorStartColour;
            
            noteBG.color = noteBGColourStart;
            // Need to somehow put this stuff into a lerp of the percentage between spawn & despawn positions
            // BUG: Since the first few notes are instantiated before the song starts playing they will change colour before they are meant to
            StartCoroutine(ChangeNoteBGColour(RhythmManager.instance.noteFallTime - (float)RhythmManager.instance.goodMargin, noteBGColourTap));     // changes the bg colour when the note is in the perfect zone
            StartCoroutine(ChangeNoteBGColour(RhythmManager.instance.noteFallTime + (float)RhythmManager.instance.goodMargin, noteBGColourMiss));    // changes the bg colour when the note leaves the perfect zone

            transform.localPosition = Vector3.up * RhythmManager.instance.noteSpawnY;


            // If the note is above the tapNoteThreshold:
                // instantiate a holdNoteTailPrefab (stored in the lane this note belongs to)
        }

        private void Update() {
            double timeSinceInstantiated = RhythmManager.GetAudioSourceTime() - (noteData.timeStamp - RhythmManager.instance.noteFallTime); // Calculates the time since the note was instansiated
            float t = (float)(timeSinceInstantiated / (RhythmManager.instance.noteFallTime * 2));     // Gets the current percentage the note pos is between the spawnPos and despawnPos
                                                                                                // if t=0 the note would be at the spawnPos, if t=0.5 then it would be at the tap
                                                                                                // position, and if t=1 then it would be at the despawn pos

            // when t == 0 the note should be at the noteSpawnY, when t == 1 the note should be at noteDespawnY
            if (t>1) {
                Destroy(gameObject);
            } else {
                // Moves the note between the spawnPos and despawnPos based on the percentage t
                transform.localPosition = Vector3.Lerp(Vector3.up * RhythmManager.instance.noteSpawnY, Vector3.up * RhythmManager.instance.noteDespawnY, t);
            }


            // ---- Handles Hit Indicator Stuff ----

            float t2 = (float)(timeSinceInstantiated / RhythmManager.instance.noteFallTime);
            if (t2 > 1) {
                hitIndicator.SetActive(false);
            } else {
                hitIndicator.transform.localScale = Vector3.Lerp(hitIndicatorStartingScale, new Vector3(2, 2, 2), t2);
                hitIndicatorSprite.color = Color.Lerp(hitIndicatorStartColour, hitIndicatorEndColour, t2);
            }


            /// IDEA: to rewind the song as an ability or something, store all destroyed notes in an first in first out list and bring them back in that sequence, 
            /// would need to probably record their pos when getting destroyed, and the points scored in that period of time, etc. to be able to effectively rewind everything.
            /// maybe even keep the points there, and have it as an opertunity to get a higher score or something.


            // If the note goes past the area where it can be hit then it counts as a miss, the note itself handles despawning so this script just leaves it to do that by itself.
            if (noteData.timeStamp + RhythmManager.instance.badMargin <= RhythmManager.GetAudioSourceTime() - (RhythmManager.instance.inputDelayInMiliseconds / 1000.0f)) {
                Miss();
                noteData.lane.removeNote(this);
                Destroy(this.gameObject);
            }
        }

        #region Public Functions

        /*
        Time Scale | Note.Length | Note.Length converted into seconds
        1/1 = 512 = 1.714 seconds
        1/2 = 256 = 0.857 seconds
        1/4 = 128 = 0.428 seconds
        1/8 = 64 = 0.214 seconds
        1/16 = 32 = 0.107 seconds
        1/32 = 16 = 0.053 seconds

        I calculated these manually but after the fact i found this nifty little website that could have done it for me: https://rechneronline.de/musik/note-length.php
        ;--------------------------; 
        oh well i guess i can consider my calculations peer reviewed or something.
        */

        public void Tapped() {
            if (noteData.timeScale > tapNoteThreshold) return;

            Debug.Log("Tap");

            RhythmManager rhythmManager = RhythmManager.instance;
            double perfectMargin = rhythmManager.perfectMargin;
            double goodMargin = rhythmManager.goodMargin;
            double badMargin = rhythmManager.badMargin;

            double audioTime = RhythmManager.GetAudioSourceTime() - (rhythmManager.inputDelayInMiliseconds / 1000.0f);

            double inputDifference = Math.Abs(audioTime - noteData.timeStamp);
            bool wasHit = false;

            // Manage Perfect/Good/Bad notes in here
            if (inputDifference < perfectMargin) {
                PerfectHit(inputDifference);
                Log($"Perfect Hit on note.");
                wasHit = true;
            } else if (inputDifference < goodMargin) {
                GoodHit(inputDifference);
                Log($"Good Hit on note.");
                wasHit = true;
            } else if (inputDifference < badMargin) {
                BadHit();
                Log($"Bad Hit on note.");
                wasHit = true;
            } else {
                Log($"Hit inaccurate on note with {inputDifference} delay.");
            }

            if (wasHit) {
                noteData.lane.removeNote(this);
                Destroy(this.gameObject);
            }
        }

        public void Held() {
            if (noteData.timeScale <= tapNoteThreshold) return;

            Debug.Log("Held");

            RhythmManager rhythmManager = RhythmManager.instance;
            double perfectMargin = rhythmManager.perfectMargin;
            double goodMargin = rhythmManager.goodMargin;
            double badMargin = rhythmManager.badMargin;

            double audioTime = RhythmManager.GetAudioSourceTime() - (rhythmManager.inputDelayInMiliseconds / 1000.0f);

            double inputDifference = Math.Abs(audioTime - noteData.timeStamp);
            bool wasHit = false;

            // Manage Perfect/Good/Bad notes in here
            if (inputDifference < perfectMargin) {
                PerfectHit(inputDifference);
                Log($"Perfect Hit on note.");
                wasHit = true;
            } else if (inputDifference < goodMargin) {
                GoodHit(inputDifference);
                Log($"Good Hit on note.");
                wasHit = true;
            } else if (inputDifference < badMargin) {
                BadHit();
                Log($"Bad Hit on note.");
                wasHit = true;
            } else {
                Log($"Hit inaccurate on note with {inputDifference} delay.");
            }

            if (wasHit) {
                noteData.lane.removeNote(this);
                Destroy(this.gameObject);
            }
        }

        #endregion


        #region Private Functions

        private void HoldNote() {

        }

        private void PerfectHit(double _disFromPerfect) {
            ScoreManager.PerfectHit(noteData.lane.hitTextPos, _disFromPerfect);
        }
        private void GoodHit(double _disFromPerfect) {
            ScoreManager.GoodHit(noteData.lane.hitTextPos, _disFromPerfect);
        }
        private void BadHit() {
            ScoreManager.BadHit(noteData.lane.hitTextPos);
        }
        private void Miss() {
            ScoreManager.Miss(noteData.lane.hitTextPos);
        }

        private void Log(string _msg) {
            if (RhythmManager.instance.debug) {
                Debug.Log("[Lane] " + _msg);
            }
        }

        private IEnumerator ChangeNoteBGColour(float _delay, Color _colour) {

            /// MIGHT CAUSE A MEMORY LEAK
            while (RhythmManager.songIsPlaying == false) {
                yield return null;
            }

            yield return new WaitForSecondsRealtime(_delay);
            noteBG.color = _colour;
        }

        #endregion
    }
}
