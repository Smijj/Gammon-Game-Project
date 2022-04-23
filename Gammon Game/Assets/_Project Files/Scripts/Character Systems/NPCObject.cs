using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems {
    [CreateAssetMenu(fileName = "New NPC List", menuName = "Character/New NPC List")]
    public class NPCObject : ScriptableObject
    {
        public GameObject npcPrefab;
        public List<NPC> container = new List<NPC>();
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
