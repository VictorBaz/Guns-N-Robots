using System;
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
        [SerializeField] private int valueGoodShotLeft;
        
        public static Action OnRoundEnd;
        public static Action OnGameStart;
        public static Action OnGameEnd;

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
        
        private void Update()
        {
            StartInGame();
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            PlayerController.OnPlayerGoodShot += OnPlayerDoneShot;
            PlayerController.OnPlayerBadShot += OnPlayerFailShot;
        }
        
        private void OnDisable()
        {
            PlayerController.OnPlayerGoodShot -= OnPlayerDoneShot;
            PlayerController.OnPlayerBadShot -= OnPlayerFailShot;
        }

        #endregion

        #region MiniGame

        
        
        private void StartInGame()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && 
                GameManager.Instance.CurrentState == GameState.InGame) 
            {
                GameManager.Instance.ChangeGameState(GameState.MiniGameRunning);
                valueGoodShotLeft = 5;
                OnGameStart?.Invoke();
            }
        }
        public bool IsGameRunning()
        {
            return GameManager.Instance.CurrentState == GameState.MiniGameRunning;
        }

        #endregion

        #region Deal With Player Shot

        private void OnPlayerFailShot()
        {
            
        }

        private void OnPlayerDoneShot()
        {
            valueGoodShotLeft--; 
    
            if (valueGoodShotLeft <= 0) 
            {
                valueGoodShotLeft = 5;
                GameManager.Instance.ChangeGameState(GameState.InGame);
                OnRoundEnd?.Invoke();
            }
        }

        #endregion
    }
}