using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy;
using Interfaces;
using Player;
using Zenject;
using Arrow;
using Pool;

namespace States
{
    public class ShootState : ICharacterState
    {
        private readonly PlayerController _playerController;
        private readonly IEnemyManager _enemyManager;
        private CancellationTokenSource _attackCTS;

        // Bu alanlar AttackRoutine'da hesaplanan parametreleri saklar.
        private int currentBaseArrowCount;
        private float currentComputedAttackSpeed;

        // Enjekte edilen arrow pool referansları.
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
            // Oyuncu ShootState'e girerken Y rotasyonunu yavaşça 90°'ye getir.
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
            while (!token.IsCancellationRequested)
            {
                float attackSpeedFactor = _playerController.playerSkills.attackSpeedIncrease
                    ? (_playerController.playerSkills.rageMode ? 4f : 2f)
                    : 1f;
                currentComputedAttackSpeed = _playerController.baseAttackSpeed * attackSpeedFactor;
                _playerController.AnimationHandler.SetShootSpeed(currentComputedAttackSpeed);

                currentBaseArrowCount = _playerController.playerSkills.arrowMultiplication ? 2 : 1;
                if (_playerController.playerSkills.rageMode)
                {
                    currentBaseArrowCount = _playerController.playerSkills.arrowMultiplication ? 4 : 2;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1f / currentComputedAttackSpeed), cancellationToken: token);
            }
        }

        public void HandleAttackAnimationEvent()
        {
            FireArrows(currentBaseArrowCount);
            if (currentComputedAttackSpeed > 10f)
            {
                int extraMultiplier = Mathf.FloorToInt(currentComputedAttackSpeed / 10f);
                int extraShots = currentBaseArrowCount * extraMultiplier;
                SpawnExtraArrows(extraShots);
            }
        }

        private async void SpawnExtraArrows(int extraShots)
        {
            float shotInterval = 0.2f;
            for (int i = 0; i < extraShots; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(shotInterval));
                FireArrows(currentBaseArrowCount);
            }
        }

        private void FireArrows(int arrowCount) {
            var spawnPoint = _playerController.ArrowTransform.position;
            var target = _enemyManager.GetNearestEnemy(_playerController.transform.position);
            if (target == null) return;

            // Hedefin gövdesinin merkezini al (Collider kullanarak)
            Vector3 targetPosition = GetTargetBodyCenter(target);

            var arrowSpeed = 50f;
            var velocity = CalculateBallisticVelocity(spawnPoint, targetPosition, arrowSpeed);

            for (int i = 0; i < arrowCount; i++) {
                Vector3 finalVelocity = velocity;
                if (arrowCount > 1) {
                    // Açısal ofseti dinamik olarak hesapla (daha küçük açılar kullan)
                    float angleOffset = (i - (arrowCount - 1) / 2f) * 2f; // 2° spread örneği
                    finalVelocity = Quaternion.Euler(0, angleOffset, 0) * velocity;
                }

                ArrowBase arrow;
                if (_playerController.playerSkills.bounceDamage) {
                    BounceArrow bounceArrow = _bounceArrowPool.Spawn();
                    arrow = bounceArrow;
                }
                else if (_playerController.playerSkills.burnDamage) {
                    BurnArrow burnArrow = _burnArrowPool.Spawn();
                    arrow = burnArrow;
                }
                else {
                    arrow = _basicArrowPool.Spawn();
                }

                arrow.Initialize(spawnPoint, finalVelocity);
                Debug.DrawRay(spawnPoint, finalVelocity.normalized * 10f, Color.red, 1f); // Debug çizgisi
            }
        }  
        
        private Vector3 GetTargetBodyCenter(IEnemy target) {
            
            return target.Position + Vector3.up * 1.5f;
        }




        private async UniTask SmoothRotateToY90(CancellationToken token)
        {
            Quaternion targetRotation = Quaternion.Euler(
                _playerController.transform.eulerAngles.x,
                90f,
                _playerController.transform.eulerAngles.z);
            while (!token.IsCancellationRequested &&
                   Quaternion.Angle(_playerController.transform.rotation, targetRotation) > 0.1f)
            {
                _playerController.transform.rotation = Quaternion.Slerp(
                    _playerController.transform.rotation,
                    targetRotation,
                    Time.unscaledDeltaTime * _playerController.rotationSpeed);
                await UniTask.Yield(token);
            }

            _playerController.transform.rotation = targetRotation;
        }

        /// <summary>
        /// Verilen spawn ve hedef konumları, arrowSpeed ve yerçekimi kullanılarak
        /// balistik (parabolik) bir hız vektörü hesaplar.
        /// </summary>
        private Vector3 CalculateBallisticVelocity(Vector3 start, Vector3 target, float speed) {
            Vector3 toTarget = target - start;
            Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
            float distance = toTargetXZ.magnitude;
            float yOffset = toTarget.y;
            float gravity = Mathf.Abs(Physics.gravity.y);
            float speedSq = speed * speed;

            float discriminant = speedSq * speedSq - gravity * (gravity * distance * distance + 2 * yOffset * speedSq);
            if (discriminant < 0) {
                // Eğer hedef çok uzaktaysa, doğrudan hedefe doğru düz bir hız vektörü kullan
                return toTarget.normalized * speed;
            }

            float sqrtDiscriminant = Mathf.Sqrt(discriminant);
            float angle = Mathf.Atan((speedSq - sqrtDiscriminant) / (gravity * distance));
            Vector3 horizontalDir = toTargetXZ.normalized;
            Vector3 result = horizontalDir * speed * Mathf.Cos(angle) + Vector3.up * speed * Mathf.Sin(angle);
            return result;
        }
    }
}