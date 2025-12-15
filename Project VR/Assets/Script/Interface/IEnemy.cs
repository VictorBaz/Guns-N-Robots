using Script.Ennemys;
using UnityEngine;

namespace Script.Interface
{
    public interface IEnemy
    {
        /// <summary>
        /// INTERFACE UTILISER PLUTÔT QUE HÉRITAGE VU QUE PAS DE SIMILITUDE
        /// DANS COMPORTEMENTS DE ENEMY RANGE ET ENEMY MELEE
        ///
        /// pas tellement d'accord, y'aurait surement un bout à mettre dans l'abstract, par exemple la gestion du takeDamage, etc
        /// et si à terme vous mettez une barre de vie, vous allez duppliquer le code
        /// </summary>
        void OnEnemyDeath();
        
        void SetParametersOnSpawn(EnemyManager enemyManager, int index, Transform playerPosition);

        void KillPlayer();

        void InitPosition();

        void ClearEnemy();

        void DestroyItSelf();
    }
}