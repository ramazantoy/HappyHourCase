
using Enemy;
using UnityEngine;

namespace Interfaces
{
    public interface IEnemyManager
    {
        void RegisterEnemy(EnemyBase enemyBase);
        void UnregisterEnemy(EnemyBase enemyBase);

        EnemyBase GetNearestEnemy(Vector3 pos);
    }
}
