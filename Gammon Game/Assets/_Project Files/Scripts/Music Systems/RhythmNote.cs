using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {
    public class RhythmNote : MonoBehaviour
    {

        //private double timeInstantiated;
        public float assignedTime;
        public GameObject sprite;
        public GameObject hitIndicator;
        private Vector3 hitIndicatorStartingScale;

        private void Start() {
            //timeInstantiated = SongManager.GetAudioSourceTime();

            //hitIndicatorLeftStartPos = hitIndicatorLeft.transform;
            //hitIndicatorRightStartPos = hitIndicatorRight.transform;
            hitIndicatorStartingScale = hitIndicator.transform.localScale;

            transform.localPosition = Vector3.up * SongManager.instance.noteSpawnY;
            // The notePrefabs sprite gameobject not activated, so it will get set to true once its spawned and the position is set.
            sprite.SetActive(true);
            hitIndicator.SetActive(true);
        }

        private void Update() {
            double timeSinceInstantiated = SongManager.GetAudioSourceTime() - (assignedTime - SongManager.instance.noteTime); // Calculates the time since the note was instansiated
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


            // ---- Handles Hit Indicator Stuff ----

            float t2 = (float)(timeSinceInstantiated / SongManager.instance.noteTime);
            if (t2 > 1) {
                hitIndicator.SetActive(false);
            } else {
                hitIndicator.transform.localScale = Vector3.Lerp(hitIndicatorStartingScale, new Vector3(1, 1, 1), t2);
            }
        }
    }
}
