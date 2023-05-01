using System;
using System.Collections;
using Calco.LD53.Components;
using Calco.LD53.GameObjects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Calco.LD53.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Canvas _pauseMenu;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _musicSlider;
        
        [SerializeField] private AudioMixerGroup _masterMixerGroup;
        [SerializeField] private AudioMixerGroup _musicMixerGroup;
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;

        [SerializeField] private AudioManagerComponent _audioManager;
        
        public AudioMixerGroup MasterMixerGroup => _masterMixerGroup;
        public AudioMixerGroup MusicMixerGroup => _musicMixerGroup;
        public AudioMixerGroup SfxMixerGroup => _sfxMixerGroup;

        public int DeathCount { get; private set; } = 0;
        public float TimeElapsed { get; private set; } = 0f;
        
        [field: SerializeField] public bool IsPaused { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log($"Game Manager instance already exists, destroying object!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(this);
        }

        public float GetSfxVolume()
        {
            _masterMixerGroup.audioMixer.GetFloat("SFXVolume", out float sfxVol);
            return sfxVol;
        } 
        
        public float GetMusicVolume()
        {
            _masterMixerGroup.audioMixer.GetFloat("MusicVolume", out float musicVol);
            return musicVol;
        }

        public void Pause()
        {
            Time.timeScale = 0f;
            IsPaused = true;
            
            _sfxSlider.value = Mathf.Pow(10f, (GetSfxVolume() + 10f) / 20f);
            _musicSlider.value = Mathf.Pow(10f, (GetMusicVolume() + 5f) / 20f);
            
            _pauseMenu.worldCamera = Camera.main;
            _pauseMenu.gameObject.SetActive(true);
        }

        public void Unpause()
        {
            Time.timeScale = 1f;
            IsPaused = false;
            
            _pauseMenu.gameObject.SetActive(false);
        }

        private void Start()
        {
            Unpause();
        }

        private void Update()
        {
            TimeElapsed += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Escape)) { 
                if (IsPaused)
                    Unpause();
                else
                    Pause();
            }
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ListenToPlayerDeath()
        {
            Player.Instance.HealthComponent.OnDied.AddListener(OnPlayerDied);
        }

        public void SwitchSceneWithTransition(string scene)
        {
            SwitchSceneWithTransition(SceneManager.GetSceneByName(scene).buildIndex);
        }
        
        public void SwitchSceneWithTransition(int buildIdx)
        {
            var transition = new TransitionManager.TransitionParams
            {
                Name = "TriangleUp",
                OnTransitionHalfway = () =>
                {
                    SceneManager.LoadSceneAsync(buildIdx, LoadSceneMode.Single);
                }
            };
            
            TransitionManager.Instance.PlayTransition(transition);
        }

        public void SetSfxVolume(float sliderValue)
        {
            _masterMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20f - 10f);
        }
        
        public void SetMusicVolume(float sliderValue)
        {
            _masterMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20f - 5f);
        }

        public void PlayAgain()
        {
            Unpause();
            
            var idx = 1;
            var transition = new TransitionManager.TransitionParams
            {
                Name = "Fade",
                OnTransitionHalfway = () =>
                {
                    SceneManager.LoadSceneAsync(idx, LoadSceneMode.Single);
                },
                OnTransitionEnd = () =>
                {
                    Time.timeScale = 1f;
                    
                    DeathCount = 0;
                    TimeElapsed = 0f;
                    
                    _audioManager.Play("main_theme");
                }
            };
            
            TransitionManager.Instance.PlayTransition(transition);
        }

        public void PlayerWon()
        {
            StartCoroutine(PlayerWonInternal());
        }

        public void RestartLevel()
        {
            SwitchSceneWithTransition(SceneManager.GetActiveScene().buildIndex);
            Unpause();
        }

        private IEnumerator PlayerWonInternal()
        {
            _audioManager.Stop("main_theme");
            Time.timeScale = 0f;
            Player.Instance.Ascend();

            yield return new WaitForSecondsRealtime(7.25f);

            var idx = SceneManager.GetActiveScene().buildIndex + 1;
            var transition = new TransitionManager.TransitionParams
            {
                Name = "Fade",
                OnTransitionHalfway = () =>
                {
                    SceneManager.LoadSceneAsync(idx, LoadSceneMode.Single);
                },
                OnTransitionEnd = () =>
                {
                    Time.timeScale = 1f;
                    _audioManager.Play("main_theme");
                }
            };
            
            TransitionManager.Instance.PlayTransition(transition);
        }
        private void OnPlayerDied()
        {
            var transition = new TransitionManager.TransitionParams
            {
                Name = "TriangleUp",
                OnTransitionStart = () =>
                {
                    Time.timeScale = 0f;
                },
                OnTransitionHalfway = () =>
                {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    Time.timeScale = 0f;
                },
                OnTransitionEnd = () =>
                {
                    Time.timeScale = 1f;
                }
            };
            
            TransitionManager.Instance.PlayTransition(transition);
            DeathCount += 1;
        }
    }
}