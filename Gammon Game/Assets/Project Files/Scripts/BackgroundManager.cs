using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement {
    public class BackgroundManager : MonoBehaviour {
        public Image bgObj;
        public BackgroundStruct[] bgArray;
        private Dictionary<string, Sprite> bgDict = new Dictionary<string, Sprite>();

        #region Unity Functions
        
        private void Start() {
            // Adds each bg from the bgArray into a dictionary. This is because the dictionary is easier to intereact with in code.
            // I didnt use a dictionary from the start tho because you cant see them in the inspector.
            foreach (BackgroundStruct bg in bgArray) {
                bgDict.Add(bg.name, bg.background);
            }

            if (!bgObj.sprite) bgObj.sprite = bgDict["Default"];
        }

        #endregion


        #region Public Functions

        public void SetBackground(string _bg) {
            bgObj.sprite = bgDict[_bg];
        }

        #endregion
    }

    [Serializable]
    public struct BackgroundStruct {
        public string name;
        public Sprite background;
    }
}
