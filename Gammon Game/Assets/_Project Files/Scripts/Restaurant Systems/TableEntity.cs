using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RestaurantSystems {
    using GameManagement;
    public class TableEntity : MonoBehaviour {

        public LayerMask WhatStopsMovement;
        public List<SeatSpot> seatingList = new List<SeatSpot>();
        [SerializeField]
        private List<GameObject> activeChairs = new List<GameObject>();
        [SerializeField]
        private List<GameObject> activeDishes = new List<GameObject>();

        private void Start() {

            foreach (SeatSpot _seat in seatingList) {
                // Check if the seating position is not blocked by a collider
                if (CheckIfPlaceableSpot(_seat.chairPos.transform.position)) {
                    // Spawn Chair
                    if (_seat.chairPrefab) activeChairs.Add(Instantiate(_seat.chairPrefab, _seat.chairPos));
                    // Spawn Dish
                    if(_seat.dishPrefab) activeDishes.Add(Instantiate(_seat.dishPrefab, _seat.dishPos));
                }
            }
            foreach (GameObject chair in activeChairs) {
                chair.GetComponent<SeatEntity>().parentTable = GetComponent<TableEntity>();
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

        private void SpawnSeating() {

        }
    }

    [System.Serializable]
    public struct SeatSpot {
        public string name;
        public Transform chairPos;
        public GameObject chairPrefab;
        public Transform dishPos;
        public GameObject dishPrefab;

        //public SeatSpot(string _name, Transform _chairPos, GameObject _chairPrefab, Transform _dishPos, GameObject _dishPrefab) {
        //    name = _name;
        //    chairPos = _chairPos;
        //    chairPrefab = _chairPrefab;
        //    dishPos = _dishPos;
        //    dishPrefab = _dishPrefab;
        //}
    }
}
