using Handlers;
using Interfaces;
using Skills;
using States;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;
        public float rotationSpeed = 10f;


        [Inject] private IEnemyManager _enemyManager;
        
        public IInputProvider InputProvider { get; private set; }
        
        public IAnimationHandler AnimationHandler { get; private set; }

        [SerializeField] private VariableJoystick _joystick;
        [SerializeField] private Transform arrowTransform;
        public Transform ArrowTransform => arrowTransform;

        public PlayerSkills playerSkills;
        public float baseAttackSpeed = 1f;

        private ICharacterState currentState;
        private IdleState idleState;
        private MoveState moveState;
        private ShootState shootState;

        [SerializeField] private PlayerState currentPlayerState;


        private void Start()
        {
            InputProvider = new JoystickInputProvider(_joystick);
            AnimationHandler = new AnimatorHandler(animator);

            idleState = new IdleState(this);
            moveState = new MoveState(this);
            shootState = new ShootState(this, /*, _arrowPool,*/ _enemyManager);

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
            newPos.x += movement.x * speed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.z += movement.z * speed * Time.deltaTime;
            newPos.z = Mathf.Clamp(newPos.z, -8, -3.5f);
            transform.position = newPos;

            if (movement.magnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            AnimationHandler.SetMovementSpeed(movement.magnitude);
        }

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