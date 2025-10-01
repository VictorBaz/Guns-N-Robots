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
    public class DebugBarel : MonoBehaviour
    {
        #region MyRegion

        [SerializeField] private List<Image> allImages;

        [SerializeField] private PlayerController player;

        [SerializeField] private GameObject barelImage;

        #endregion
        
        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += IncrementBarrelRotation;
        }
        
        private void OnDisable()
        {
            TickManager.OnTick -= IncrementBarrelRotation;
        }

        #endregion


        #region Unity Methods

        private void Start()
        {
            InitBarrelVisuals();
        }

        #endregion
        
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
                allImages[i].color = barel[i] == CylinderHoleState.Empty ? Color.chartreuse : Color.darkRed;
            }
        }
    }
}