using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MusicSystem {
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;
        public AudioSource hitSFX;
        public AudioSource missSFX;
        public TextMeshProUGUI comboScoreText;
        static int comboScore;


        private void Start() {
            instance = this;
            comboScore = 0;
        }

        public static void Hit() {
            instance.hitSFX.Play();
            comboScore++;
        }
        public static void Miss() {
            instance.missSFX.Play();
            comboScore = 0;
        }

        private void Update() {
            if (comboScoreText) comboScoreText.text = comboScore.ToString();
        }
    }
}
