using System;
using Calco.LD53.Managers;
using TMPro;
using UnityEngine;

namespace Calco.LD53.UI
{
    public class EndScreenUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _deathCountText;
        [SerializeField] private TextMeshProUGUI _timeText;

        private void Start()
        {
            _deathCountText.text = $"{GameManager.Instance.DeathCount}";
            TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.TimeElapsed);
            _timeText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds:000}";
        }

        public void Quit()
        { 
            GameManager.Instance.Quit();   
        }

        public void PlayAgain()
        {
            GameManager.Instance.PlayAgain();
        }
    }
}