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
    public class UiCylinder : MonoBehaviour
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
            EventManager.OnplayerShoot += RefreshVisualsAfterShot;
            EventManager.OnPlayerReload += ResetVisualsOnReload;
            EventManager.OnReloadStart += LockRotation;   
            EventManager.OnReloadEnd += UnlockRotation;
        }
        
        private void OnDisable()
        {
            TickManager.OnTick -= IncrementBarrelRotation;
            EventManager.OnplayerShoot -= RefreshVisualsAfterShot;
            EventManager.OnPlayerReload -= ResetVisualsOnReload;
            EventManager.OnReloadStart -= LockRotation;   
            EventManager.OnReloadEnd -= UnlockRotation;
        }

        #endregion
        
        #region Unity Methods

        private void Start()
        {
            InitBarrelVisuals();
        }

        #endregion

        //TODO fix color or change because ugly visuals ?
        #region Debug Cylinder

        private void IncrementBarrelRotation()
        {
            if (locked) return;
            
            // pareil pas ouf le 60, faudrait faire un ratio par le nombre de balles ( 360 / bulletsCounts )
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
                // ca pourrait être pas mal de mettre de serializefield ces couleurs
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