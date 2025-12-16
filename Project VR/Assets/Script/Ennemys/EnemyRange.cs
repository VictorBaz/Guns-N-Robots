using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Interface;
using Script.Manager;
using Script.Utility;
using UnityEngine;

namespace Script.Ennemys
{
    public class EnemyRange : AbstractEnemy, IDamagable, IEnemy
    {

        #region  Fields

        [Header("Transform")]
        [SerializeField] private Transform enemyTransform;
        [SerializeField] private Transform shootPoint; 
        [SerializeField] private Transform enemyGo;

        [Header("Tick Timings")]
        [SerializeField] private int tickTransitionStartAttackToAttack = 5; 
        [SerializeField] private int tickNeedToAttack = 3; 
        [SerializeField] private int tickStopAttackDuration = 2; 

        [Header("Visuals")]
        [SerializeField] private LineRenderer laserSight;
        [SerializeField] private float laserMaxDistance = 100f;
        [SerializeField] private LayerMask shooterLayerMask;
        
        [Header("Laser Visual Settings")]
        [SerializeField] private float aimingLaserWidth = 0.1f;      
        [SerializeField] private float shootingLaserWidth = 0.02f;    
        [SerializeField] private Color aimingLaserColor = Color.red;  
        [SerializeField] private Color shootingLaserColor = Color.yellow;
        
        [Header("Animation")]
        [SerializeField] private Animator robotRangeAnimator;
        
        [SerializeField] private MaterialUpdater materialUpdater;
        
        [Header("SFX")]
        [SerializeField] private List<ParticleSystem> muzzle;

        [Header("Audio")] [SerializeField] private AudioSource audioSourceEnemyRange;
        
        private EnemyRangeState enemyState = EnemyRangeState.Spawn;
        
        
        private int tickStartAttackToAttack = 0;
        private int tickBeforeAttack = 0;
        private int tickInStopAttack = 0;

        private bool hasShot = false;
        private bool isDead;
        
        private EnemyManager enemyManager;
        private int indexInEnnemyManager;
        private Transform playerPos;

        private Vector3 aimDirection;
        private float currentLerpValue = 0f;
        
        
        private static readonly int Shoot = Animator.StringToHash("Shoot");
        private static readonly int Death = Animator.StringToHash("Death");


        #endregion
        
        #region Unity Methods

        

        private void Update()
        {
            if (enemyState is EnemyRangeState.StartAttacking or EnemyRangeState.Attacking 
                && laserSight != null && laserSight.enabled)
            {
                
                
                if (enemyState is EnemyRangeState.StartAttacking or EnemyRangeState.Attacking)
                {
                    UpdateLaserVisualProgressive();
                }
            }
        }

        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += BehaviorByTick;
            EventManager.OnGameEnd += ClearEnemy;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= BehaviorByTick;
            EventManager.OnGameEnd -= ClearEnemy;
        }

        #endregion

        #region Initialization

        public void InitPosition()
        {
            enemyTransform.LookAt(enemyTransform.position - (playerPos.position - enemyTransform.position));
            enemyTransform.localEulerAngles = new Vector3(0, enemyTransform.localEulerAngles.y, 0);
        }

        public void SetParametersOnSpawn(EnemyManager enemyManager, int index, Transform playerPosition)
        {
            this.enemyManager = enemyManager;
            indexInEnnemyManager = index;
            playerPos = playerPosition;
        }

        #endregion

        #region State Machine

        private void BehaviorByTick()
        {
            if (isDead) return;
            
            ExecuteCurrentStateBehavior();
            CheckTransitionState();
        }

