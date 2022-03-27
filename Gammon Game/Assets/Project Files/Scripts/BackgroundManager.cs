using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

namespace GameManagement {
    public class BackgroundManager : MonoBehaviour {
        
        [Header("BG Settings")]
        public Image bgImage;
        
        [Header("Achievement Settings")]
        public Image itemImage;
        public GameObject itemBackdrop;
        public Image charImage;
        public GameObject achievementBackdrop;

        public GameObject subPanel0;
        public GameObject subPanel1;
        public GameObject subPanel2;

        [Header("Sprite Arrays")]
        public SpriteStruct[] bgArray;
        public SpriteStruct[] itemArray;
        public SpriteStruct[] charArray;
        

        private Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();


        #region Unity Functions

        private void Start() {
            // Adds each bg from the bgArray into a dictionary. This is because the dictionary is easier to intereact with in code.
            // I didnt use a dictionary from the start tho because you cant see them in the inspector.
            foreach (SpriteStruct item in bgArray) {
                spriteDict.Add(item.name, item.sprite);
            }
            foreach (SpriteStruct item in itemArray) {
                spriteDict.Add(item.name, item.sprite);
            }
            foreach (SpriteStruct item in charArray) {
                spriteDict.Add(item.name, item.sprite);
            }

            //GetSubPanels(2f);

            if (!bgImage.sprite) bgImage.sprite = spriteDict["DefaultBG"];

            if (achievementBackdrop.activeSelf) achievementBackdrop.SetActive(false);
            if (itemBackdrop.activeSelf) itemBackdrop.SetActive(false);
        }

        

        #endregion


        #region Public Functions

        public void SetBackground(string _bgName) {
            bgImage.sprite = spriteDict[_bgName];
        }

        public void GiveItem(string _itemName) {
            InitAchievement();

            itemImage.sprite = spriteDict[_itemName];
            itemBackdrop.SetActive(true);
        }
        
        public void IncreaseCharacterRelationship(string _charName) {
            InitAchievement();

            charImage.sprite = spriteDict[_charName];
            achievementBackdrop.SetActive(true);
        }

        public void HideAchievement() {
            itemBackdrop.SetActive(false);
            achievementBackdrop.SetActive(false);
            //subPanel0.SetActive(true);
            //subPanel1.SetActive(true);
            //subPanel2.SetActive(true);
        }

        #endregion


        #region Private Functions

        private void InitAchievement() {
            SetBackground("Achievement");
            //subPanel0.SetActive(false);
            //subPanel1.SetActive(false);
            //subPanel2.SetActive(false);

            //DialogueManager.PlaySequence("HidePanel(0,portrait); HidePanel(1, portrait); HidePanel(2, portrait);");
        }

        private IEnumerator GetSubPanels(float _delay) {
            yield return new WaitForSeconds(_delay);
            subPanel0 = GameObject.FindGameObjectWithTag("SubPanel0");
            subPanel1 = GameObject.FindGameObjectWithTag("SubPanel1");
            subPanel2 = GameObject.FindGameObjectWithTag("SubPanel2");
        }

        #endregion
    }

    [Serializable]
    public struct SpriteStruct {
        public string name;
        public Sprite sprite;
    }
}
