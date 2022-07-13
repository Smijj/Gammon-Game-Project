using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem {
    public class RecipeCard : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
        [Header("Debug")]
        [Tooltip("Gets set automatically.")]
        public RecipeObject recipe;


        [Header("Set These Variables")]
        [Tooltip("True if this script is being used to set an InfoPanel object.")]
        public bool isDisplayCard = false;

        public Image itemSprite;
        public Image dishTypeSprite;
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemValueText;
        public TextMeshProUGUI itemQuantityText;
        public TextMeshProUGUI itemDescText;

        [Header("Hover Settings")]
        [Range(0, 1)]
        public float onHoverEnterDim = 0.8f;
        [Range(0, 1)]
        public float onHoverExitDim = 1f;

        private RecipeCard itemInfoPanel;

        private void Start() {
            if (recipe)
                UpdateRecipeCard();

            itemInfoPanel = GameObject.FindGameObjectWithTag("RecipeInfoPanel").GetComponent<RecipeCard>();
        }

        public void UpdateRecipeCard() {
            if (itemNameText)
                itemNameText.text = recipe.name;
            if (recipe.foodSprite)
                itemSprite.sprite = recipe.foodSprite;
            if (recipe.dishTypeSprite)
                dishTypeSprite.sprite = recipe.dishTypeSprite;
            if (itemValueText)
                itemValueText.text = recipe.dishValue.ToString();
            if (itemDescText) {
                itemDescText.text = recipe.description;
                if (itemQuantityText) itemQuantityText.text = "Quantity: " + recipe.quantity.ToString();
            } else {
                if (itemQuantityText) itemQuantityText.text = "x" + recipe.quantity.ToString();
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

        public void OnPointerEnter(PointerEventData eventData) {
            if (recipe && !isDisplayCard) {
                // Darken Recipe Card UI
                GetComponent<CanvasGroup>().alpha = onHoverEnterDim;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (recipe && !isDisplayCard) {
                // Lighten Recipe Card UI
                GetComponent<CanvasGroup>().alpha = onHoverExitDim;
            }
        }
    }
}
