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

        [SerializeField] private AnimationClip anim;

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
            _animator.speed = anim.length / TickManager.TimeBetweenTick;
        }

        private void ResetAndStartAnimation()
        {
            _animator.Play(0, 0, 0f);
            SetAnimator();
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTickChange += SetAnimator;
            EventManager.OnGameStart += ResetAndStartAnimation;
        }

        private void OnDisable()
        {
            TickManager.OnTickChange -= SetAnimator;
            EventManager.OnGameStart -= ResetAndStartAnimation;
        }

        #endregion
        
    }
}