using Script.Manager;
using UnityEngine;

namespace Script.Ennemys
{
    public class EnnemyBehaviour : MonoBehaviour
    {

        #region Fields
        [Header("Ticks linked")] [Space]
        [SerializeField] private int ticksBeforeAttack;
        [SerializeField] private int ticksForAttack;
        private int ticksSinceSpawn = 0;
        [Space]
        [Header("Player Ref")] [Space]
        [SerializeField] private Transform playerPos;
        private Vector3 moveDistance;
        private bool isDead;
        private Vector3? nextPosition = null;
        [Space]
        [Header("Threshold Shift")] [Space] [Range(0,1)]
        [SerializeField] private float lerpAmount;

        private int indexInEnnemyManager;

        private EnemyManager enemyManager;

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
        }

        private void FixedUpdate()
        {
            if (nextPosition != null)
            {
                transform.position = Vector3.Lerp(transform.position, (Vector3)nextPosition, TickManager.TimeBetweenTick*lerpAmount);   
            }
        }

        private void OnDestroy()
        {
            OnEnemyDeath();
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
        }

        private void EnnemyAction()
        {
            if (isDead)
            {
                Destroy(gameObject);
            }
            else if (ticksSinceSpawn == ticksBeforeAttack)
            {
                Debug.Log("attention j'attaque");
                //attack
            }
            else
            {
                Move();
            }
        }

        private void Move()
        {
            nextPosition += moveDistance / (ticksBeforeAttack+ticksForAttack);
            ticksSinceSpawn++;
        }

        #endregion

        public void SetParametersOnSpawn(EnemyManager enemyManager, int index, Transform playerPosition)
        {
            this.enemyManager = enemyManager;
            indexInEnnemyManager = index;
            playerPos = playerPosition;
        }
    
    }
}
