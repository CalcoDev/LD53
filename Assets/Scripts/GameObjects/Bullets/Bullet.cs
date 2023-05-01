using Calco.LD53.Components;
using UnityEngine;

namespace Calco.LD53.GameObjects.Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject _hitFX;
        
        public float Speed;
        public Vector2 Dir;
        public float Lifespan;
        public float Damage
        {
            get => _hitboxComponent.Damage;
            set => _hitboxComponent.Damage = value;
        }

        public Faction Faction
        {
            get => _factionComponent.Faction;
            set => _factionComponent.Faction = value;
        }
        
        [SerializeField] protected Rigidbody2D _rb;
        [SerializeField] protected HitboxComponent _hitboxComponent;
        [SerializeField] protected FactionComponent _factionComponent;

        public virtual void Fire()
        {
            _rb.velocity = Dir * Speed;
            _rb.rotation = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        }

        protected virtual void Start()
        {
            _hitboxComponent.OnHit.AddListener(_ =>
            {
                Die();
            });
        }

        private void Update()
        {
            Lifespan -= Time.deltaTime;

            if (Lifespan < 0)
                Die();
        }

        protected virtual void OnCollisionEnter2D(Collision2D col)
        {
            Die();
        }

        private void Die()
        {
            if (_hitFX != null)
                Instantiate(_hitFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}