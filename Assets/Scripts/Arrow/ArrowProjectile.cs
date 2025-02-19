using System;
using Enemy;
using Interfaces;
using Pool;
using UnityEngine;
using Zenject;

namespace Arrow
{
    public class ArrowProjectile : MonoBehaviour
    {
        [Inject] private static IEnemyManager _enemyManager;
        

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

        private void Initialize(Vector3 startPosition, Vector3 direction, float speed, float damage,
            bool bounceActive, int bounceCount, bool burnActive, float burnDuration)
        {
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction);
            this.speed = speed;
            this.damage = damage;
            this.bounceDamageActive = bounceActive;
            this.bounceCount = bounceCount;
            this.burnDamageActive = burnActive;
            this.burnDuration = burnDuration;

            rb.velocity = direction * speed;
            lifeTimer = lifetime;
            isInitialized = true;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!Physics.Raycast(transform.position, rb.velocity.normalized, out var hit,
                    speed * Time.deltaTime)) return;
            
            
            var enemyBase = hit.collider.GetComponent<EnemyBase>();
            
            if (enemyBase == null) return;
            
            enemyBase.TakeDamage(damage);
            if (burnDamageActive)
            {
                // Örneğin: enemy.ApplyBurn(burnDuration);
            }

            if (!bounceDamageActive || bounceCount <= 0) return;
            
            var nextEnemyBase = _enemyManager.GetNearestEnemy(hit.point);
            
            if (nextEnemyBase == null || nextEnemyBase == enemyBase) return;
            
            var newDir = (nextEnemyBase.transform.position - hit.point).normalized;
            bounceCount--;
            Initialize(hit.point, newDir, speed, damage, bounceDamageActive, bounceCount,
                burnDamageActive, burnDuration);
            return;
        }

        private void Update()
        {
            if (!isInitialized) return;
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Deactivate();
            }
        
        }

        private void Deactivate()
        {
            isInitialized = false;
            rb.velocity = Vector3.zero;
            gameObject.SetActive(false);

        }

        public void InitializeWithVelocity(Vector3 startPosition, Vector3 velocity, float damage,
            bool bounceActive, int bounceCount, bool burnActive, float burnDuration)
        {
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(velocity);
            this.damage = damage;
            this.bounceDamageActive = bounceActive;
            this.bounceCount = bounceCount;
            this.burnDamageActive = burnActive;
            this.burnDuration = burnDuration;

            rb.velocity = velocity;
            lifeTimer = lifetime;
            isInitialized = true;
        }
    }
}