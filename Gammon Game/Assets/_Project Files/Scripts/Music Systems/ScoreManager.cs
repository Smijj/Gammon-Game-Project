using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MusicSystem {
    public class ScoreManager : MonoBehaviour
    {
        static int comboScore;
        public static ScoreManager instance;
        
        public AudioSource perfectHitSFX;
        public AudioSource goodHitSFX;
        public AudioSource badHitSFX;
        public AudioSource missSFX;
        public TextMeshProUGUI comboScoreText;

        public GameObject hitText;
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


        public static void PerfectHit() {
            instance.perfectHitSFX.Play();
            comboScore++;
        }
        public static void GoodHit() {
            instance.goodHitSFX.Play();
            comboScore++;
        }
        public static void BadHit() {
            instance.badHitSFX.Play();
            comboScore = 0;
        }
        public static void Miss() {
            instance.missSFX.Play();
            comboScore = 0;
        }

        private void Update() {
            if (comboScoreText) comboScoreText.text = "- " + comboScore.ToString() + " -";
        }
    }
}
