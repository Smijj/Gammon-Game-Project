using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems {
    public class PlayerController : MonoBehaviour
    {
        private MoveController move;

        [SerializeField]
        private Vector2 input;

        //public Animator anim;
        private Camera cam;

        [SerializeField]
        private bool enableMouseMovement = false;
        [SerializeField]
        private bool moveOnMouseRelease = true;


        private void Start() {
            cam = Camera.main;
            move = GetComponent<MoveController>();
        }

        private void Update() {

            if (enableMouseMovement) {
                // Get input for mouse movement
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                    if (moveOnMouseRelease) move.AutoPath(cam.ScreenToWorldPoint(Input.mousePosition), true, true);
                    else move.AutoPath(cam.ScreenToWorldPoint(Input.mousePosition), true);
                }
                if (moveOnMouseRelease && Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                    move.SetPathing(true);
                }
            }

            // get input for WASD movement
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (!move.isMoving) {
                if (move.useDiagonalMovement) {
                    if (Mathf.Abs(input.normalized.magnitude) > 0) {
                        move.IncrementPosition(input);
                    }
                } else {
                    // Moving on the x-axis
                    if (Mathf.Abs(input.x) == 1f) {
                        move.IncrementXPosition(input.x);
                    }
                    // Moving on the y-axis
                    else if (Mathf.Abs(input.y) == 1f) {
                        move.IncrementYPosition(input.y);
                    }
                }
            }else {
                // Cancels pathing if there is any input
                if (Mathf.Abs(input.normalized.magnitude) > 0) {
                    move.SetPathing(false);
                }
            }
        }
    }
}
