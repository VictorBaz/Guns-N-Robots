using System;
using Script.Controller;
using Script.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class TickManager : MonoBehaviour
    {
        #region Fields

        [Header("Actions")] 
        public static Action OnTick;
        public static Action OnTickChange;

        [SerializeField] private float timeBetweenTick = 0.666f; 
        
        public static float TimeBetweenTick;
        public static float Timer;

        private float timer;

        [SerializeField] private float defaultValueTimer = 0.666f; 

        [Header("BPM Settings")]
        [SerializeField] private float currentBPM = 90f;        
        [SerializeField] private float maxBPM = 180f;           
        [SerializeField] private int maxRoundForMaxBPM = 40;    

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeTickFromMusic();
        }

        private void Update()
        {
            TimeBetweenTick = timeBetweenTick;
            Timer = timer;
            UpdateTickByTime();
        }

        #endregion

        #region Initialise

        private void InitializeTickFromMusic()
        {
            if (SoundManager.Instance != null)
            {
                float beatInterval = SoundManager.Instance.GetBeatInterval();
                currentBPM = 60f / beatInterval;      

                timeBetweenTick = beatInterval;
                defaultValueTimer = timeBetweenTick;

                SoundManager.Instance.UpdateMusicSpeed(timeBetweenTick, defaultValueTimer);
            }
            else
            {
                timeBetweenTick = 60f / currentBPM;
                defaultValueTimer = timeBetweenTick;
            }
        }

        #endregion

        #region TickManager

        private void UpdateTickByTime()
        {
            if (GameManager.Instance.CurrentState == GameState.MiniGamePaused 
                || GameManager.Instance.CurrentState == GameState.MiniGameRunning)
            {
                timer += Time.deltaTime;
    
                if (timer >= timeBetweenTick)
                {
                    timer -= timeBetweenTick;
                    OnTick?.Invoke();
                }
            }
        }
        
        private void IncreaseSpeed()
        {
            int round = MiniGameManager.Instance.GetCurrentRound();
            float t = Mathf.Clamp01((float)round / maxRoundForMaxBPM);

            currentBPM = Mathf.Lerp(90f, maxBPM, t);

            timeBetweenTick = 60f / currentBPM;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.UpdateMusicSpeed(timeBetweenTick, defaultValueTimer);
            }
            
            OnTickChange?.Invoke();
        }

        private void ResetTimerAndMusic()
        {
            timeBetweenTick = defaultValueTimer;
            
            if (SoundManager.Instance != null)
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
            EventManager.OnGameStart += InitializeTickFromMusic;
            EventManager.OnGameEnd += EndGame;
        }

        private void OnDisable()
        {
            EventManager.OnRoundEnd -= IncreaseSpeed;
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
