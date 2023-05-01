using System;
using System.Collections;
using System.Collections.Generic;
using Calco.LD53.Managers;
using UnityEngine;

namespace Calco.LD53.Components
{
    public class AudioComponent : MonoBehaviour
    {
        public string Name;
        public AudioClip[] AudioClips;
        public float Volume = 1f;
        // public float Radius = 10f;

        public bool Loop;
        public bool PlayRandom;
        public bool PlayIncrementing;
        
        public bool IsMusic;
        
        public bool PlayOnAwake;
        
        public bool PlayRegardlessOfParentDeath;
        public float TimeToLive = 1f;

        private int _currentIdx;
        private AudioSource[] _sources;

        private void Start()
        {
            _sources = new AudioSource[AudioClips.Length];

            _currentIdx = 0;
            foreach (var audioClip in AudioClips)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.clip = audioClip;
                source.volume = Volume;
                source.loop = Loop;
                source.playOnAwake = false;
                
                // TODO(calco): Look into this
                source.spatialBlend = 0f;
                // source.maxDistance = Radius;

                source.outputAudioMixerGroup = IsMusic ? GameManager.Instance.MusicMixerGroup 
                    : GameManager.Instance.SfxMixerGroup;

                _sources[_currentIdx] = source;
                _currentIdx += 1;
            }
            _currentIdx = 0;

            if (PlayRegardlessOfParentDeath)
                transform.parent = GameManager.Instance.transform;
            
            if (PlayOnAwake)
                Play();
        }

        private IEnumerator DestroySelf()
        {
            yield return new WaitForSeconds(TimeToLive);
            
            Destroy(gameObject);
        }

        public void Play()
        {
            if (PlayIncrementing)
                _currentIdx = (_currentIdx + 1) % AudioClips.Length;
            if (PlayRandom)
                _currentIdx = UnityEngine.Random.Range(0, AudioClips.Length);
            
            _sources[_currentIdx].Play();
            
            if (PlayRegardlessOfParentDeath)
                StartCoroutine(DestroySelf());
        }

        public void Stop()
        {
            foreach (var audioSource in _sources)
                audioSource.Stop();
        }
    }
}