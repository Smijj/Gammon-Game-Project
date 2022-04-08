using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu {
    using GameManagement;

    public class MenuManager : MonoBehaviour
    {

        #region General

        public void General_ClosePage() {
            PageController.singleton.TurnPageOff(PageController.activePage);
        }

        #endregion


        #region Main Menu

        /*public void Play() {
            try {
                SceneManager.LoadScene("Tutorial");
            }
            catch (Exception e) {
                Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'Play' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
            }
        }*/


        public void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        #endregion


        #region Menu Buttons        

        public void Menu_Meals() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Meals);
        }

        public void Menu_Planner() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Planner);
        }

        public void Menu_Recipes() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Recipes);
        }

        public void Menu_Settings() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Settings);
        }
        
        public void Menu_Orders() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Orders);
        }

        public void Menu_Stats() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Stats);
        }

        public void Menu_Contacts() {
            PageController.singleton.TurnPageOff(PageType.Menu, PageType.Settings);
        }

        public void Menu_Save() {
            // Save
        }

        public void Menu_Back() {
            PageController.singleton.TurnPageOff(PageController.activePage, PageType.Menu);
        }

        #endregion
    }
}
