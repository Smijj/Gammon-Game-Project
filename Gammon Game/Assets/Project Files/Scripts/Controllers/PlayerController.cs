using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class PlayerController : MonoBehaviour
    {
        // Can probably refactor all this movement stuff into its own script dedicated to movement that can be placed on any character.
        public float moveSpeed = 10f;
        public int moveDistance = 1;
        public Vector2 input;
        public Vector2 lastInput;
        public LayerMask WhatStopsMovement;
        [SerializeField]
        private Vector3 targetPoint;
        [SerializeField]
        private Vector3 lastPoint;

        public Animator anim;

        private void Start() {
            targetPoint = transform.position;
            lastPoint = transform.position;
        }

        private void Update() {

            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // If the player object is not at the target position, move the player object to the target position.
            if (transform.position != targetPoint) {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);
                
                // For cancelling a movement operation. Since the player cannot input anything while the object is
                // moving to the target position, i added these 2 if statements to cancel that movement and return
                // the player object to the last position it was at.
                // e.g. the player moves to the right but presses the left key while their moving to move back.
                // lastInput.x = 1, input.x = -1, thus the if statement triggers and the targetPoint becomes the lastPoint.
                if (lastInput.x != 0 && input.x != 0 && input.x != lastInput.x) {
                    targetPoint = lastPoint;
                } 
                else if (lastInput.y != 0 && input.y != 0 && input.y != lastInput.y) {
                    targetPoint = lastPoint;
                }

                // If the player isnt at the targetPoint, then they must be moving towards the targetPoint thus use the walking animation.
                //anim.SetBool("moving", true);

            } else {
                if (lastInput.x != 0) lastInput.x = 0;
                if (lastInput.y != 0) lastInput.y = 0;

                // If the player is at the targetPoint, stop movement animation
                //anim.SetBool("moving", false);

                if (Mathf.Abs(input.x) == 1f) {
                    if (!Physics2D.OverlapCircle(targetPoint + new Vector3(input.x * moveDistance, 0, 0), 0.2f, WhatStopsMovement)) {
                        lastInput.x = input.x;
                        lastPoint = transform.position;
                        targetPoint += new Vector3(input.x * moveDistance, 0, 0);
                    }
                }
                // Moving on the y-axis
                else if (Mathf.Abs(input.y) == 1f) {
                    if (!Physics2D.OverlapCircle(targetPoint + new Vector3(0, input.y * moveDistance, 0), 0.2f, WhatStopsMovement)) {
                        lastInput.y = input.y;
                        lastPoint = transform.position;
                        targetPoint += new Vector3(0, input.y * moveDistance, 0);
                    }
                }
            }
        }
    }
}
