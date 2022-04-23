using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement {
    using InventorySystem;


    public class RecipeManager : MonoBehaviour {
        [Header("Inventory Object References")]
        public InventoryObject recipeInventory;
        public InventoryObject preloadInv;

        public GameObject contentWrapper;
        public RecipeCard recipeInfoPanel;
        
        [SerializeField]
        private List<GameObject> recipeCards;


        private void Start() {
            // If there is an inventory in the preloadInv var, this will preload the inventory with that. Otherwise will pass through null.
            InstantiateInventory(preloadInv ? preloadInv : null, contentWrapper);            
        }


        // instantiate inventory
        private void InstantiateInventory(InventoryObject _preloadInv = null, GameObject _contentWrapper = null) {
            // If there are any instaniated recipe cards, destroy them then clear the RecipeCards list
            if (recipeCards.Count > 0) {
                // Destroys all the gameobjects that tied to the recipeCards list
                foreach (GameObject i in recipeCards) {
                    Destroy(i);
                }

                // Clears recipeCards list data
                recipeCards.Clear();
            }

            // If there is any recipes in the inventory, clear them.
            if (recipeInventory.Container.Count > 0) {
                // Clears the inventory list data
                recipeInventory.ClearInventory();
            }

            if (_preloadInv != null) {
                // Populate the recipeInventory container list with all items in _preloadInv
                for (int i = 0; i < _preloadInv.Count(); i++) {
                    recipeInventory.AddItem(_preloadInv.Container[i].recipe);
                }
                
                if (recipeInventory.Count() > 0) {
                    // Populate recipeCards list with all items in the player inventory
                    for (int i = 0; i < recipeInventory.Count(); i++) {
                        recipeCards.Add(Instantiate(recipeInventory.recipeCard, _contentWrapper == null ? this.transform : _contentWrapper.transform));
                        recipeCards[i].GetComponent<RecipeCard>().recipe = recipeInventory.Container[i].recipe;
                    }
                }
            }

            recipeInfoPanel.recipe = recipeCards[0].GetComponent<RecipeCard>().recipe;
            recipeInfoPanel.UpdateRecipeCard();

            Debug.Log("Initalized Recipes");
        }
    }
}
