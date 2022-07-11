using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {

    public class RhythmNote : MonoBehaviour
    {
        public static int tapNoteThreshold = 64;
        private RhythmManager rm;

        [Header("Note Info: ")]
        public NoteData noteData;
        
        private double noteLength;
        private bool holding = false;
        private GameObject noteTailPrefab;

        [Header("Refs: ")]
        public SpriteRenderer foodSprite;
        public SpriteRenderer noteBG;
        public GameObject hitIndicator;

        private Vector3 hitIndicatorStartingScale;
        private SpriteRenderer hitIndicatorSprite;



        private void Start() {
            rm = RhythmManager.instance;

            hitIndicatorSprite = hitIndicator.GetComponentInChildren<SpriteRenderer>();
            hitIndicatorStartingScale = hitIndicator.transform.localScale;
            hitIndicatorSprite.color = rm.hitIndicatorStartColour;
            
            noteBG.color = rm.noteBGColourStart;
            // Need to somehow put this stuff into a lerp of the percentage between spawn & despawn positions
            // BUG: Since the first few notes are instantiated before the song starts playing they will change colour before they are meant to
            StartCoroutine(ChangeNoteBGColour(rm.noteFallTime - (float)rm.goodMargin, rm.noteBGColourTap));     // changes the bg colour when the note is in the perfect zone
            StartCoroutine(ChangeNoteBGColour(rm.noteFallTime + (float)rm.goodMargin, rm.noteBGColourMiss));    // changes the bg colour when the note leaves the perfect zone

            transform.localPosition = Vector3.up * rm.noteSpawnY;

            noteLength = noteData.lengthInSeconds;
        }

        private void Update() {
            double timeSinceInstantiated = RhythmManager.GetAudioSourceTime() - (noteData.timeStamp - rm.noteFallTime); // Calculates the time since the note was instansiated
            float t = (float)(timeSinceInstantiated / (rm.noteFallTime * 2));     // Gets the current percentage the note pos is between the spawnPos and despawnPos
                                                                                                // if t=0 the note would be at the spawnPos, if t=0.5 then it would be at the tap
                                                                                                // position, and if t=1 then it would be at the despawn pos
            
            // Stops the note from moving if its being held
            if (!holding) {
                // when t == 0 the note should be at the noteSpawnY, when t == 1 the note should be at noteDespawnY
                if (t>1) {
                    Destroy(gameObject);
                } else {
                    // Moves the note between the spawnPos and despawnPos based on the percentage t
                    transform.localPosition = Vector3.Lerp(Vector3.up * rm.noteSpawnY, Vector3.up * rm.noteDespawnY, t);
                }
            }


            // ---- Handles Hit Indicator Stuff ----

            float t2 = (float)(timeSinceInstantiated / rm.noteFallTime);
            if (t2 > 1) {
                hitIndicator.SetActive(false);
            } else {
                hitIndicator.transform.localScale = Vector3.Lerp(hitIndicatorStartingScale, Vector3.one, t2);
                hitIndicatorSprite.color = Color.Lerp(rm.hitIndicatorStartColour, rm.hitIndicatorEndColour, t2);
            }


            /// IDEA: to rewind the song as an ability or something, store all destroyed notes in an first in first out list and bring them back in that sequence, 
            /// would need to probably record their pos when getting destroyed, and the points scored in that period of time, etc. to be able to effectively rewind everything.
            /// maybe even keep the points there, and have it as an opertunity to get a higher score or something.

            
            // Handles hold note behaviour while the player is holding the key down, will destroy the note upon key release or note finish.
            if (holding) {
                if (Input.GetKey(noteData.lane.input) || Input.GetKey(noteData.lane.secondaryInput)) {
                    if (noteLength >= 0) {
                        // reduce the note tail length based on the time the left to hold
                        noteLength -= Time.unscaledDeltaTime;
                    } else {
                        // upon the hold time being complete, calculate score base on the length of the hold time
                        ScoreManager.CaclulateHoldScore(noteData.lengthInSeconds);
                        
                        DestroyNote();
                    }
                } else {
                    // if the player doesnt complete the hold note (lets go of the key too early)
                    Miss();
                    DestroyNote();
                }

            }




            if (noteData.timeScale > tapNoteThreshold) {
                if (noteData.timeStamp + noteData.lengthInSeconds + rm.badMargin <= RhythmManager.GetAudioSourceTime() - (rm.inputDelayInMiliseconds / 1000.0f)) {
                    Miss();
                    DestroyNote();
                }

                // Draws the hold note tail
                if (noteTailPrefab) Destroy(noteTailPrefab);
                noteTailPrefab = DrawRhythmUI.instance.DrawHoldNoteTailUI(noteLength, 1f, transform, rm.holdNoteTailColour);
            } else {
                // If the note goes past the area where it can be hit then it counts as a miss, the note itself handles despawning so this script just leaves it to do that by itself.
                if (noteData.timeStamp + rm.badMargin <= RhythmManager.GetAudioSourceTime() - (rm.inputDelayInMiliseconds / 1000.0f)) {
                    Miss();
                    DestroyNote();
                }
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
            RhythmManager rhythmManager = rm;
            double perfectMargin = rhythmManager.perfectMargin;
            double goodMargin = rhythmManager.goodMargin;
            double badMargin = rhythmManager.badMargin;

            double audioTime = RhythmManager.GetAudioSourceTime() - (rhythmManager.inputDelayInMiliseconds / 1000.0f);

            double inputDifference = Math.Abs(audioTime - noteData.timeStamp);

            if (noteData.timeScale <= tapNoteThreshold) {
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
                    DestroyNote();
                }

            } else {
                // Manage Perfect/Good/Bad notes in here
                if (inputDifference < perfectMargin) {
                    PerfectHit(inputDifference);
                    Log($"Perfect Hit on note.");
                    holding = true;
                } else if (inputDifference < goodMargin) {
                    GoodHit(inputDifference);
                    Log($"Good Hit on note.");
                    holding = true;
                } else if (inputDifference < badMargin) {
                    BadHit();
                    Log($"Bad Hit on note.");
                } else {
                    Log($"Hit inaccurate on note with {inputDifference} delay.");
                }         
                
                if (!holding) {
                    DestroyNote();
                } else {
                    Instantiate(rm.holdEffect, transform);
                }
            }
        }

        #endregion


        #region Private Functions

        private void DestroyNote() {
            noteData.lane.removeNote(this);
            Destroy(this.gameObject);
            Instantiate(rm.hitEffect, transform.position, transform.rotation);
        }


        private void PerfectHit(double _disFromPerfect) {
            ScoreManager.PerfectHit(_disFromPerfect);
        }
        private void GoodHit(double _disFromPerfect) {
            ScoreManager.GoodHit(_disFromPerfect);
        }
        private void BadHit() {
            ScoreManager.BadHit();
        }
        private void Miss() {
            ScoreManager.Miss();
        }

        private void Log(string _msg) {
            if (rm.debug) {
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


    [Serializable]
    public class NoteData {
        public double timeStamp;
        public long timeScale;
        public double lengthInSeconds;
        public Lane lane;

        public NoteData(double _timeStamp, long _timeScale, double _lengthInSeconds, Lane _lane) {
            timeStamp = _timeStamp;
            timeScale = _timeScale;
            lengthInSeconds = _lengthInSeconds;
            lane = _lane;
        }
    }
}
