using System;
using Interfaces;
using UnityEngine;

namespace Arrow
{
    public abstract class ArrowBase : MonoBehaviour
    {
        [SerializeField] protected float speed = 20f;

        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float lifetime = 5f;

        [SerializeField] protected float gravity = 9.8f;

        [SerializeField] protected float airResistance = 0.1f;
        [SerializeField] protected float stabilizationFactor = 5f;


        [SerializeField] protected float rotationSmoothing = 10f;

        protected float lifeTimer;
        protected bool isInitialized = false;
        protected Vector3 position;
        protected Vector3 velocity;
        protected Quaternion targetRotation;
        protected Vector3 worldUp = Vector3.up;


        // Fizik hesaplamaları FixedUpdate’de yapılıyor.
        protected virtual void FixedUpdate()
        {
            if (!isInitialized) return;

            lifeTimer -= Time.fixedDeltaTime;
            if (lifeTimer <= 0f)
            {
                ReturnToPool();
                return;
            }

            UpdatePhysics();
        }

        protected virtual void UpdatePhysics()
        {
            float dt = Time.fixedDeltaTime;

            // Yerçekimi uygulaması
            velocity += Vector3.down * gravity * dt;

            // Hava direnci
            float speedSqr = velocity.sqrMagnitude;
            if (speedSqr > 0.01f)
            {
                Vector3 dragForce = -velocity.normalized * speedSqr * airResistance;
                velocity += dragForce * dt;
            }

            // Stabilizasyon
            if (velocity.sqrMagnitude > 5f && Vector3.Dot(velocity.normalized, Vector3.down) < 0.8f)
            {
                Vector3 verticalVelocity = Vector3.Project(velocity, Vector3.up);
                Vector3 horizontalVelocity = velocity - verticalVelocity;
                if (horizontalVelocity.magnitude > 0.1f)
                {
                    Vector3 stabilizationForce = -horizontalVelocity.normalized * stabilizationFactor * dt;
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
                float currentSpeed = velocity.magnitude;
                float speedRatio = Mathf.Clamp01(currentSpeed / (speed * 0.8f));
                float currentRotationSpeed = rotationSmoothing * speedRatio;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dt * currentRotationSpeed);
            }
        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IEnemy enemy)) return;

            enemy.TakeDamage(damage);


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
            velocity = direction.normalized * speed;
            lifeTimer = lifetime;
            isInitialized = true;
        }
    }
}