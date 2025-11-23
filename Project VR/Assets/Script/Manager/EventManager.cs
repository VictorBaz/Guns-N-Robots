using System;
using Script.Enum;

namespace Script.Manager
{
    public static class EventManager
    {
        public static Action OnRoundEnd;
        public static Action OnRoundStart;
        public static Action OnGameStart;
        public static Action OnGameEnd;

        public static Action OnEnemyKilled;
        public static Action OnEnemySpawn;

        public static Action<ShotDone> OnBadShoot;
        public static Action<ShotDone> OnGoodShot;
        public static Action<ShotDone> OnPerfectShot;

        public static void StartGame()
        {
            OnGameStart?.Invoke();
        }

        public static void EndRound()
        {
            OnRoundEnd?.Invoke();
        }

        public static void RoundStart()
        {
            OnRoundStart?.Invoke();
        }

        public static void GameEnd()
        {
            OnGameEnd?.Invoke();
        }

        public static void EnemyKilled()
        {
            OnEnemyKilled?.Invoke();
        }

        public static void EnemySpawn()
        {
            OnEnemySpawn?.Invoke();
        }

        public static void BadShot()
        {
            OnBadShoot?.Invoke(ShotDone.Bad);
        }

        public static void GoodShot()
        {
            OnGoodShot?.Invoke(ShotDone.Good);
        }

        public static void PerfectShot()
        {
            OnPerfectShot?.Invoke(ShotDone.Perfect);
        }
        
    }
}