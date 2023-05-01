using System.Collections;
using Calco.LD53.Components;
using Calco.LD53.GameObjects.Bullets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Calco.LD53.GameObjects.Enemies
{
    public class MumboJumbo : MonoBehaviour
    {
        [SerializeField] private GameObject _ragdollController;
        [SerializeField] private GameObject _normalController;

        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private AudioManagerComponent _audioManager;
        
        public HealthComponent HealthComponent => _healthComponent;

        [SerializeField] private Transform _aimArn;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _torsoRb;

        [Header("Bullet")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private float _shootInterval;

        private float _shootTimer;

        private bool _didCombust;

        private void Start()
        {
            DisableRagdoll();
            _shootTimer = 2f + Random.value * 0.5f * _shootInterval;
            
            _healthComponent.OnHealthChanged.AddListener(OnHealthChanged);
        }

        private void OnHealthChanged(float prev, float curr, float max)
        {
            if (curr > prev)
                return;
            
            _audioManager.Play("mumbo_jumbo_hurt");
        }

        private void Update()
        {
            if (_didCombust)
                return;
            
            var player = Player.Instance;
            var dir = (player.transform.position - _aimArn.position).normalized;
            
            bool lineOfSight = false;
            if ((player.transform.position - _aimArn.position).sqrMagnitude < 30f * 30f)
            {
                transform.localScale = new Vector3(-Mathf.Sign(dir.x), 1f, 1f);
                var angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
                if (transform.localScale.x < 0)
                    angle += 180f;
                _aimArn.rotation = Quaternion.Euler(0f, 0f,  angle);
                
                lineOfSight = true;
            }
            
            _shootTimer -= Time.deltaTime;
            if (_shootTimer < 0 && lineOfSight)
            {
                _audioManager.Play("mumbo_jumbo_shoot");
                
                _shootTimer = _shootInterval;
                
                var bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
                bullet.Dir = dir.normalized;
                bullet.Faction = Faction.Enemy;
                bullet.Lifespan = 100f;
                bullet.Speed = 15f;
                bullet.Damage = 100f;
                    
                bullet.Fire();
            }
        }

        public void DisableRagdoll()
        {
            _ragdollController.SetActive(false);
            _normalController.SetActive(true);
            
            for (int i = 0; i < _normalController.transform.childCount; i++)
            {
                var c1 = _normalController.transform.GetChild(i);
                var c2 = _ragdollController.transform.GetChild(i);
                
                c1.position = c2.position;
                c1.rotation = c2.rotation;
            }
        }

        public void EnableRagdoll()
        {
            _ragdollController.SetActive(true);
            _normalController.SetActive(false);
            
            for (int i = 0; i < _normalController.transform.childCount; i++)
            {
                var c1 = _normalController.transform.GetChild(i);
                var c2 = _ragdollController.transform.GetChild(i);
                
                c2.position = c1.position;
                c2.rotation = c1.rotation;
            }
        }
        
        public void StartDie(Rigidbody2D rb)
        {
            if (_didCombust || _healthComponent.Health > 0f)
                return;
            
            _audioManager.Play("mumbo_jumbo_combust");
            
            _didCombust = true;
            EnableRagdoll();

            var dir = UnityEngine.Random.insideUnitCircle.normalized;
            dir.y = Mathf.Abs(dir.y);

            // untie the rigidbody from the ragdoll
            var b = rb.gameObject.GetComponent<HingeJoint2D>();
            Destroy(b);
            rb.AddForce(dir * 30f, ForceMode2D.Impulse);
            rb.AddTorque(5f, ForceMode2D.Impulse);
            
            foreach (var rb2 in _ragdollController.GetComponentsInChildren<Rigidbody2D>())
            {
                if (rb2 == rb)
                    continue;
                
                dir = UnityEngine.Random.insideUnitCircle.normalized;
                dir.y = Mathf.Abs(dir.y);
                
                rb2.AddForce(dir * 15f, ForceMode2D.Impulse);
                rb2.AddTorque(1f, ForceMode2D.Impulse);
            }
            
            StartCoroutine(Die());
        }
        
        private IEnumerator Die()
        {
            _audioManager.Play("mumbo_jumbo_crumble");
            
            var t = 0f;
            while (t < 5f)
            {
                t += Time.deltaTime;
                
                foreach (var rb in _ragdollController.GetComponentsInChildren<Rigidbody2D>())
                {
                    rb.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / 5f);
                }
                
                yield return null;
            }
            
            Destroy(gameObject);
        }
    }
}
