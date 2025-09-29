using System;
using System.Collections;
using Script.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Manager
{
    public class GameManageur : MonoBehaviour
    {
        private static GameManageur _instance;
        
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Menu;
        
        [Header("Scene Names")]
        [SerializeField] private string menuSceneName = "Menu";
        [SerializeField] private string gameSceneName = "Game";
        
        public static event Action<GameState> OnGameStateChanged;
        
        public static GameManageur Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManageur>();
                    
                    if (_instance == null)
                    {
                        GameObject gameManagerGO = new GameObject("GameManager");
                        _instance = gameManagerGO.AddComponent<GameManageur>();
                    }
                }
                return _instance;
            }
        }
        
        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGameState();
        }

        private void Update()
        {
            if (currentState == GameState.Menu && Input.anyKeyDown)
            {
                StartGame();
            }
        }

        private void InitializeGameState()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            
            if (currentSceneName == menuSceneName)
                ChangeGameState(GameState.Menu);
            else if (currentSceneName == gameSceneName)
                ChangeGameState(GameState.InGame);
        }

        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            
            OnGameStateChanged?.Invoke(newState);
        }

        #region Principal Méthodes

        public void StartGame()
        {
            LoadGameScene();
        }

        public void GameOver()
        {
            ChangeGameState(GameState.GameFinished);
        }

        public void RestartGame()
        {
            ChangeGameState(GameState.InGame);
        }

        public void ReturnToMenu()
        {
            LoadMenuScene();
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
    }
}