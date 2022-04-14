using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem {
    public class RecipeInventory : MonoBehaviour {
        [Header("Inventory Object References")]
        public InventoryObject recipeInventory;
        public InventoryObject preloadInv;
        
        [Header("Inv Settings")]
        private GridLayoutGroup gridGroup;
        private RectTransform invRect;

        [SerializeField]
        private float invWidth = 0;
        [SerializeField]
        private float invHeight = 0;

        [SerializeField]
        private List<GameObject> itemCards;


        private void Start() {
            // If there is an inventory in the preloadInv var, this will preload the inventory with that. Otherwise will pass through null.
            InstantiateInventory(preloadInv ? preloadInv : null);

            gridGroup = GetComponent<GridLayoutGroup>();
            invRect = GetComponent<RectTransform>();

            
        }

        private void Update() {

            // TODO. Not Working yet. 
            //if (invWidth != invRect.rect.width || invHeight != invRect.rect.height) {
            //    invWidth = invRect.rect.width;
            //    invHeight = invRect.rect.height;
            //}
        }


        // instantiate inventory
        private void InstantiateInventory(InventoryObject _preloadInv = null, GameObject _contentWrapper = null) {
            // Destroys all the gameobjects that tied to the itemCards list
            foreach (GameObject i in itemCards) {
                Destroy(i);
            }

            // Clears itemCards list data
            itemCards.Clear();

            // Clears the inventory list data
            recipeInventory.ClearInventory();

            if (_preloadInv != null) {
                // Populate the recipeInventory container list with all items in _preloadInv
                for (int i = 0; i < _preloadInv.Count(); i++) {
                    recipeInventory.AddItem(_preloadInv.Container[i].foodItem);
                    //_preloadInv.Container[i].foodItem.SetUnlocked(true);
                    //_preloadInv.Container[i].foodItem.AddQuantity(1);
                }
                
                if (recipeInventory.Count() > 0) {
                    // Populate itemCards list with all items in the player inventory
                    for (int i = 0; i < recipeInventory.Count(); i++) {
                        itemCards.Add(Instantiate(recipeInventory.foodCard, _contentWrapper == null ? this.transform : _contentWrapper.transform));
                        itemCards[i].GetComponent<ItemCard>().foodItem = recipeInventory.Container[i].foodItem;
                    }
                }
            }
        }
    }
}
