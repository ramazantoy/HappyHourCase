using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// Enemy'lerin interface'i ok bir enemye isabet ederse interface'i üzerinden enemy'de yer alan fonksiyonlardan sadece interface'de yer alanları tetikliyor.
    /// </summary>
    public interface IEnemy 
    {
       void TakeDamage( float damage);
       
       Vector3 Position { get; }

       void ApplyBurn(float damagePerSecond, float duration);
    }
}
