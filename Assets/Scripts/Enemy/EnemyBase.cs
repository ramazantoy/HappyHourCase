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
        IEnemyManager _enemyManager;

        private SpawnPoint _spawnPoint;

        [Inject]
        public void Constructor(IEnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }

        [SerializeField] private GameObject _flameParticle; // Yanma efekti için particle sistemi
        private int _burnStack = 0;
        private CancellationTokenSource _burnCTS; // Yanma işlemini iptal etmek için

        private float _health = 100f;

        public Vector3 Position => transform.position;

        [SerializeField] private Image _fillImage; // Can barı fill image
        [SerializeField] private Color _fullHealthColor = Color.green; // Tam can rengi
        [SerializeField] private Color _midHealthColor = Color.yellow; // Orta can rengi
        [SerializeField] private Color _lowHealthColor = Color.red; // Düşük can rengi

        protected void OnEnable()
        {
            _health = 100f;
            _enemyManager.RegisterEnemy(this);
            _burnCTS = new CancellationTokenSource();
            UpdateFillColor(); // Can barı rengini güncelle
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

            // Yanma işlemini durdur
            _burnCTS?.Cancel();
            _burnCTS?.Dispose();
        }

        public virtual void TakeDamage(float damage)
        {
            
            if(!gameObject.activeInHierarchy) return;
            
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
            _burnStack++; // Yanma stack sayısını artır
            if (!_flameParticle.activeSelf)
            {
                _flameParticle.SetActive(true); // Yanma efekti aktif değilse aktif et
            }

            // Her bir yanma efekti için ayrı bir işlem başlat
            BurnAsync(damagePerSecond, duration, _burnCTS.Token).Forget();
        }

        private async UniTaskVoid BurnAsync(float damagePerSecond, float duration, CancellationToken token)
        {
            float endTime = Time.time + duration;

            try
            {
                while (Time.time < endTime && _health > 0 && !token.IsCancellationRequested)
                {
                    TakeDamage(damagePerSecond); // Saniyede hasar ver
                    await UniTask.Delay(1000, cancellationToken: token); // 1 saniye bekle
                }
            }
            catch (System.OperationCanceledException)
            {
                // İptal edildiğinde buraya düşer
            }
            finally
            {
                _burnStack--; // Yanma stack sayısını azalt
                if (_burnStack <= 0)
                {
                    _flameParticle.SetActive(false); // Yanma efekti bitince particle'ı kapat
                }
            }
        }

        /// <summary>
        /// Can barı rengini günceller.
        /// </summary>
        private void UpdateFillColor()
        {
            float healthRatio = _health / 100f;

            if (healthRatio > 0.5f)
            {
                // Yeşilden sarıya geçiş
                _fillImage.color = Color.Lerp(_midHealthColor, _fullHealthColor, (healthRatio - 0.5f) * 2f);
            }
            else
            {
                // Sarıdan kırmızıya geçiş
                _fillImage.color = Color.Lerp(_lowHealthColor, _midHealthColor, healthRatio * 2f);
            }
        }
    }
}