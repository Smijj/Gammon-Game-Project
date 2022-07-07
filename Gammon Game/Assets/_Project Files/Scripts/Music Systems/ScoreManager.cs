using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MusicSystem {
    public class ScoreManager : MonoBehaviour {
        public static ScoreManager instance;
        public static double score;
        public static int largestCombo = 0;
        static int comboScore;
        static int multiplier = 1;
        static public int perfectHits = 0;
        static public int goodHits = 0;
        static public int badHits = 0;
        static public int missedNotes = 0;
        static public bool newHighscore = false;

        [Header("Audio Refs: ")]
        public AudioSource perfectHitSFX;
        public AudioSource goodHitSFX;
        public AudioSource badHitSFX;
        public AudioSource missSFX;

        [Header("Object Refs: ")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI comboScoreText;
        public TextMeshProUGUI multiplierText;
        public GameObject hitText;
        public GameObject lastScoreText;
        public Transform hitTextPos;
        public Transform lastScoreTextPos;

        [Header("Score Settings: ")]
        public int maxMulitplier = 6;
        public float perfectScore = 15;
        public float goodScore = 11;

        public string perfectHitText;
        public string goodHitText;
        public string badHitText;
        public string missText;
        public string finishHoldNoteText;


        private void Awake() {
            instance = this;
            comboScore = 0;
        }
        private void Update() {
            if (scoreText) scoreText.text = score.ToString("0");
            if (comboScoreText) comboScoreText.text = comboScore.ToString();
            if (multiplierText) multiplierText.text = $"x{multiplier}";
        }


        #region Public Functions

        public void CalculateAndSaveSongStats() {
            RhythmManager rm = RhythmManager.instance;
            if (rm.song == null) return;

            // Sets highscore and other stats
            if (score > rm.song.highscore) {
                rm.song.highscore = score;
                newHighscore = true;
            }
            if (largestCombo > rm.song.largestCombo) rm.song.largestCombo = largestCombo;
        }

        public static void ResetStatistics() {
            score = 0;
            largestCombo = 0;
            comboScore = 0;
            multiplier = 1;
            perfectHits = 0;
            goodHits = 0;
            badHits = 0;
            missedNotes = 0;
            newHighscore = false;
        }

        public static void CaclulateHoldScore(double _noteLength) {
            if (multiplier < instance.maxMulitplier) multiplier++;

            // Score caluclations
            double hitScore = instance.perfectScore * _noteLength * multiplier;
            score += hitScore;

            // Displaying Score
            instance.InstantiateHitText(/*hitScore.ToString("0") + " | " +*/ instance.finishHoldNoteText, instance.hitText, instance.hitTextPos);
            instance.InstantiateHitText("+ " + hitScore.ToString("0"), instance.lastScoreText, instance.lastScoreTextPos);
            instance.goodHitSFX.Play();
        }

        public static void PerfectHit(double _disFromPerfect) {
            perfectHits++;
            IncrementComboScore();
            if (multiplier < instance.maxMulitplier) multiplier++;

            // Score caluclations
            double hitScore = GetHitScore(_disFromPerfect, multiplier);
            score += hitScore;

            // Displaying Score
            instance.InstantiateHitText(/*hitScore.ToString("0") + " | " +*/ instance.perfectHitText, instance.hitText, instance.hitTextPos);
            instance.InstantiateHitText("+ " + hitScore.ToString("0"), instance.lastScoreText, instance.lastScoreTextPos);
            //"+ " + hitScore.ToString("0") + $" x{multiplier}"
            instance.perfectHitSFX.Play();
        }

        public static void GoodHit(double _disFromPerfect) {
            goodHits++;
            IncrementComboScore();
            if (multiplier < instance.maxMulitplier) multiplier++;   // Might have it so Good hits dont add to the multiplier but dont take away from it either

            // Score caluclations
            double hitScore = GetHitScore(_disFromPerfect, multiplier);
            score += hitScore;

            // Displaying Score
            instance.InstantiateHitText(/*hitScore.ToString("0") + " | " + */instance.goodHitText, instance.hitText, instance.hitTextPos);
            instance.InstantiateHitText("+ " + hitScore.ToString("0"), instance.lastScoreText, instance.lastScoreTextPos);
            instance.goodHitSFX.Play();
        }

        public static void BadHit() {
            badHits++;
            comboScore = 0;
            multiplier = 1;

            // Displaying Score
            instance.InstantiateHitText(/*"0 | " + */instance.badHitText, instance.hitText, instance.hitTextPos);
            instance.badHitSFX.Play();
        }
        public static void Miss() {
            missedNotes++;
            comboScore = 0;
            multiplier = 1;

            // Displaying Score
            instance.InstantiateHitText(/*"0 | " + */instance.missText, instance.hitText, instance.hitTextPos);
            instance.missSFX.Play();
        }

        #endregion


        #region Private Functions

        private static double GetHitScore(double _disFromPerfect, int _multiplier) {
            double _percentageOfPerfect = 1 - Math.Abs(_disFromPerfect) / RhythmManager.instance.noteFallTime;
            return instance.perfectScore * _percentageOfPerfect * _multiplier;
        }

        private static void IncrementComboScore() {
            comboScore++;
            if (comboScore > largestCombo) largestCombo = comboScore;
        }

        private void InstantiateHitText(string _text, GameObject _prefab, Transform _pos) {
            Instantiate(_prefab, _pos).GetComponent<TextMeshProUGUI>().text = _text;
        }

        #endregion
    }
}
