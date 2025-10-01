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
        [SerializeField] private int valueGoodShotLeft;
        
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
                !isGameRunning && GameManager.Instance.CurrentState == GameState.InGame) // in VR this will be the barill animation
            {
                isGameRunning = true;
                valueGoodShotLeft = 5; // TODO fix magic number depends of future
                OnGameStart?.Invoke();
            }
        }
        public bool IsGameRunning()
        {
            return isGameRunning;
        }

        #endregion

        #region Deal With Player Shot

        private void OnPlayerFailShot()
        {
            
        }

        private void OnPlayerDoneShot()
        {
            if (valueGoodShotLeft > 0) //so still need to play 
            {
                valueGoodShotLeft--;
            }
            else //won the round then
            {
                valueGoodShotLeft = 5; //Reset value Do not forget
                OnRoundEnd.Invoke();
            }
        }

        #endregion
    }
}