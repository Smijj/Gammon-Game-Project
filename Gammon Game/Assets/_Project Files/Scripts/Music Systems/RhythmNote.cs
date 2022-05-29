using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {
    public class RhythmNote : MonoBehaviour
    {
        [Header("Note Info: ")]
        public float assignedTime;

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
            StartCoroutine(ChangeNoteBGColour(SongManager.instance.noteTime - (float)SongManager.instance.goodMargin, noteBGColourTap));     // changes the bg colour when the note is in the perfect zone
            StartCoroutine(ChangeNoteBGColour(SongManager.instance.noteTime + (float)SongManager.instance.goodMargin, noteBGColourMiss));    // changes the bg colour when the note leaves the perfect zone

            transform.localPosition = Vector3.up * SongManager.instance.noteSpawnY;
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
                hitIndicator.transform.localScale = Vector3.Lerp(hitIndicatorStartingScale, new Vector3(2, 2, 2), t2);
                hitIndicatorSprite.color = Color.Lerp(hitIndicatorStartColour, hitIndicatorEndColour, t2);
            }
        }

        private IEnumerator ChangeNoteBGColour(float _delay, Color _colour) {
            yield return new WaitForSeconds(_delay);
            noteBG.color = _colour;
        }
    }
}
