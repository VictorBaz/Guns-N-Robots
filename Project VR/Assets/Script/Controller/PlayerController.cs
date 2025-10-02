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

        [field: SerializeField, ReadOnly] private List<CylinderHoleState> cylinder = new List<CylinderHoleState>();

        [SerializeField] private CylinderHoleState currentCylinderHole;

        private int indexInBarel = 0;

        private CylinderManager cylinderManager = new CylinderManager();

        public static Action OnPlayerGoodShot;
        public static Action OnPlayerBadShot;
        public static Action OnPlayerMissedShot;

        private bool hasShot = false;
        #endregion

        #region Unity Methods

        private void Awake()
        {
            cylinderManager.SetupBarrel(cylinder);
        }

        private void Start()
        {
            GameManager.Instance.playerRef = this;
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
            MiniGameManager.OnRoundEnd += ResetPlayerAfterRound;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= GetCurrentBarrelHoleByTick;
            MiniGameManager.OnRoundEnd -= ResetPlayerAfterRound;
        }

        #endregion

        #region Player Methods

        private void PlayerFire() 
        {
            if (Input.GetMouseButtonDown(0) && 
                GameManager.Instance.CurrentState == GameState.MiniGameRunning &&
                !hasShot)
            {
                hasShot = true;
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
            if (!hasShot && currentCylinderHole == CylinderHoleState.Empty)
            {
                OnPlayerMissedShot?.Invoke(); 
            }
            
            indexInBarel = cylinderManager.IncrementBarrelByTick(cylinder, indexInBarel);
            currentCylinderHole = cylinder[indexInBarel];
            hasShot = false;
        }

        #endregion

        #region Getter Setter

        public List<CylinderHoleState> GetBarrel()
        {
            return cylinder;
        }

        private void ResetPlayerAfterRound()
        {
            indexInBarel = 0;
            hasShot = false;
        }
        
        #endregion
        
        
    }
}