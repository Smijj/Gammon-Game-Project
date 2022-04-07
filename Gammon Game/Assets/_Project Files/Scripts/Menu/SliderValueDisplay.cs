using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Menu {
    public class SliderValueDisplay : MonoBehaviour
    {
        public TMP_InputField text;

		private Slider slider;

		private void OnEnable() {
			slider = GetComponent<Slider>();
			slider.onValueChanged.AddListener(UpdateText);
			text.onEndEdit.AddListener(UpdateSlider);
		}
		private void OnDisable() {
			slider.onValueChanged.RemoveListener(UpdateText);	
			text.onEndEdit.RemoveListener(UpdateSlider);
		}

		public void UpdateText(float _float) {
            text.text = (_float * 100).ToString("0");
		}

		public void UpdateSlider(string _string) {
			float temp = 0;
			if (float.TryParse(_string, out temp)) {
				// Ensure the value is between the upper and lower bounds set by the slider & normalise temp.
				temp = (temp > slider.maxValue * 100 ? slider.maxValue : (temp < slider.minValue * 100 ? slider.minValue : temp / 100));
				slider.value = temp;
				UpdateText(temp);
			}
		}
    }
}

