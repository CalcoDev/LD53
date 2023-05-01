using Calco.LD53.GameObjects.Bullets;
using UnityEngine;

namespace Calco.LD53.Resources
{
    [CreateAssetMenu(fileName = "Player Ability", menuName = "Calco/PlayerAbility", order = 0)]
    public class PlayerAbility : ScriptableObject
    {
        public string Name;
        public Texture Icon;

        public bool UseMouse = true;
        
        public float Cooldown = 0.5f;
        
        public float BulletSpeed = 20f;
        public float BulletDamage = 1f;
        public float BulletLifespan = 1f;
        public Bullet BulletPrefab;
        
        public float MilkCost = 1f;
    }
}