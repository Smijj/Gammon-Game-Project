using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MusicSystem {
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;
        static int comboScore;
        static int multiplier = 1;
        static double score;
        
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
        public Transform lastScoreTextPos;

        [Header("Score Settings: ")]
        public int maxMulitplier = 6;
        public float perfectScore = 15;
        public float goodScore = 11;

        public string perfectHitText;
        public string goodHitText;
        public string badHitText;
        public string missText;


        private void Start() {
            instance = this;
            comboScore = 0;
        }
        private void Update() {
            if (scoreText) scoreText.text = score.ToString("0");
            if (comboScoreText) comboScoreText.text = comboScore.ToString();
            if (multiplierText) multiplierText.text = $"x{multiplier}";
        }


        public static void PerfectHit(Transform _pos, double _disFromPerfect) {
            _disFromPerfect = Math.Abs(_disFromPerfect * 100);
            comboScore++;
            if(multiplier < instance.maxMulitplier) multiplier++;
            double hitScore = instance.perfectScore - _disFromPerfect;
            score += hitScore * multiplier;

            instance.InstantiateHitText(hitScore.ToString("0") + " | " + instance.perfectHitText, instance.hitText, _pos);
            instance.InstantiateHitText("+ " + (hitScore * multiplier).ToString("0"), instance.lastScoreText, instance.lastScoreTextPos);
            //"+ " + hitScore.ToString("0") + $" x{multiplier}"
            instance.perfectHitSFX.Play();
        }
        public static void GoodHit(Transform _pos, double _disFromPerfect) {
            _disFromPerfect = Math.Abs(_disFromPerfect * 100);
            comboScore++;
            if(multiplier < instance.maxMulitplier) multiplier++;   // Might have it so Good hits dont add to the multiplier but dont take away from it either
            double hitScore = instance.goodScore - _disFromPerfect;
            score += hitScore * multiplier;

            instance.InstantiateHitText(hitScore.ToString("0") + " | " + instance.goodHitText, instance.hitText, _pos);
            instance.InstantiateHitText("+ " + (hitScore * multiplier).ToString("0"), instance.lastScoreText, instance.lastScoreTextPos);
            instance.goodHitSFX.Play();
        }
        public static void BadHit(Transform _pos) {
            comboScore = 0;
            multiplier = 1;

            instance.InstantiateHitText("0 | " + instance.badHitText, instance.hitText, _pos);
            instance.badHitSFX.Play();
        }
        public static void Miss(Transform _pos) {
            comboScore = 0;
            multiplier = 1;

            instance.InstantiateHitText("0 | " + instance.missText, instance.hitText,  _pos);
            instance.missSFX.Play();
        }


        private void InstantiateHitText(string _text, GameObject _prefab, Transform _pos) {
            Instantiate(_prefab, _pos).GetComponent<TextMeshPro>().text = _text;
        }
    }
}
