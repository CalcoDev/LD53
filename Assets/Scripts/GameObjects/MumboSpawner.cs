using System;
using System.Collections;
using System.Collections.Generic;
using Calco.LD53.GameObjects.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Calco.LD53
{
    public class MumboSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _mumboJumboPrefab;
        [SerializeField] private BoxCollider2D _spawnArea;
        
        [SerializeField] private TextMeshProUGUI _mumboJumboCountText;

        [SerializeField] private GameObject _milk;
        
        [SerializeField] private int _mumboJumboCount;
        [SerializeField] private Vector2Int _minPerWave = new(2, 5);
        [SerializeField] private float _spawnInterval;

        public UnityEvent OnStart;
        public UnityEvent OnAllMumboJumbosDied;

        private bool _started = false;
        
        private int _count;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("sdsadasdasd");
            
            if (_started || !other.CompareTag("Player")) 
                return;

            _started = true;
            
            _count = _mumboJumboCount;
            
            if (_mumboJumboCountText != null)
                _mumboJumboCountText.text = _count.ToString();
            
            if (_milk != null)
                _milk.SetActive(false);
            
            _mumboJumboPrefab.SetActive(false);
            StartCoroutine(SpawnMumboJumboCoroutine());
            
            OnStart?.Invoke();
        }

        private IEnumerator SpawnMumboJumboCoroutine()
        {
            var mumboJumboCount = Random.Range(_minPerWave.x, _minPerWave.y);
            for (var i = 0; i < mumboJumboCount; i++)
            {
                var spawnPosition = new Vector3(
                    Random.Range(_spawnArea.bounds.min.x, _spawnArea.bounds.max.x),
                    Random.Range(_spawnArea.bounds.min.y, _spawnArea.bounds.max.y),
                    0f);
                
                var b = Instantiate(_mumboJumboPrefab, spawnPosition, Quaternion.identity);
                b.SetActive(true);
                b.GetComponent<MumboJumbo>().HealthComponent.OnDied.AddListener(() =>
                {
                    _count--;
                    
                    if (_count <= 0)
                    {
                        if (_mumboJumboCountText != null)
                            _mumboJumboCountText.text = "MILK";
                        
                        if (_milk != null)
                            _milk.SetActive(true);
                        
                        OnAllMumboJumbosDied?.Invoke();
                        
                        return;
                    }
                    
                    if (_mumboJumboCountText != null)
                        _mumboJumboCountText.text = _count.ToString();
                });
            }
            
            _mumboJumboCount -= mumboJumboCount;
            if (_mumboJumboCount > 0)
            {
                yield return new WaitForSecondsRealtime(_spawnInterval);
                StartCoroutine(SpawnMumboJumboCoroutine());
            }
        }
    }
}
