using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Interact;
using Script.Interface;
using Script.Manager;
using Script.Utility;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Script.Ennemys
{
    public class EnemyManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Door[] DoorList = { };
        [SerializeField] private AbstractEnemy enemyPrefab;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private int TickBetweenTwoEnnemy;

        [SerializeField] private int numberElementToPool;
        
        private int lastEnemySpawn;
        private List<int> availableDoor = new List<int>();
        private List<IEnemy> activeEnemies = new List<IEnemy>();
        private string enemyPoolKey;
        
    
        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += TickBehaviour;
            EventManager.OnRoundEnd += CleanupRound;
            EventManager.OnGameEnd += CleanupAllEnemies;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= TickBehaviour;
            EventManager.OnRoundEnd -= CleanupRound;
            EventManager.OnGameEnd -= CleanupAllEnemies;
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeAvailableDoors();
            SetupPoolEnemy();
        }

        #endregion

        #region Initialization

        private void InitializeAvailableDoors()
        {
            availableDoor.Clear();
            for (int i = 0; i < DoorList.Length; i++)
            {
                availableDoor.Add(i);
            }
        }

        #endregion

        #region Door Management

        public void ReleaseEnemyPlacement(int index)
        {
            if (!availableDoor.Contains(index))
            {
                availableDoor.Add(index);
            }
        }

        #endregion

        #region Tick Behavior

        private void TickBehaviour()
        {
            if (lastEnemySpawn >= TickBetweenTwoEnnemy)
            {
                if (CanSpawnEnemy())
                {
                    PickRandomDoor();
                    lastEnemySpawn = 0;   
                }
            }
            else
            {
                lastEnemySpawn++;
            }
        }

        private bool CanSpawnEnemy()
        {
            return GameManager.Instance.CurrentState == GameState.Game 
                   && MiniGameManager.Instance.CanSpawn()
                   && availableDoor.Count > 0;
        }

        #endregion

        #region Enemy Spawning

        private void PickRandomDoor()
        {
            if (availableDoor.Count == 0) return;
            
            int randomIndex = Random.Range(0, availableDoor.Count);
            int doorIndex = availableDoor[randomIndex];
            Door door = DoorList[doorIndex];
            
            AbstractEnemy enemyAbstract = SpawnEnemy(door.doorReference.transform);
            
            IEnemy enemy = enemyAbstract.GetComponent<IEnemy>() 
                           ?? enemyAbstract.GetComponentInChildren<IEnemy>();
            
            switch (enemy)
            {
                case null:
                    return;
                case EnemyRange enemyRange:
                    enemy.SetParametersOnSpawn(this, doorIndex, GetPlayerHead());
                    break;
                default:
                    enemy.SetParametersOnSpawn(this, doorIndex, GetPlayerTransform());
                    break;
            }

            enemy.ResetEnemy();
            availableDoor.RemoveAt(randomIndex);
            activeEnemies.Add(enemy);
            door.TriggerDoorOpen();
        }

        
        private AbstractEnemy SpawnEnemy(Transform doorTransform)
        {
            EventManager.EnemySpawn();
            AbstractEnemy enemy = ObjectPooler.DequeueObject<AbstractEnemy>(enemyPoolKey); 
            
            enemy.ResetT();
            
            enemy.gameObject.SetActive(true);
            enemy.gameObject.transform.position = doorTransform.position;
            return enemy;
        }

        #endregion

        #region Utility

        private Transform GetPlayerHead() => GameManager.Instance.playerRef.GetHeadTransform();
        
        private Transform GetPlayerTransform()
        {
            if (playerTransform == null && GameManager.Instance?.playerRef != null)
            {
                playerTransform = GameManager.Instance.playerRef.transform;
            }
            return playerTransform;
        }

        #endregion

        #region Cleanup

        private void CleanupRound()
        {
            //jsp mais azy peut être utile dans le futur ?
        }

        private void CleanupAllEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.DestroyItSelf();
                }
            }
            activeEnemies.Clear();
            
            InitializeAvailableDoors();
            lastEnemySpawn = 0;
        }

        public void RemoveEnemyFromList(IEnemy enemy)
        {
            activeEnemies.Remove(enemy);
        }

        #endregion

        #region Pooling System

        private void SetupPoolEnemy()
        {
            
            enemyPoolKey = enemyPrefab is EnemyRange ? "EnemyRange" : "EnemyMelee";
            ObjectPooler.SetupPool(enemyPrefab, numberElementToPool, enemyPoolKey);
        }

        #endregion
    }
}