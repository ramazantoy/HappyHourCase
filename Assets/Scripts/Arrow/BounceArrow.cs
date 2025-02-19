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
        [SerializeField] protected TrailRenderer trailRenderer;
        [Inject] private IEnemyManager _enemyManager;
        [Inject] private ObjectPool<BounceArrow> _pool;

        [SerializeField] private int maxBounceCount = 1;
        [SerializeField] private float bounceSpeed = 25f;

        private int remainingBounces;
        private bool isBouncing = false;
        private CancellationTokenSource _cts;

        private List<IEnemy> _hitEnemies = new List<IEnemy>();

        protected void Awake()
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }
            _cts = new CancellationTokenSource();
        }

        public override void Initialize(Vector3 startPosition, Vector3 direction)
        {
            base.Initialize(startPosition, direction);

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
                trailRenderer.Clear();
            }
            isBouncing = false;
            _hitEnemies.Clear();
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            
        }

        public void OnRageMode()
        {
            remainingBounces = maxBounceCount * 2;
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            
     
            if (!isInitialized) return;
            
            if (!other.TryGetComponent(out IEnemy enemy)) return;
            
            if (isBouncing) return;

            if (!_hitEnemies.Contains(enemy))
            {
                enemy.TakeDamage(BaseDamage);
                _hitEnemies.Add(enemy);
            }
            
            isBouncing = true;
            BounceLoopAsync(_cts.Token).Forget();
        }
        
        private async UniTask BounceLoopAsync(CancellationToken token)
        {
            while (remainingBounces > 0 && !token.IsCancellationRequested)
            {
                await UniTask.Delay(50, cancellationToken: token);
                
                if (!gameObject.activeInHierarchy) break;
        
                var nextEnemy = FindNextEnemy();
                if (nextEnemy == null)
                {
                    break;
                }
        
                Vector3 nextTargetPos = nextEnemy.Position + Vector3.up * 1.5f;
                await PerformBounceAsync(transform.position, nextTargetPos, token);
        
                if (!_hitEnemies.Contains(nextEnemy))
                {
                    nextEnemy.TakeDamage(BaseDamage);
                    _hitEnemies.Add(nextEnemy);
                    remainingBounces--;
                }
            }
            ReturnToPool();
        }

     
        private async UniTask PerformBounceAsync(Vector3 startPoint, Vector3 targetPoint, CancellationToken cancellationToken)
        {
            var distance = Vector3.Distance(startPoint, targetPoint);
            var bounceTravelTime = distance / bounceSpeed; 
            var startTime = Time.time;
            var rotationSmoothSpeed = 10f;

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
        }

        private void RotateTowardsTarget(Vector3 targetPoint, float rotationSmoothSpeed)
        {
            var lookTarget = targetPoint;
            var desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
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

        protected override void Update()
        {
            if (!isInitialized) return;
            // Bounce loop kendi içinde remaining kontrolünü yapıyor.
        }

        protected override void ReturnToPool()
        {
            if (_cts != null)
            {
                _cts?.Cancel();
            }
    
            
            remainingBounces = maxBounceCount;
            
            base.ReturnToPool();
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
    
        }


        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts= null;
        }
    }
}
