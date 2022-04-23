using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UI {
    using InventorySystem;

    public class StatCard : MonoBehaviour, IPointerDownHandler {
        public Stat stat;

        public bool isDisplayCard = false;

        public TextMeshProUGUI itemName;
        //public TextMeshProUGUI itemLevel;
        public TextMeshProUGUI itemExp;
        public ValueBar expBar;
        public TextMeshProUGUI itemDescText;

        private StatCard statInfoPanel;

        private void Start() {
            statInfoPanel = GameObject.FindGameObjectWithTag("StatInfoPanel").GetComponent<StatCard>();
            
            if (stat != null)
                UpdateStatCard();
        }

        public void UpdateStatCard() {
            if (stat != null) {
                itemName.text = stat.name + " lvl." + stat.level.ToString();
                //itemLevel.text = stat.level.ToString();
                itemExp.text = "EXP: " + stat.exp.ToString() + "/" + stat.expLimit.ToString();
                if (itemDescText) {
                    itemDescText.text = stat.description;
                }

                if (expBar) {
                    expBar.SetMaxValue(stat.expLimit);
                    expBar.SetValue(stat.exp);
                }
            } else {
                Debug.LogWarning("[StatCard] there is no stat assigned to this StatCard");
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (stat != null && !isDisplayCard) {
                if (!statInfoPanel)
                    statInfoPanel = GameObject.FindGameObjectWithTag("StatInfoPanel").GetComponent<StatCard>();
                statInfoPanel.stat = stat;
                statInfoPanel.UpdateStatCard();
            }
        }
    }
}
