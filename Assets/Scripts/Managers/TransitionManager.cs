using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calco.LD53.Managers
{
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance { get; private set; }

        public bool IsTransitioning { get; private set; }
        
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log($"Transition Manager instance already exists, destroying object!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            DontDestroyOnLoad(this);
        }

        public class TransitionParams
        {
            public string Name;
            
            public Action OnTransitionStart;
            public Action OnTransitionHalfway;
            public Action OnTransitionEnd;
        }

        public void PlayTransition(TransitionParams transitionParams)
        {
            StartCoroutine(PlayTransitionCoroutine(transitionParams));
        }
        
        public IEnumerator PlayTransitionCoroutine(TransitionParams transitionParams)
        {
            if (IsTransitioning)
                yield break;
            IsTransitioning = true;
            
            transitionParams.OnTransitionStart?.Invoke();
            
            _animator.Play(transitionParams.Name + "_In");
            yield return new WaitForSecondsRealtime(1f);
            
            transitionParams.OnTransitionHalfway?.Invoke();
            
            _animator.Play(transitionParams.Name + "_Out");
            yield return new WaitForSecondsRealtime(1f);
            
            transitionParams.OnTransitionEnd?.Invoke();
            
            IsTransitioning = false;
        }
    }
}