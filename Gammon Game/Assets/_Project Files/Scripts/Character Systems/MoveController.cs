using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CharacterSystems {
    using GameManagement;

    public class MoveController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 8f;
        public int moveDistance = 1;
        public LayerMask WhatStopsMovement;

        public bool useDiagonalMovement = true;
        public bool moveOnMouseRelease = true;

        [Header("Debug")]
        //[HideInInspector]
        public Vector3 lastPoint;
        //[HideInInspector]
        public Vector3 movePoint;



        [Header("Pathfinding Settings")]
        public GameObject pathHighlightPrefab;
        private List<GameObject> pathHighlightList = new List<GameObject>();

        private Vector3Int targetNodePos;
        private Vector3Int currentNodePos;

        private List<PathNode> openSet;
        private List<PathNode> closedSet;
        private List<PathNode> movementPath;

        private Grid grid;
        private Tilemap map;

        [Header("Debug")]
        public bool isMoving  = false;
        public bool atDestination = false;
        [SerializeField]
        private bool pathing = false;
        [SerializeField]
        private bool pathFound = false;
        

        #region Unity Functions

        private void Start() {
            map = GameManager.map;
            grid = GameManager.grid;
            currentNodePos = grid.WorldToCell(transform.position);
            transform.position = grid.GetCellCenterWorld(currentNodePos);
            movePoint = currentNodePos;
            lastPoint = currentNodePos;
        }

        private void Update() {
            if (!GameManager.isPaused) {
                if (pathing) {
                    if(!isMoving) {
                        if (movementPath.Count > 0) {
                            movePoint = movementPath[0].worldPos;
                            lastPoint = transform.position;

                            movementPath.RemoveAt(0);
                            isMoving = true;
                        } else {
                            ClearPathHighlight();
                            pathing = false;
                            pathFound = false;
                        }
                    } 
                    else {
                        Move(movePoint);
                    }
                } 
                else {
                    if (isMoving) {
                        Move(movePoint);
                    }
                }
            }
        }

        #endregion


        #region Public Functions

        // This function is used for diagonal movement
        public void IncrementPosition(Vector3 _input) {
            currentNodePos = grid.WorldToCell(transform.position);
            Vector3Int targetNode = currentNodePos + new Vector3Int((int)_input.x, (int)_input.y, 0);
            
            // Checks to make sure the player cant move into a collider or off the map.
            if (WalkableTileCheck(targetNode)) return;
            if (!map.HasTile(targetNode)) return;

            lastPoint = transform.position;
            movePoint = grid.GetCellCenterWorld(targetNode);
            isMoving = true;
        }

        // These two functions are used for non-diagonal movement
        public void IncrementXPosition(float _xInput) {
            currentNodePos = grid.WorldToCell(transform.position);
            Vector3Int targetNode = currentNodePos + new Vector3Int((int)_xInput, 0, 0);
            if (WalkableTileCheck(targetNode)) return;

            lastPoint = transform.position;
            movePoint = grid.GetCellCenterWorld(targetNode);
            isMoving = true;
        }
        public void IncrementYPosition(float _yInput) {
            currentNodePos = grid.WorldToCell(transform.position);
            Vector3Int targetNode = currentNodePos + new Vector3Int(0, (int)_yInput, 0);
            if (WalkableTileCheck(targetNode)) return;

            lastPoint = transform.position;
            movePoint = grid.GetCellCenterWorld(targetNode);
            isMoving = true;
        }

        // Moves this gameObject to the movePoint
        public void Move(Vector3 _movePoint) {
            if (transform.position != _movePoint) {
                // Moves the gameobject to the target position.
                transform.position = Vector3.MoveTowards(transform.position, _movePoint, moveSpeed * Time.deltaTime);
                if (atDestination) atDestination = false;
            } else {
                isMoving = false;
                if (!atDestination) atDestination = true;
            }
        }


        private Vector3Int targetPosTemp;
        private Vector3Int currentPosTemp;
        /// <summary>
        /// Moves the gameobject that has this script to the tile at _worldPos if there is one.
        /// </summary>
        /// <param name="_worldPos">Position to move to.</param>
        /// <param name="_highlightPath">Determines if the path is visually shown</param>
        /// <param name="_onlyFindPath">If true the path will be found but the object wont move until the 'pathing' variable is set to true using the SetPathing() function.</param>
        /// <returns>True if finding a path was successful, False if it was unsuccessful.</returns>
        public bool AutoPath(Vector3 _worldPos, bool _highlightPath = false, bool _onlyFindPath = false) {
            if (!GameManager.isPaused) {
                targetNodePos = grid.WorldToCell(_worldPos);
                currentNodePos = grid.WorldToCell(transform.position);
            
                // if the targetnode or the current node have changed, the path is different and no longer found
                if (targetNodePos != targetPosTemp | currentNodePos != currentPosTemp) {
                    pathFound = false;
                }

                // This if statement is here to prevent this code from being called repeatedly for no reason
                if (!pathFound) {
                    targetPosTemp = targetNodePos;
                    currentPosTemp = currentNodePos;

                    if (map.HasTile(targetNodePos)) {
                        if (!WalkableTileCheck(targetNodePos)) {
                            pathFound = FindPath(currentNodePos, targetNodePos);
                            
                            if (pathFound) {
                                if (_highlightPath) HighlightPath(movementPath); // Show path visually

                                // This is here so that if for some reason you dont want to path imediately after finding a
                                // path (like you want to wait for the player to release the mouse button) Then the path will be found and
                                // set but it wont move until the pathing variable is set to true; 
                                if (!_onlyFindPath) pathing = true;
                                else pathing = false;

                                return true;
                            }
                            return false;
                        }
                        else {
                            Debug.Log("Cannot Move to this Position.");
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// If a path exists, will set the pathing bool to the _setPathing variable passed through
        /// </summary>
        /// <param name="_setPathing"></param>
        public void SetPathing(bool _setPathing = true) {
            if (pathFound) {
                pathing = _setPathing;
                if (_setPathing == false)
                    ClearPathHighlight();
            } else {
                ClearPathHighlight();
            }
        }


        /// <summary>
        /// Used to check for colliders at a target position. 
        /// </summary>
        /// <param name="_targetNode"></param>
        /// <returns>True if there is a collider on the layer "WhatStopsMovement" at the target position. Otherwise returns false.</returns>
        public bool WalkableTileCheck(Vector3Int _targetNode) {
            Vector3 checkPos = grid.GetCellCenterWorld(_targetNode);

            if (Physics2D.OverlapCircle(checkPos, 0.2f, WhatStopsMovement)) {
                return true;
            }
            return false;
        }

        #endregion


        #region Private Functions

        // Finds a path using an A* algorithm
        private bool FindPath(Vector3Int _startPos, Vector3Int _targetPos) {
            // Sets the start and destination nodes
            PathNode startNode = new PathNode(grid.GetCellCenterWorld(_startPos), _startPos);
            PathNode targetNode = new PathNode(grid.GetCellCenterWorld(_targetPos), _targetPos);

            // Inits the lists containing nodes-to-search and nodes that have already been searched
            openSet = new List<PathNode> { startNode };
            closedSet = new List<PathNode>();

            int maxSize = 300;  // Stops the program from taking up too much memory if the area that is needs to be searched is very large.
            // The core loop that the algorithm will execute in 
            while (openSet.Count > 0 && openSet.Count < maxSize) {
                PathNode currentNode = openSet[0];
                
                // Finds the node with the smallest fCost in the openSet of nodes and sets it as the current node
                for (int i = 1; i < openSet.Count; i++) {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].fCost < currentNode.hCost) {
                        currentNode = openSet[i];
                    }
                }              

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // Found path
                if (currentNode.gridPos == targetNode.gridPos) {
                    //print("Found target Node.");
                    //print("Openset Size: " + openSet.Count);
                    movementPath = RetracePath(startNode, currentNode);
                    return true;
                }

                // Explore nodes surrounding the currentNode.
                foreach (PathNode neighbour in GetNeighbours(currentNode)) {

                    // If the neighbour being explored isn't traversable or is in the closedset, skip it.
                    if (WalkableTileCheck(neighbour.gridPos) || CheckList(closedSet, neighbour)) {
                        continue;
                    }

                    // Calculate the gCost from the currentNode to the neighbour being explored
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    // if that gCost is smaller than the existing gCost of this neighbour or if this neighbour isn't in the openSet,
                    // then update the gCost, hCost, and parent of the current neighbour and add it to the openList (if it isnt already in there).
                    if (newMovementCostToNeighbour < neighbour.gCost || !CheckList(openSet, neighbour)) {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!CheckList(openSet, neighbour)) {
                            openSet.Add(neighbour);                            
                        }
                    }
                }
            }
            // Prints if a path could not be found and the while loop is exited
            Debug.Log("Could not find a path, or the exceeded the search limit.");
            movementPath.Clear();            
            return false;
        }

        private List<PathNode> RetracePath(PathNode _startNode, PathNode _endNode) {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = _endNode;

            while (currentNode.gridPos != _startNode.gridPos) {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        private void HighlightPath(List<PathNode> _path) {
            ClearPathHighlight();
            foreach (PathNode node in _path) {
                pathHighlightList.Add(Instantiate(pathHighlightPrefab, node.worldPos, Quaternion.identity));
            }
        }

        private void ClearPathHighlight() {
            if (pathHighlightList.Count > 0) {
                foreach (GameObject item in pathHighlightList) {
                    Destroy(item);
                }
                pathHighlightList.Clear();
            }
        }


        private List<PathNode> GetNeighbours(PathNode _node) {
            List<PathNode> neighbours = new List<PathNode>();
            
            if(useDiagonalMovement) { 
                // for through a 3x3 block around the current node.
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        // When the for loop is looking at the center node, skip it.
                        if (x == 0 && y == 0) continue;

                        Vector3Int checkNode = _node.gridPos + new Vector3Int(x, y, 0);

                        if (map.HasTile(checkNode)) {
                            neighbours.Add(new PathNode(grid.GetCellCenterWorld(checkNode), checkNode));
                        }
                    }
                }
            } else {
                // Find only directly adjacent neighbours
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        // When the for loop is looking at the center node, skip it.
                        if (x == -1 && y == -1 || x == 1 && y == -1 || x == 0 && y == 0 || x == -1 && y == 1 || x == 1 && y == 1) continue;

                        Vector3Int checkNode = _node.gridPos + new Vector3Int(x, y, 0);

                        if (map.HasTile(checkNode)) {
                            neighbours.Add(new PathNode(grid.GetCellCenterWorld(checkNode), checkNode));
                        }
                    }
                }
            }

            return neighbours;
        }

        private int GetDistance(PathNode _node1, PathNode _node2) {
            int dstX = Mathf.Abs(_node1.gridPos.x - _node2.gridPos.x);
            int dstY = Mathf.Abs(_node1.gridPos.y - _node2.gridPos.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private bool CheckList(List<PathNode> _nodeSet, PathNode _node) {
            foreach (PathNode item in _nodeSet) {
                if (item.gridPos == _node.gridPos) {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator SpawnSphere(Vector3 _pos, float _timeAlive) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);//Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), _pos, Quaternion.identity);
            obj.transform.position = _pos;
            yield return new WaitForSeconds(_timeAlive);
            Destroy(obj);
        }

        #endregion
    }
}
