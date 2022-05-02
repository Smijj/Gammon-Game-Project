using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems {
    [CreateAssetMenu(fileName = "New NPC List", menuName = "Character/New NPC List")]
    public class NPCObject : ScriptableObject
    {
        public GameObject npcPrefab;
        public List<NPC> container = new List<NPC>();

        public NPC GetNPC(string _name) {
            for (int i = 0; i < container.Count; i++) {
                if (container[i].name == _name) {
                    return container[i];
                }
            }
            return null;
        }
        public NPC GetNPC(NPC _npc) {
            for (int i = 0; i < container.Count; i++) {
                if (container[i] == _npc) {
                    return container[i];
                }
            }
            return null;
        }

        public GameObject GenerateNPC(Vector3Int _spawnPos, string _npcName = null) {
            NPC npc;
            if (_npcName != null) {
                if (GetNPC(_npcName) == null) return null;  // Returns if the NPC doesnt exist
                npc = GetNPC(_npcName); // Gets the NPC with the given name
            } else {
                if (GetRandomNPC() == null) return null;    // Returns if the NPC doesnt exist
                npc = GetRandomNPC();   // If no name is specified will give a random NPC from the list
            }

            GameObject npcObject;
            npcObject = Instantiate(npcPrefab);
            npcObject.transform.position = _spawnPos;
            npcObject.GetComponent<NPCController>().npc = npc;

            return npcObject;
        }

        private NPC GetRandomNPC() {
            if (container.Count > 0) {
                return container[Random.Range(0, container.Count)];
            }
            return null;
        }
    
    }

    [System.Serializable]
    public class NPC {
        public string name;
        public Sprite charArt;
        [TextArea(3, 10)]
        public string description;
        public NPC(string _name, Sprite _charArt, string _description) {
            name = _name;
            charArt = _charArt;
            description = _description;
        }

    }
}
