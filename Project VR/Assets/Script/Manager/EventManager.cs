using System;
using Script.Enum;

namespace Script.Manager
{
    public static class EventManager
    {
        #region Round and Game Event

        public static Action OnRoundEnd;
        public static Action OnRoundStart;
        public static Action OnGameStart;
        public static Action OnGameEnd;
        
        public static void StartGame()
        {
            OnGameStart?.Invoke();
        }

        public static void EndRound()
        {
            OnRoundEnd?.Invoke();

            if (DataManager.Instance != null)
            {
                DataManager.Instance.IncrementCell("B51");
            }
        }

        public static void RoundStart()
        {
            OnRoundStart?.Invoke();
        }

        public static void GameEnd()
        {
            OnGameEnd?.Invoke();
        }

        #endregion
        
        #region Enemy Event

        public static Action OnEnemyKilled;
        public static Action OnEnemySpawn;
        
        public static void EnemyKilled()
        {
            OnEnemyKilled?.Invoke();
        }

        public static void EnemySpawn()
        {
            OnEnemySpawn?.Invoke();
        }

        #endregion

        #region Shoot Event
        
        public static Action<ShotDone> OnShootState;
        
        public static void ShootState(ShotDone stateShot)
        {
            OnShootState?.Invoke(stateShot);
        }

        #endregion

        #region Player Event

        public static Action OnplayerShoot;
        public static Action OnReloadStart;
        public static Action OnPlayerReload;
        public static Action OnReloadEnd;
        
        public static void PlayerShoot()
        {
            OnplayerShoot?.Invoke();
        }
        
        public static void ReloadStart()
        {
            OnReloadStart?.Invoke();
        }
        
        public static void PlayerReload()
        {
            OnPlayerReload?.Invoke();
        }
        
        public static void ReloadEnd()
        {
            OnReloadEnd?.Invoke();
        }

        #endregion
        
    }
}