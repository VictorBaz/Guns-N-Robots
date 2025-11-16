using System;
using System.Collections;
using Script.Controller;
using Script.Enum;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace Script.Manager
{
    public class MiniGameManager : MonoBehaviour
    {
        #region Singleton

        private static MiniGameManager _instance;

        public static MiniGameManager Instance => _instance;

        #endregion
        
        #region Fields
        
        private int round;

        private int toSpawnEnemy;
        private int objectifEnemy;
            
        [SerializeField] private int defaultEnemyNumberToKill;
        
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
    
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        

        #endregion

        #region Observer

        private void OnEnable()
        {
            EventManager.OnGameStart += InitFirstRound;
            EventManager.OnEnemyKilled += UpdateEnemyCount;
            EventManager.OnRoundEnd += TransitionRound;
            EventManager.OnEnemySpawn += SpawnEnemy;
        }
        
        private void OnDisable()
        {
            EventManager.OnGameStart -= InitFirstRound;
            EventManager.OnEnemyKilled -= UpdateEnemyCount;
            EventManager.OnRoundEnd -= TransitionRound;
            EventManager.OnEnemySpawn -= SpawnEnemy;
        }

        #endregion

        #region MiniGame
        
        public void StartInGame()
        {
            if (GameManager.Instance.CurrentState == GameState.InGame || GameManager.Instance.CurrentState == GameState.GameFinished) 
            {
                GameManager.Instance.ChangeGameState(GameState.MiniGameRunning);
                EventManager.StartGame();
            }
        }
        public bool IsGameRunning()
        {
            return GameManager.Instance.CurrentState == GameState.MiniGameRunning;
        }

        public bool CanSpawn() => toSpawnEnemy > 0;
        
        #endregion

        #region Round System

        private void SpawnEnemy()
        {
            toSpawnEnemy--;
        }
        
        private void UpdateEnemyCount()
        {
            objectifEnemy--;

            if (objectifEnemy == 0 && toSpawnEnemy == 0)
            {
                EventManager.EndRound();
            }
        }

        private void InitFirstRound()
        {
            round = 0;
            objectifEnemy = defaultEnemyNumberToKill;
            toSpawnEnemy = defaultEnemyNumberToKill;
        }

        private void TransitionRound()
        {
            StartCoroutine(TransitionRoundCoroutine());
        }
        
        private IEnumerator TransitionRoundCoroutine()
        {
            round++;
            yield return new WaitForSeconds(5);
            
            toSpawnEnemy = defaultEnemyNumberToKill;
            objectifEnemy = defaultEnemyNumberToKill;
            EventManager.RoundStart();
        } 

        #endregion
        
        
    }
}