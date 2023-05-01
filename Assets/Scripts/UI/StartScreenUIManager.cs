using Calco.LD53.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Calco.LD53.UI
{
    public class StartScreenUIManager : MonoBehaviour
    {
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _musicSlider;
        
        public void Quit()
        { 
            GameManager.Instance.Quit();   
        }

        public void Play()
        {
            GameManager.Instance.PlayAgain();
        }
        
        public void SetSfxVolume(float volume)
        {
            GameManager.Instance.MasterMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f - 10f);
        }
        
        public void SetMusicVolume(float volume)
        {
            GameManager.Instance.MasterMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f - 5f);
        }

        public void HandleSettings()
        {
            _sfxSlider.value = Mathf.Pow(10f, (GameManager.Instance.GetSfxVolume() + 10f) / 20f);
            _musicSlider.value = Mathf.Pow(10f, (GameManager.Instance.GetMusicVolume() + 5f) / 20f);
        }
    }
}