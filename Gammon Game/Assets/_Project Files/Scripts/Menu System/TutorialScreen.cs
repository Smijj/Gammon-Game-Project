using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreen : MonoBehaviour
{
    public void Continue() {
        try {
            SceneManager.LoadScene("Play");
        }
        catch (Exception e) {
            Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'Continue' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
        }
    }

    public void Back() {
        try {
            SceneManager.LoadScene("Main Menu");
        }
        catch (Exception e) {
            Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'Back' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
        }
    }
}
