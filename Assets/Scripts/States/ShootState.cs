using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Interfaces;
using Player;
using Arrow;
using Pool;

namespace States
{
    /// <summary>
    /// Karakterin shoot state'i arrowların spawn edilmesi hedefe göre setlenmesi gibi işlemleri yapıyor.
    /// CTS token kullanıyor yani async bir işlem yaparken bu state'den çıkılırsa işlemleri iptal ediyor.
    /// Örneğin çoklu ok spawn mekaniği animasyondan gelen event'a göre çalışmıyor async bir işlem eğer player harekete geçerse çoklu ok spawn işlemi iptal oluyor.
    /// 
    /// </summary>
    public class ShootState : ICharacterState
    {
        private readonly PlayerController _playerController;
        private readonly IEnemyManager _enemyManager;
        private CancellationTokenSource _attackCTS;


        private int currentBaseArrowCount;
        private float currentComputedAttackSpeed;


        //Inject
        private readonly ObjectPool<BasicArrow> _basicArrowPool;
        private readonly ObjectPool<BounceArrow> _bounceArrowPool;
        private readonly ObjectPool<BurnArrow> _burnArrowPool;

        public ShootState(PlayerController playerController, IEnemyManager enemyManager,
            ObjectPool<BasicArrow> basicArrowPool,
            ObjectPool<BounceArrow> bounceArrowPool,
            ObjectPool<BurnArrow> burnArrowPool)
        {
            _playerController = playerController;
            _enemyManager = enemyManager;
            _basicArrowPool = basicArrowPool;
            _bounceArrowPool = bounceArrowPool;
            _burnArrowPool = burnArrowPool;
        }

        public void Enter()
        {
            _playerController.AnimationHandler.SetAttack(true);
            _attackCTS = new CancellationTokenSource();
            SmoothRotateToY90(_attackCTS.Token).Forget();
            AttackRoutineAsync(_attackCTS.Token).Forget();
        }

        public void Exit()
        {
            if (_attackCTS != null)
            {
                _attackCTS.Cancel();
                _attackCTS.Dispose();
            }

            _playerController.AnimationHandler.SetAttack(false);
        }

        public void OnTick()
        {
        }

        private async UniTaskVoid AttackRoutineAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var attackSpeedFactor = 1f;
                    if (_playerController.playerSkills.attackSpeedIncrease)
                    {
                        attackSpeedFactor = _playerController.playerSkills.rageMode ? 4f : 2f;
                    }

                    currentComputedAttackSpeed = _playerController.BaseAttackSpeed * attackSpeedFactor;
                    _playerController.AnimationHandler.SetShootSpeed(currentComputedAttackSpeed);

                    currentBaseArrowCount = _playerController.playerSkills.arrowMultiplication
                        ? (_playerController.playerSkills.rageMode ? 4 : 2)
                        : 1;

                    await UniTask.Delay(TimeSpan.FromSeconds(1f / currentComputedAttackSpeed), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
            }
   
        }

        public void HandleAttackAnimationEvent()
        {
            FireArrows(currentBaseArrowCount);
            if (currentComputedAttackSpeed > 10f)
            {
                var extraMultiplier = Mathf.FloorToInt(currentComputedAttackSpeed / 10f);
                var extraShots = currentBaseArrowCount * extraMultiplier;
                SpawnExtraArrows(extraShots).Forget();
            }
        }

        private async UniTaskVoid SpawnExtraArrows(int extraShots)
        {
            var shotInterval = 0.1f;
            try
            {
                for (var i = 0; i < extraShots; i++)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(shotInterval),cancellationToken:_attackCTS.Token);
                    FireArrows(currentBaseArrowCount);
                }
            }
            catch (OperationCanceledException)
            {
          
            }
       
        }

        private void FireArrows(int arrowCount)
        {
            var spawnPoint = _playerController.ArrowTransform.position;
            var target = _enemyManager.GetNearestEnemy(_playerController.transform.position);
            if (target == null) return;


            var targetPosition = GetTargetBodyCenter(target);

            var arrowSpeed = 50f;
            var velocity = CalculateBallisticVelocity(spawnPoint, targetPosition, arrowSpeed);

            for (int i = 0; i < arrowCount; i++)
            {
                var finalVelocity = velocity;
                if (arrowCount > 1)
                {
                    float angleOffset = (i - (arrowCount - 1) / 2f) * 2f;
                    finalVelocity = Quaternion.Euler(0, angleOffset, 0) * velocity;
                }

                ArrowBase arrow;
                if (_playerController.playerSkills.bounceDamage)
                {
                    var bounceArrow = _bounceArrowPool.Spawn();
                    arrow = bounceArrow;
                    if (_playerController.playerSkills.rageMode)
                    {
                        bounceArrow.OnRageMode();
                    }
                }
                else if (_playerController.playerSkills.burnDamage)
                {
                    var burnArrow = _burnArrowPool.Spawn();

                    if (_playerController.playerSkills.rageMode)
                    {
                        burnArrow.OnRageMode();
                    }

                    arrow = burnArrow;
                }
                else
                {
                    arrow = _basicArrowPool.Spawn();
                }

                arrow.Initialize(spawnPoint, finalVelocity);
#if UNITY_EDITOR
                Debug.DrawRay(spawnPoint, finalVelocity.normalized * 10f, Color.red, 1f);    
#endif
             
            }
        }

        private Vector3 GetTargetBodyCenter(IEnemy target)
        {
            return target.Position + Vector3.up * 1.5f;
        }

        private async UniTask SmoothRotateToY90(CancellationToken token)
        {
            try
            {
                var targetRotation = Quaternion.Euler(
                    _playerController.transform.eulerAngles.x,
                    90f,
                    _playerController.transform.eulerAngles.z);
                while (!token.IsCancellationRequested &&
                       Quaternion.Angle(_playerController.transform.rotation, targetRotation) > 0.1f)
                {
                    _playerController.transform.rotation = Quaternion.Slerp(
                        _playerController.transform.rotation,
                        targetRotation,
                        Time.unscaledDeltaTime * _playerController.RotationSpeed);
                    await UniTask.Yield(token);
                }

                _playerController.transform.rotation = targetRotation;
            }
            catch (OperationCanceledException)
            {
            
            }
     
        }

        /// <summary>
        /// Verilen spawn ve hedef konumları, arrowSpeed ve yerçekimi kullanılarak
        /// balistik (parabolik) bir hız vektörü hesaplar.
        /// </summary>
        private Vector3 CalculateBallisticVelocity(Vector3 start, Vector3 target, float speed)
        {
            var toTarget = target - start;
            var toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
            var distance = toTargetXZ.magnitude;
            var yOffset = toTarget.y;
            var gravity = Mathf.Abs(Physics.gravity.y);
            var speedSq = speed * speed;

            var discriminant = speedSq * speedSq - gravity * (gravity * distance * distance + 2 * yOffset * speedSq);
            if (discriminant < 0)
            {
                return toTarget.normalized * speed;
            }

            var sqrtDiscriminant = Mathf.Sqrt(discriminant);
            var angle = Mathf.Atan((speedSq - sqrtDiscriminant) / (gravity * distance));
            var horizontalDir = toTargetXZ.normalized;
            var result = horizontalDir * speed * Mathf.Cos(angle) + Vector3.up * speed * Mathf.Sin(angle);
            return result;
        }
    }
}