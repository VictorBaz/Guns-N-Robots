using Script.Ennemys;
using UnityEngine;

namespace Script.Interface
{
    public interface IEnemy
    {
        /// <summary>
        /// INTERFACE UTILISER PLUTÔT QUE HÉRITAGE VU QUE PAS DE SIMILITUDE
        /// DANS COMPORTEMENTS DE ENEMY RANGE ET ENEMY MELEE
        /// </summary>
        void OnEnemyDeath();
        
        void SetParametersOnSpawn(EnemyManager enemyManager, int index, Transform playerPosition);

        void KillPlayer();

        void InitPosition();

        void ClearEnemy();

        void DestroyItSelf();
    }
}