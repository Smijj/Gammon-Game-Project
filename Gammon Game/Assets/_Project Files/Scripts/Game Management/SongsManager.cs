using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement {
    using InventorySystem;
    using UI;

    public class SongsManager : MonoBehaviour
    {
        public static SongsManager instance;

        public SongsObject songsObject;

        public GameObject contentWrapper;
        public SongCard songInfoPanel;

        [SerializeField]
        private List<GameObject> songCards;


        private void OnEnable() {
            instance = this;
            InitializeStats();
        }

        //private SongCard GetSongCard(string _name) {
        //    if (songCards.Count > 0) {
        //        foreach (GameObject songCard in songCards) {
        //            if (songCard.GetComponent<SongCard>().song.name == _name) {
        //                return songCard.GetComponent<SongCard>();
        //            }
        //        }
        //    }
        //    return null;
        //}


        private void InitializeStats() {
            // If the songCards object list
            if (songCards.Count > 0) {
                // Destroys all the gameobjects that tied to the songCards list
                foreach (GameObject i in songCards) {
                    Destroy(i);
                }

                // Clears songCards list data
                songCards.Clear();
            }

            // If there are any songs in the songs inventory
            if (songsObject.songs.Count > 0) {
                // Populate songCards list with all songs
                for (int i = 0; i < songsObject.songs.Count; i++) {
                    songCards.Add(Instantiate(songsObject.songCard, contentWrapper == null ? this.transform : contentWrapper.transform));
                    songCards[i].GetComponent<SongCard>().song = songsObject.songs[i];
                }

                // Initialize the display panel section with the first song in the songs list
                songInfoPanel.song = songCards[0].GetComponent<SongCard>().song;
                songInfoPanel.UpdateSongCard();
            }
        }
    }
}
