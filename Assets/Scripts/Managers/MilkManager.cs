using Calco.LD53.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Calco.LD53.Managers
{
    public class MilkManager : MonoBehaviour
    {
        public static MilkManager Instance { get; private set; }

        [FormerlySerializedAs("_milk")] [SerializeField] public HealthComponent Milk;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log($"Milk Manager instance already exists, destroying object!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}