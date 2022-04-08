using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu {
    using GameManagement;
    public enum PageType {
        None,
        Loading,
        Menu,
        Meals,
        Planner,
        Recipes,
        Settings,
        Orders,
        Stats,
        Contacts,
    }

    public class PageController : MonoBehaviour
    {
		#region Singleton
		public static PageController singleton;
		private void CheckSingleton() {
			if (!singleton) {
				singleton = this;
				m_Pages = new Hashtable();
				RegisterAllPages();
				if (enteryPage != PageType.None) {
					TurnPageOn(enteryPage);
				}
                menuManager = GetComponent<MenuManager>();
				DontDestroyOnLoad(this);
			} else {
				Destroy(this);
			}
		}
		#endregion

		public bool debug;
		public PageType enteryPage;
		public Page[] pages;
		[SerializeField]
		public static bool pageIsActive { get; private set; }
		public static PageType activePage;

		private Hashtable m_Pages;

        [HideInInspector]
        public MenuManager menuManager;


		#region Unity Function

		private void OnEnable() {
			CheckSingleton();
		}


		private void Update() {
            if (!GameManager.isLoading) {
                if (Input.GetKeyDown(KeyCode.Escape) && GameManager.currentScene.name == "Play") {
                    if (activePage == PageType.None || activePage == PageType.Menu)
                        TurnPageOn(PageType.Menu);
                    else
                        TurnPageOff(activePage, PageType.Menu);

                } else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.currentScene.name == "Main Menu") {
                    TurnPageOff(activePage);
                }
            }
		}

        #endregion


        #region Public Functions

        /// <summary>
        /// Turns on a page of PageType _type. If that PageType is already active it will toggle it off.
        /// </summary>
        /// <param name="_type"></param>
        public void TurnPageOn(PageType _type, bool _pauseGame = true) {
            if (_type == PageType.None) return;     // Checks the page isnt null
            if (!PageExists(_type)) {                // Makes sure the page type exists
                LogWarning("You are trying to turn a page on [" + _type + "] that has not been registered.");
                return;
            }

            Page _page = GetPage(_type);
            if (!_page.gameObject.activeSelf && !pageIsActive) {     // If the page isn't already on:
                _page.gameObject.SetActive(true);           // Set the page gameobject to active so it can be seen
                _page.Animate(true);   // If the page as elected to have animation then it will run the animation

                pageIsActive = true;                        // bool that defines whether there is a page already open or not.
                activePage = _type;

                if (_pauseGame && !GameManager.isPaused && GameManager.currentScene.name != "Main Menu")  // pause the game.
                    GameManager.PauseGame();
            
            } else if (_page.gameObject.activeSelf) {     // If the page was already on:
                TurnPageOff(_type);                         // Turn the page off
            }
        }


        public void TurnPageOff(PageType _off, PageType _on = PageType.None, bool _waitForExit = false) {
            if (_off == PageType.None) return;      // Checks to see if the incoming page is of type none
            if (!PageExists(_off)) {                // Makes sure the page type exists
                LogWarning("You are trying to turn a page off [" + _off + "] that has not been registered.");
                return;
            }

            Page _offPage = GetPage(_off);
            if (_offPage.gameObject.activeSelf) {    // If the page game object is active
                _offPage.Animate(false);                // set the page to animate out (off)

                pageIsActive = false;                   // bool that defines whether there is a page already open or not.
                activePage = PageType.None;

                if (GameManager.isPaused && GameManager.currentScene.name != "Main Menu")   // unpause the game.
                    GameManager.UnpauseGame();
            }

            if (_on != PageType.None) {
                pageIsActive = false;                   // This needs to get set to false so that another page can be turned on.
                activePage = PageType.None;
                Page _onPage = GetPage(_on);
                if (_waitForExit) {
                    //StopCoroutine("WaitForPageExit");       // Stop existing Coroutines before starting a new one to avoid multiple running simultaneously
                    StartCoroutine(WaitForPageExit(_onPage, _offPage));
                } else {
                    TurnPageOn(_on);
                }
            }
        }

        #endregion


        #region Private Functions

        /// <summary>
        /// Waits for the _off page to return type None (meaning it is off), then activates the _on page.
        /// </summary>
        /// <param name="_on">The Page that is going to be turned on.</param>
        /// <param name="_off">The Page that is being turned off.</param>
        /// <returns>IEnumerator</returns>
        private IEnumerator WaitForPageExit(Page _on, Page _off) {
            while (_off.targetState != Page.FLAG_NONE) {    // If the _off page is animating, wait for it to finish.
                yield return null;
            }

            TurnPageOn(_on.type);
        }

        private void RegisterAllPages() {
            foreach (Page _page in pages) {
                RegisterPage(_page);
            }
        }

        private void RegisterPage(Page _page) {
            if (PageExists(_page.type)) {                   // Check if the page already exists.
                LogWarning("You are trying to register a page [" + _page.type + "] that has already been registered: " + _page.gameObject.name);
                return;
            }

            m_Pages.Add(_page.type, _page);                 // Add new page to the m_Pages hashtable
            Log("Registered new page [" + _page.type + "]");    // Log this operation
        }

        private Page GetPage(PageType _type) {
            if (!PageExists(_type)) {
                LogWarning("You are trying to get a page [" + _type + "] that has not been registered.");
                return null;
            }

            return (Page)m_Pages[_type];    // Returning a page from the hashtable with the key of _type. 
                                            // Needs to be casted to a specific type (Page) as hashtables are a generic collection. A key is not enough the compiler needs to know the type as well.
        }

        private bool PageExists(PageType _type) {
            return m_Pages.ContainsKey(_type);
        }

        private void Log(string _msg) {
            if (!debug) return;
            Debug.Log("[PageController]: " + _msg);
        }

        private void LogWarning(string _msg) {
            if (!debug) return;
            Debug.LogWarning("[PageController]: " + _msg);
        }

        #endregion

    }
}
