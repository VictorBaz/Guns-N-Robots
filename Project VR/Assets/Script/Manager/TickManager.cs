using System;
using Script.Controller;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class TickManager : MonoBehaviour
    {
        #region Fields

        [Header("Action")] 
        public static Action OnTick;
        public static Action OnTickChange;
        public static Action OnTickSetup;

        [SerializeField] private float timeBetweenTick = 0.666f; 
        
        public static float TimeBetweenTick;

        private float timer;
        
        [SerializeField] private float speedIncreaseAmount = 0.05f; 
        [SerializeField] private float minTimeBetweenTick = 0.2f;
        [SerializeField] private float defaultValueTimer = 0.666f; 

        [Header("Music Sync")]
        [SerializeField] private bool syncWithMusic = true;

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeTickFromMusic();
        }

        private void Update()
        {
            TimeBetweenTick = timeBetweenTick; 
            UpdateTickByTime();
        }

        #endregion

        #region Initialise

        private void InitializeTickFromMusic()
        {
            if (syncWithMusic && SoundManager.Instance != null)
            {
                float beatInterval = SoundManager.Instance.GetBeatInterval();
                
                timeBetweenTick = beatInterval;
                defaultValueTimer = timeBetweenTick;
                
            }
        }

        #endregion

        #region TickManager

        private void UpdateTickByTime()
        {
            if (GameManager.Instance.CurrentState != GameState.MiniGameRunning) return;
    
            timer += Time.deltaTime; 
    
            if (timer >= timeBetweenTick) 
            {
                timer -= timeBetweenTick; 
                OnTick?.Invoke();
            }
        }
        
        private void IncreaseSpeed()
        {
            timeBetweenTick -= speedIncreaseAmount;
        
            if (timeBetweenTick < minTimeBetweenTick)
            {
                timeBetweenTick = minTimeBetweenTick;
            }
            
            if (syncWithMusic && SoundManager.Instance != null)
            {
                SoundManager.Instance.UpdateMusicSpeed(timeBetweenTick, defaultValueTimer);
            }
            
            OnTickChange?.Invoke();
        }
        
        private void TickBehaviorAfterRoundEnd()
        {
            timer = 0;
            timeBetweenTick = defaultValueTimer;
            
            if (syncWithMusic && SoundManager.Instance != null)
            {
                SoundManager.Instance.UpdateMusicSpeed(timeBetweenTick, defaultValueTimer);
            }
        }

        private void ResetTimerOnRoundStart()
        {
            timer = 0;
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            EventManager.OnRoundEnd += IncreaseSpeed;
            EventManager.OnRoundStart += ResetTimerOnRoundStart;
            EventManager.OnGameStart += InitializeTickFromMusic;
            EventManager.OnGameEnd += EndGame;
        }

        private void OnDisable()
        {
            EventManager.OnRoundEnd -= IncreaseSpeed;
            EventManager.OnRoundStart -= ResetTimerOnRoundStart;
            EventManager.OnGameStart -= InitializeTickFromMusic;
            EventManager.OnGameEnd -= EndGame;
        }

        #endregion

        #region State

        private void EndGame()
        {
            ResetTimerOnRoundStart();
        }

        #endregion
        
    }
}