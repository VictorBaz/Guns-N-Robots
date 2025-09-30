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

        [field: SerializeField, ReadOnly] private List<BarrelHoleState> barel = new List<BarrelHoleState>();

        private BarrelHoleState currentBarrelHole;

        private int indexInBarel = -1;

        private BarelManager barelManager = new BarelManager();
        
        

        #endregion

        #region Unity Methods

        private void Start()
        {
            barelManager.SetupBarrel(barel);
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
                Debug.Log("Fire");
                switch (currentBarrelHole)
                {
                    case BarrelHoleState.Empty : // DECREMENTATION FOR BALL TO SHOT
                        Debug.Log("YOU DID WELL");
                        break;
                    case BarrelHoleState.Full :
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
            indexInBarel = barelManager.IncrementBarrelByTick(barel, indexInBarel);
            currentBarrelHole = barel[indexInBarel];
            Debug.Log(indexInBarel);
        }

        public List<BarrelHoleState> GetBarrel()
        {
            return barel;
        }
    }
}