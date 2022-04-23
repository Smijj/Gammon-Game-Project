using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem {

    public enum DishType {
        Entree,
        Main,
        Dessert,
        Drink
    }

    [CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory System/New Recipe Item")]
    public class RecipeObject : ScriptableObject
    {
        [Header("Basic Information")]
        public new string name;
        public DishType dishType;
        [TextArea(3, 10)]
        public string description;
        
        [Header("Stats")]
        [Min(0)]
        public int dishValue = 0;
        public int quantity = 0;
        public bool unlocked = false;

        [Header("Sprites")]
        public Sprite foodSprite;
        public Sprite dishTypeSprite;
    }
}
