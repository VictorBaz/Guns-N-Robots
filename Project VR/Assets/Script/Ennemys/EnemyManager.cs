using System;
using System.Collections.Generic;
using Script.Enum;
using Script.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.Ennemys
{
    public class EnemyManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Door[] DoorList = { };
        [SerializeField] private GameObject enemyPrefabs;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private int TickBetweenTwoEnnemy;
        
        private int lastEnemySpawn;
        private List<int> availableDoor = new List<int>();
        private List<EnnemyBehaviour> activeEnemies = new List<EnnemyBehaviour>();
    
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
            return GameManager.Instance.CurrentState == GameState.MiniGameRunning 
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
            
            EnnemyBehaviour enemy = SpawnEnemy(door.doorReference.transform);
            if (enemy != null)
            {
                enemy.SetParametersOnSpawn(this, doorIndex, GetPlayerTransform());
                availableDoor.RemoveAt(randomIndex);
                activeEnemies.Add(enemy);
            }
        }

        private EnnemyBehaviour SpawnEnemy(Transform doorTransform)
        {
            GameObject enemyObj = Instantiate(enemyPrefabs, doorTransform.position, Quaternion.identity);
            EventManager.EnemySpawn();
            
            // Chercher le component sur le root d'abord, puis dans les enfants
            EnnemyBehaviour enemy = enemyObj.GetComponent<EnnemyBehaviour>();
            if (enemy == null)
            {
                enemy = enemyObj.GetComponentInChildren<EnnemyBehaviour>();
            }

            if (enemy == null)
            {
                Debug.LogError($"EnnemyBehaviour component not found on spawned enemy prefab!");
                Destroy(enemyObj);
            }

            return enemy;
        }

        #endregion

        #region Utility

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
            //jsp
        }

        private void CleanupAllEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
            activeEnemies.Clear();
            
            InitializeAvailableDoors();
            lastEnemySpawn = 0;
        }

        public void RemoveEnemyFromList(EnnemyBehaviour enemy)
        {
            activeEnemies.Remove(enemy);
        }

        #endregion

    }

    [Serializable]
    public class Door
    {
        public GameObject doorReference;
        [HideInInspector] public bool isAvalaible;
    }
}