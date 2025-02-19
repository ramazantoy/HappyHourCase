
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Interfaces
{
    
    /// <summary>
    /// Enemy manager'a ihtiyacı olan objelerin ihtiyacı olan fonksiyonları kullanması amacıyla tasarlandı.
    /// </summary>
    public interface IEnemyManager
    {
        void RegisterEnemy(EnemyBase enemyBase);
        void UnregisterEnemy(EnemyBase enemyBase);

        EnemyBase GetNearestEnemy(Vector3 pos);

        List<EnemyBase>  GetNearestEnemies(Vector3 pos, int order);
    }
}
