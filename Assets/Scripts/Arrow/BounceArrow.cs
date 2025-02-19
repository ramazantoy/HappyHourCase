using UnityEngine;
using Enemy;
using Zenject;
using Interfaces;

namespace Arrow
{
    public class BounceArrow : ArrowBase
    {
        [Inject] private IEnemyManager _enemyManager;
        public int bounceCount = 1; // Standart modda 1, Rage Mode'da 2 olabilir.

        protected override void OnHit(RaycastHit hit)
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (bounceCount > 0)
            {
                EnemyBase nextEnemy = _enemyManager.GetNearestEnemy(hit.point);
                if (nextEnemy != null && nextEnemy != enemy)
                {
                    Vector3 newDirection = (nextEnemy.transform.position - hit.point).normalized;
                    bounceCount--;
                    // Yeni hedefe doğru yönlendirilmek üzere ok yeniden başlatılır.
                    Initialize(hit.point, newDirection, speed, damage);
                    return;
                }
            }
            Deactivate();
        }
    }
}