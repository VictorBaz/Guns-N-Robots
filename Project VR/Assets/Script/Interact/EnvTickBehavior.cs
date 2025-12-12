using System;
using Script.Manager;
using UnityEngine;

namespace Script.Interact
{
    public class EnvTickBehavior : MonoBehaviour
    {
        /// <summary>
        /// ATTENTION FAUT QUE L'ANIM DE BASE EST UNE LONGUER DE 1s
        /// </summary>
        
        #region Fields

        [SerializeField] private Animator _animator;

        #endregion

        #region Unity Methods

        private void Start()
        {
            SetAnimator();
        }

        #endregion

        #region Env Tick

        private void SetAnimator()
        {
            _animator.speed = TickManager.TimeBetweenTick;
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTickChange += SetAnimator;
            EventManager.OnGameStart += SetAnimator;
        }

        private void OnDisable()
        {
            TickManager.OnTickChange -= SetAnimator;
            EventManager.OnGameStart -= SetAnimator;
        }

        #endregion
        
    }
}
