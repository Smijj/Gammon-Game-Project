using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    using UI;

    public class StatsManager : MonoBehaviour
    {
        public StatsObject statsObject;
        public GameObject _contentWrapper;
        
        [SerializeField]
        private List<GameObject> statCards;


        private void Start() {
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
            if (statsObject.stats.Count > 0) {
                // Populate statCards list with all stats
                for (int i = 0; i < statsObject.stats.Count; i++) {
                    statCards.Add(Instantiate(statsObject.statCard, _contentWrapper == null ? this.transform : _contentWrapper.transform));
                    statCards[i].GetComponent<StatCard>().stat = statsObject.stats[i];
                    statCards[i].GetComponent<StatCard>().stat.expLimit = statsObject.GetExpLimit(statsObject.stats[i].level);
                }
            }
        }
    }
}