        private void ExecuteCurrentStateBehavior()
        {
            switch (enemyState)
            {
                case EnemyRangeState.Spawn:
                    SpawnBehavior();
                    break;
                case EnemyRangeState.StartAttacking:
                    StartAttackingBehavior();
                    break;
                case EnemyRangeState.Attacking:
                    AttackingBehavior();
                    break;
                case EnemyRangeState.StopAttacking:
                    StopAttackingBehavior();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckTransitionState()
        {
            if (isDead) return;
            
            if (enemyState == EnemyRangeState.StartAttacking && tickStartAttackToAttack >= tickTransitionStartAttackToAttack)
            {
                TransitionToState(EnemyRangeState.Attacking);
            }
            else if (enemyState == EnemyRangeState.Attacking && hasShot)
            {
                TransitionToState(EnemyRangeState.StopAttacking);
            }
            else if (enemyState == EnemyRangeState.StopAttacking && tickInStopAttack >= tickStopAttackDuration)
            {
                TransitionToState(EnemyRangeState.StartAttacking);
            }
        }

        public void TransitionToState(EnemyRangeState newState)
        {
            if (enemyState is EnemyRangeState.StartAttacking or EnemyRangeState.Attacking)
            {
                if (laserSight != null && newState != EnemyRangeState.StartAttacking && newState != EnemyRangeState.Attacking)
                {
                    laserSight.enabled = false;
                }
            }

            enemyState = newState;

            switch (newState)
            {
                case EnemyRangeState.StartAttacking:
                    tickStartAttackToAttack = 0;
                    currentLerpValue = 0f; 
                    if (laserSight != null)
                    {
                        laserSight.enabled = true;
                        
                        laserSight.startColor = aimingLaserColor;
                        laserSight.endColor = aimingLaserColor;
                        laserSight.startWidth = aimingLaserWidth;
                        laserSight.endWidth = aimingLaserWidth;
                    }
                    break;
                    
                case EnemyRangeState.Attacking:
                    tickBeforeAttack = 0;
                    hasShot = false;
                    break;
                    
                case EnemyRangeState.StopAttacking:
                    tickInStopAttack = 0;
                    if (laserSight != null)
                    {
                        laserSight.enabled = false;
                    }
                    break;
            }
        }

        #endregion

        #region State Behaviors

        private void SpawnBehavior()
        {
            //do nothing for the moment
        }

        private void StartAttackingBehavior()
        {
            tickStartAttackToAttack++;
            UpdateLaserSight();
        }

        private void AttackingBehavior()
        {
            tickBeforeAttack++;
    
            if (tickBeforeAttack >= tickNeedToAttack && !hasShot) 
            {
                PlayAttackAnimation();
                PlayMuzzleRange();
                PlayShotEnemyRangeSound();
                hasShot = true;
            }
        }

        private void StopAttackingBehavior()
        {
            tickInStopAttack++;
        }

        #endregion

        #region Combat Methods

        private void UpdateAimDirection()
        {
            if (playerPos == null || shootPoint == null) return;
            
            aimDirection = (playerPos.position - shootPoint.position).normalized;
        }

        private void UpdateLaserSight()
        {
            if (shootPoint == null || laserSight == null) return;

            UpdateAimDirection();

            Vector3 startPoint = shootPoint.position;
            Vector3 endPoint = startPoint + aimDirection * laserMaxDistance;

            laserSight.SetPosition(0, startPoint);
            laserSight.SetPosition(1, endPoint);
        }


        private void UpdateLaserVisualProgressive()
        {
            if (laserSight == null) return;

            int totalTicksForAiming = tickTransitionStartAttackToAttack + tickNeedToAttack;
            int currentTick = 0;
            
            switch (enemyState)
            {
                case EnemyRangeState.StartAttacking:
                    currentTick = tickStartAttackToAttack;
                    break;
                case EnemyRangeState.Attacking:
                    currentTick = tickTransitionStartAttackToAttack + tickBeforeAttack;
                    break;
            }

            float targetLerpValue = (float)currentTick / totalTicksForAiming;
            
            currentLerpValue = Mathf.Lerp(currentLerpValue, targetLerpValue, Time.deltaTime * 5f); 

            Color currentColor = Color.Lerp(aimingLaserColor, shootingLaserColor, currentLerpValue);
            laserSight.startColor = currentColor;
            laserSight.endColor = currentColor;

            float currentWidth = Mathf.Lerp(aimingLaserWidth, shootingLaserWidth, currentLerpValue);
            laserSight.startWidth = currentWidth;
            laserSight.endWidth = currentWidth;
        }

        public void ShootRaycast()
        {
            if (shootPoint == null) return;

            if (Physics.Raycast(shootPoint.position, aimDirection, out RaycastHit hit, laserMaxDistance, ~shooterLayerMask,QueryTriggerInteraction.Collide) 
                && hit.transform.gameObject.CompareTag("Head")) //La condition c'est un Lundi
            {
                EventManager.GameEnd();
            }
            
        }

        public void TakeDamage()
        {
            if (!isDead)
            {
                isDead = true;
                hasShot = false;
                if (laserSight != null)
                {
                    laserSight.enabled = false;
                }
                materialUpdater.UpdateMaterials();
                PlayDieAnimation();
                OnEnemyDeath();
            }
        }

        public void KillPlayer()
        {
            if (!isDead && hasShot)
            {
                EventManager.GameEnd();
            }
        }

        #endregion

        #region Death & Cleanup

        public void OnEnemyDeath()
        {
            enemyManager.ReleaseEnemyPlacement(indexInEnnemyManager);
            enemyManager.RemoveEnemyFromList(this);
            EventManager.EnemyKilled();
        }

        public void ClearEnemy()
        {
            enemyManager.ReleaseEnemyPlacement(indexInEnnemyManager);
        }
        
        public void OnDeathAnimationComplete()
        {
            materialUpdater.ResetMaterial();
            DestroyItSelf();
        }

        public void DestroyItSelf()
        {
            ObjectPooler.EnqueueObject(this, "EnemyRange");
        }

        public GameObject GetReferenceGo()
        {
            return transform.parent.gameObject;
        }

        public void ResetEnemy()
        {
            enemyState = EnemyRangeState.Spawn;
            tickStartAttackToAttack = 0;
            tickBeforeAttack = 0;
            tickInStopAttack = 0;
            hasShot = false;
            isDead = false;
            currentLerpValue = 0f;
            laserSight.enabled = false;
            materialUpdater.ResetMaterial();
            
            InitPosition();
            enemyGo.position = Vector3.zero;
            enemyGo.localEulerAngles = Vector3.zero;
            
            if (laserSight != null)
            {
                laserSight.enabled = false;
            }
            PlaySpawnAnimation();
            
        }

        #endregion

        #region  Anim & Animator Handle
        
        private void PlayAttackAnimation()
        {
            if (robotRangeAnimator != null)
            {
                robotRangeAnimator.SetTrigger(Shoot);
            }
        }

        private void PlayDieAnimation()
        {
            if (robotRangeAnimator != null)
            {
                robotRangeAnimator.SetTrigger(Death);
            }
        }

        private void PlaySpawnAnimation()
        {
            if (robotRangeAnimator != null)
            {
                robotRangeAnimator.Play("spawnRange");
            }
        }

        #endregion

        #region SFX

        private void PlayMuzzleRange()
        {
            foreach (var particle in muzzle) particle.Play();
        }

        #endregion

        #region Audio Enemy

        private void PlayShotEnemyRangeSound()
        {
            if (SoundManager.Instance == null) return;
            audioSourceEnemyRange.PlayOneShot(SoundManager.Instance.RobotAttackRangeSound());
        }

        #endregion
    }
}