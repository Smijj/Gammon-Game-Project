using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace UI {
    using InventorySystem;
    using GameManagement;

    public class SongCard : MonoBehaviour, IPointerDownHandler {
        
        public Song song;

        public bool isDisplayCard = false;
        public TextMeshProUGUI songName;
        public TextMeshProUGUI highScore;
        public TextMeshProUGUI comboScore;

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
    }
}
