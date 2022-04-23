using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace InventorySystem {
    using UI;

    [CreateAssetMenu(fileName = "New Stats", menuName = "Inventory System/New Stats Object")]
    public class StatsObject : ScriptableObject
    {
        public GameObject statCard;

        public List<float> levelExpRequirement = new List<float> { 100 };
        public List<Stat> stats = new List<Stat>();

        // function to add exp to a stat
        public void AddExp(StatCard _statCard, float _exp) {
            if (_statCard == null) return;   // If this stat doesnt exist return.

            float expLimit = GetExpLimit(_statCard.stat.level);

            _statCard.stat.exp += _exp;
            if (_statCard.stat.exp >= expLimit) {
                while (_statCard.stat.exp > expLimit) {
                    _statCard.stat.level += 1;
                    _statCard.stat.exp -= expLimit;
                    expLimit = GetExpLimit(_statCard.stat.level);
                }
            } else if (_statCard.stat.exp < 0) {
                if (_statCard.stat.level > 0) {
                    _statCard.stat.level -= 1;
                    expLimit = GetExpLimit(_statCard.stat.level);
                    _statCard.stat.exp += expLimit;
                } else {
                    _statCard.stat.exp = 0;
                }
            }

            _statCard.stat.expLimit = expLimit;
        }

        public void SetLevel(StatCard _statCard, int _level) {
            if (_statCard == null) return;   // If this stat doesnt exist return.

            _statCard.stat.level = _level;
            if (_statCard.stat.level < 0) _statCard.stat.level = 0;
        }

        public float GetExpLimit(int _level) {
            float expLimit;
            if (_level < levelExpRequirement.Count) {
                expLimit = levelExpRequirement[_level];
            }
            else {
                // if the stat level is higher than the largest set exp requirement,
                // it will just set the limit to the last value in the ExpReq list.
                if (levelExpRequirement.Count > 0) {
                    expLimit = levelExpRequirement.Last();
                } else {
                    // This is only here to avoid any issues in the case that the ExpReq list is empty for some reason.
                    expLimit = 100;
                }
            }

            return expLimit;
        }

        public Stat GetStat(string _statName) {
            for (int i = 0; i < stats.Count; i++) {
                if (stats[i].name == _statName)
                    return stats[i];
            }
            return null;
        }

    }

    [System.Serializable]
    public class Stat {
        public string name;
        public int level;
        public float exp;
        [HideInInspector]
        public float expLimit;
        [TextArea(3, 10)]
        public string description;
        public Stat(string _name, int _level, float _exp, float _expLimit, string _description) {
            name = _name;
            level = _level;
            exp = _exp;
            expLimit = _expLimit;
            description = _description;
        }

    }
}
