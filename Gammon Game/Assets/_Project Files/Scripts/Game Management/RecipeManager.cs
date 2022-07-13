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

        public GameObject invContentWrapper;
        public RecipeCard recipeInfoPanel;
        
        [SerializeField]
        private List<GameObject> recipeCards;

        private void OnEnable() {
            if (!recipeInventory.initialized) {
                // If there is an inventory in the preloadInv var, this will preload the inventory with that. Otherwise will pass through null and instantiate the inv normally.
                InstantiateInventory(preloadInv ? preloadInv : null);
            } else {
                ReloadDisplayCardList();
            }
        }

        public void ReloadDisplayCardList() {
            // If there are any instaniated item card objs, destroy them then clear the recipeCards list
            if (recipeCards.Count > 0) {
                // Destroys all the gameobjects that tied to the recipeCards list
                foreach (GameObject i in recipeCards) {
                    Destroy(i);
                }

                // Clears recipeCards list data
                recipeCards.Clear();
            }

            if (recipeInventory.Count() > 0) {
                // Populate recipeCards list with all items in the player inventory
                for (int i = 0; i < recipeInventory.Count(); i++) {
                    if (recipeInventory.Container[i].recipe.unlocked) {
                        recipeCards.Add(Instantiate(recipeInventory.recipeCard, invContentWrapper == null ? this.transform : invContentWrapper.transform));
                        recipeCards[i].GetComponent<RecipeCard>().recipe = recipeInventory.Container[i].recipe;
                        //recipeCards[i].GetComponent<RecipeCard>().parentInvManager = this;
                    }
                }

                recipeInfoPanel.recipe = recipeCards[0].GetComponent<RecipeCard>().recipe;
                recipeInfoPanel.UpdateRecipeCard();
            }
        }


        // instantiate inventory
        private void InstantiateInventory(InventoryObject _preloadInv = null) {

            // If there is any recipes in the inventory, clear them.
            if (recipeInventory.Count() > 0) {
                // Clears the inventory list data
                recipeInventory.ClearInventory();
            }

            // If there is a inventory to preload into this one
            if (_preloadInv != null) {
                // Populate the recipeInventory container list with all items in _preloadInv
                for (int i = 0; i < _preloadInv.Count(); i++) {
                    recipeInventory.AddItem(_preloadInv.Container[i].recipe);
                }
            }

            ReloadDisplayCardList();

            recipeInventory.initialized = true;
        }
    }
}
