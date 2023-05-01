using UnityEngine;

namespace Calco.LD53.GameObjects.Bullets
{
    public class Molotov : Bullet
    {
        private void Update()
        {
            _rb.rotation += 360f * Time.deltaTime;
        }

        public override void Fire()
        {
            _rb.AddForce(Dir.normalized * Speed, ForceMode2D.Impulse);
        }
    }
}
