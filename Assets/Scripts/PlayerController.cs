using System;
using Enemy;
using Handlers;
using Interfaces;
using States;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Shoot
    }

    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    public float rotationSpeed = 10f;

    [Inject] private IEnemyManager _enemyManager;
    [Inject] private IArrowPool _arrowPool;
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
    
    [SerializeField]
    private PlayerState currentPlayerState; // Editörde gözlemlenebilir state

    private void Start()
    {
        InputProvider = new JoystickInputProvider(_joystick);
        AnimationHandler = new AnimatorHandler(animator);

        idleState = new IdleState(this);
        moveState = new MoveState(this);
        shootState = new ShootState(this, _arrowPool, _enemyManager);

        currentState = idleState;
        currentPlayerState = PlayerState.Idle;
    }

    private void Update()
    {
        float horizontal = InputProvider.GetHorizontal();
        // Hareket input'u varsa, her durumda MoveState'e geç.
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            ChangeState(moveState);
        }
        else
        {
            // Eğer enemy varsa ShootState, yoksa IdleState'e geç.
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

    public void MoveCharacter(float horizontal)
    {
        Vector3 newPos = transform.position;
        newPos.x += horizontal * speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        transform.position = newPos;

        if (Mathf.Abs(horizontal) > 0.1f)
        {
            Vector3 targetDirection = new Vector3(horizontal, 0f, 0f);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        AnimationHandler.SetMovementSpeed(Mathf.Abs(horizontal));
    }

    // Bu metod, animasyon event'inden çağrılır.
    public void OnAttackAnimationEvent() {
        if (currentState is ShootState shootState)
        {
            shootState.HandleAttackAnimationEvent();
        }
    }

    private void OnDestroy()
    {
        moveState.Exit();
        idleState.Exit();
        shootState.Exit();
    }
}
