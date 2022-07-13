using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace UI {
    using InventorySystem;
    using GameManagement;

    public class SongCard : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
        
        public Song song;

        public bool isDisplayCard = false;
        public TextMeshProUGUI songName;
        public TextMeshProUGUI highScore;
        public TextMeshProUGUI comboScore;

        [Header("Hover Settings")]
        [Range(0, 1)]
        public float onHoverEnterDim = 0.8f;
        [Range(0, 1)]
        public float onHoverExitDim = 1f;

        private void Start() {
            if (song != null)
                UpdateSongCard();
        }

        public void PlaySong() {
            if (song != null)
                GameManager.singleton.PlaySong(song);
        }

        public void UpdateSongCard() {
            if (song != null) {
                if (songName) songName.text = song.name;
                if (highScore) highScore.text = "Highscore: " + song.highscore.ToString("0");
                if (comboScore) comboScore.text = "Largest Combo: " + song.largestCombo.ToString("0");
            } else {
                Debug.LogWarning("[SongCard] there is no song assigned to this StatCard");
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (song != null && !isDisplayCard) {
                SongsManager.instance.songInfoPanel.song = song;
                SongsManager.instance.songInfoPanel.UpdateSongCard();
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (song != null && !isDisplayCard) {
                // Darken Recipe Card UI
                GetComponent<CanvasGroup>().alpha = onHoverEnterDim;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (song != null && !isDisplayCard) {
                // Lighten Recipe Card UI
                GetComponent<CanvasGroup>().alpha = onHoverExitDim;
            }
        }
    }
}
