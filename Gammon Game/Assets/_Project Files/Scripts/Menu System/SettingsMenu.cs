using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MenuSystem {
    public class SettingsMenu : MonoBehaviour {

        public AudioMixer mainMixer;

        public void SetQuality(int _qualityIndex) {
            QualitySettings.SetQualityLevel(_qualityIndex);
        }

        public void SetVolume(float _volume) {
            mainMixer.SetFloat("volume", _volume);
        }
    }
}