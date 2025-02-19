using UnityEngine;
using Zenject;

namespace Pool
{
    /// <summary>
    /// Zenject MemoryPool tabanlı generic object pool.
    /// T, MonoBehaviour’den türetilmiş olmalı (örneğin ArrowBase türevleri).
    /// </summary>
    public  class ObjectPool<T> : MemoryPool<T> where T : MonoBehaviour
    {
        protected override void OnSpawned(T item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(T item)
        {
            item.gameObject.SetActive(false);
        }
    }
}