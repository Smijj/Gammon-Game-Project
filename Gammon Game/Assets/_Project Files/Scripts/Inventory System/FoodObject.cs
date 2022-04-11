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

    [CreateAssetMenu(fileName = "New Food", menuName = "Inventory System/New Food Item")]
    public class FoodObject : ScriptableObject
    {
        [Header("Basic Information")]
        public new string name;
        public Sprite foodSprite;
        [Min(0)]
        public int dishValue = 0;
        
        public DishType dishType;
        public Sprite dishTypeSprite;
        
        
        [TextArea(5, 10)]
        public string description;
    }
}
