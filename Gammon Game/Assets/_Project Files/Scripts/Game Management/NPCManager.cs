using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement {
    using CharacterSystems;

    public class NPCManager : MonoBehaviour
    {

        public Transform spawnLocation;
        public float spawnTimer;
        private float spawnTimerCounter;

        // List of npcs
        public NPCObject npcs;
        [SerializeField]
        private List<GameObject> activeNPCs = new List<GameObject>();

        // List of seats


        private void Start() {
            spawnTimerCounter = spawnTimer;
        }

        private void Update() {
            HandleNPCSpawning();

        }


        private void HandleNPCSpawning() {
            if (spawnTimerCounter >= spawnTimer) {
                if (Random.Range(0, 2) == 0) {
                    // Spawn an NPC
                    activeNPCs.Add(npcs.GenerateNPC(GameManager.grid.WorldToCell(spawnLocation.position)));
                }
                spawnTimerCounter = 0;
            } else {
                spawnTimerCounter += Time.deltaTime;
            }
        }


    }
}
