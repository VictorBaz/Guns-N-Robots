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
            //CLICK
        }

        #endregion
        // NEED TO SET CONDITION EXMP VICTORY LOOSE CONTINUE
        //for me 3 possibilites : 
        // Full you die
        // Empty => you make it complete
        //Already complete dont know yet ;(
        private void PlayerFire() 
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (currentBarrelHole)
                {
                    case BarrelHoleState.Empty :
                        currentBarrelHole = BarrelHoleState.Complete; 
                        break;
                    case BarrelHoleState.Full :
                        Debug.Log("YOU DIE SADDLY");
                        break;
                    case BarrelHoleState.Complete : 
                        Debug.Log("Already Complete");
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
    }
}