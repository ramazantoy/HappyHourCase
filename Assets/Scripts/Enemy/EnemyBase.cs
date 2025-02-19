using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using Zenject;
using System.Threading;
using UnityEngine.UI;

namespace Enemy
{
    /// <summary>
    /// Enemy'in base sınıfı ilerde başka enemyle eklenirse bu sınıftan inherit olup özellik olarak ayrıştırılabilirler
    /// </summary>
    public class EnemyBase : MonoBehaviour, IEnemy
    {
        [SerializeField]
        private EnemyBaseDataContainer _enemyBaseDataContainer;

        [SerializeField] private GameObject _flameParticle;

        [SerializeField] private Image _fillImage;

        public Vector3 Position => transform.position;
        
        private float _health = 100f;
        private int _burnStack = 0;
        
        private CancellationTokenSource _burnCTS;
        

        private SpawnPoint _spawnPoint;

        IEnemyManager _enemyManager;

        [Inject]
        public void Constructor(IEnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }
        
        protected void OnEnable()
        {
            _health = 100f;
            _enemyManager.RegisterEnemy(this);
            _burnCTS = new CancellationTokenSource();
            UpdateFillColor();
        }

        protected void OnDisable()
        {
            if (_spawnPoint != null)
            {
                _spawnPoint.isOccupied = false;
            }


            if (EnemyManager.IsQuitting) // Zenject bir hata veriyordu onu fixlemek için flag ekledim.
                return;

            if (_enemyManager != null)
            {
                _enemyManager.UnregisterEnemy(this);
            }

            _burnCTS?.Cancel();
            _burnCTS?.Dispose();
        }

        public virtual void TakeDamage(float damage)
        {
            if (EnemyManager.IsQuitting) 
                return;
            
            if (!gameObject.activeInHierarchy) return;

            _health -= damage;
            if (_health <= 0f)
            {
                _enemyManager.UnregisterEnemy(this);
                gameObject.SetActive(false);
            }

            _fillImage.fillAmount = _health / 100f;
            UpdateFillColor(); // Can barı rengini güncelle
        }

        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            _spawnPoint = spawnPoint;
            _spawnPoint.isOccupied = true;
        }

        /// <summary>
        /// Yanma efekti başlatır. Birden fazla çağrıldığında stacklenir.
        /// </summary>
        /// <param name="damagePerSecond">Saniyede verilecek hasar.</param>
        /// <param name="duration">Yanma süresi (saniye).</param>
        public void ApplyBurn(float damagePerSecond, float duration)
        {
            _burnStack++;
            if (!_flameParticle.activeSelf)
            {
                _flameParticle.SetActive(true);
            }

            BurnAsync(damagePerSecond, duration, _burnCTS.Token).Forget();
        }

        private async UniTaskVoid BurnAsync(float damagePerSecond, float duration, CancellationToken token)
        {
            var endTime = Time.time + duration;

            try
            {
                while (Time.time < endTime && _health > 0 && !token.IsCancellationRequested)
                {
                    TakeDamage(damagePerSecond);
                    await UniTask.Delay(1000, cancellationToken: token);
                }
            }
            catch (System.OperationCanceledException)
            {
            }
            finally
            {
                _burnStack--;
                if (_burnStack <= 0)
                {
                    _flameParticle.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Can barı rengini günceller.
        /// </summary>
        private void UpdateFillColor()
        {
            var healthRatio = _health / 100f;
            _fillImage.color = healthRatio > 0.5f
                ? Color.Lerp(_enemyBaseDataContainer.MidHealthColor, _enemyBaseDataContainer.FullHealthColor,
                    (healthRatio - 0.5f) * 2f)
                : Color.Lerp(_enemyBaseDataContainer.LowHealthColor, _enemyBaseDataContainer.MidHealthColor,
                    healthRatio * 2f);
        }
    }
}