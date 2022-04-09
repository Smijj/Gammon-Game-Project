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
            PageManager.singleton.TurnPageOff(PageManager.activePage);
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
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Meals);
        }

        public void Menu_Planner() {
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Planner);
        }

        public void Menu_Recipes() {
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Recipes);
        }

        public void Menu_Settings() {
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Settings);
        }
        
        public void Menu_Orders() {
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Orders);
        }

        public void Menu_Stats() {
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Stats);
        }

        public void Menu_Contacts() {
            PageManager.singleton.TurnPageOff(PageType.Menu, PageType.Settings);
        }

        public void Menu_Save() {
            // Save
        }

        public void Menu_Back() {
            PageManager.singleton.TurnPageOff(PageManager.activePage, PageType.Menu);
        }

        #endregion
    }
}
