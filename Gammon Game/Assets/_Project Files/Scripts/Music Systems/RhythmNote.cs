using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {
    public class RhythmNote : MonoBehaviour
    {
        enum Type {
            Tap,
            Hold
        }

        [Header("Note Info: ")]
        public float assignedTime;
        public Lane lane;
        public double noteLenth;

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
            // Since the first few notes are instantiated before the song starts playing they will change colour before they are meant to
            StartCoroutine(ChangeNoteBGColour(RhythmManager.instance.noteTime - (float)RhythmManager.instance.goodMargin, noteBGColourTap));     // changes the bg colour when the note is in the perfect zone
            StartCoroutine(ChangeNoteBGColour(RhythmManager.instance.noteTime + (float)RhythmManager.instance.goodMargin, noteBGColourMiss));    // changes the bg colour when the note leaves the perfect zone

            transform.localPosition = Vector3.up * RhythmManager.instance.noteSpawnY;
        }

        private void Update() {
            double timeSinceInstantiated = RhythmManager.GetAudioSourceTime() - (assignedTime - RhythmManager.instance.noteTime); // Calculates the time since the note was instansiated
            float t = (float)(timeSinceInstantiated / (RhythmManager.instance.noteTime * 2));     // Gets the current percentage the note pos is between the spawnPos and despawnPos
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

            float t2 = (float)(timeSinceInstantiated / RhythmManager.instance.noteTime);
            if (t2 > 1) {
                hitIndicator.SetActive(false);
            } else {
                hitIndicator.transform.localScale = Vector3.Lerp(hitIndicatorStartingScale, new Vector3(2, 2, 2), t2);
                hitIndicatorSprite.color = Color.Lerp(hitIndicatorStartColour, hitIndicatorEndColour, t2);
            }



            // If the note goes past the area where it can be hit then it counts as a miss, the note itself handles despawning so this script just leaves it to do that by itself.
            if (assignedTime + RhythmManager.instance.badMargin <= RhythmManager.GetAudioSourceTime() - (RhythmManager.instance.inputDelayInMiliseconds / 1000.0f)) {
                Miss();
                lane.removeNote(this);
                Destroy(this.gameObject);
            }
        }

        #region Public Functions

        public void Tapped() {
            RhythmManager rhythmManager = RhythmManager.instance;
            double perfectMargin = rhythmManager.perfectMargin;
            double goodMargin = rhythmManager.goodMargin;
            double badMargin = rhythmManager.badMargin;

            double audioTime = RhythmManager.GetAudioSourceTime() - (rhythmManager.inputDelayInMiliseconds / 1000.0f);

            double inputDifference = Math.Abs(audioTime - assignedTime);
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
                lane.removeNote(this);
                Destroy(this.gameObject);
            }
        }

        public void Held() {
            RhythmManager rhythmManager = RhythmManager.instance;
            double perfectMargin = rhythmManager.perfectMargin;
            double goodMargin = rhythmManager.goodMargin;
            double badMargin = rhythmManager.badMargin;

            double audioTime = RhythmManager.GetAudioSourceTime() - (rhythmManager.inputDelayInMiliseconds / 1000.0f);

            double inputDifference = Math.Abs(audioTime - assignedTime);
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
                lane.removeNote(this);
                Destroy(this.gameObject);
            }
        }

        #endregion


        #region Private Functions

        private void HoldNote() {

        }

        private void PerfectHit(double _disFromPerfect) {
            ScoreManager.PerfectHit(lane.hitTextPos, _disFromPerfect);
        }
        private void GoodHit(double _disFromPerfect) {
            ScoreManager.GoodHit(lane.hitTextPos, _disFromPerfect);
        }
        private void BadHit() {
            ScoreManager.BadHit(lane.hitTextPos);
        }
        private void Miss() {
            ScoreManager.Miss(lane.hitTextPos);
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
