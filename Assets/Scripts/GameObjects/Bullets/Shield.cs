using System;
using UnityEngine;

namespace Calco.LD53.GameObjects.Bullets
{
    public class Shield : Bullet
    {
        [SerializeField] private Collider2D _collider;
        
        public override void Fire()
        {
            transform.position += Vector3.right * (Mathf.Sign(Dir.x) * 1.5f);
            transform.rotation = Quaternion.identity;
            transform.localScale = new Vector3(Mathf.Sign(Dir.x), 1f, 1f);
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {

            if (!col.gameObject.TryGetComponent(out Bullet bullet) || bullet.Faction == Faction)
            {
                Physics2D.IgnoreCollision(col.collider, _collider);
                return;
            }
            
            if (bullet != null)
                Debug.Log($"Bullet: {bullet.Faction} collided with shield: {Faction}");
            
            Destroy(bullet.gameObject);
        }
    }
}