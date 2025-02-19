using UnityEngine;
using Enemy;

namespace Arrow
{
    public class BurnArrow : ArrowBase
    {
        public float burnDuration = 3f; // Standart modda 3 sn, Rage Mode'da 6 sn olabilir.

        protected override void OnHit(RaycastHit hit)
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                // Burada yanma etkisinin uygulanması yapılır.
                // Örneğin: enemy.ApplyBurn(burnDuration);
            }
            Deactivate();
        }
    }
}