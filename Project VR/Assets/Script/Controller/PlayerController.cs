using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Manager;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace Script.Controller
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        [field: SerializeField, ReadOnly] private List<CylinderHoleState> cylinder = new List<CylinderHoleState>();

        public CylinderHoleState currentCylinderHole { get; private set; }

        private int indexInBarel = 0;

        private CylinderManager cylinderManager = new CylinderManager();

        public static Action OnplayerShoot;
        public static Action OnPlayerMissedShot;
        
        [SerializeField]
        XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");

        [SerializeField] private XRIDefaultInputActions inputActions;

        private bool hasShot = false;
        [SerializeField] private LineRenderer lineRenderer;
        private RaycastHit hit;

        [SerializeField] private Grabing grabing;
        LayerMask layerMask;
        private bool canShoot;
        [SerializeField] private Transform bulletOrigin;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            cylinderManager.SetupBarrel(cylinder);
            layerMask = LayerMask.GetMask( "Head", "Wall");
            inputActions = new XRIDefaultInputActions();
        }

        private void Start()
        {
            GameManager.Instance.playerRef = this;
        }

        private void Update()
        {
            PlayerFire();
            if (m_TriggerInput != null)
            {
                var triggerVal = m_TriggerInput.ReadValue();
            
                if (triggerVal < 0.0001f && grabing.isGunInHand)
                {
                    canShoot = true;
                }
            }
        }
        

        #endregion

        #region Observer

        private void OnEnable()
        {
            inputActions.Enable();
            TickManager.OnTick += GetCurrentBarrelHoleByTick;
            MiniGameManager.OnRoundEnd += ResetPlayerAfterRound;
            inputActions.XRIRightInteraction.ReloadButton.started += ReloadHandleStart;
            inputActions.XRIRightInteraction.ReloadButton.canceled += ReloadHandleStart;
        }

        private void OnDisable()
        {
            inputActions.Dispose();
            inputActions.Disable();
            TickManager.OnTick -= GetCurrentBarrelHoleByTick;
            MiniGameManager.OnRoundEnd -= ResetPlayerAfterRound;
            inputActions.XRIRightInteraction.ReloadButton.started -= ReloadHandleStart;
            inputActions.XRIRightInteraction.ReloadButton.canceled -= ReloadHandleStart;
     
        }

        #endregion

        #region Player Methods

        private void PlayerFire() 
        {
            //TODO LINK THAT WITH GRABBING 
            var triggerVal = m_TriggerInput.ReadValue();
            if (triggerVal > 0 && 
                GameManager.Instance.CurrentState == GameState.MiniGameRunning &&
                !hasShot)
            {
                hasShot = true;

                switch (currentCylinderHole)
                {
                    case CylinderHoleState.Empty:
                        //future needed
                        break;
                    case CylinderHoleState.Full:
                        if (Physics.Raycast(bulletOrigin.position, bulletOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
                        {
                            lineRenderer.enabled = true;
                            lineRenderer.SetPosition(0, bulletOrigin.position);
                            lineRenderer.SetPosition(1, hit.point);
                            Debug.Log("hihi");
                        }
                        cylinder[indexInBarel] = CylinderHoleState.Empty;
                        OnplayerShoot.Invoke();
                        break;
                }
                /*switch (currentCylinderHole)
                {
                    case CylinderHoleState.Full : 
                        OnPlayerGoodShot?.Invoke();
                        break;
                    case CylinderHoleState.Empty :
                        OnPlayerBadShot?.Invoke();
                        break;
                }*/
            }
        }
        
        private void GetCurrentBarrelHoleByTick()
        {
            if (!hasShot && currentCylinderHole == CylinderHoleState.Full)
            {
                OnPlayerMissedShot?.Invoke(); 
            }
            
            indexInBarel = cylinderManager.IncrementBarrelByTick(cylinder, indexInBarel);
            currentCylinderHole = cylinder[indexInBarel];
            hasShot = false;

            SoundManager.Instance?.PlayMusicOneShot(currentCylinderHole == CylinderHoleState.Empty
                ? SoundManager.Instance.emptyLoad
                : SoundManager.Instance.fullLoad);
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

        #region Reload Methods

        private void ReloadHandleStart(InputAction.CallbackContext ctx)
        {
            if (ctx.started) //start
            {
                Debug.Log("TTTATATATATAT");
            }
            if (ctx.canceled) //start
            {
                Debug.Log("IUIUIUIUUIUU");
            }
        }
       

        #endregion
        
        
    }
}