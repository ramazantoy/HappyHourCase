using UnityEngine;
using Interfaces;
using Zenject;
using Pool;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Arrow
{
    public class BurnArrow : ArrowBase
    {
        [Inject] private ObjectPool<BurnArrow> _pool;
        [SerializeField] private float burnDuration = 3f;
        [SerializeField] private ParticleSystem flameEffect;
        [SerializeField] private Light fireLight;
        [SerializeField] private float fireLightIntensity = 1.5f;
        [SerializeField] private Color fireColor = new Color(1f, 0.7f, 0.3f);

        private CancellationTokenSource _cts;

        protected override void Awake()
        {
            base.Awake();
            _cts = new CancellationTokenSource();
        }

        public override void Initialize(Vector3 startPosition, Vector3 direction)
        {
            base.Initialize(startPosition, direction);
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            if (flameEffect != null)
            {
                var main = flameEffect.main;
                main.startSize = 0.3f;
                flameEffect.Play();
                UpdateFlameEffectBySpeedAsync(_cts.Token).Forget();
            }

            if (fireLight != null)
            {
                fireLight.intensity = fireLightIntensity * 0.3f;
                fireLight.color = fireColor;
            }
        }

        private async UniTaskVoid UpdateFlameEffectBySpeedAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (isInitialized && !cancellationToken.IsCancellationRequested)
                {
                    if (flameEffect != null)
                    {
                        float speedRatio = Mathf.Clamp01(velocity.magnitude / speed);
                        var main = flameEffect.main;
                        main.startSize = Mathf.Lerp(0.2f, 0.5f, speedRatio);
                        var emission = flameEffect.emission;
                        emission.rateOverTime = Mathf.Lerp(10, 20, speedRatio);
                    }

                    await UniTask.Yield(cancellationToken);
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                ApplyBurnDamageOverTimeAsync(enemy, _cts.Token).Forget();
            }

            StartBurningEffect();
        }

        private async UniTaskVoid ApplyBurnDamageOverTimeAsync(Interfaces.IEnemy enemy,
            CancellationToken cancellationToken)
        {
            float elapsed = 0;
            float tickInterval = 0.5f;
            float tickDamage = damage * 0.1f;
            try
            {
                while (elapsed < burnDuration && !cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Delay((int)(tickInterval * 1000), cancellationToken: cancellationToken);
                    elapsed += tickInterval;
                    enemy.TakeDamage(tickDamage * (1f - (elapsed / burnDuration)));
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }

        private void StartBurningEffect()
        {
            isInitialized = false;
            velocity = Vector3.zero;
            // Okun yüzeye saplanmasını engellemek için pozisyon ve rotasyon ayarı yapılır.
            Vector3 hitDir = transform.forward;
            transform.position = transform.position - hitDir * 0.02f;
            Vector3 reflectDir = Vector3.Reflect(transform.forward, Vector3.up);
            reflectDir = Vector3.Lerp(reflectDir, -Vector3.up, 0.4f);
            transform.rotation = Quaternion.LookRotation(reflectDir);
            if (flameEffect != null)
            {
                var main = flameEffect.main;
                main.startSize = 1.0f;
                var emission = flameEffect.emission;
                emission.rateOverTime = 25;
            }

            if (fireLight != null)
            {
                PulsatingLightAsync(_cts.Token).Forget();
            }

            DelayedReturnAsync(Mathf.Min(burnDuration * 0.7f, 2f), _cts.Token).Forget();
        }

        private async UniTaskVoid PulsatingLightAsync(CancellationToken cancellationToken)
        {
            float startTime = Time.time;
            float duration = Mathf.Min(burnDuration, 2f);
            try
            {
                while (Time.time < startTime + duration && !cancellationToken.IsCancellationRequested)
                {
                    float timeProgress = (Time.time - startTime) / duration;
                    float noise = Mathf.PerlinNoise(Time.time * 4f, 0) * 0.3f;
                    float pulseValue = 0.8f + Mathf.PingPong((Time.time - startTime) * 3f, 0.4f) + noise;
                    float fadeMultiplier = 1f - Mathf.Pow(timeProgress, 2f);
                    if (fireLight != null)
                    {
                        fireLight.intensity = fireLightIntensity * pulseValue * fadeMultiplier;
                        fireLight.range = 3f + pulseValue * 0.5f;
                        Color currentColor = Color.Lerp(fireColor, new Color(0.8f, 0.3f, 0.1f), timeProgress);
                        fireLight.color = currentColor;
                    }

                    await UniTask.Yield(cancellationToken);
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }

        private async UniTaskVoid DelayedReturnAsync(float delay, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay((int)(delay * 1000), cancellationToken: cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    ReturnToPool();
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }

        protected override void ReturnToPool()
        {
            _cts?.Cancel();
            if (flameEffect != null)
            {
                flameEffect.Stop();
            }

            if (fireLight != null)
            {
                fireLight.intensity = 0;
            }

            base.ReturnToPool();
            _pool.Despawn(this);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}