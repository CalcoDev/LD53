using System;
using Calco.LD53.Components;
using UnityEngine;
using UnityEngine.U2D;

namespace Calco.LD53.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private PixelPerfectCamera _ppCam;
        [SerializeField] private Transform _target;

        [SerializeField] private VelocityComponent _targetVelocity;
        [SerializeField] private float FollowTime;

        [SerializeField] private float Clamp = 20f;
        [SerializeField] private float Multiplier = 2f;
        
        private Vector3 _posDampVel;

        [SerializeField]

        private void LateUpdate()
        {
            if (_target == null)
                return;

            // Movement
            {
                
                var mouseOffset = (Vector2) Input.mousePosition - new Vector2(Screen.width / 2f, Screen.height / 2f);
                mouseOffset /= new Vector2(Screen.width, Screen.height);
                mouseOffset *= 15f;
                var velOffset = _targetVelocity.Value * 0.57142f;
                var posOffset = Vector2.zero;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    float x = Input.GetAxisRaw("Horizontal");
                    float y = Input.GetAxisRaw("Vertical");
                    
                    posOffset += new Vector2(x, y) * new Vector2(13.33f, 7.5f) * 1.5f;
                }

                var targetPos = _target.position + new Vector3(posOffset.x, posOffset.y, 0);
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _posDampVel, FollowTime);
            }
            
            // Zoom
            {
                // var vel = _targetVelocity.Value.magnitude;
                //
                // vel = Mathf.Clamp(vel, 0f, Clamp);
                //
                // _ppCam.refResolutionX = 320 + (int) (vel * Multiplier);
                // _ppCam.refResolutionY = 180 + (int) (vel * Multiplier);
            }
            
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
    }
}