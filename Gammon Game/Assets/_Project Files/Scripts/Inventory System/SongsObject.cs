using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem {


    [CreateAssetMenu(fileName = "New Songs Inventory", menuName = "Inventory System/New Songs Inventory")]
    public class SongsObject : ScriptableObject {

        public GameObject songCard;

        public List<Song> songs = new List<Song>();


    }

    [System.Serializable]
    public class Song {
        public string name;
        [Tooltip("The name of the midi file stored in StreamingAssets that is to be played alongside the music.")]
        public string midiFileName;
        [Tooltip("The name of the AudioClip file stored in StreamingAssets that is to be played alongside the midi file.")]
        public string songFileName;
        public Song(string _name, string _midiFileName, string _songFileName) {
            name = _name;
            midiFileName = _midiFileName;
            songFileName = _songFileName;
        }
    }
}
