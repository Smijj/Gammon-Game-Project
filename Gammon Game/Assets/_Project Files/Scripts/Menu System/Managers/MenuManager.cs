using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuSystem {
    using GameManagement;
    using MusicSystem;
    using InventorySystem;

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

        public void Menu_Recipes() {
            PageManager.singleton.TurnPageOff(PageType.Menu, true, PageType.Recipes);
        }

        public void Menu_Settings() {
            PageManager.singleton.TurnPageOff(PageType.Menu, true, PageType.Settings);
        }

        public void Menu_Stats() {
            PageManager.singleton.TurnPageOff(PageType.Menu, true, PageType.Stats);
        }

        public void Menu_Contacts() {
            PageManager.singleton.TurnPageOff(PageType.Menu, true, PageType.Settings);
        }

        public void Menu_Map() {
            PageManager.singleton.TurnPageOff(PageType.Menu, true, PageType.Map);
        }

        public void Menu_Save() {
            // Save
        }
        
        public void Menu_Songs() {
            PageManager.singleton.TurnPageOff(PageType.Menu, true, PageType.Songs);
        }

        public void Menu_Back() {
            PageManager.singleton.TurnPageOff(PageManager.activePage, true, PageType.Menu);
        }

        #endregion


        #region RhythmGame

        public void RhythmGame_Close() {
            General_ClosePage();

            if (RhythmManager.instance) Destroy(RhythmManager.instance.gameObject);
        }

        public void RhythmGame_Resume() {
            PageManager.singleton.TurnPageOn(PageType.SongPause, false, false); // Will close the song pause page without unpausing the game
            if (RhythmManager.instance) {
                RhythmManager.instance.PauseSong(false);
            }
        }

        public void RhythmGame_GiveUp() {
            General_ClosePage();
            if (RhythmManager.instance) {
                RhythmManager.instance.FinishSong();
            }
        }
        public void RhythmGame_Restart() {
            General_ClosePage();
            if (RhythmManager.instance) {
                Song song = RhythmManager.instance.song;
                Destroy(RhythmManager.instance.gameObject);
                GameManager.singleton.PlaySong(song);
            }
        }


        #endregion
    }
}
