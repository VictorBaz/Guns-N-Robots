using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Script.Enum;
using Script.Interface;
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
        public static Action OnPlayerReload;
        
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
                            
                            hit.transform.GetComponent<IDamagable>()?.TakeDamage();
                        }
                        cylinder[indexInBarel] = CylinderHoleState.Empty;
                        OnplayerShoot.Invoke();
                        break;
                }
            }
        }
        
        private void GetCurrentBarrelHoleByTick()
        {
            /*if (!hasShot && currentCylinderHole == CylinderHoleState.Full)
            {
                OnPlayerMissedShot?.Invoke(); 
            }*/
            
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

        private Vector3 startReloadPosition;
        private float startTimeReload;
        private Vector3 endReloadPosition;
        private float endTimeReload;

        [SerializeField] private float minSpeedToReload;

        private void ReloadHandleStart(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                startReloadPosition = bulletOrigin.transform.position;
                startTimeReload = Time.time;
            }
            if (ctx.canceled)
            {
                endReloadPosition = bulletOrigin.transform.position;
                endTimeReload = Time.time;
                float distance = Vector3.Distance(startReloadPosition, endReloadPosition);
                float time = endTimeReload - startTimeReload;

                float speedOfGun = distance / time; // V= D/T
                Debug.Log($"speed of gun : {speedOfGun}, distance : {distance}, time : {time}");

                if (speedOfGun > minSpeedToReload) //then reload
                {
                    cylinderManager.Reload(cylinder);
                    OnPlayerReload?.Invoke();
                }
            }
            
            
        }
       

        #endregion
        
        
    }
}