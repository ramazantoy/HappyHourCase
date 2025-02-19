using UnityEngine;
using Enemy;

namespace Arrow
{
    public class BasicArrow : ArrowBase
    {
        protected override void OnHit(RaycastHit hit)
        {
            EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Deactivate();
        }
    }
}