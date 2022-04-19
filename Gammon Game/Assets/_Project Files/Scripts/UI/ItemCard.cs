using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem {
    public class ItemCard : MonoBehaviour, IPointerDownHandler {
        public RecipeObject foodItem;

        public Image itemSprite;
        public Image dishTypeSprite;
        public TextMeshProUGUI itemValueText;
        public TextMeshProUGUI itemQuantityText;
        public TextMeshProUGUI itemDescText;

        private ItemCard displayItemCard;

        private void Start() {
            if (foodItem)
                UpdateItemCard(GetComponent<ItemCard>(), foodItem);

            displayItemCard = GameObject.FindGameObjectWithTag("ItemDisplayPanel").GetComponent<ItemCard>();
        }

        public void UpdateItemCard(ItemCard _card, RecipeObject _item) {
            if (_item.foodSprite)
                _card.itemSprite.sprite = _item.foodSprite;
            if (_item.dishTypeSprite)
                _card.dishTypeSprite.sprite = _item.dishTypeSprite;
            _card.itemValueText.text = _item.dishValue.ToString();
            _card.itemQuantityText.text = "x" + _item.quantity.ToString();
            if (_card.itemDescText) {
                _card.itemDescText.text = _item.description;
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            if(foodItem) {
                if (!displayItemCard)
                    displayItemCard = GameObject.FindGameObjectWithTag("ItemDisplayPanel").GetComponent<ItemCard>();
                UpdateItemCard(displayItemCard, foodItem);
            }
        }
    }
}
