using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RestaurantSystems {
    using GameManagement;

    public enum ChairType {
        None,
        ChairFront,
        ChairLeft,
        ChairRight,
        Couch,
        Table,
    }

    public class SeatEntity : MonoBehaviour {

        //References
        private GameManager gm;
        private SpriteRenderer spriteRen;

        public ChairType chairType = ChairType.None;
        public Transform accessPoint;
        public bool occupied { get; private set; } = false;

        // Parent Table can instantiate its own chairs in any available preset positions around the table. As it spawns them it will set itself as their parent. 
        // Might need its own script.
        // A Table might get set to 'private' or 'taken' or 'friends only' when a NPC sits at it, only allowing friends of that NPC to sit with them, or nobody at all.
        public TableEntity parentTable;

        private void Start() {
            gm = GameManager.singleton;
            spriteRen = GetComponent<SpriteRenderer>();

            if (chairType != ChairType.None) {
                gm.seatList.Add(GetComponent<SeatEntity>());
            }
        }


        public void SetOccupied(bool _isOccupied) {
            if (_isOccupied) {
                occupied = true;
                // Makes sure the NPC sitting in the chair appears in front of the chair.
                spriteRen.sortingOrder = -1;    // Might be better to make the NPC be on layer 1 instead, to avoid any issues if another NPC passed behind the vhair and the chair is rendered behind them.
            } else {
                occupied = false;
                spriteRen.sortingOrder = 0;
            }
        }

    }
}
