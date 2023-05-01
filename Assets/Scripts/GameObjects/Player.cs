using System;
using System.Collections;
using Calco.LD53.Components;
using Calco.LD53.Managers;
using Calco.LD53.Resources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Calco.LD53.GameObjects
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private VelocityComponent _velocityComponent;
        [field: SerializeField] public HealthComponent HealthComponent { get; private set; }
        [SerializeField] private HurtboxComponent _hurtboxComponent;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _sprite;

        [Header("Audio")]
        [SerializeField] private AudioManagerComponent _audioManager;

        [Header("FX")] 
        [SerializeField] private ParticleSystem _slideFx;
        [SerializeField] private ParticleSystem _jumpFx;
        [SerializeField] private ParticleSystem _landFx;
        
        private ParticleSystem.EmissionModule _slideFxEm;

        [Header("Run")]
        [SerializeField] private float MaxRunSpeed = 14f * 8.5f;
        [SerializeField] private float RunAccel = 200f * 8f;
        [SerializeField] private float RunReduce = 62f * 8f;
        
        [SerializeField] private float WalkSFXTime;

        [Header("Jump")] 
        [SerializeField] private float JumpForce = 204f + 8f;
        [SerializeField] private float JumpHBoost = 13f * 5f;
        [SerializeField] private float CoyoteTime = 0.1f;
        [SerializeField] private float JumpBufferTime = 0.1f;
        [SerializeField] private float VariableJumpTime = 0.2f;
        [SerializeField] private float VariableJumpMultiplier = 0.5f;
        [SerializeField] private int JumpCount = 2;

        [Header("Slide")] 
        [SerializeField] private float SlideHBoost = 13f;

        [Header("Abilities")]
        [SerializeField] private PlayerAbility[] Abilities;
        private float[] _abilityCooldowns;
        
        public UnityEvent<PlayerAbility> OnAbilityChanged;
        
        // Getters and setters
        public Rigidbody2D Rigidbody => _rb;
        public HurtboxComponent Hurtbox => _hurtboxComponent;
        public VelocityComponent VelocityComponent => _velocityComponent;
        public AudioManagerComponent AudioManager => _audioManager;
        
        public Vector2 AdditionalVelocity { get; set; }

        // Properties
        public bool IsGrounded { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsSliding { get; private set; }
        
        // Timers
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private float _variableJumpTimer;

        private float _slideTimer;
        
        private float _walkTimer;
        
        // Input
        private float _inputX;
        private float _lastNonZeroX;
        
        private bool _inputJumpPressed;
        private bool _inputJumpReleased;

        private bool _inputSlidePressed;
        private bool _inputSlideReleased;
        
        private bool _inputShootPressed;
        private bool _inputShieldPressed;
        
        // Abilities
        private PlayerAbility _currentAbility;
        private int _currentAbilityIndex;

        private int _jumpCounter;

        private float MaxFall = 35f;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log($"Player Controller instance already exists, destroying object!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private bool _prevGrounded;

        private void Start()
        {
            _slideFxEm = _slideFx.emission;
            _slideFxEm.enabled = false;

            _abilityCooldowns = new float[Abilities.Length];
            
            OnAbilityChanged.AddListener(ability =>
            {
                Debug.Log("Ability changed to " + ability.Name);
            });
            
            HealthComponent.OnHealthChanged.AddListener((prev, curr, _) =>
            {
                if (prev > curr)
                {
                    _audioManager.Play("player_hurt");
                    Debug.Log("aaaaaaaaaaaaa");
                }
            });
            
            HealthComponent.OnDied.AddListener(() =>
            {
                _audioManager.Play("player_died");
            });
            
            // for (int i = 0; i < Abilities.Length; i++)
            // {
            //     if (Abilities[i].Name != "Revolver")
            //         continue;
            //     
            //     _currentAbilityIndex = i;
            //     _currentAbility = Abilities[i];
            // }

            StartCoroutine(AA());

            GameManager.Instance.ListenToPlayerDeath();
            
            _walkTimer = WalkSFXTime;
        }

        private IEnumerator AA()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (Abilities.Length <= 0) 
                yield break;
            
            _currentAbilityIndex = 0;
            _currentAbility = Abilities[0];
            OnAbilityChanged?.Invoke(_currentAbility);
        }

        private void Update()
        {
            if (Time.timeScale == 0f)
                return;
            
            _inputX = Input.GetAxisRaw("Horizontal");
            _inputJumpPressed = Input.GetKeyDown(KeyCode.Space);
            _inputJumpReleased = Input.GetKeyUp(KeyCode.Space);

            _inputSlidePressed = Input.GetKeyDown(KeyCode.S);
            _inputSlideReleased = Input.GetKeyUp(KeyCode.S);
            
            _inputShootPressed = Input.GetMouseButtonDown(0);
            _inputShieldPressed = Input.GetMouseButtonDown(1);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                _inputX = 0f;
                _inputSlidePressed = false;
                _inputSlideReleased = false;
            }
            
            if (Input.mouseScrollDelta.y != 0f)
            {
                int delta = (int) Mathf.Sign(Input.mouseScrollDelta.y);
                _currentAbilityIndex = (_currentAbilityIndex + delta + Abilities.Length) % Abilities.Length;
                _currentAbility = Abilities[_currentAbilityIndex];
                
                OnAbilityChanged?.Invoke(_currentAbility);
            }
            
            for (int i = 0; i < Abilities.Length; i++)
            {
                if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) 
                    continue;
                
                _currentAbilityIndex = i;
                _currentAbility = Abilities[i];
                    
                OnAbilityChanged?.Invoke(_currentAbility);
            }

            if (Mathf.Abs(_inputX) >= 0.005f)
                _lastNonZeroX = _inputX;
            
            if (_inputJumpPressed)
                _jumpBufferTimer = JumpBufferTime;

            _sprite.flipX = _lastNonZeroX < 0f;

            // Grounded
            {
                _prevGrounded = IsGrounded;
                IsGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * 1.35f, 
                    new Vector2(1.2f, 0.2f), 0f,
                    LayerMask.GetMask("LevelGeometry"));
                
                if (!IsGrounded && _prevGrounded)
                {
                    _coyoteTimer = CoyoteTime;
                }

                if (IsGrounded && !_prevGrounded)
                {
                    _jumpCounter = JumpCount;
                    IsJumping = false;
                    
                    Instantiate(_landFx, transform.position + Vector3.down * 1.35f, Quaternion.identity);
                }
            }

            // Shooting and stuff
            if (_abilityCooldowns.Length > 0)
            {
                // _abilityTimer -= Time.deltaTime;
                _abilityCooldowns[_currentAbilityIndex] -= Time.deltaTime;
                
                if (_inputShootPressed && _abilityCooldowns[_currentAbilityIndex] <= 0f)
                {
                    if (MilkManager.Instance.Milk.Health < _currentAbility.MilkCost)
                        return;
                    
                    _abilityCooldowns[_currentAbilityIndex] = _currentAbility.Cooldown;
                    MilkManager.Instance.Milk.TakeDamage(_currentAbility.MilkCost);
                    
                    var bullet = Instantiate(_currentAbility.BulletPrefab, transform.position, Quaternion.identity);
                    bullet.Speed = _currentAbility.BulletSpeed;
                    bullet.Damage = _currentAbility.BulletDamage;
                    
                    var mouseDir = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                    bullet.Dir = _currentAbility.UseMouse ? mouseDir.normalized : (_lastNonZeroX * Vector2.right).normalized;
                    bullet.Faction = Faction.Player;
                    bullet.Lifespan = _currentAbility.BulletLifespan;
                    
                    _audioManager.Play("player_shoot");
                    
                    bullet.Fire();
                }
            }
            
            _velocityComponent.SetVelocity(_rb.velocity);
            
            // Sliding
            {
                _slideTimer -= Time.deltaTime;

                if (IsSliding)
                {
                    if (IsGrounded && !_prevGrounded)
                        _slideFxEm.enabled = true;
                    
                    if (!IsGrounded && _prevGrounded)
                        _slideFxEm.enabled = false;
                }

                if (_inputSlidePressed)
                {
                    _rb.gravityScale = 0.7f;
                    MaxFall = 20f;
                    
                    _audioManager.Play("player_slide_begin");
                    
                    if (_slideTimer < 0f)
                        _velocityComponent.AddX(SlideHBoost * Mathf.Sign(_inputX));

                    // StartCoroutine(SlideCoroutine());
                
                    IsSliding = true;
                    _animator.Play("Slide");

                    _slideFxEm.enabled = IsGrounded;
                    _slideFx.Play();
                }
                
                /*
                 *
                 * People are at my house now, it looks like deelopment will take a halt for a bit. 
                 * 
                 */

                if (_inputSlideReleased && IsSliding)
                {
                    _rb.gravityScale = 1f;
                    MaxFall = 35f;
                    
                    IsSliding = false;
                    _slideTimer = 0.2f;
                
                    _animator.Play("Idle");
                    _slideFxEm.enabled = false;
                }
            }

            // Jumping
            {
                _coyoteTimer -= Time.deltaTime;
                _jumpBufferTimer -= Time.deltaTime;
                _variableJumpTimer -= Time.deltaTime;
                
                // (_coyoteTimer > 0f || IsGrounded)
                if (_jumpBufferTimer > 0f && _jumpCounter > 0)
                {
                    _audioManager.Play("player_jump");
                    
                    _coyoteTimer = -1f;
                    _jumpBufferTimer = -1f;
                    _jumpCounter -= 1;

                    _velocityComponent.AddX(JumpHBoost * Mathf.Sign(_inputX));
                    _velocityComponent.SetVelocityY(JumpForce);

                    Instantiate(_jumpFx, transform.position + Vector3.down * 1.35f, Quaternion.identity);

                        _variableJumpTimer = VariableJumpTime;
                    IsJumping = true;
                }

                // Variable Jump
                if (_variableJumpTimer > 0f && _inputJumpReleased)
                {
                    _variableJumpTimer = -1f;
                    if (_velocityComponent.Y > 0)
                        _velocityComponent.MultiplyY(VariableJumpMultiplier);

                    IsJumping = false;
                }
            }
            
            // Wlkaing
            {
                if (IsGrounded)
                    _walkTimer -= Time.deltaTime;
                
                if (_walkTimer < 0f && Mathf.Abs(_inputX) > 0.0001f)
                {
                    _walkTimer = WalkSFXTime;
                    _audioManager.Play("player_walk");
                }
            }
            
            // Velocity Y
            {
                if (!IsGrounded)
                {
                    if (_velocityComponent.Y <= -MaxFall)
                        _velocityComponent.SetVelocityY(-MaxFall);
                }
            }

            _velocityComponent.AddX(AdditionalVelocity.x);
            _velocityComponent.AddY(AdditionalVelocity.y);
            AdditionalVelocity = Vector2.zero;
            
            _rb.velocity = new Vector2(_velocityComponent.X, _velocityComponent.Y);
        }

        private IEnumerator SlideCoroutine()
        {
            _rb.gravityScale = 0f;
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);

            yield return new WaitForSeconds(0.25f);
            
            _rb.gravityScale = 1f;
        }

        private void FixedUpdate()
        {
            _velocityComponent.SetVelocity(_rb.velocity);

            // Horizontal
            {
                float accel = RunAccel;
                bool sameDir = Math.Abs(Mathf.Sign(_velocityComponent.X) - Mathf.Sign(_inputX)) < 0.00001f;
                
                if (Mathf.Abs(_velocityComponent.X) > MaxRunSpeed && sameDir)
                    accel = RunReduce;
                
                if (Mathf.Abs(_inputX) > 0f && !sameDir)
                    accel *= 2f;

                _velocityComponent.ApproachX(_inputX * MaxRunSpeed, accel);
            }

            _rb.velocity = new Vector2(_velocityComponent.X, _velocityComponent.Y);
        }

        public void Ascend()
        {
            _sprite.sortingOrder = 1000;
            
            if (_sprite.flipX)
                _sprite.flipX = false;
            
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _animator.Play("Ascend");
            _audioManager.Play("player_ascend");
        }

        public void Kill()
        {
            HealthComponent.TakeDamage(999f);
        }

        public void ResetYVelocity()
        {
            _velocityComponent.SetVelocityY(0f);
            AdditionalVelocity = new Vector2(AdditionalVelocity.x, 0f);
        }
    }
}