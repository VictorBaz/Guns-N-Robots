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

        [SerializeField] private float timeBetweenTick;
        
        public static float TimeBetweenTick;

        private float timer;
        
        [SerializeField] private float speedIncreaseAmount = 0.1f; 
        [SerializeField] private float minTimeBetweenTick = 0.2f;
        [SerializeField] private float defaultValueTimer = 1f; 

        #endregion

        #region Unity Methods

        private void Update()
        {
            TimeBetweenTick = timeBetweenTick; 
            UpdateTickByTime();
        }

        #endregion

        #region Initialise

        //tempo

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
            OnTickChange?.Invoke();
        
        }
        
        private void TickBehaviorAfterRoundEnd()
        {
            timer = 0;
            timeBetweenTick = defaultValueTimer;
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            MiniGameManager.OnRoundEnd += TickBehaviorAfterRoundEnd;
            PlayerController.OnPlayerMissedShot += IncreaseSpeed;
        }

        private void OnDisable()
        {
            MiniGameManager.OnRoundEnd -= TickBehaviorAfterRoundEnd;
            PlayerController.OnPlayerMissedShot -= IncreaseSpeed;
        }

        #endregion
        
    }
}
