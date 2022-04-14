using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem {

    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public GameObject foodCard;
        public List<InventoryItem> Container = new List<InventoryItem>();

        #region Public Functions

        public bool AddItem(RecipeObject _foodItem) {
            if (CheckForItem(_foodItem)) {
                Debug.Log("Save Failed, item is already in the inventory or is a placeholder.");
                return false;            
            }
            Debug.Log("Saved!");
            Container.Add(new InventoryItem(_foodItem));
            return true;
        }

        public bool CheckForItem(RecipeObject _foodItem) {
            for (int i = 0; i < Container.Count; i++) {
                if (Container[i].foodItem == _foodItem) {
                    return true;
                }
            }
            return false;
        }

        public void ClearInventory() {
            Container.Clear();
        }

        /// <summary>
        /// Gets the size of the Inventory container.
        /// </summary>
        /// <returns>The size of the Inventory list as an int.</returns>
        public int Count() {
            return Container.Count;
        }

        #endregion
    }


    [System.Serializable]
    public class InventoryItem {
        public RecipeObject foodItem;
        public InventoryItem(RecipeObject _foodItem) {
            foodItem = _foodItem;
        }

    }
}
