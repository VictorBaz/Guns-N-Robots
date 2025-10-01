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
        
        

        #endregion

        #region Unity Methods

        private void Start()
        {
            cylinderManager.SetupBarrel(barel);
        }

        private void Update()
        {
            PlayerFire();
        }

        #endregion
        
        private void PlayerFire() 
        {
            if (Input.GetMouseButtonDown(0) && GameManageur.Instance.IsGameRunning())
            {
                switch (currentCylinderHole)
                {
                    case CylinderHoleState.Empty : // DECREMENTATION FOR BALL TO SHOT
                        Debug.Log("YOU DID WELL");
                        break;
                    case CylinderHoleState.Full :
                        Debug.Log("YOU DIE SADDLY"); // HANDLE DIE
                        break;
                }
            }
        }

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

        private void GetCurrentBarrelHoleByTick()
        {
            indexInBarel = cylinderManager.IncrementBarrelByTick(barel, indexInBarel);
            currentCylinderHole = barel[indexInBarel];
        }

        public List<CylinderHoleState> GetBarrel()
        {
            return barel;
        }
    }
}