using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Vector3 worldPos;

    public float gCost;
    public float hCost;
    public float fCost {
        get {
            return gCost + hCost;
        }
    }

    public PathNode nodeParent;

    public PathNode(Vector3 _worldPos) {
        worldPos = _worldPos;
    }

    public override string ToString() {
        return worldPos.x + "," + worldPos.y;
    }
}
