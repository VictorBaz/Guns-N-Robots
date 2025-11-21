using Script.Ennemys;
using UnityEngine;

namespace Script.Interface
{
    public interface IEnemy
    {
        void OnEnemyDeath();
        
        void SetParametersOnSpawn(EnemyManager enemyManager, int index, Transform playerPosition);

        void KillPlayer();

        void InitPosition();

        void ClearEnemy();

        void DestroyItSelf();
    }
}