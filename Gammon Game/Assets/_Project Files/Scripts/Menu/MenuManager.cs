using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu {
    using GameManagement;

    public class MenuManager : MonoBehaviour
    {
        #region Menu Buttons

        /*public void Play() {
            try {
                SceneManager.LoadScene("Tutorial");
            }
            catch (Exception e) {
                Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'Play' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
            }
        }*/

        public void CloseMenu() {
            PageController.singleton.TurnPageOff(PageController.activePage);
        }

        public void Settings() {
            if (GameManager.currentScene.name == "Main Menu") {
                PageController.singleton.TurnPageOn(PageType.Settings);
            } else {
                PageController.singleton.TurnPageOff(PageType.Menu, PageType.Settings);
            }
        }

        public void Stats() {
            if (GameManager.currentScene.name == "Main Menu") {
                PageController.singleton.TurnPageOn(PageType.Stats);
            }
            else { 
                PageController.singleton.TurnPageOff(PageType.Menu, PageType.Stats);
            }

        }

        public void Back() {
            if (GameManager.currentScene.name == "Main Menu") {
                CloseMenu();
            } else {
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
    }
}
