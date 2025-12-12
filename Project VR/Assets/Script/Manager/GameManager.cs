using System;
using System.Collections;
using Script.Controller;
using Script.Enum;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// PAS DU TOUT EXPLOITER VRAIMENT INUTILE PRESQUE
        /// </summary>
        #region Singleton

        private static GameManager _instance;

        public static GameManager Instance => _instance;

        #endregion

        #region Fields
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Menu;
        
        [Header("Scene Names")]
        [SerializeField] private string menuSceneName = "Menu";
        [SerializeField] private string gameSceneName = "Game";
        
        public static event Action<GameState> OnGameStateChanged;
        
        public GameState CurrentState => currentState;

        public PlayerController playerRef;

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

        private void Start()
        {
            InitializeGameState();
        }

        

        #endregion

        #region Initialize

        private void InitializeGameState()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            if (currentSceneName == menuSceneName)
                ChangeGameState(GameState.Menu);
            else if (currentSceneName == gameSceneName)
                ChangeGameState(GameState.InGame);
        }

        #endregion
        
        #region Main Methodes

        //didnt exploit it sadly
        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            
            OnGameStateChanged?.Invoke(newState);
        }
        
        
        #endregion

        #region Loading Scène

        public void LoadMenuScene()
        {
            StartCoroutine(LoadSceneAndChangeState(menuSceneName, GameState.Menu));
        }

        public void LoadGameScene()
        {
            Time.timeScale = 1;
            StartCoroutine(LoadSceneAndChangeState(gameSceneName, GameState.InGame));
        }

        private IEnumerator LoadSceneAndChangeState(string sceneName, GameState newState)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (asyncLoad is { isDone: false })
            {
                yield return null;
            }
            
            ChangeGameState(newState);
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            EventManager.OnRoundEnd += PauseInGame;
            EventManager.OnRoundStart += RoundStart;
            EventManager.OnGameEnd += OnEndGame;
        }

        private void OnDisable()
        {
            EventManager.OnRoundEnd -= PauseInGame;
            EventManager.OnRoundStart -= RoundStart;
            EventManager.OnGameEnd -= OnEndGame;
        }

        private void RoundStart()
        {
            currentState = GameState.MiniGameRunning;
        }
        
        private void PauseInGame()
        {
            currentState = GameState.MiniGamePaused;
        }

        private void OnEndGame()
        {
            ChangeGameState(GameState.GameFinished);
        }

        #endregion
        
    }
}