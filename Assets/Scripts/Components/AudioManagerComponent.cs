using System.Collections.Generic;
using UnityEngine;

namespace Calco.LD53.Components
{
    public class AudioManagerComponent : MonoBehaviour
    {
        [SerializeField] private AudioComponent[] _audioComponents;

        private Dictionary<string, AudioComponent> _dict = new();
        
        public void Awake()
        {
            foreach (var audioComponent in _audioComponents)
                _dict.Add(audioComponent.Name, audioComponent);
        }
        
        public void Play(string audioName)
        {
            _dict[audioName].Play();
        }
        
        public void Stop(string audioName)
        {
            _dict[audioName].Stop();
        }
        
        public void StopAll()
        {
            foreach (var audioComponent in _audioComponents)
                audioComponent.Stop();
        }
    }
}