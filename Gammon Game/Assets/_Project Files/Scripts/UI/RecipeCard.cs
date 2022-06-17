using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem {
    public class RecipeCard : MonoBehaviour, IPointerDownHandler {
        public RecipeObject recipe;

        public bool isDisplayCard = false;

        public Image itemSprite;
        public Image dishTypeSprite;
        public TextMeshProUGUI itemValueText;
        public TextMeshProUGUI itemQuantityText;
        public TextMeshProUGUI itemDescText;

        private RecipeCard itemInfoPanel;

        private void Start() {
            if (recipe)
                UpdateRecipeCard();

            itemInfoPanel = GameObject.FindGameObjectWithTag("RecipeInfoPanel").GetComponent<RecipeCard>();
        }

        public void UpdateRecipeCard() {
            if (recipe.foodSprite)
                itemSprite.sprite = recipe.foodSprite;
            if (recipe.dishTypeSprite)
                dishTypeSprite.sprite = recipe.dishTypeSprite;
            itemValueText.text = recipe.dishValue.ToString();
            if (itemDescText) {
                itemDescText.text = recipe.description;
                itemQuantityText.text = "Quantity: " + recipe.quantity.ToString();
            } else {
                itemQuantityText.text = "x" + recipe.quantity.ToString();
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if(recipe && !isDisplayCard) {
                if (!itemInfoPanel)
                    itemInfoPanel = GameObject.FindGameObjectWithTag("RecipeInfoPanel").GetComponent<RecipeCard>();
                itemInfoPanel.recipe = recipe;
                itemInfoPanel.UpdateRecipeCard();
            }
        }
    }
}
