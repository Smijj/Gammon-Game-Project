using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

namespace CharacterSystems {
    using Pathfinding;
    using GameManagement;

    public class MoveController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 7f;
        public int moveDistance = 1;
        public LayerMask WhatStopsMovement;

        public bool useDiagonalMovement = true;
        public bool moveOnMouseRelease = true;

        [HideInInspector]
        public Vector3 lastPoint;
        [HideInInspector]
        public Vector3 movePoint;


        [Header("Pathfinding Settings")]
        public Grid grid;
        public Tilemap map;

        public GameObject pathHighlightPrefab;
        private List<GameObject> pathHighlightList = new List<GameObject>();

        private Vector3Int targetNodePos;
        private Vector3Int currentNodePos;

        private List<PathNode> openSet;
        private List<PathNode> closedSet;
        private List<PathNode> movementPath;


        public bool isMoving { get; private set; } = false;
        private bool activePath = false;
        private bool pathFound = false;
        

        #region Unity Functions

        private void OnEnable() {
            movePoint = transform.position;
            lastPoint = transform.position;
            currentNodePos = grid.WorldToCell(transform.position);
        }

        private void Update() {
            if (!GameManager.isPaused) {
                if (activePath) {
                    if(!isMoving) {
                        if (movementPath.Count > 0) {
                            movePoint = movementPath[0].worldPos;
                            lastPoint = transform.position;

                            movementPath.RemoveAt(0);
                            isMoving = true;
                        } else {
                            ClearPathHighlight();
                            activePath = false;
                            pathFound = false;
                        }
                    } 
                    else {
                        Move(movePoint);
                    }
                } 
                else {
                    if (isMoving)
                        Move(movePoint);
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
            if (NodeColliderCheck(targetNode)) return;
            if (!map.HasTile(targetNode)) return;

            lastPoint = transform.position;
            movePoint = grid.GetCellCenterWorld(targetNode);
            isMoving = true;
        }

        // These two functions are used for non-diagonal movement
        public void IncrementXPosition(float _xInput) {
            currentNodePos = grid.WorldToCell(transform.position);
            Vector3Int targetNode = currentNodePos + new Vector3Int((int)_xInput, 0, 0);
            if (NodeColliderCheck(targetNode)) return;

            lastPoint = transform.position;
            movePoint = grid.GetCellCenterWorld(targetNode);
            isMoving = true;
        }
        public void IncrementYPosition(float _yInput) {
            currentNodePos = grid.WorldToCell(transform.position);
            Vector3Int targetNode = currentNodePos + new Vector3Int(0, (int)_yInput, 0);
            if (NodeColliderCheck(targetNode)) return;

            lastPoint = transform.position;
            movePoint = grid.GetCellCenterWorld(targetNode);
            isMoving = true;
        }

        /// <summary>
        /// If a path exists, will set the activePath bool to the _setActivePath variable passed through
        /// </summary>
        /// <param name="_setActivePath"></param>
        public void SetActivePath(bool _setActivePath = true) {
            if (pathFound) {
                activePath = _setActivePath;
                if (_setActivePath == false)
                    ClearPathHighlight();
                } else {
                ClearPathHighlight();
            }
        }

        // Moves this gameObject to the movePoint
        public void Move(Vector3 _movePoint) {
            if (transform.position != _movePoint) {
                // Moves the gameobject to the target position.
                transform.position = Vector3.MoveTowards(transform.position, _movePoint, moveSpeed * Time.deltaTime);
            } else {
                isMoving = false;
            }
        }


        private Vector3Int targetPosTemp;
        private Vector3Int currentPosTemp;
        public void AutoPath(Vector3 _worldPos) {
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
                        if (!NodeColliderCheck(targetNodePos)) {
                            if (activePath) activePath = false;
                            FindPath(currentNodePos, targetNodePos);
                            //Debug.Log("Moving to cell: " + targetNodePos);
                        }
                        else {
                            Debug.Log("Cannot Move to this Position.");
                        }
                    }
                }
            }
        }

        #endregion


        #region Private Functions

        // Finds a path using an A* algorithm
        private void FindPath(Vector3Int _startPos, Vector3Int _targetPos) {
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
                    pathFound = true;
                    return;
                }

                // Explore nodes surrounding the currentNode.
                foreach (PathNode neighbour in GetNeighbours(currentNode)) {

                    // If the neighbour being explored isn't traversable or is in the closedset, skip it.
                    if (NodeColliderCheck(neighbour.gridPos) || CheckList(closedSet, neighbour)) {
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
            pathFound = false;
            movementPath.Clear();            
        }

        private List<PathNode> RetracePath(PathNode _startNode, PathNode _endNode) {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = _endNode;

            while (currentNode.gridPos != _startNode.gridPos) {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            // Show path visually
            HighlightPath(path);

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


        /// <summary>
        /// Used to check for colliders at a target position. 
        /// </summary>
        /// <param name="_targetNode"></param>
        /// <returns>True if there is a collider on the layer "WhatStopsMovement" at the target position. Otherwise returns false.</returns>
        private bool NodeColliderCheck(Vector3Int _targetNode) {
            Vector3 checkPos = grid.GetCellCenterWorld(_targetNode);

            if (Physics2D.OverlapCircle(checkPos, 0.2f, WhatStopsMovement)) {
                return true;
            }
            return false;
        }

        #endregion
    }
}
