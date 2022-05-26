using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MusicSystem {
    public class ScoreManager : MonoBehaviour
    {
        static int comboScore;
        static int multiplier;
        static double score;
        public static ScoreManager instance;
        
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



        public static void InstantiateHitText(string _text, Transform _pos) {
            Instantiate(instance.hitText, _pos).GetComponent<TextMeshPro>().text = _text;
        }


        public static void PerfectHit(double _disFromPerfect) {
            _disFromPerfect = Math.Abs(_disFromPerfect * 100);
            comboScore++;
            if(multiplier < instance.maxMulitplier) multiplier++;
            score += (instance.perfectScore - _disFromPerfect) * multiplier;

            instance.perfectHitSFX.Play();
        }
        public static void GoodHit(double _disFromPerfect) {
            _disFromPerfect = Math.Abs(_disFromPerfect * 100);
            comboScore++;
            if(multiplier < instance.maxMulitplier) multiplier++;   // Might have it so Good hits dont add to the multiplier but dont take away from it either
            score += (instance.goodScore - _disFromPerfect) * multiplier;

            instance.goodHitSFX.Play();
        }
        public static void BadHit() {
            comboScore = 0;
            multiplier = 1;

            instance.badHitSFX.Play();
        }
        public static void Miss() {
            comboScore = 0;
            multiplier = 1;
            
            instance.missSFX.Play();
        }

        private void Update() {
            if (scoreText) scoreText.text = score.ToString("0");
            if (comboScoreText) comboScoreText.text = "- " + comboScore.ToString() + " -";
            if (multiplierText) multiplierText.text = "x" + multiplier.ToString();
        }
    }
}
