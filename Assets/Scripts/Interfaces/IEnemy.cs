using UnityEngine;

namespace Interfaces
{
    public interface IEnemy 
    {
       void TakeDamage( float damage);
       
       Vector3 Position { get; }

       void ApplyBurn(float damagePerSecond, float duration);
    }
}
