using Script.Interface;
using Script.Manager;
using UnityEngine;

namespace Script.Ennemys
{
    public class EnnemyBehaviour : MonoBehaviour, IDamagable
    {

        #region Fields
        [Header("Ticks linked")] 
        [SerializeField] private int ticksBeforeAttack;
        [SerializeField] private int ticksForAttack;
        private int ticksSinceSpawn = 0;

        [Header("Player Ref")]
        [SerializeField] private Transform playerPos;
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

        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += EnnemyAction;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= EnnemyAction;
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitPosition();
            PlayWalkAnimation();
        }

        private void FixedUpdate()
        {
            if (nextPosition != null && !isDead && !isAttacking)
            {
                transform.position = Vector3.Lerp(transform.position, (Vector3)nextPosition, TickManager.TimeBetweenTick * lerpAmount);   
            }
        }
        

        #endregion

        #region Init

        private void InitPosition()
        {
            moveDistance = playerPos.position - transform.position;
            nextPosition = transform.position;
        }

        #endregion

        #region Ennemy Methods

        private void OnEnemyDeath()
        {
            enemyManager.ReleaseEnemyPlacement(indexInEnnemyManager);
            EventManager.EnemyKilled();
        }

        private void EnnemyAction()
        {
            if (isDead)
            {
                Destroy(gameObject);
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
        }

        private void StartAttack()
        {
            isAttacking = true;
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

        #region Animation Events (Called from Animation Clips)

        public void KillPlayer()
        {
            if (!isDead && isAttacking)
            {
                // TODO: Implémenter la mort du joueur
                // EventManager.PlayerKilled();
                Debug.Log("Player killed by enemy!");
            }
        }

        public void OnDeathAnimationComplete()
        {
            Destroy(gameObject);
        }

        #endregion

        #region Public Methods

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
            }
        }

        #endregion
    }
}