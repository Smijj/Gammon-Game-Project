using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Menu {
    using GameManagement;
    public class StatsMenu : MonoBehaviour
    {
        [Header("Overall Summary")]
        public TextMeshProUGUI lastScoreText;
        public TextMeshProUGUI highScoreText_Overall;
        public TextMeshProUGUI MostGiftsText;
        public TextMeshProUGUI MostEnemiesStunnedText;
        public TextMeshProUGUI MostPickupsCollectedText;
        public TextMeshProUGUI TotalDamageText;
        public TextMeshProUGUI TotalTimeSlowedText;

        [Header("Game Summary")]
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI highScoreText_Game;
        public TextMeshProUGUI GiftsDeliveredText;
        public TextMeshProUGUI EnemiesStunnedText;
        public TextMeshProUGUI PickupsCollectedText;
        public TextMeshProUGUI DamageTakenText;
        public TextMeshProUGUI TimeSlowedText;

        /*public void UpdateStatsMenuValues(GameStats _stats) {
            lastScoreText.text = _stats.lastScore.ToString("0");
            highScoreText_Overall.text = _stats.highScore.ToString("0");
            MostGiftsText.text = _stats.mostGiftsDelivered.ToString("0");
            MostEnemiesStunnedText.text = _stats.mostEnemiesStunned.ToString("0");
            MostPickupsCollectedText.text = _stats.mostPickupsCollected.ToString("0");
            TotalDamageText.text = _stats.totalDamageTaken.ToString("0");
            TotalTimeSlowedText.text = _stats.totalTimeSlowed.ToString("0");

            ScoreText.text = _stats.lastScore.ToString("0");
            highScoreText_Game.text = _stats.highScore.ToString("0");
            GiftsDeliveredText.text = _stats.giftsDelivered.ToString("0");
            EnemiesStunnedText.text = _stats.enemiesStunned.ToString("0");
            PickupsCollectedText.text = _stats.pickupsCollected.ToString("0");
            DamageTakenText.text = _stats.damageTaken.ToString("0");
            TimeSlowedText.text = _stats.timeSlowed.ToString("0");
        }*/

    }
}
