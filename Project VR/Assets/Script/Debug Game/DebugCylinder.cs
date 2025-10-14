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

        #endregion
        
        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += IncrementBarrelRotation;
            MiniGameManager.OnRoundEnd += ResetVisualsCylinderAfterRound;
            PlayerController.OnplayerShoot += RefreshVisualsAfterShot;
            PlayerController.OnPlayerReload += RefreshVisualsAfterShot;
        }
        
        private void OnDisable()
        {
            TickManager.OnTick -= IncrementBarrelRotation;
            MiniGameManager.OnRoundEnd -= ResetVisualsCylinderAfterRound;
            PlayerController.OnplayerShoot -= RefreshVisualsAfterShot;
            PlayerController.OnPlayerReload -= RefreshVisualsAfterShot;
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
            wishedRotation += 60;
            barelImage.transform.DOKill();
            
            barelImage.transform.DOLocalRotate(
                new Vector3(barelImage.transform.localEulerAngles.x
                    ,barelImage.transform.localEulerAngles.y,
                    wishedRotation),
                TickManager.TimeBetweenTick/10f).SetEase(Ease.Linear);
            InitBarrelVisuals();
        }

        private float wishedRotation = 0;

        private void InitBarrelVisuals()
        {
            List<CylinderHoleState> barel = player.GetBarrel();

            for (int i = 0; i < barel.Count; i++)
            {
                allImages[i].color = barel[i] == CylinderHoleState.Empty ? Color.darkRed : Color.chartreuse;
            }
        }

        private void RefreshVisualsAfterShot() => InitBarrelVisuals();

        private void ResetVisualsCylinderAfterRound()
        {
            //NEED TO REFRESH For New pattern fucking need Barel for fuck sake
            cylinderManager.SetupBarrel(GameManager.Instance.playerRef.GetBarrel());
            wishedRotation = 0;
            barelImage.transform.DOKill();
            barelImage.transform.DOLocalRotate(
                new Vector3(barelImage.transform.localEulerAngles.x
                    ,barelImage.transform.localEulerAngles.y,
                    wishedRotation),
                TickManager.TimeBetweenTick).SetEase(Ease.Linear);
            InitBarrelVisuals();
        }

        #endregion
        
    }
}