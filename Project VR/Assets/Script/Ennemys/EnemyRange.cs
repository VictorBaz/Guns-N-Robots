using System;
using Script.Interface;
using Script.Manager;
using UnityEngine;

namespace Script.Ennemys
{
    public class EnemyRange : MonoBehaviour, IDamagable, IEnemy
    {
        #region Enums

        enum EnemyRangeState
        {
            Spawn,
            StartAttacking,
            Attacking,
            StopAttacking,
        }

        #endregion

        #region  Fields

        [Header("Transform")]
        [SerializeField] private Transform enemyTransform;
        [SerializeField] private Transform shootPoint; 

        [Header("Tick Timings")]
        [SerializeField] private int tickTransitionSpawnToStartAttack;
        [SerializeField] private int tickTransitionStartAttackToAttack = 5; 
        [SerializeField] private int tickNeedToAttack = 3; 
        [SerializeField] private int tickStopAttackDuration = 2; 

        [Header("Visuals")]
        [SerializeField] private LineRenderer laserSight;
        [SerializeField] private float laserMaxDistance = 100f;
        [SerializeField] private LayerMask shootLayerMask;
        
        [Header("Laser Visual Settings")]
        [SerializeField] private float aimingLaserWidth = 0.1f;      
        [SerializeField] private float shootingLaserWidth = 0.02f;    
        [SerializeField] private Color aimingLaserColor = Color.red;  
        [SerializeField] private Color shootingLaserColor = Color.yellow;
        
        private EnemyRangeState enemyState = EnemyRangeState.Spawn;
        
        private int tickSinceSpawn = 0;
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

        #endregion


        #region Unity Methods

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

        private void Start()
        {
            InitPosition();
            
            if (laserSight != null)
            {
                laserSight.enabled = false;
            }
        }

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

        #region Initialization

        public void InitPosition()
        {
            enemyTransform.LookAt(enemyTransform.position - (playerPos.position - enemyTransform.position));
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
    
            if (enemyState == EnemyRangeState.Spawn && tickSinceSpawn >= tickTransitionSpawnToStartAttack)
            {
                TransitionToState(EnemyRangeState.StartAttacking);
            }
            else if (enemyState == EnemyRangeState.StartAttacking && tickStartAttackToAttack >= tickTransitionStartAttackToAttack)
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

        private void TransitionToState(EnemyRangeState newState)
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
            tickSinceSpawn++;
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
                ShootRaycast();
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

        private void ShootRaycast()
        {
            if (shootPoint == null) return;

            if (Physics.Raycast(shootPoint.position, aimDirection, out RaycastHit hit, laserMaxDistance, shootLayerMask))
            {
                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.TakeDamage();
                    Debug.Log("Player hit by sniper!");
                }
            }
            else
            {
                Debug.Log("Sniper shot missed!");
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
                
                OnEnemyDeath();
                DestroyItSelf();
            }
        }

        public void KillPlayer()
        {
            if (!isDead && hasShot)
            {
                EventManager.GameEnd();
                Debug.Log("Player killed by sniper!");
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

        public void DestroyItSelf()
        {
            Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
        }

        #endregion
    }
}