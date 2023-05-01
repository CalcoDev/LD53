using Calco.LD53.Managers;
using UnityEngine;

namespace Calco.LD53.GameObjects
{
    public class Milk : MonoBehaviour
    {
        [SerializeField] private float Frequency;
        [SerializeField] private float Amplitude;

        [SerializeField] private float RotationSpeed;
        
        private float _time;
        private float _initialY;
        
        private void Start()
        {
            _initialY = transform.position.y;
        }
        
        private void Update()
        {
            _time += Time.deltaTime;
            var y = _initialY + Mathf.Sin(_time * Frequency) * Amplitude;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);

            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject);
            
            if (!other.CompareTag("Player")) 
                return;
            
            GameManager.Instance.PlayerWon();
            Destroy(gameObject);
        }
    }
}