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

        #endregion

        
    }
}
