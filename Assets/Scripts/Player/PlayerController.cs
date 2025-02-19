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
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerDataContainer _playerDataContainer;
        public float RotationSpeed => _playerDataContainer.RotationSpeed;
        public float BaseAttackSpeed => _playerDataContainer.BaseAttackSpeed;
        
        public Transform ArrowTransform => arrowTransform;
        
        
        [Header("**Editor Values**")]
        [SerializeField] private PlayerState currentPlayerState;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform arrowTransform;
    

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
            AnimationHandler = new AnimatorHandler(animator);

            idleState = new IdleState(this);
            moveState = new MoveState(this);
            // ShootState artık arrow pool referanslarını da alıyor.
            shootState = new ShootState(this, _enemyManager, _basicArrowPool, _bounceArrowPool, _burnArrowPool);

            currentState = idleState;
            currentPlayerState = PlayerState.Idle;
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

            currentPlayerState = currentState switch
            {
                IdleState => PlayerState.Idle,
                MoveState => PlayerState.Move,
                ShootState => PlayerState.Shoot,
                _ => currentPlayerState
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

        // Animasyon event'inden çağrılır.
        public void OnAttackAnimationEvent()
        {
            if (currentPlayerState != PlayerState.Shoot) return;
            shootState.HandleAttackAnimationEvent();
        }

        private void OnDestroy()
        {
            moveState.Exit();
            idleState.Exit();
            shootState.Exit();
        }
    }
}
