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

        //private Vector3 targetPosition;
        //private Tile targetTile;

        [Header("Grid Stuff")]
        public Grid grid;
        public Tilemap map;
        private Vector3 mousePos;
        private Vector3Int targetNodePos;
        private Vector3 targetNodeCenter;
        private Vector3 currentPos;


        private List<PathNode> openList;
        private HashSet<PathNode> closedList;

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
            targetNodePos = grid.WorldToCell(_worldPos);
            targetNodeCenter = grid.GetCellCenterWorld(targetNodePos);

            if (map.HasTile(targetNodePos)) {
                if (!ColliderCheck(targetNodeCenter)) {
                    FindPath(currentPos, targetNodeCenter);


                    Debug.Log("Moving to cell: " + targetNodePos);
                }
                else {
                    Debug.Log("Cannot Move to this Position.");
                }
            }
        }

        #endregion


        #region Private Functions

        // Finds a path using an A* algorithm
        private void FindPath(Vector3 _startPos, Vector3 _targetPos) {
            // Sets the start and destination nodes
            PathNode startNode = new PathNode(_startPos);
            PathNode targetNode = new PathNode(_targetPos);

            // Inits the lists containing nodes-to-search and nodes that have already been searched
            openList = new List<PathNode> { startNode };
            closedList = new HashSet<PathNode>();

            // The core loop that the algorithm will execute in 
            while (openList.Count > 0) {
                PathNode currentNode = openList[0];
                
                // Finds the node with the smallest fCost in the openList of nodes and sets it as the current node
                for (int i = 1; i < openList.Count; i++) {
                    if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].fCost < currentNode.hCost) {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Found path
                if (currentNode == targetNode) {
                    return;
                }

                // Explore nodes surrounding the currentNode.

            }

        }


        private float CalculateDistance(Vector3 _worldPos1, Vector3 _worldPos2) {
            return Vector3.Distance(_worldPos1, _worldPos2);
        }



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
