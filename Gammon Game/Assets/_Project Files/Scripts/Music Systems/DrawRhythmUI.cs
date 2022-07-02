using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicSystem {

    public class DrawRhythmUI : MonoBehaviour
    {
        public static DrawRhythmUI instance;

        RhythmManager rm;
        public GameObject drawPrefab;       // A simple gameobject that will be spawned to indicate where the player needs to tap


        private void Start() {
            instance = this;
            rm = RhythmManager.instance;
        }

        // Takes an transform to parent the prefab too, its time period (length in relation to time), 
        public GameObject DrawUI(double _timePeriod, float _width, Transform _parent, Color _colour, double _posOffset = 0f, bool _accountForInputDelay = false) {
            if (!drawPrefab) {
                Debug.LogWarning("[DrawRhythmUI] There is currently no DrawPrefab available");
                return null;
            }

            Vector3 marginYPos1 = GetMarginPos(_timePeriod, _accountForInputDelay);
            Vector3 marginYPos2 = GetMarginPos(0, _accountForInputDelay);
            
            Vector3 offsetYPos1 = GetMarginPos(_posOffset, _accountForInputDelay);
            Vector3 offsetYPos2 = GetMarginPos(0, _accountForInputDelay);
            float offsetDisY = Vector3.Distance(offsetYPos1, offsetYPos2);

            GameObject prefabInst = Instantiate(drawPrefab, _parent);
            Vector3 yPosVec = (marginYPos1 + marginYPos2) / 2;
            float disY = Vector3.Distance(marginYPos1, marginYPos2);

            prefabInst.transform.localPosition = new Vector3(0, offsetDisY, 0);
            prefabInst.transform.localScale = new Vector3(_width, disY, 1);
            prefabInst.GetComponent<SpriteRenderer>().color = _colour;

            return prefabInst;
        }


        private Vector3 GetMarginPos(double _timePeriod, bool _accountForInputDelay = false) {
            Vector3 spawnYPos = new Vector3(0, rm.noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, rm.noteDespawnY, 0);

            double timeToGetToMargin = !_accountForInputDelay ? rm.noteFallTime - _timePeriod : rm.noteFallTime - (rm.inputDelayInMiliseconds / 1000.0f) - _timePeriod;
            float marginPercentage = (float)timeToGetToMargin / (rm.noteFallTime * 2);
            Vector3 marginYPos = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage);
            return marginYPos;
        }


        /// <summary>
        /// Used for drawning a horizontal line across the screen in relation to the tapPos.
        /// </summary>
        /// <param name="_timeMargin">Negative number to place before the tapPos, positive for after. This is the time in seconds before or after the tapPos you want to draw something.</param>
        /// <param name="_lineColour"></param>
        /// <param name="_accountForInputDelay">Controls whether or not the drawn objects pos will account for input delay or not (i.e. will it visually move to match the input delay).</param>
        public GameObject DrawAroundTapPos(double _timeMargin, Color _lineColour, bool _accountForInputDelay = false) {
            Vector3 spawnYPos = new Vector3(0, rm.noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, rm.noteDespawnY, 0);

            double timeToGetToMargin = !_accountForInputDelay ? rm.noteFallTime + _timeMargin : rm.noteFallTime - (rm.inputDelayInMiliseconds / 1000.0f) + _timeMargin;
            float marginPercentage = (float)timeToGetToMargin / (rm.noteFallTime * 2);
            Vector3 marginYPos = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage);

            if (drawPrefab && rm.gameArea) {
                GameObject prefabInst = Instantiate(drawPrefab, rm.gameArea);

                Vector3 yPosVec = marginYPos;
                prefabInst.transform.localPosition = new Vector3(0, yPosVec.y, 0);
                prefabInst.transform.localScale = new Vector3(rm.tapIndicatorWidth, 0.1f, 1);
                prefabInst.GetComponent<SpriteRenderer>().color = _lineColour;

                return prefabInst;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_timeMargin"></param>
        /// <param name="_blockColour"></param>
        /// <param name="_accountForInputDelay"></param>
        public GameObject DrawBlockAroundTapPos(double _timeMargin, Color _blockColour, bool _accountForInputDelay = false) {
            Vector3 spawnYPos = new Vector3(0, rm.noteSpawnY, 0);
            Vector3 despawnYPos = new Vector3(0, rm.noteDespawnY, 0);

            double timeToGetToMargin1 = !_accountForInputDelay ? rm.noteFallTime - _timeMargin : rm.noteFallTime - (rm.inputDelayInMiliseconds / 1000.0f) - _timeMargin;
            float marginPercentage1 = (float)timeToGetToMargin1 / (rm.noteFallTime * 2);
            Vector3 marginYPos1 = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage1);

            double timeToGetToMargin2 = !_accountForInputDelay ? rm.noteFallTime + _timeMargin : rm.noteFallTime - (rm.inputDelayInMiliseconds / 1000.0f) + _timeMargin;
            float marginPercentage2 = (float)timeToGetToMargin2 / (rm.noteFallTime * 2);
            Vector3 marginYPos2 = Vector3.Lerp(spawnYPos, despawnYPos, marginPercentage2);

            if (drawPrefab && rm.gameArea) {
                GameObject prefabInst = Instantiate(drawPrefab, rm.gameArea);

                Vector3 yPosVec = (marginYPos1 + marginYPos2) / 2;
                prefabInst.transform.localPosition = new Vector3(0, yPosVec.y, 0);
                float disY = Vector3.Distance(marginYPos1, marginYPos2);
                prefabInst.transform.localScale = new Vector3(rm.tapIndicatorWidth, disY, 1);

                prefabInst.GetComponent<SpriteRenderer>().color = _blockColour;

                return prefabInst;
            }
            return null;
        }
    }
}
