using System;
using UnityEngine;

namespace Script.Manager
{
    public class TickManager : MonoBehaviour
    {
        #region Fields

        [Header("Action")] 
        public static Action OnTick;

        [SerializeField] private float timeBetweenTick;
        
        public static float TimeBetweenTick;

        private float timer;
        [SerializeField] private float defaultValueTimer = 1f; 

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            TimeBetweenTick = timeBetweenTick; //ok tier selon Jacques dans le futur faire un singleton TODO
            UpdateTickByTime();
        }

        #endregion

        #region Initialise

        //tempo

        #endregion

        #region TickManager

        private void UpdateTickByTime()
        {
            if (!MiniGameManager.Instance.IsGameRunning()) return;
            
            if (timer < timeBetweenTick)
            {
                timer += Time.fixedDeltaTime;
            }
            else
            {
                timer = 0;
                OnTick.Invoke();
            }
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
        }

        private void OnDisable()
        {
            MiniGameManager.OnRoundEnd -= TickBehaviorAfterRoundEnd;
        }

        #endregion
        
    }
}
