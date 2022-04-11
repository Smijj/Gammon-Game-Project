using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameManagement;
using MenuSystem;

public class TestingScript: MonoBehaviour
{
    public DataManager data;
    public PageManager page;

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
        if (Input.GetKeyDown(KeyCode.P)) {
            page.TurnPageOn(PageType.Menu);
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

