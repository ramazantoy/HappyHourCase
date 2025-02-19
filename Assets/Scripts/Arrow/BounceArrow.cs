using UnityEngine;
using Interfaces;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Zenject;
using Pool;

namespace Arrow
{
    public class BounceArrow : ArrowBase
    {
        [Inject] private IEnemyManager _enemyManager;
        [Inject] private ObjectPool<BounceArrow> _pool;

        [SerializeField] private int maxBounceCount = 1;
        [SerializeField] private float bounceSpeed = 25f;

        private int remainingBounces;
        private bool isBouncing = false;

        private CancellationTokenSource _cts;

        // Vurulan enemy'leri tutmak için liste
        private List<IEnemy> _hitEnemies = new List<IEnemy>();

        protected override void Awake()
        {
            base.Awake();
            _cts = new CancellationTokenSource();
        }

        public override void Initialize(Vector3 startPosition, Vector3 direction)
        {
            base.Initialize(startPosition, direction);
            remainingBounces = maxBounceCount;
            isBouncing = false;
            _hitEnemies.Clear();
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
        }

        // Eğer bounce başladıysa, OnTriggerEnter çağrılarını yoksayalım.
        protected override void OnTriggerEnter(Collider other)
        {
       

            var enemy = other.GetComponent<IEnemy>();

            if (enemy != null && !_hitEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                _hitEnemies.Add(enemy);
            }

            if (remainingBounces > 0)
            {
                var nextEnemy = FindNextEnemy();

                if (nextEnemy != null && isBouncing == false)
                {
                    Vector3 nextTargetPos = nextEnemy.Position + Vector3.up * 1.5f;
                    PerformBounceAsync(transform.position, nextTargetPos, _cts.Token).Forget();
                    return;
                }
            }

            ReturnToPool();
        }

        private async UniTaskVoid PerformBounceAsync(Vector3 startPoint, Vector3 targetPoint, CancellationToken cancellationToken)
        {
            isBouncing = true;
            remainingBounces--;

            float distance = Vector3.Distance(startPoint, targetPoint);
            float bounceTravelTime = distance / bounceSpeed; // Dinamik travel time
            float startTime = Time.time;
            float rotationSmoothSpeed = 10f;

            try
            {
                while (Time.time < startTime + bounceTravelTime && !cancellationToken.IsCancellationRequested)
                {
                    float normalizedTime = (Time.time - startTime) / bounceTravelTime;
                    position = Vector3.Lerp(startPoint, targetPoint, normalizedTime);
                    transform.position = position;

                    RotateTowardsTarget(targetPoint, rotationSmoothSpeed);

                    await UniTask.Yield(cancellationToken);
                }
            }
            catch (System.OperationCanceledException)
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested) return;

            position = targetPoint;
            transform.position = position;

            var finalDirection = (targetPoint - transform.position).normalized;
            velocity = finalDirection * bounceSpeed;
            transform.rotation = Quaternion.LookRotation(finalDirection);
            lifeTimer = Mathf.Max(lifeTimer, 1.5f);
            isBouncing = false;
        }
        
        private void RotateTowardsTarget(Vector3 targetPoint, float rotationSmoothSpeed)
        {
            Vector3 lookTarget = targetPoint;
            Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
            desiredRotation.x = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
        }
        
        private IEnemy FindNextEnemy()
        {
            var candidates = _enemyManager.GetNearestEnemies(transform.position, 10);
            foreach (var cand in candidates)
            {
                if (!_hitEnemies.Contains(cand))
                {
                    return cand;
                }
            }
            return null;
        }


        protected override void ReturnToPool()
        {
            base.ReturnToPool();
            _cts?.Cancel();
            _pool.Despawn(this);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}