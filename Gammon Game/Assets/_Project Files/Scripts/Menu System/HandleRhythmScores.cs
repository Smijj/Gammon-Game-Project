using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MenuSystem {
    using MusicSystem;
    using InventorySystem;
    using UI;

    public class HandleRhythmScores : MonoBehaviour
    {
        public TextMeshProUGUI highscoreText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI newHighscoreText;
        public TextMeshProUGUI maxComboScoreText;

        public TextMeshProUGUI perfectHitsText;
        public TextMeshProUGUI goodHitsText;
        public TextMeshProUGUI badHitsText;
        public TextMeshProUGUI missesText;

        private void OnEnable() {
            Song song;

            if (newHighscoreText) newHighscoreText.gameObject.SetActive(false);

            // Checks if the script is on an object with a SongCard script (i.e. a SongDisplayPanel) and grabs the song from that. 
            try {
                song = GetComponent<SongCard>().song;
            }
            catch {
                // If the ScoreManager or song instance dont exist then return
                if (!ScoreManager.instance || RhythmManager.instance.song == null) return;

                // Otherwise set the song
                song = RhythmManager.instance.song;
            }

            if (song == null) return;

            if (highscoreText) highscoreText.text = "- HighScore -\n".Replace("\\n", "\n") + song.highscore.ToString("0");
            if (scoreText) scoreText.text = "- Score -\n".Replace("\\n", "\n") + ScoreManager.score.ToString("0");
            if (maxComboScoreText) maxComboScoreText.text = "- Max Combo -\n".Replace("\\n", "\n") + song.largestCombo.ToString("0");
            if (ScoreManager.newHighscore && newHighscoreText) newHighscoreText.gameObject.SetActive(false);

            if (ScoreManager.instance) {
                if (perfectHitsText) perfectHitsText.text = "Perfect: " + ScoreManager.perfectHits.ToString("0");
                if (goodHitsText) goodHitsText.text = "Good: " + ScoreManager.goodHits.ToString("0");
                if (badHitsText) badHitsText.text = "Bad: " + ScoreManager.badHits.ToString("0");
                if (missesText) missesText.text = "Miss: " + ScoreManager.missedNotes.ToString("0");
            }
        }
    }
}
