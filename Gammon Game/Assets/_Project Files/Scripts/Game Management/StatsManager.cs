using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement {
    using UI;
    using InventorySystem;

    public class StatsManager : MonoBehaviour
    {
        public StatsObject statsObject;
        
        public GameObject contentWrapper;
        public StatCard statInfoPanel;

        [SerializeField]
        private List<GameObject> statCards;


        private void OnEnable() {
            InitializeStats();
        }

        public void AddExp(string _statName, float _amount) {
            StatCard statCard = GetStatCard(_statName);
            if (statCard == null) return;   // If this stat doesnt exist return.

            statsObject.AddExp(statCard, _amount);
            statCard.GetComponent<ValueBar>().SetValue(statCard.stat.exp);
            statCard.UpdateStatCard();
        }


        private StatCard GetStatCard(string _name) {
            if (statCards.Count > 0) {
                foreach (GameObject statCard in statCards) {
                    if (statCard.GetComponent<StatCard>().stat.name == _name) {
                        return statCard.GetComponent<StatCard>();
                    }
                }
            }
            return null;
        }


        private void InitializeStats() {
            // If the statCards object list
            if (statCards.Count > 0) {
                // Destroys all the gameobjects that tied to the statCards list
                foreach (GameObject i in statCards) {
                    Destroy(i);
                }

                // Clears statCards list data
                statCards.Clear();
            }

            // If there are any stats in the stats inventory
            if (statsObject.stats.Count > 0) {
                // Populate statCards list with all stats
                for (int i = 0; i < statsObject.stats.Count; i++) {
                    statCards.Add(Instantiate(statsObject.statCard, contentWrapper == null ? this.transform : contentWrapper.transform));
                    statCards[i].GetComponent<StatCard>().stat = statsObject.stats[i];
                    statCards[i].GetComponent<StatCard>().stat.expLimit = statsObject.GetExpLimit(statsObject.stats[i].level);
                }
                
                statInfoPanel.stat = statCards[0].GetComponent<StatCard>().stat;
                statInfoPanel.UpdateStatCard();
            }

            //Debug.Log("Initalized Stats");
        }
    }
}
