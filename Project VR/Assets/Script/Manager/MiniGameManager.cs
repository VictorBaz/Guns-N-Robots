using System;
using Script.Enum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Manager
{
    public class MiniGameManager : MonoBehaviour
    {
        #region Singleton

        private static MiniGameManager _instance;
        
        public static MiniGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<MiniGameManager>();
                    
                    if (_instance == null)
                    {
                        GameObject miniGameManager = new GameObject("MiniGameManager");
                        _instance = miniGameManager.AddComponent<MiniGameManager>();
                    }
                }
                return _instance;
            }
        }

        #endregion
        
        #region Fields
        
        private int round;
        private int valueGoodShotLeft;
        
        public static Action OnRoundEnd;
        public static Action OnGameStart;
        public static Action OnGameEnd;
        
        private bool isGameRunning = false;

        #endregion

        #region Unity Methods

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

        #endregion
        
        private void StartInGame()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && !isGameRunning && GameManager.Instance.CurrentState == GameState.InGame)
            {
                isGameRunning = true;
                OnGameStart?.Invoke();
            }
        }
        
        private void Update()
        {
            StartInGame();
        }
        
        public bool IsGameRunning()
        {
            return isGameRunning;
        }
    }
}