using System;
using Script.Manager;
using UnityEngine;

namespace Script.Interact
{
    public class EnvTickBehavior : MonoBehaviour
    {
        
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
        }

        private void OnDisable()
        {
            TickManager.OnTickChange -= SetAnimator;
        }

        #endregion
        
    }
}
