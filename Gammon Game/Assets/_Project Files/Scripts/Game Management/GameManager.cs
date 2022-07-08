using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace GameManagement {
    using MenuSystem;
    using RestaurantSystems;
    using MusicSystem;
    using InventorySystem;

    public class GameManager : MonoBehaviour {
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

        public static Grid grid;
        public List<Tilemap> tilemaps;

        [HideInInspector]
        public NPCManager npcManager;
        public GameObject rhythmManager;

        public List<ChairEntity> activeChairList = new List<ChairEntity>();

        #region Unity Functions        

        private void Awake() {
            // If there isnt already an instance (following the Singleton pattern) then:
            CheckSingleton();
            npcManager = GetComponent<NPCManager>();

            try {
                grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
            } catch {
                Debug.Log("There is no grid in this scene");
            }

            try {
                GameObject[] tilemapObjs = GameObject.FindGameObjectsWithTag("BaseMap");
                for (int i = 0; i < tilemapObjs.Length; i++) {
                    tilemaps.Add(tilemapObjs[i].GetComponent<Tilemap>());
                }
            } catch (System.Exception e) {
                Debug.Log(e);
            }

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

        public void PlaySong(Song _song) {
            // Instantiate a SongManager
            Instantiate(rhythmManager).GetComponent<RhythmManager>().InitSong(_song);
            PageManager.singleton.TurnPageOff(PageManager.activePage);
            if (!isPaused) PauseGame();
        }

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
