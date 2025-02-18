using Enemy;
using Interfaces;
using Pool;
using UnityEngine;
using Zenject;

namespace Arrow
{
    public class ArrowProjectile : MonoBehaviour
    {


        [Inject]
        private static IEnemyManager _enemyManager;
        
        public float speed = 20f;
        public float damage = 10f;
        public float lifetime = 5f;
        private float lifeTimer;
        private Rigidbody rb;
        private bool isInitialized = false;

        // Skill etkilerine göre
        public bool bounceDamageActive = false;
        public int bounceCount = 1; // Rage Mode’da 2 olacak.
        public bool burnDamageActive = false;
        public float burnDuration = 3f; // Rage Mode’da 6 saniye.

        public void Initialize(Vector3 startPosition, Vector3 direction, float speed, float damage,
            bool bounceActive, int bounceCount, bool burnActive, float burnDuration) {
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction);
            this.speed = speed;
            this.damage = damage;
            this.bounceDamageActive = bounceActive;
            this.bounceCount = bounceCount;
            this.burnDamageActive = burnActive;
            this.burnDuration = burnDuration;

            if(rb == null)
                rb = GetComponent<Rigidbody>();
            rb.velocity = direction * speed;
            lifeTimer = lifetime;
            isInitialized = true;
        }

        private void Awake() {
            rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            if (!isInitialized) return;
            lifeTimer -= Time.deltaTime;
            if(lifeTimer <= 0f) {
                Deactivate();
            }
            // Raycast ile ileriye doğru çarpışma kontrolü (kolayca OnCollisionEnter ile de yapılabilir)
            RaycastHit hit;
            if(Physics.Raycast(transform.position, rb.velocity.normalized, out hit, speed * Time.deltaTime)) {
                Enemy.EnemyBase enemyBase = hit.collider.GetComponent<Enemy.EnemyBase>();
                if(enemyBase != null) {
                    enemyBase.TakeDamage(damage);
                    // Burn Damage aktifse enemy’ye ek hasar (stackable burn) uygulayabiliriz.
                    if(burnDamageActive) {
                        // Örneğin: enemy.ApplyBurn(burnDuration);
                    }
                    // Bounce Damage: Eğer aktifse, kalan bounce hakkına göre ok hedefi değiştirir.
                    if(bounceDamageActive && bounceCount > 0) {
                       
                        var nextEnemyBase = _enemyManager.GetNearestEnemy(hit.point);
                        if(nextEnemyBase != null && nextEnemyBase != enemyBase) {
                            var newDir = (nextEnemyBase.transform.position - hit.point).normalized;
                            bounceCount--;
                            Initialize(hit.point, newDir, speed, damage, bounceDamageActive, bounceCount, burnDamageActive, burnDuration);
                            return;
                        }
                    }
                }
                Deactivate();
            }
        }

        private void Deactivate() {
            isInitialized = false;
            rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
            // Havuz sistemine geri gönderiyoruz.
           // ArrowPool.Instance.ReturnArrow(this);
        }
    }
}
