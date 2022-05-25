using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {
    public class RhythmNote : MonoBehaviour
    {
        private double timeInstantiated;
        public float assignedTime;
        public float t2;
        public GameObject sprite;
        public GameObject hitIndicator;
        //public GameObject hitIndicatorLeft;
        //public GameObject hitIndicatorRight;

        //private Transform hitIndicatorLeftStartPos;
        //private Transform hitIndicatorRightStartPos;
        private Vector3 hitIndicatorStartingScale;

        private void Start() {
            timeInstantiated = SongManager.GetAudioSourceTime();

            //hitIndicatorLeftStartPos = hitIndicatorLeft.transform;
            //hitIndicatorRightStartPos = hitIndicatorRight.transform;
            hitIndicatorStartingScale = hitIndicator.transform.localScale;

            transform.localPosition = Vector3.up * SongManager.instance.noteSpawnY;
            // The notePrefabs sprite gameobject not activated, so it will get set to true once its spawned and the position is set.
            sprite.SetActive(true);
            hitIndicator.SetActive(true);
        }

        private void Update() {
            double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated; // Calculates the time since the note was instansiated
            float t = (float)(timeSinceInstantiated / (SongManager.instance.noteTime * 2));     // Gets the current percentage the note pos is between the spawnPos and despawnPos
                                                                                                // if t=0 the note would be at the spawnPos, if t=0.5 then it would be at the tap
                                                                                                // position, and if t=1 then it would be at the despawn pos

            // when t == 0 the note should be at the noteSpawnY, when t == 1 the note should be at noteDespawnY
            if (t>1) {
                Destroy(gameObject);
            } else {
                // Moves the note between the spawnPos and despawnPos based on the percentage t
                transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.instance.noteSpawnY, Vector3.up * SongManager.instance.noteDespawnY, t);
            }


            // ---- Handles Indicator Stuff ----

            t2 = (float)(timeSinceInstantiated / SongManager.instance.noteTime);
            // when t2 == 0 the HitIndicator should be at the hitIndicatorStartPos, when t2 == 1 the HtiIndicator should be at Vector3(0, 1, 0)
            if (t2 > 1) {
                //hitIndicatorLeft.SetActive(false);
                //hitIndicatorRight.SetActive(false);
                hitIndicator.SetActive(false);
            } else {
                // Moves the note between the hitIndicatorStartPos and 0 based on the percentage t2
                //hitIndicatorLeft.transform.localPosition = Vector3.Lerp(hitIndicatorLeftStartPos.position, Vector3.zero, t2);
                //hitIndicatorRight.transform.localPosition = Vector3.Lerp(hitIndicatorRightStartPos.position, Vector3.zero, t2);
                hitIndicator.transform.localScale = Vector3.Lerp(hitIndicatorStartingScale, new Vector3(0.6f, 0.6f, 0.6f), t2);
            }
        }
    }
}
