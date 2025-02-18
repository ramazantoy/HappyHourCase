using System.Collections.Generic;
using Arrow;
using Interfaces;
using UnityEngine;

namespace Pool
{
    public class ArrowPool : MonoBehaviour, IArrowPool {
   
        public ArrowProjectile arrowPrefab;
        
        public int initialPoolSize = 20;
        
        private Queue<ArrowProjectile> _poolObjects = new Queue<ArrowProjectile>();

        private void Awake() {
            InitializePool();
        }

        private void InitializePool() {
            for (var i = 0; i < initialPoolSize; i++) {
                ArrowProjectile arrow = Instantiate(arrowPrefab, transform);
                arrow.gameObject.SetActive(false);
                _poolObjects.Enqueue(arrow);
            }
        }

        public ArrowProjectile GetArrow() {
            if(_poolObjects.Count > 0) {
                ArrowProjectile arrow = _poolObjects.Dequeue();
                arrow.gameObject.SetActive(true);
                return arrow;
            } else {
                ArrowProjectile arrow = Instantiate(arrowPrefab, transform);
                return arrow;
            }
        }

        public void ReturnArrow(ArrowProjectile arrow) {
            _poolObjects.Enqueue(arrow);
        }
    }
}