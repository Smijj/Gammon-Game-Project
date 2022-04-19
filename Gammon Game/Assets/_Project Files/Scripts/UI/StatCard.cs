using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UI {
    using Data;

    public class StatCard : MonoBehaviour, IPointerDownHandler {
        public Stat stat;

        public TextMeshProUGUI itemName;
        //public TextMeshProUGUI itemLevel;
        public TextMeshProUGUI itemExp;
        public ValueBar expBar;
        public TextMeshProUGUI itemDescText;

        private StatCard displayStatCard;

        private void Start() {
            if (stat != null)
                UpdateStatCard();

            displayStatCard = GameObject.FindGameObjectWithTag("StatDisplayPanel").GetComponent<StatCard>();
        }

        public void UpdateStatCard() {
            itemName.text = stat.name + " lvl." + stat.level.ToString();
            //itemLevel.text = stat.level.ToString();
            itemExp.text = "EXP: " + stat.exp.ToString();
            if (itemDescText) {
                itemDescText.text = stat.description;
            }

            expBar.SetMaxValue(stat.expLimit);
            expBar.SetValue(stat.exp);
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (stat != null) {
                if (!displayStatCard)
                    displayStatCard = GameObject.FindGameObjectWithTag("StatDisplayPanel").GetComponent<StatCard>();
                UpdateStatCard();
            }
        }
    }
}
