using Arrow;
using Handlers;
using Interfaces;
using Pool;
using Skills;
using States;
using UnityEngine;
using Zenject;

namespace Player
{
    
    /// <summary>
    /// Player Controller sınıfı oyuncunun hareket etmesi state değiştirmesi ateş etmesi gibi işlemleri yapıyor.
    /// State Machine aracılığıyla statelerini yönetiyorç
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerDataContainer _playerDataContainer;
        public float RotationSpeed => _playerDataContainer.RotationSpeed;
        public float BaseAttackSpeed => _playerDataContainer.BaseAttackSpeed;
        
        public Transform ArrowTransform => _arrowTransform;
        
        
    
        [Header("**Editor Values**")]
        [SerializeField] private PlayerState _currentPlayerState; 
        [SerializeField] private Animator _animator; 
        [SerializeField] private Transform _arrowTransform; 
        [SerializeField] private AudioSource _audioSource;
    

        [Inject] private IEnemyManager _enemyManager;
        
        public IInputProvider InputProvider { get; private set; }
        public IAnimationHandler AnimationHandler { get; private set; }

        [SerializeField] private VariableJoystick _joystick;
     


        public PlayerSkills playerSkills;

        
        [Inject] private ObjectPool<BasicArrow> _basicArrowPool;
        [Inject] private ObjectPool<BounceArrow> _bounceArrowPool;
        [Inject] private ObjectPool<BurnArrow> _burnArrowPool;

        private ICharacterState currentState;
        private IdleState idleState;
        private MoveState moveState;
        private ShootState shootState;

 

  

        private void Start()
        {
            InputProvider = new JoystickInputProvider(_joystick);
            AnimationHandler = new AnimatorHandler(_animator);

            idleState = new IdleState(this);
            moveState = new MoveState(this);
            shootState = new ShootState(this, _enemyManager, _basicArrowPool, _bounceArrowPool, _burnArrowPool);

            currentState = idleState;
            _currentPlayerState = PlayerState.Idle;
        }

        private void Update()
        {
            var movement = InputProvider.GetMovement();
            if (movement.magnitude > 0.1f)
            {
                ChangeState(moveState);
            }
            else
            {
                if (_enemyManager.GetNearestEnemy(transform.position) != null)
                {
                    ChangeState(shootState);
                }
                else
                {
                    ChangeState(idleState);
                }
            }

            currentState.OnTick();
        }

        private void ChangeState(ICharacterState newState)
        {
            if (currentState == newState) return;

            currentState.Exit();
            currentState = newState;

            _currentPlayerState = currentState switch
            {
                IdleState => PlayerState.Idle,
                MoveState => PlayerState.Move,
                ShootState => PlayerState.Shoot,
                _ => _currentPlayerState
            };

            currentState.Enter();
        }

        public void MoveCharacter(Vector3 movement)
        {
            var newPos = transform.position;
            newPos.x += movement.x * _playerDataContainer.MovementSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, _playerDataContainer.MovementClamps.MinX, _playerDataContainer.MovementClamps.MaxX);
            newPos.z += movement.z * _playerDataContainer.MovementSpeed * Time.deltaTime;
            newPos.z = Mathf.Clamp(newPos.z, _playerDataContainer.MovementClamps.MinZ, _playerDataContainer.MovementClamps.MaxZ);
            transform.position = newPos;

            if (movement.magnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _playerDataContainer.RotationSpeed);
            }

            AnimationHandler.SetMovementSpeed(movement.magnitude);
        }

 
        public void OnAttackAnimationEvent()
        {
            if (_currentPlayerState != PlayerState.Shoot) return;
            shootState.HandleAttackAnimationEvent();
            if (_playerDataContainer.ShootSfx)
            {
                _audioSource.PlayOneShot(_playerDataContainer.ShootClip);
            }
        }
        


        private void OnDestroy()
        {
            moveState.Exit();
            idleState.Exit();
            shootState.Exit();
        }
    }
}
