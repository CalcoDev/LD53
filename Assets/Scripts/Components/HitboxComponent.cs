using UnityEngine;
using UnityEngine.Events;

namespace Calco.LD53.Components
{
    public class HitboxComponent : MonoBehaviour
    {
        public Faction Faction => _factionComponent.Faction;

        public float Damage;
        [SerializeField] private FactionComponent _factionComponent;

        public UnityEvent<HurtboxComponent> OnHit;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.TryGetComponent<HurtboxComponent>(out var hurtbox))
                return;

            if (hurtbox.Faction == Faction)
                return;
            
            hurtbox.Hit(this);
            OnHit?.Invoke(hurtbox);
        }
    }
}