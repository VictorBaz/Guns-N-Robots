using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Manager;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Controller
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        [field: SerializeField, ReadOnly] private List<CylinderHoleState> barel = new List<CylinderHoleState>();

        [SerializeField] private CylinderHoleState currentCylinderHole;

        private int indexInBarel = 0;

        private CylinderManager cylinderManager = new CylinderManager();

        public static Action OnPlayerGoodShot;
        public static Action OnPlayerBadShot;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            cylinderManager.SetupBarrel(barel);
        }

        private void Update()
        {
            PlayerFire();
        }

        #endregion
        
        

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += GetCurrentBarrelHoleByTick;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= GetCurrentBarrelHoleByTick;
        }

        #endregion

        #region Player Methods

        private void PlayerFire() 
        {
            if (Input.GetMouseButtonDown(0) && MiniGameManager.Instance.IsGameRunning())
            {
                switch (currentCylinderHole)
                {
                    case CylinderHoleState.Empty : 
                        OnPlayerGoodShot?.Invoke();
                        break;
                    case CylinderHoleState.Full :
                        OnPlayerBadShot?.Invoke();
                        break;
                }
            }
        }
        
        private void GetCurrentBarrelHoleByTick()
        {
            indexInBarel = cylinderManager.IncrementBarrelByTick(barel, indexInBarel);
            currentCylinderHole = barel[indexInBarel];
        }

        #endregion

        #region Getter Setter

        public List<CylinderHoleState> GetBarrel()
        {
            return barel;
        }

        #endregion
        
        
    }
}