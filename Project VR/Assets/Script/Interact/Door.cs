using System;
using Script.Manager;
using UnityEngine;

namespace Script.Interact
{
    [Serializable]
    public class Door
    {
        #region Fields

        private static readonly int OpenDoor = Animator.StringToHash("OpenDoor");

        public GameObject doorReference;
        [HideInInspector] public bool isAvalaible;

        [SerializeField] private bool isPhysicsDoor;
        
        [SerializeField] private Animator doorAnimator;

        [SerializeField] private AudioSource audioSource;

        #endregion

        #region Anim

        public void TriggerDoorOpen()
        {
            if (!isPhysicsDoor) return;

            doorAnimator.SetTrigger(OpenDoor);
            OpenDoorSound();
        }

        #endregion

        #region Sound

        public void OpenDoorSound()
        {
            if (SoundManager.Instance != null)
            {
                audioSource.PlayOneShot(SoundManager.Instance.DoorSound());
            }
        }

        #endregion
        
    }
}