using System;
using UnityEngine;

namespace Arrow
{
    public abstract class ArrowBase : MonoBehaviour
    {
        [SerializeField] protected float speed = 20f;
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float lifetime = 5f;
        
        
        protected float lifeTimer;
        protected Rigidbody rb;
        protected bool isInitialized = false;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            if (!isInitialized) return;
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Deactivate();
            }
        }

        protected virtual void Deactivate()
        {
            isInitialized = false;
            rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Okun başlangıç pozisyonu, yönü, hızı ve hasarı belirlenir.
        /// </summary>
        public virtual void Initialize(Vector3 startPosition, Vector3 direction, float speed, float damage)
        {
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction);
            this.speed = speed;
            this.damage = damage;
            rb.velocity = direction * speed;
            lifeTimer = lifetime;
            isInitialized = true;
        }

        protected abstract void OnHit(RaycastHit hit);

        protected virtual void FixedUpdate()
        {
            if (!isInitialized) return;
            if (Physics.Raycast(transform.position, rb.velocity.normalized, out RaycastHit hit, speed * Time.deltaTime))
            {
                OnHit(hit);
            }
        }
    }
}