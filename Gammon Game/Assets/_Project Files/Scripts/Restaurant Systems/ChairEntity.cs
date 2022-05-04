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
    }

    public class ChairEntity : MonoBehaviour {
        
        public SeatingData seatData;

        //References
        private GameManager gm;
        public SpriteRenderer spriteRen;

        public ChairType chairType = ChairType.None;
        public Transform accessPoint;
        public bool occupied = false;


        private void Start() {
            gm = GameManager.singleton;
            spriteRen = GetComponent<SpriteRenderer>();

            if (chairType != ChairType.None) {
                gm.activeChairList.Add(GetComponent<ChairEntity>());
            }
        }
    }
}
