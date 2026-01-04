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

        [SerializeField] private int minEnemySpawn;
        
        [SerializeField] private int maxEnemySpawn;
        
        [SerializeField] private int maxEnemyPallier;

        [SerializeField] private bool isTutorial = false;

        private int enemyKilled = 0;
        
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
        }

        private void Start()
        {
            if (isTutorial)
            {
                StartCoroutine(StartInGameIe());
            }
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            if (isTutorial) return;
            
            EventManager.OnGameStart += InitFirstRound;
            EventManager.OnEnemyKilled += UpdateEnemyCount;
            EventManager.OnRoundEnd += TransitionRound;
            EventManager.OnEnemySpawn += SpawnEnemy;
            EventManager.OnGameStart += ResetStats;
        }

        private void OnDisable()
        {
            if (isTutorial) return;
            
            EventManager.OnGameStart -= InitFirstRound;
            EventManager.OnEnemyKilled -= UpdateEnemyCount;
            EventManager.OnRoundEnd -= TransitionRound;
            EventManager.OnEnemySpawn -= SpawnEnemy;
            EventManager.OnGameStart -= ResetStats;
        }

        #endregion

        #region MiniGame

        private IEnumerator StartInGameIe()
        {
            yield return new WaitForEndOfFrame();
            StartInGame();
        }
        
        public void StartInGame()
        {
            if (GameManager.Instance.CurrentState != GameState.Menu &&
                GameManager.Instance.CurrentState != GameState.GameFinished
                && GameManager.Instance.CurrentState != GameState.Tuto) return;
            
            if (!isTutorial)    GameManager.Instance.ChangeGameState(GameState.Game);

            EventManager.StartGame();
        }
        public bool IsGameRunning()
        {
            return GameManager.Instance.CurrentState == GameState.Game;
        }

        public bool CanSpawn()
        {
            if (isTutorial) return true;
            return toSpawnEnemy > 0;
        }

        public int ScorePlayer() => enemyKilled;
        
        
        #endregion

        #region Round System

        private void SpawnEnemy()
        {
            toSpawnEnemy--;
        }
        
        private void UpdateEnemyCount()
        {
            objectifEnemy--;
            enemyKilled++;
            
            if (objectifEnemy == 0 && toSpawnEnemy == 0)
            {
                EventManager.EndRound();
            }
        }

        private void InitFirstRound()
        {
            round = 0;
            SetEnemyCountForRound();
        }

        private void TransitionRound()
        {
            StartCoroutine(TransitionRoundCoroutine());
        }
        
        private IEnumerator TransitionRoundCoroutine()
        {
            round++;
            yield return new WaitForSeconds(5);
            
            SetEnemyCountForRound();
            EventManager.RoundStart();
        } 
        
        private void SetEnemyCountForRound()
        {
            float progression = round * 0.5f;
            int baseEnemies = UnityEngine.Random.Range(minEnemySpawn, maxEnemySpawn + 1);
            int enemiesThisRound = Mathf.RoundToInt(baseEnemies + progression);
            enemiesThisRound = Mathf.Min(enemiesThisRound, maxEnemyPallier);
            objectifEnemy = enemiesThisRound;
            toSpawnEnemy = enemiesThisRound;
        }

        public int GetCurrentRound() => round;
        
        private void ResetStats()
        {
            enemyKilled = 0;
        }

        #endregion
        
    }
}