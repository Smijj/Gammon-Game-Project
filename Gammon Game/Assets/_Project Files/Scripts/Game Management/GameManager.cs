using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement {
    using MenuSystem;

    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager singleton;
        private void CheckSingleton() {
            if (!singleton) {
                singleton = this;
                DontDestroyOnLoad(this);
            } else {
                Destroy(this.gameObject);
            }
        }
        #endregion

        public static bool isPaused = false;
        public static bool isLoading = false;
        public static Scene currentScene;

        #region Unity Functions

        private void Awake() {
            // If there isnt already an instance (following the Singleton pattern) then:
            CheckSingleton();
        }

        #region Scene Management
        private void OnEnable() {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        private void OnDisable() {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled.
            //Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
        private void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _mode) {
            PageManager.singleton.TurnPageOff(PageManager.activePage);

            //if (_scene.name == "Play") InitGame();
            currentScene = _scene;
        }
        #endregion

        #endregion

        #region Public Functions

        public static void PauseGame() {
            isPaused = true;
            Time.timeScale = 0;
            Debug.Log("Pause Game");
        }

        public static void UnpauseGame() {
            isPaused = false;
            Time.timeScale = 1;
            Debug.Log("Unpause Game");
        }

        #endregion
    }
}
