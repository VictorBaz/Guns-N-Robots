using Script.Ennemys;
using Script.Enum;
using Script.Interface;
using UnityEngine;

namespace Script.Utility
{
    public class EnemyAnimationEvents : MonoBehaviour
    {
        private IEnemy enemy;

        private void Awake()
        {
            enemy = GetComponentInParent<IEnemy>();
        }
        
        public void KillPlayer()
        {
            enemy?.KillPlayer();
        }

        public void OnDeathAnimationComplete()
        {
            if (enemy is EnemyRange or EnnemyBehaviour)
            {
                enemy.OnDeathAnimationComplete();
            }
        }

        public void PlayAttackSoundMelee()
        {
            if (enemy is EnnemyBehaviour melee)
            {
                melee.PlayAttackSoundMelee();
            }
        }

        public void ShootRaycast()
        {
            if (enemy is EnemyRange range)
            {
                range.ShootRaycast();
            }
        }

        public void TransitionToState(EnemyRangeState state)
        {
            if (enemy is EnemyRange range)
            {
                range.TransitionToState(state);
            }
        }
    }
}
