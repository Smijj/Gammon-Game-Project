using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    namespace Menu {
        public class Page : MonoBehaviour {

            public static readonly string FLAG_ON = "On";
            public static readonly string FLAG_OFF = "Off";
            public static readonly string FLAG_NONE = "None";

            public PageType type;
            public bool debug;
            public bool useAnimation;
            public string targetState { get; private set; }

            private Animator m_Animator;


            #region Unity Functions

            private void OnEnable() {
                CheckAnimatorIntegrity();
            }

            #endregion


            #region Public Functions

            public void Animate(bool _on) {
                if (useAnimation) {
                    m_Animator.SetBool("on", _on);              // Uses the animator object to trigger the on state based on the parameter that comes in.

                    // Use Coroutine to handle animation state
                    StopCoroutine("AwaitAnimation");         // Stop existing Coroutines before starting a new one to avoid multiple running simultaneously 
                    StartCoroutine("AwaitAnimation", _on);
                } else {
                    if (!_on) {
                        gameObject.SetActive(false);
                    }
                }
            }

            #endregion


            #region Private Functions

            private IEnumerator AwaitAnimation(bool _on) {
                targetState = _on ? FLAG_ON : FLAG_OFF;

                // Wait for the animator to reach the target state.
                while (!m_Animator.GetCurrentAnimatorStateInfo(0).IsName(targetState)) {    // While the animators name on layer 0 doesn't equal the name of the targetState then wait.
                    yield return null;
                }

                // Wait for the animator to finish animating
                while (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) {    // While the animator has not finished the animation (normalizedTime < 1, 0 to 1 with 1 being finished) wait.
                    yield return null;
                }

                targetState = FLAG_NONE;

                Log("Page of [" + type+"] finished transitioning to "+(_on ? "on." : "off."));

                if (!_on) {
                    gameObject.SetActive(false);
                }
            }

            private void CheckAnimatorIntegrity() {
                if (useAnimation) {
                    m_Animator = GetComponent<Animator>();
                    if (!m_Animator) {
                        LogWarning("You opted to animate a page ["+type+"], but no Animator component exists on the object.");
                    }
                }
            }

            private void Log(string _msg) {
                if (!debug) return;
                Debug.Log("[Page]: " + _msg);
            } 

            private void LogWarning(string _msg) {
                if (!debug) return;
                Debug.LogWarning("[Page]: " + _msg);
            }

            #endregion

        }
    }



