using System;
using UnityEngine;

namespace Calco.LD53.GameObjects
{
    public class JumpPad : MonoBehaviour
    {
        [SerializeField] private float _jumpForce;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) 
                return;

            Player.Instance.ResetYVelocity();
            Player.Instance.AdditionalVelocity = Vector2.up * _jumpForce;
            Player.Instance.AudioManager.Play("player_jump_pad");
        }
    }
}