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


        private void Start()
        {
            barelManager.SetupBarrel(barel);
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