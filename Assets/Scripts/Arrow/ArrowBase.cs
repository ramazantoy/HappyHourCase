using System;
using Interfaces;
using UnityEngine;

namespace Arrow
{
    /// <summary>
    /// Arrow'ların base class'ı arrowlar bu class'dan inherit alıp farklı özellikleri  kendi içlerinde kullanacaklar.
    /// Arrow'un hareket etmesi ayarlanması gibi işlemler bu sınıf aracılığıyla yapılıyor.
    /// </summary>
    public abstract class ArrowBase : MonoBehaviour
    {

        [SerializeField]
        private ArrowBaseDataContainer _arrowBaseDataContainer;

        public float BaseDamage => _arrowBaseDataContainer.Damage;

        protected float lifeTimer;
        protected bool isInitialized = false;
        protected Vector3 position;
        protected Vector3 velocity;
        protected Quaternion targetRotation;
        protected Vector3 worldUp = Vector3.up;


        // Fizik hesaplamaları FixedUpdate’de yapılıyor.

        protected virtual void Update()
        {
            if (!isInitialized) return;

            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                ReturnToPool();
         
    
            }
        }

        protected virtual void FixedUpdate()
        {
            UpdatePhysics();
        }

        protected virtual void UpdatePhysics()
        {
            var dt = Time.fixedDeltaTime;

            // Yerçekimi uygulaması
            velocity += Vector3.down * _arrowBaseDataContainer.Gravity * dt;

            // Hava direnci
            var speedSqr = velocity.sqrMagnitude;
            if (speedSqr > 0.01f)
            {
                Vector3 dragForce = -velocity.normalized * speedSqr * _arrowBaseDataContainer.AirResistance;
                velocity += dragForce * dt;
            }

            // Stabilizasyon
            if (velocity.sqrMagnitude > 5f && Vector3.Dot(velocity.normalized, Vector3.down) < 0.8f)
            {
                Vector3 verticalVelocity = Vector3.Project(velocity, Vector3.up);
                Vector3 horizontalVelocity = velocity - verticalVelocity;
                if (horizontalVelocity.magnitude > 0.1f)
                {
                    Vector3 stabilizationForce = -horizontalVelocity.normalized * _arrowBaseDataContainer.StabilizationFactor * dt;
                    velocity += stabilizationForce;
                }
            }

            // Pozisyon güncelleme
            position += velocity * dt;
            transform.position = position;

            // Rotasyon güncellemesi
            if (velocity.sqrMagnitude > 0.1f)
            {
                targetRotation = Quaternion.LookRotation(velocity.normalized, worldUp);
                var currentSpeed = velocity.magnitude;
                var speedRatio = Mathf.Clamp01(currentSpeed / (_arrowBaseDataContainer.MovementSpeed * 0.8f));
                var currentRotationSpeed = _arrowBaseDataContainer.RotationSmoothing * speedRatio;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dt * currentRotationSpeed);
            }
        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!isInitialized) return;
            if (!other.TryGetComponent(out IEnemy enemy)) return;

            enemy.TakeDamage(_arrowBaseDataContainer.Damage);


            ReturnToPool();
        }

        protected virtual void ReturnToPool()
        {
            isInitialized = false;
            velocity = Vector3.zero;

            gameObject.SetActive(false);
        }

        public virtual void Initialize(Vector3 startPosition, Vector3 direction)
        {
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction);
            position = startPosition;
            velocity = direction.normalized *_arrowBaseDataContainer. MovementSpeed;
            lifeTimer = _arrowBaseDataContainer.Lifetime;
            isInitialized = true;
        }
    }
}