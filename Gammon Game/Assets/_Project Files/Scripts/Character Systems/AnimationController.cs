using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems {

    // Animation States
    public enum AnimState {
        Idle_N,
        Idle_NE,
        Idle_E,
        Idle_SE,
        Idle_S,
        Idle_SW,
        Idle_W,
        Idle_NW,

        Walk_N,
        Walk_NE,
        Walk_E,
        Walk_SE,
        Walk_S,
        Walk_SW,
        Walk_W,
        Walk_NW,
    }

    public class AnimationController : MonoBehaviour
    {
        private Animator animator;
        private MoveController moveController;
        private AnimState currentState;

        private void Start() {
            animator = GetComponent<Animator>();
            moveController = GetComponent<MoveController>();
        }

        private void FixedUpdate() {
            if (!moveController.isMoving) {
                switch (moveController.currentFacing) {
                    default:
                        ChangeAnimState(currentState);
                        break;
                    case CharFacing.N:
                        ChangeAnimState(AnimState.Idle_N);
                        break;
                    case CharFacing.NE:
                        ChangeAnimState(AnimState.Idle_NE);
                        break;
                    case CharFacing.E:
                        ChangeAnimState(AnimState.Idle_E);
                        break;
                    case CharFacing.SE:
                        ChangeAnimState(AnimState.Idle_SE);
                        break;
                    case CharFacing.S:
                        ChangeAnimState(AnimState.Idle_S);
                        break;
                    case CharFacing.SW:
                        ChangeAnimState(AnimState.Idle_SW);
                        break;
                    case CharFacing.W:
                        ChangeAnimState(AnimState.Idle_W);
                        break;
                    case CharFacing.NW:
                        ChangeAnimState(AnimState.Idle_NW);
                        break;
                }
            } else {
                switch (moveController.currentFacing) {
                    //default:
                    //    ChangeAnimState(AnimState.Walk_S);
                    //    break;
                    case CharFacing.N:
                        ChangeAnimState(AnimState.Walk_N);
                        break;
                    case CharFacing.NE:
                        ChangeAnimState(AnimState.Walk_NE);
                        break;
                    case CharFacing.E:
                        ChangeAnimState(AnimState.Walk_E);
                        break;
                    case CharFacing.SE:
                        ChangeAnimState(AnimState.Walk_SE);
                        break;
                    case CharFacing.S:
                        ChangeAnimState(AnimState.Walk_S);
                        break;
                    case CharFacing.SW:
                        ChangeAnimState(AnimState.Walk_SW);
                        break;
                    case CharFacing.W:
                        ChangeAnimState(AnimState.Walk_W);
                        break;
                    case CharFacing.NW:
                        ChangeAnimState(AnimState.Walk_NW);
                        break;
                }
            }
        }

        public void ChangeAnimState(AnimState _newState) {

            if (animator == null) return;

            // stops anim from interupting itself
            if (currentState == _newState) return;
            // reassign the current state
            currentState = _newState;

            try {
                // play animation
                animator.Play(_newState.ToString());
            }
            catch (System.Exception) {
                Debug.LogWarning($"[AnimationController] There is no animation clip corresponding to {_newState} in [CharacterStats.name]'s Animator Controller.");
            }   
            
        }
    }
}
