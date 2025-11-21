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
        [SerializeField] private int tickTransitionStartAttackToAttack = 0;
        [SerializeField] private int tickNeedToAttack;

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
        private int tickStartAttackToAttack;
        private int tickBeforeAttack = 0;

        private bool isAttacking;
        private bool isDead;
        
        private EnemyManager enemyManager;
        private int indexInEnnemyManager;
        private Transform playerPos;

        private Vector3 aimDirection; 

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
            if ((enemyState == EnemyRangeState.StartAttacking || enemyState == EnemyRangeState.Attacking) 
                && laserSight != null && laserSight.enabled)
            {
                UpdateLaserSight();
                
                if (enemyState == EnemyRangeState.Attacking)
                {
                    UpdateLaserVisualDuringAttack();
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
                tickStartAttackToAttack = 0;
            }
            else if (enemyState == EnemyRangeState.StartAttacking && tickStartAttackToAttack >= tickTransitionStartAttackToAttack)
            {
                TransitionToState(EnemyRangeState.Attacking);
                tickBeforeAttack = 0;
            }
            else if (enemyState == EnemyRangeState.Attacking && isAttacking)
            {
                isAttacking = false;
                TransitionToState(EnemyRangeState.StopAttacking);
            }
            else if (enemyState == EnemyRangeState.StopAttacking)
            {
                TransitionToState(EnemyRangeState.StartAttacking);
                tickStartAttackToAttack = 0;
            }
        }

        private void TransitionToState(EnemyRangeState newState)
        {
            if (enemyState == EnemyRangeState.StartAttacking)
            {
                if (laserSight != null)
                {
                    laserSight.enabled = false;
                }
            }

            enemyState = newState;

            if (newState == EnemyRangeState.StartAttacking)
            {
                if (laserSight != null)
                {
                    laserSight.enabled = true;
                }
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
            UpdateAimDirection();
        }

        private void AttackingBehavior()
        {
            tickBeforeAttack++;
    
            if (tickBeforeAttack >= tickNeedToAttack) 
            {
                ShootRaycast();
                isAttacking = true;
            }
        }

        private void StopAttackingBehavior()
        {
            //no idea for the moment
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
            Vector3 endPoint;

            if (Physics.Raycast(startPoint, aimDirection, out RaycastHit hit, laserMaxDistance, shootLayerMask))
            {
                endPoint = hit.point;
            }
            else
            {
                endPoint = startPoint + aimDirection * laserMaxDistance;
            }

            laserSight.SetPosition(0, startPoint);
            laserSight.SetPosition(1, endPoint);
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
                isAttacking = false;
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
            if (!isDead && isAttacking)
            {
                EventManager.GameEnd();
                Debug.Log("Player killed by enemy!");
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

        #region Utility

        private void ResetTick()
        {
            tickSinceSpawn = 0;
            tickBeforeAttack = 0;
            tickStartAttackToAttack = 0;
        }
        
        private void UpdateLaserVisualDuringAttack()
        {
            if (laserSight == null) return;

            float ratio = Mathf.Clamp01((float)tickBeforeAttack / tickNeedToAttack);

            Color startColor = aimingLaserColor;
            Color endColor = shootingLaserColor;

            Color lerpedColor = Color.Lerp(startColor, endColor, ratio);
            laserSight.startColor = lerpedColor;
            laserSight.endColor = lerpedColor;

            float lerpedWidth = Mathf.Lerp(aimingLaserWidth, shootingLaserWidth, ratio);
            laserSight.startWidth = lerpedWidth;
            laserSight.endWidth = lerpedWidth;
        }


        #endregion
    }
}