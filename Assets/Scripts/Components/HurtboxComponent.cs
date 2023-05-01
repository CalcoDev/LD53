using UnityEngine;
using UnityEngine.Events;

namespace Calco.LD53.Components
{
    public class HurtboxComponent : MonoBehaviour
    {
        public Faction Faction => _factionComponent.Faction;
        
        [SerializeField] private FactionComponent _factionComponent;
        [SerializeField] private HealthComponent _healthComponent;

        public UnityEvent<HitboxComponent> OnHit;

        public void Hit(HitboxComponent other)
        {
            _healthComponent.TakeDamage(other.Damage);
            OnHit?.Invoke(other);
        }
    }
}