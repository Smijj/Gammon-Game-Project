using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem {
    public class Inventory : MonoBehaviour
    {
        public InventoryObject foodInventory;
        public InventoryObject preloadInv;

        [SerializeField]
        private List<GameObject> itemCards;


        private void Start() {
            // If there is an inventory in the preloadInv var, this will preload the inventory with that. Otherwise will pass through null.
            InstantiateInventory(preloadInv ? preloadInv : null);
        }


        // instantiate inventory
        private void InstantiateInventory(InventoryObject _inv = null, GameObject _contentWrapper = null) {
            // Destroys all the gameobjects that tied to the itemCards list
            foreach (GameObject i in itemCards) {
                Destroy(i);
            }

            // Clears itemCards list data
            itemCards.Clear();

            // Clears the inventory list data
            foodInventory.ClearInventory();

            if (_inv != null) {
                // Populate the foodInventory container list with all items in _inv
                for (int i = 0; i < _inv.Count(); i++) {
                    foodInventory.AddItem(_inv.Container[i].foodItem);
                }
                
                if (foodInventory.Count() > 0) {
                    // Populate itemCards list with all items in the player inventory
                    for (int i = 0; i < foodInventory.Count(); i++) {
                        itemCards.Add(Instantiate(foodInventory.foodCard, _contentWrapper == null ? this.transform : _contentWrapper.transform));
                        itemCards[i].GetComponent<ItemCard>().foodItem = foodInventory.Container[i].foodItem;
                    }
                }
            }
        }
    }
}
