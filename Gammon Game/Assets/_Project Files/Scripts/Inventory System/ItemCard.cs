using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem {
    public class ItemCard : MonoBehaviour
    {
        public FoodObject foodItem;

        public Image foodSprite;
        public Image dishTypeSprite;
        public TextMeshProUGUI foodValueText;

        private void Start() {
            UpdateItemCard();
        }

        public void UpdateItemCard() {
            if (foodItem.foodSprite)
                foodSprite.sprite = foodItem.foodSprite;                
            if (foodItem.dishTypeSprite)
                dishTypeSprite.sprite = foodItem.dishTypeSprite;
            foodValueText.text = foodItem.dishValue.ToString();
        }
    }
}
