using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {
    public class RhythmNote : MonoBehaviour
    {
        private double timeInstantiated;
        public float assignedTime;
        private SpriteRenderer sprite;

        private void Start() {
            timeInstantiated = SongManager.GetAudioSourceTime();
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update() {
            double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
            float t = (float)(timeSinceInstantiated / (SongManager.instance.noteTime * 2));

            // when t == 0 the note should be at the noteSpawnY, when t == 1 the note should be at noteDespawnY
            if (t>1) {
                Destroy(gameObject);
            } else {
                transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.instance.noteSpawnY, Vector3.up * SongManager.instance.noteDespawnY, t);
                if (sprite.enabled == false) sprite.enabled = true;
            }
        }
    }
}
