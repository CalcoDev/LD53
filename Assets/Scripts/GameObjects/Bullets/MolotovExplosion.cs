using System.Collections;
using Calco.LD53.Components;
using UnityEngine;

namespace Calco.LD53.GameObjects.Bullets
{
    public class MolotovExplosion : MonoBehaviour
    {
        [SerializeField] private HitboxComponent _hitbox;
        [SerializeField] private ParticleSystem FX;
        
        [SerializeField] private float Radius = 6f;
        [SerializeField] private float Damage = 100f;
        [SerializeField] private float Force = 30f;
        
        [SerializeField] private AnimationCurve DamageFalloff;
        [SerializeField] private AnimationCurve ForceFalloff;

        public void Start()
        {
            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            FX.gameObject.SetActive(true);
            FX.Play();

            yield return new WaitForSeconds(0.1f);
            
            var colliders = Physics2D.OverlapCircleAll(transform.position, Radius);
            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent(out HurtboxComponent hurtbox))
                    continue;

                var dist = Vector2.Distance(transform.position, collider.transform.position);
                var damage = Damage * DamageFalloff.Evaluate(dist / Radius);
                var force = Force * ForceFalloff.Evaluate(dist / Radius);
                
                _hitbox.Damage = damage;
                if (hurtbox != Player.Instance.Hurtbox)
                    hurtbox.Hit(_hitbox);

                var dir = ((Vector2)(collider.transform.position - transform.position)).normalized;
                dir = dir.normalized; // wtf unity

                // Debug.Log($"Force: {force}, dir: {dir}");
                if (hurtbox == Player.Instance.Hurtbox)
                    Player.Instance.AdditionalVelocity = dir * force * 2f;
                else
                    collider.attachedRigidbody.AddForce(dir * force, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(5f);
            
            Destroy(gameObject);
        }
    }
}
