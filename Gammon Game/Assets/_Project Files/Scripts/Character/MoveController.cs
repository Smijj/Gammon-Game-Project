using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Character {
    public class MoveController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 7f;
        public int moveDistance = 1;
        public LayerMask WhatStopsMovement;

        public bool isMoving = false;
        
        private Vector3 movePoint;
        [HideInInspector]
        public Vector3 lastPoint;

        private Vector3 targetPosition;


        [Header("Grid Stuff")]
        public Grid grid;
        public Tilemap map;
        public Vector3 mousePos;
        public Vector3Int cellPos;
        public Vector3 cellPosCenter;


        #region Unity Functions

        private void OnEnable() {
            movePoint = transform.position;
            lastPoint = transform.position;
        }

        private void Update() {

        }

        #endregion


        #region Public Functions

        public void IncrementXPosition(float _xinput) {
            if (!ColliderCheck(movePoint + new Vector3(_xinput * moveDistance, 0, 0))) {
                lastPoint = transform.position;
                movePoint += new Vector3(_xinput * moveDistance, 0, 0);
                isMoving = true;
            }
        }
        public void IncrementYPosition(float _yinput) {
            if (!ColliderCheck(movePoint + new Vector3(0, _yinput * moveDistance, 0))) {
                lastPoint = transform.position;
                movePoint += new Vector3(0, _yinput * moveDistance, 0);
                isMoving = true;
            }
        }

        public void SetTargetPosition(Vector3 _movePoint) {
            movePoint = _movePoint;
        } 

        public void Move() {
            if (transform.position != movePoint) {
                // Moves the gameobject to the target position.
                transform.position = Vector3.MoveTowards(transform.position, movePoint, moveSpeed * Time.deltaTime);
            } else {
                isMoving = false;
            }
        }
        
        public void AutoPath(Vector3 _worldPos) {
            cellPos = grid.WorldToCell(_worldPos);
            cellPosCenter = grid.GetCellCenterWorld(cellPos);

            if (map.HasTile(cellPos)) {
                if (!ColliderCheck(cellPosCenter)) {
                    targetPosition = cellPosCenter;
                    Debug.Log("Moving to cell: " + cellPos);
                }
                else {
                    Debug.Log("Cannot Move to this Position.");
                }
            }
        }

        #endregion


        #region Private Functions


        /// <summary>
        /// Used to check for colliders at a target position. 
        /// </summary>
        /// <param name="_TargetPos"></param>
        /// <returns>True if there is a collider on the layer "WhatStopsMovement" at the target position. Otherwise returns false.</returns>
        private bool ColliderCheck(Vector3 _TargetPos) {
            if (Physics2D.OverlapCircle(_TargetPos, 0.2f, WhatStopsMovement)) {
                return true;
            }
            return false;
        }

        #endregion
    }
}
