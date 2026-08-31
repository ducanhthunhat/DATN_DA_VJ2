using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DucAnh
{
    public class OptionsPanel : UICanvas
    {
        [Header("UI Sliders")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        public override void Setup()
        {
            base.Setup();

            // Load giá trị ban đầu lên UI từ AudioManager
            if (AudioManager.Instance != null)
            {
                if (musicSlider != null)
                {
                    musicSlider.value = AudioManager.Instance.MusicVolume;
                    musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                }

                if (sfxSlider != null)
                {
                    sfxSlider.value = AudioManager.Instance.SFXVolume;
                    sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                }
            }
        }

        public void OnMusicVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(volume);
            }
        }

        public void OnSFXVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSFXVolume(volume);
            }
        }

        public void CloseOptions()
        {
            // Do khi ở Main Menu hoặc Pause thì Time.timeScale = 0, lệnh delay bằng Invoke sẽ bị đóng băng
            // Nền phải dùng CloseUIDirectly() để đóng ngay lập tức không cần chờ delay 0.2s
            UIManager.Instance.CloseUIDirectly<OptionsPanel>();
        }

        private void OnDestroy()
        {
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }
}
