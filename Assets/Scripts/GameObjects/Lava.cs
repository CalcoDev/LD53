using System;
using UnityEngine;

namespace Calco.LD53.GameObjects
{
    public class Lava : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) 
                return;

            Player.Instance.Kill();
        }
    }
}