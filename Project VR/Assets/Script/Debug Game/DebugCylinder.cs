using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Controller;
using Script.Enum;
using Script.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Debug_Game
{
    public class DebugCylinder : MonoBehaviour
    {
        #region Fields

        [SerializeField] private List<Image> allImages;

        [SerializeField] private PlayerController player;

        [SerializeField] private GameObject barelImage;

        private CylinderManager cylinderManager = new CylinderManager();

        private float wishedRotation = 0;
        
        private bool locked = false;

        #endregion
        
        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += IncrementBarrelRotation;
            PlayerController.OnplayerShoot += RefreshVisualsAfterShot;
            PlayerController.OnPlayerReload += ResetVisualsOnReload;
            PlayerController.OnReloadStart += LockRotation;   
            PlayerController.OnReloadEnd += UnlockRotation;
        }
        
        private void OnDisable()
        {
            TickManager.OnTick -= IncrementBarrelRotation;
            PlayerController.OnplayerShoot -= RefreshVisualsAfterShot;
            PlayerController.OnPlayerReload -= ResetVisualsOnReload;
            PlayerController.OnReloadStart -= LockRotation;   
            PlayerController.OnReloadEnd -= UnlockRotation;
        }

        #endregion
        
        #region Unity Methods

        private void Start()
        {
            InitBarrelVisuals();
        }

        #endregion

        #region Debug Cylinder

        private void IncrementBarrelRotation()
        {
            if (locked) return;
            
            wishedRotation += 60;
            barelImage.transform.DOKill();
            
            barelImage.transform.DOLocalRotate(
                new Vector3(barelImage.transform.localEulerAngles.x,
                    barelImage.transform.localEulerAngles.y,
                    wishedRotation),
                TickManager.TimeBetweenTick * 0.1f).SetEase(Ease.Linear);
            
            InitBarrelVisuals();
        }

        private void InitBarrelVisuals()
        {
            List<CylinderHoleState> barel = player.GetBarrel();

            for (int i = 0; i < barel.Count; i++)
            {
                allImages[i].color = barel[i] == CylinderHoleState.Empty ? Color.red : Color.green;
            }
        }

        private void RefreshVisualsAfterShot() => InitBarrelVisuals();

        private void ResetVisualsOnReload()
        {
            wishedRotation = 0;
            barelImage.transform.DOKill();
            barelImage.transform.localEulerAngles = new Vector3(barelImage.transform.localEulerAngles.x,
                barelImage.transform.localEulerAngles.y,
                wishedRotation);
            InitBarrelVisuals();
        }

        
        private void LockRotation() => locked = true;
        private void UnlockRotation() => locked = false;
        #endregion
        
    }
}