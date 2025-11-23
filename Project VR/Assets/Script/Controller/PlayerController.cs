using System;
using System.Collections;
using System.Collections.Generic;
using Script.Enum;
using Script.Interface;
using Script.Manager;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace Script.Controller
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private VisualsController visuals;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Grabing grabing;
        [SerializeField] private Transform bulletOrigin;
        [SerializeField] private XRIDefaultInputActions inputActions;
        [SerializeField] private XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
        [SerializeField] private float minSpeedToReload;
        [SerializeField, ReadOnly] private List<CylinderHoleState> cylinder = new List<CylinderHoleState>();
        
        public CylinderHoleState currentCylinderHole { get; private set; }

        #endregion

        #region Private Fields

        private CylinderManager cylinderManager = new CylinderManager();
        private LayerMask layerMask;
        private RaycastHit hit;
        
        private int indexInBarel = 0;
        private bool hasShot = false;
        private bool canShoot = false;
        private bool reloading = false;
        
        private Vector3 startReloadPosition;
        private Vector3 endReloadPosition;
        private float startTimeReload;
        private float endTimeReload;
        private Coroutine currentCoroutineReloading;
        

        #endregion

        #region Events

        public static Action OnplayerShoot;
        public static Action OnReloadStart;
        public static Action OnPlayerReload;
        public static Action OnReloadEnd;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            cylinderManager.SetupBarrel(cylinder);
            layerMask = LayerMask.GetMask("Head", "Wall");
            inputActions = new XRIDefaultInputActions();
        }

        private void Start()
        {
            GameManager.Instance.playerRef = this;
        }

        private void Update()
        {
            HandleTriggerInput();
            PlayerFire();
            
        }

        private void OnEnable()
        {
            inputActions.Enable();
            TickManager.OnTick += GetCurrentBarrelHoleByTick;
            inputActions.XRIRightInteraction.ReloadButton.started += ReloadHandleStart;
            inputActions.XRIRightInteraction.ReloadButton.canceled += ReloadHandleStart;
        }

        private void OnDisable()
        {
            inputActions.Disable();
            inputActions.Dispose();
            TickManager.OnTick -= GetCurrentBarrelHoleByTick;
            inputActions.XRIRightInteraction.ReloadButton.started -= ReloadHandleStart;
            inputActions.XRIRightInteraction.ReloadButton.canceled -= ReloadHandleStart;
        }

        #endregion

        #region Input Handling

        private void HandleTriggerInput()
        {
            if (m_TriggerInput == null) return;

            var triggerVal = m_TriggerInput.ReadValue();
            
            if (triggerVal < 0.0001f && grabing.isGunInHand)
            {
                canShoot = true;
            }
        }

        #endregion

        #region Shooting

        private void PlayerFire()
        {
            if (reloading) return;
            if (!CanFireWeapon()) return;

            hasShot = true;

            if (currentCylinderHole == CylinderHoleState.Full)
            {
                ExecuteShot();
            }
            else
            {
                EventManager.BadShot();
            }
        }

        private bool CanFireWeapon()
        {
            var triggerVal = m_TriggerInput.ReadValue();
            return triggerVal > 0 &&
                   GameManager.Instance.CurrentState == GameState.MiniGameRunning &&
                   !hasShot &&
                   canShoot;
        }

        private void ExecuteShot()
        {
            canShoot = false;
            
            bool perfectShot = EvaluationShot();
            if (Physics.Raycast(bulletOrigin.position, bulletOrigin.TransformDirection(Vector3.forward), 
                out hit, Mathf.Infinity, layerMask))
            {
                
                DisplayBulletTracer();
                visuals.Sparks(hit.point, hit.normal);
                hit.transform.GetComponent<IDamagable>()?.TakeDamage();
            }
            
            
            
            visuals.Shoot();
            visuals.Muzzle();
            visuals.BulletShell();

            if (!perfectShot)
            {
                cylinder[indexInBarel] = CylinderHoleState.Empty;
            }
            
            
            OnplayerShoot?.Invoke();
        }

        private void DisplayBulletTracer()
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, bulletOrigin.position);
            lineRenderer.SetPosition(1, hit.point);
        }

        private bool EvaluationShot()
        {
            float timeBetween = TickManager.TimeBetweenTick;
            float currentTime = TickManager.Timer;

            float delta = Mathf.Abs(currentTime);

            float perfectTime = timeBetween * 0.35f; 
            
            if (delta <= perfectTime)
            {
                EventManager.PerfectShot();
                return true;
            }
            else
            {
                EventManager.GoodShot();
                return false;
            }
        }

        #endregion

        #region Barrel Management

        private void GetCurrentBarrelHoleByTick()
        {
            if (reloading) return;
            
            indexInBarel = cylinderManager.IncrementBarrelByTick(cylinder, indexInBarel);
            currentCylinderHole = cylinder[indexInBarel];
            hasShot = false;

            PlayBarrelSound();
        }

        private void PlayBarrelSound()
        {
            if (SoundManager.Instance == null) return;

            var soundToPlay = currentCylinderHole == CylinderHoleState.Empty
                ? SoundManager.Instance.emptyLoad
                : SoundManager.Instance.fullLoad;

            SoundManager.Instance.PlayMusicOneShot(soundToPlay);
        }

        public List<CylinderHoleState> GetBarrel()
        {
            return cylinder;
        }

        #endregion

        #region Reload System

        private void ReloadHandleStart(InputAction.CallbackContext ctx)
        {
            if (reloading) return;

            if (ctx.started)
            {
                RecordReloadStartPosition();
            }
            else if (ctx.canceled)
            {
                TryExecuteReload();
            }
        }

        private void RecordReloadStartPosition()
        {
            startReloadPosition = bulletOrigin.transform.position;
            startTimeReload = Time.time;
        }

        private void TryExecuteReload()
        {
            endReloadPosition = bulletOrigin.transform.position;
            endTimeReload = Time.time;
            float reloadSpeed = CalculateReloadSpeed();
            if (reloadSpeed > minSpeedToReload)
            {
                ExecuteReload();
            }
        }

        private float CalculateReloadSpeed()
        {
            float distance = Vector3.Distance(startReloadPosition, endReloadPosition);
            float time = endTimeReload - startTimeReload;
            return distance / time; // V = D/T
        }

        private void ExecuteReload()
        {
            float timeReload = visuals.Reload();
            currentCoroutineReloading = StartCoroutine(ToggleReloadState(timeReload));
            cylinderManager.Reload(cylinder);
            
            OnReloadStart?.Invoke();
        }

        private IEnumerator ToggleReloadState(float time)
        {
            reloading = true;
            yield return new WaitForSeconds(time);
            reloading = false;
            indexInBarel = 0;
            currentCylinderHole = cylinder[indexInBarel];
            OnReloadEnd?.Invoke();
            OnPlayerReload?.Invoke();
        }

        #endregion

        #region Round Management

        private void ResetPlayerAfterRound()
        {
            indexInBarel = 0;
            hasShot = false;
        }

        #endregion
    }
}