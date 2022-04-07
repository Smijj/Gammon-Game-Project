using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    using Menu;

    public class TestData : MonoBehaviour
    {
        public DataController data;
        public PageController page;

        #region Unity Functions

#if UNITY_EDITOR
        private void Update() {
            if (Input.GetKeyDown(KeyCode.R)) {
                TestAddScore(1);
            }
            
            if (Input.GetKeyDown(KeyCode.T)) {
                TestAddScore(-1);
            }
            
            if (Input.GetKeyDown(KeyCode.Space)) {
                TestResetScore();
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                page.TurnPageOn(PageType.Loading);
            }

        }
#endif

        #endregion


        #region Private Functions
        
        private void TestAddScore(int _score) {
            data.score += _score;
            Log("Score: " + data.score + " | Highscore: " + data.highscore);
        }

        private void TestResetScore() {
            data.score = 0;
            Log("Score: " + data.score + " | Highscore: " + data.highscore);
        }

        private void Log(string _msg) {
            Debug.Log("[TestData] " + _msg);
        }

        #endregion
    }
}
