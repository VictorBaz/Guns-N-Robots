using Script.Interface;
using Script.Manager;
using Script.Utility;
using UnityEngine;

namespace Script.Ennemys
{
    [RequireComponent(typeof(AudioSource))]
    public class EnnemyBehaviour : AbstractEnemy, IDamagable, IEnemy
    {

        #region Fields

        [Header("Transform and Position")] [SerializeField]
        private Transform enemyTransform;
        [Header("Ticks linked")] 
        [SerializeField] private int ticksBeforeAttack;
        [SerializeField] private int ticksForAttack;
        private int ticksSinceSpawn = 0;

        [Header("Player Ref")]
        private Transform playerPos;

        [SerializeField] private Transform enemyGo;
        
        private Vector3 moveDistance;
        private bool isDead;
        private bool isAttacking;
        private Vector3? nextPosition = null;

        [Header("Threshold Shift")] 
        [SerializeField] private float lerpAmount;

        [Header("Animation")] 
        [SerializeField] private Animator animator;

        private int indexInEnnemyManager;
        private EnemyManager enemyManager;

        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DieHash = Animator.StringToHash("Die");

        [SerializeField] private MaterialUpdater materialUpdater;

        [SerializeField] private AudioSource audioSourceEnemyMelee;

        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += EnnemyAction;
            EventManager.OnGameEnd += ClearEnemy;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= EnnemyAction;
            EventManager.OnGameEnd -= ClearEnemy;
        }

        #endregion

        #region Unity Methods

        

        private void FixedUpdate()
        {
            if (nextPosition != null && !isDead && !isAttacking)
            {
                enemyTransform.position = Vector3.Lerp(enemyTransform.position, (Vector3)nextPosition,
                    TickManager.TimeBetweenTick * lerpAmount);   
            }
        }
        

        #endregion

        #region Init

        public void InitPosition()
        {
            moveDistance = playerPos.position - enemyTransform.position;
            moveDistance.y = 0;
            nextPosition = enemyTransform.position;
            enemyTransform.LookAt(enemyTransform.position - (playerPos.position - enemyTransform.position));
        }

        #endregion

        #region Ennemy Methods

        public void ClearEnemy()
        {
            enemyManager.ReleaseEnemyPlacement(indexInEnnemyManager);
            OnDeathAnimationComplete();
        }

        public void DestroyItSelf()
        {
            ObjectPooler.EnqueueObject(this, "EnemyMelee");
        }

        public void OnEnemyDeath()
        {
            enemyManager.ReleaseEnemyPlacement(indexInEnnemyManager);
            enemyManager.RemoveEnemyFromList(this);
            EventManager.EnemyKilled();
        }

        private void EnnemyAction()
        {
            if (isDead)
            {
                return;
            }
            
            if (ticksSinceSpawn >= ticksBeforeAttack && !isAttacking)
            {
                StartAttack();
            }
            else if (!isAttacking)
            {
                Move();
            }
        }

        private void Move()
        {
            nextPosition += moveDistance / (ticksBeforeAttack + ticksForAttack);
            ticksSinceSpawn++;
            PlayStepSound();
        }

        private void StartAttack()
        {
            isAttacking = true;
            PlayChargeAttackSound();
            PlayAttackAnimation();
        }

        #endregion

        #region Animation Methods

        private void PlayWalkAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(WalkHash);
            }
        }

        private void PlayAttackAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(AttackHash);
            }
        }

        private void PlayDieAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(DieHash);
            }
        }

        #endregion

        #region Animation Events

        public void KillPlayer()
        {
            if (!isDead && isAttacking)
            {
                EventManager.GameEnd();
                Debug.Log("Player killed by enemy!");
            }
        }

        public void OnDeathAnimationComplete()
        {
            materialUpdater.ResetMaterial();
            DestroyItSelf();
        }

        #endregion

        #region Public Methods Interface

        public void SetParametersOnSpawn(EnemyManager enemyManager, int index, Transform playerPosition)
        {
            this.enemyManager = enemyManager;
            indexInEnnemyManager = index;
            playerPos = playerPosition;
            
        }

        public void TakeDamage()
        {
            if (!isDead)
            {
                isDead = true;
                isAttacking = false;
                PlayDieAnimation();
                OnEnemyDeath();
                materialUpdater.UpdateMaterials();
            }
        }
        
        public GameObject GetReferenceGo()
        {
            return transform.parent.gameObject;
        }

        public void ResetEnemy()
        {
            ticksSinceSpawn = 0;
            isDead = false;
            isAttacking = false;
            nextPosition = enemyTransform.position;
            stepSound = false;
            materialUpdater.ResetMaterial();
            InitPosition();
            enemyGo.position = Vector3.zero;
            enemyGo.localEulerAngles = Vector3.zero;
            PlayWalkAnimation();
            
        }

        #endregion

        #region Sound and Effect

        private bool stepSound = false;

        private void PlayChargeAttackSound()
        {
            if (SoundManager.Instance == null) return;
            audioSourceEnemyMelee.PlayOneShot(SoundManager.Instance.RobotAttackMeleeSoundCharge());
        }

        public void PlayAttackSoundMelee()
        {
            //use soundManager because object destroyed and audiosource linked to it
            if (SoundManager.Instance == null) return;
            SoundManager.Instance.PlayMusicOneShot(SoundManager.Instance.RobotAttackMeleeSound());
        }
        private void PlayStepSound()
        {
            if (SoundManager.Instance == null) return;
            
            audioSourceEnemyMelee.PlayOneShot(stepSound
                ? SoundManager.Instance.RobotWalkSound1()
                : SoundManager.Instance.RobotWalkSound2());
            stepSound = !stepSound;
        }

        #endregion

    }
}