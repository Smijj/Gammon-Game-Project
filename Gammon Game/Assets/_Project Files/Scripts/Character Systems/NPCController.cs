using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems {
    using GameManagement;

    public class NPCController : MonoBehaviour
    {
        private MoveController move;
        private Grid grid;

        public Transform followTarget;

        public bool idling;
        public bool movePointSet;
        public Vector3Int movePoint;

        private void Start() {
            move = GetComponent<MoveController>();
            grid = GameManager.grid;
        }

        private void Update() {
            if (grid.WorldToCell(transform.position) != grid.WorldToCell(followTarget.position)) {
                move.AutoPath(followTarget.position);
                move.SetActivePath(true);
            }
        }

    }
}
