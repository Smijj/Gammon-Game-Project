using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class PlayerController : MonoBehaviour
    {
        private Movement movement;

        [HideInInspector]
        public Vector2 input;
        private Vector2 lastInput;

        public Animator anim;


        private void Start() {
            movement = GetComponent<Movement>();
        }

        private void Update() {

            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");


            if (!movement.isMoving) {
                if (lastInput.x != 0) lastInput.x = 0;
                if (lastInput.y != 0) lastInput.y = 0;
                
                // Moving on the x-axis
                if (Mathf.Abs(input.x) == 1f) {
                    movement.IncrementXPosition(input.x);
                    lastInput.x = input.x;
                }
                // Moving on the y-axis
                else if (Mathf.Abs(input.y) == 1f) {
                    movement.IncrementYPosition(input.y);
                    lastInput.y = input.y;
                }
            }
            else {
                movement.Move();

                // For cancelling a movement operation. Since the player cannot input anything while the object is
                // moving to the target position, i added these 2 if statements to cancel that movement and return
                // the player object to the last position it was at.
                // e.g. the player moves to the right but presses the left key while their moving to move back.
                // lastInput.x = 1, input.x = -1, thus the if statement triggers and the targetPoint becomes the lastPoint.
                if (lastInput.x != 0 && input.x != 0 && input.x != lastInput.x) {
                    movement.SetTargetPosition(movement.lastPoint);
                } 
                else if (lastInput.y != 0 && input.y != 0 && input.y != lastInput.y) {
                    movement.SetTargetPosition(movement.lastPoint);
                }
            }
        }
    }
}
