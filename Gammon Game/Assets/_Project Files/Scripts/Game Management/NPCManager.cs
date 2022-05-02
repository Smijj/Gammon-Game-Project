using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement {
    using CharacterSystems;

    public class NPCManager : MonoBehaviour
    {

        public int maxNPCs = 10;
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
            if (activeNPCs.Count <= maxNPCs)
                HandleNPCSpawning();
        }


        public void DestroyNPC(GameObject _npcObject, string _npcName = "") {
            if (activeNPCs.Contains(_npcObject)) {
                Debug.Log("Found item in list");
                activeNPCs.Remove(_npcObject);
            }
            Destroy(_npcObject);

            //foreach (GameObject npcObject in activeNPCs) {
            //    if (npcObject.GetComponent<NPCController>().npc.name == _npcName) {

            //    }
            //}
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
