using System;
using Script.Manager;
using UnityEngine;

namespace Script.Interact
{
    public class EnvTickBehavior : MonoBehaviour
    {
        /// <summary>
        /// ATTENTION IL FAUT QUE L'ANIM DE BASE EST UNE LONGEUR DE 1s
        /// </summary>
        
        #region Fields

        [SerializeField] private Animator _animator;

        [SerializeField] private int anim;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _animator.speed = 0;
        }

        #endregion

        #region Env Tick

        private void SetAnimator()
        {
            _animator.speed = 1/TickManager.TimeBetweenTick;
        }

        private void ResetAndStartAnimation()
        {
            SetAnimator();
            _animator.Play(0, 0, 0f);
            
        }

        private void SetAnimToZero()
        {
            _animator.speed = 0;
            _animator.Play(0, 0, 0f);
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTickChange += SetAnimator;
            EventManager.OnGameStart += ResetAndStartAnimation;
            EventManager.OnGameEnd += SetAnimToZero;
        }

        private void OnDisable()
        {
            TickManager.OnTickChange -= SetAnimator;
            EventManager.OnGameStart -= ResetAndStartAnimation;
            EventManager.OnGameEnd -= SetAnimToZero;
        }

        #endregion
        
    }
}