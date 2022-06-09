using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameManagement;
using MenuSystem;

public class TestingScript: MonoBehaviour
{
    public DataManager data;
    public PageManager page;
    public StatsManager stats;

    #region Unity Functions

#if UNITY_EDITOR
    private void Update() {
        // testing player prefs
        if (Input.GetKeyDown(KeyCode.R)) {
            TestAddScore(1);
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            TestAddScore(-1);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            TestResetScore();
        }

        // Testing page system
        if (Input.GetKeyDown(KeyCode.L)) {
            page.TurnPageOn(PageType.Loading);
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            page.TurnPageOn(PageType.Songs);
        }

        // Testing Stats exp system
        if (Input.GetKeyDown(KeyCode.M)) {
            stats.AddExp("Fitness", 15);
            Debug.Log("Adding 15 Exp.");
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            stats.AddExp("Fitness", -15);
            Debug.Log("Subtracting 15 Exp.");
        }

        // Testing Pausing
        if (Input.GetKeyDown(KeyCode.V)) {
            GameManager.PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            GameManager.UnpauseGame();
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

