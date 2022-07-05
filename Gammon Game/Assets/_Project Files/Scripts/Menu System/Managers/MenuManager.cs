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

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        #endregion


        #region Main Menu

        public void MainMenu_Play()
        {
            try
            {
                SceneManager.LoadScene("Play");
            }
            catch (Exception e)
            {
                Debug.LogError("[MenuSceneManager]: Failed to advance scene in 'Play' function. Are the scenes configured correctly in the build settings? \nReason:\n" + e.Message);
            }
        }

        public void MainMenu_Recipes()
        {
            PageManager.singleton.TurnPageOn(PageType.Recipes);
        }
        public void MainMenu_Settings()
        {
            PageManager.singleton.TurnPageOn(PageType.Settings);
        }
        public void MainMenu_Songs()
        {
            PageManager.singleton.TurnPageOn(PageType.Songs);
        }


        #endregion


        #region Menu Buttons        

        public void PauseMenu_Recipes() {
            PageManager.singleton.TurnPageOff(PageType.PauseMenu, true, PageType.Recipes);
        }

        public void PauseMenu_Settings() {
            PageManager.singleton.TurnPageOff(PageType.PauseMenu, true, PageType.Settings);
        }

        public void PauseMenu_Stats() {
            PageManager.singleton.TurnPageOff(PageType.PauseMenu, true, PageType.Stats);
        }

        public void PauseMenu_Contacts() {
            PageManager.singleton.TurnPageOff(PageType.PauseMenu, true, PageType.Settings);
        }

        public void PauseMenu_Map() {
            PageManager.singleton.TurnPageOff(PageType.PauseMenu, true, PageType.Map);
        }

        public void PauseMenu_Save() {
            // Save
        }
        
        public void PauseMenu_Songs() {
            PageManager.singleton.TurnPageOff(PageType.PauseMenu, true, PageType.Songs);
        }

        public void PauseMenu_Back() {
            PageManager.singleton.TurnPageOff(PageManager.activePage, true, PageType.PauseMenu);
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
