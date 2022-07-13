using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem {

    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public GameObject recipeCard;
        public List<InventoryItem> Container = new List<InventoryItem>();
        public bool initialized = false;

        #region Public Functions

        public bool AddItem(RecipeObject _recipe) {
            if (CheckForItem(_recipe)) {
                Debug.Log("Save Failed, item is already in the inventory or is a placeholder.");
                return false;            
            }
            Container.Add(new InventoryItem(_recipe));
            return true;
        }

        public bool CheckForItem(RecipeObject _recipe) {
            for (int i = 0; i < Container.Count; i++) {
                if (Container[i].recipe == _recipe) {
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
        public RecipeObject recipe;
        public InventoryItem(RecipeObject _recipe) {
            recipe = _recipe;
        }

    }
}
