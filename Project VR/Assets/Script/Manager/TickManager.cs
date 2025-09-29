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

        private float timer;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            UpdateTickByTime();
        }

        #endregion

        #region Initialise

        //tempo

        #endregion

        #region TickManager

        private void UpdateTickByTime()
        {
            if (!GameManageur.Instance.IsGameRunning()) return;
            
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
