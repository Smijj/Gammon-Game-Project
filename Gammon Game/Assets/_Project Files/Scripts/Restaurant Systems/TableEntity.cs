using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RestaurantSystems {
    using GameManagement;
    public enum Direction {
        Left,
        Right,
    }

    public class TableEntity : MonoBehaviour {

        public LayerMask WhatStopsMovement;
        public List<SeatingDirection> seatingDirections = new List<SeatingDirection>();
        public List<SeatingData> seatingList = new List<SeatingData>();
        
        [SerializeField]
        private List<SeatingData> activeSeating = new List<SeatingData>();

        private void Start() {

            for (int i = 0; i < seatingList.Count; i++) {
                // Check if the seating position is blocked by a collider, if it is skip to the next iteration
                if (!CheckIfPlaceableSpot(seatingList[i].chairPos.transform.position)) continue;

                // Set the SeatingData's prefabs, if it fails skip to the next iteration
                if (!seatingList[i].SetPrefabs(seatingDirections)) continue;

                // If the necessary prefabs are not available skip to the next iteration
                if (!seatingList[i].chairPrefab || !seatingList[i].dishPrefab) continue;

                // Otherwise spawn the seating
                seatingList[i].tableParent = GetComponent<TableEntity>();
                seatingList[i].chairInstance = Instantiate(seatingList[i].chairPrefab, seatingList[i].chairPos);
                seatingList[i].chairInstance.GetComponent<ChairEntity>().seatData = seatingList[i];
                seatingList[i].dishInstance = Instantiate(seatingList[i].dishPrefab, seatingList[i].dishPos);
                activeSeating.Add(seatingList[i]);
            }
        }

        private bool CheckIfPlaceableSpot(Vector3 _checkPos) {
            Vector3Int posCell = GameManager.grid.WorldToCell(_checkPos);
            Vector3 posCellCenter = GameManager.grid.GetCellCenterWorld(posCell);
            if (Physics2D.OverlapCircle(posCellCenter, 0.15f, WhatStopsMovement)) {
                return false;
            }
            return true;
        }
    }

    [System.Serializable]
    public class SeatingData {
        public string name;
        public Direction seatingDir;
        [HideInInspector]
        public TableEntity tableParent;
        public Transform chairPos;
        [HideInInspector]
        public GameObject chairPrefab;
        [HideInInspector]
        public GameObject chairInstance;
        public Transform dishPos;
        [HideInInspector]
        public GameObject dishPrefab;
        [HideInInspector]
        public GameObject dishInstance;

        public SeatingData(string _name, TableEntity _tableParent, Transform _chairPos, GameObject _chairPrefab, GameObject _chairInstance, Transform _dishPos, GameObject _dishPrefab, GameObject _dishInstance) {
            name = _name;
            tableParent = _tableParent;
            chairPos = _chairPos;
            chairPrefab = _chairPrefab;
            chairInstance = _chairInstance;
            dishPos = _dishPos;
            dishPrefab = _dishPrefab;
            dishInstance = _dishInstance;
        }

        public bool SetPrefabs(List<SeatingDirection> _seatingDirections) {
            foreach (SeatingDirection _seatDir in _seatingDirections) {
                if(seatingDir == _seatDir.seatingDir) {
                    chairPrefab = _seatDir.chairPrefab;
                    dishPrefab = _seatDir.dishPrefab;
                    return true;
                }
            }
            return false;
        }
    }

    [System.Serializable]
    public class SeatingDirection {
        public string name;
        public Direction seatingDir;
        public GameObject chairPrefab;
        public GameObject dishPrefab;

        public SeatingDirection(string _name, Direction _seatingDir, GameObject _chairPrefab, GameObject _dishPrefab) {
            name = _name;
            seatingDir = _seatingDir;
            chairPrefab = _chairPrefab;
            dishPrefab = _dishPrefab;
        }
    }


}
