using System.Collections;
using UnityEngine;

namespace Calco.LD53.GameObjects.Bullets
{
    public class Fist : Bullet
    {
        protected override void Start()
        {
            // Disable the rigidbody
            Destroy(_rb);
        }

        public override void Fire()
        {
            transform.position += Vector3.right * (Mathf.Sign(Dir.x) * 1.5f);
            transform.localScale = new Vector3(Mathf.Sign(Dir.x), 1f, 1f);
        }
    }
}