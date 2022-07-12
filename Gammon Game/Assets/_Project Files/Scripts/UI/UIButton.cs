using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace UI {
    public class UIButton : MonoBehaviour {
        [Header("Button Info")]
        public string text = "Button";
        public Sprite icon;
        public Color BGColour = Color.white;
        [Tooltip("Changes the text and icon colours")]
        public Color buttonContentColour = Color.black;

        [Header("Button Refs")]
        public TextMeshProUGUI textComponent;
        public Image iconComponent;
        public Image BGComponent;
        public Button.ButtonClickedEvent onClickFunction;

        private Button button;

        private void Start() {
            try {
                button = GetComponent<Button>();
                if (button && onClickFunction != null) button.onClick = onClickFunction;
            } catch (Exception) {
                try {
                    button = GetComponentInChildren<Button>();
                    if (button && onClickFunction != null) button.onClick = onClickFunction;
                } catch (Exception e) {
                    Debug.LogWarning($"[UIButton] Couldn't find a button component on the object: {gameObject.name}. Error: {e}");
                }
            }

            if (textComponent) {
                textComponent.text = text;
                textComponent.color = buttonContentColour;
            }
            if (iconComponent) {
                iconComponent.sprite = icon;
                iconComponent.color = buttonContentColour;
            }
            BGComponent.color = BGColour;
        }

    }
}
