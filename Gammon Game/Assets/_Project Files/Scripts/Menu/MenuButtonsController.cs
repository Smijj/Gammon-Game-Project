using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu {
    using GameManagement;
    public class MenuButtonsController : MonoBehaviour
    {

        #region Menu Buttons

        public void Play() {
            try {
                SceneManager.LoadScene("Tutorial");
            }
            catch (Exception e) {
                Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'Play' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
            }
        }

        public void CloseMenu() {
            PageController.singleton.TurnPageOff(PageController.activePage);
        }

        /*public void Controls() {
            if (GameManager.currentScene.name == "Main Menu") {
                PageController.singleton.TurnPageOn(PageType.Controls);
            } else { 
                PageController.singleton.TurnPageOff(PageType.Menu, PageType.Controls);
            }
        }*/

        public void Settings() {
            if (GameManager.currentScene.name == "Main Menu") {
                PageController.singleton.TurnPageOn(PageType.Settings);
            } else {
                PageController.singleton.TurnPageOff(PageType.Menu, PageType.Settings);
            }
        }
        
        /*public void Credits() {
            if (GameManager.currentScene.name == "Main Menu") {
                PageController.singleton.TurnPageOn(PageType.Credits);
            }
            else { 
                PageController.singleton.TurnPageOff(PageType.Menu, PageType.Credits);
            }
        }

        public void Stats() {
            if (GameManager.currentScene.name == "Main Menu") {
                PageController.singleton.TurnPageOn(PageType.OverallStats);
            }
            else { 
                PageController.singleton.TurnPageOff(PageType.Menu, PageType.OverallStats);
            }

        }*/

        public void Back() {
            if (GameManager.currentScene.name == "Main Menu") {
                CloseMenu();
            } 
            else {
                PageController.singleton.TurnPageOff(PageController.activePage, PageType.Menu);
            }
        }



        public void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        #endregion


        #region GameOver stuff

        public void PlayAgain() {
            try {
                SceneManager.LoadScene("Play");
            }
            catch (Exception e) {
                Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'PlayAgain' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
            }
        }

        public void MainMenu() {
            try {
                SceneManager.LoadScene("Main Menu");
            }
            catch (Exception e) {
                Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'MainMenu' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
            }
        }

        #endregion
    }
}
