using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement {
    public class DataManager : MonoBehaviour
    {
        private static readonly string DATA_SCORE = "score";
        private static readonly string DATA_HIGHSCORE = "highscore";
        private static readonly int DEFAULT_INT = 0;

        #region Properties

        public int score {
            get {
                return GetInt(DATA_SCORE);
            }
            set {
                SaveInt(DATA_SCORE, value);
                int _score = this.score;
                if (_score > this.highscore)
                    this.highscore = _score;
            }
        }
        public int highscore {
            get {
                return GetInt(DATA_HIGHSCORE);
            }
            private set {
                SaveInt(DATA_HIGHSCORE, value);
            }
        }

        #endregion


        #region Private Functions

        private void SaveInt(string _key, int _value) {
            PlayerPrefs.SetInt(_key, _value);
        }

        private int GetInt(string _key) {
            return PlayerPrefs.GetInt(_key, DEFAULT_INT);
        }

        #endregion
    }
}
