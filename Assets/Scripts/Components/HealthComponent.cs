using UnityEngine;
using UnityEngine.Events;

namespace Calco.LD53.Components
{
    public class HealthComponent : MonoBehaviour
    {
        public float MaxHealth = 100f;
        public float Health = 100f;

        public UnityEvent<float, float, float> OnHealthChanged;
        public UnityEvent OnDied;

        public bool IsDead => Health <= 0;

        private void Start()
        {
            Health = MaxHealth;
        }

        public float GetHealthPercentage()
        {
            if (MaxHealth <= 0)
                return 0;

            return Mathf.Min(Health / MaxHealth, 1f);
        }

        public void TakeDamage(float damage)
        {
            var oldHealth = Health;
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                OnDied?.Invoke();
            }

            OnHealthChanged?.Invoke(oldHealth, Health, MaxHealth);
        }

        public void Heal(float amount)
        {
            var oldHealth = Health;

            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;

            OnHealthChanged?.Invoke(oldHealth, Health, MaxHealth);
        }

        public void ResetHealth()
        {
            var oldHealth = Health;
            Health = MaxHealth;
            OnHealthChanged?.Invoke(oldHealth, Health, MaxHealth);
        }
    }
}