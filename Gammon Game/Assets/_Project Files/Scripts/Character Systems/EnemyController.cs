using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems {
    public class EnemyController : MonoBehaviour
    {
        private MoveController move;


        private void Start() {
            move = GetComponent<MoveController>();
        }

    }
}
