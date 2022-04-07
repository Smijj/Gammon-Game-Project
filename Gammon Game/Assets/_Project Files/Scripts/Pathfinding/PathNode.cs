using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
    public class PathNode
    {
        public Vector3 worldPos;
        public Vector3Int gridPos;
        public PathNode parent;

        public int gCost = 0;
        public int hCost = 0;
        public int fCost {
            get {
                return gCost + hCost;
            }
        }


        public PathNode(Vector3 _worldPos, Vector3Int _gridPos) {
            worldPos = _worldPos;
            gridPos = _gridPos;
        }

        public override string ToString() {
            return worldPos.x + "," + worldPos.y;
        }
    }
}
