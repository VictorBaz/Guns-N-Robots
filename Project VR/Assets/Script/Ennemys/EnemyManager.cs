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
    
        #endregion

        #region Observer

        private void OnEnable()
        {
            TickManager.OnTick += TickBehaviour;
        }

        private void OnDisable()
        {
            TickManager.OnTick -= TickBehaviour;
        }

        #endregion

        private void Start()
        {
            for (int i = 0; i < DoorList.Length; i++)
            {
                availableDoor.Add(i);
            }

            playerTransform = GameManager.Instance.playerRef.transform;
        }

        public void ReleaseEnemyPlacement(int index)
        {
            availableDoor.Add(index);
        }

        private void TickBehaviour()
        {
            if (lastEnemySpawn >= TickBetweenTwoEnnemy)
            {
                if (GameManager.Instance.CurrentState == GameState.MiniGameRunning && MiniGameManager.Instance.CanSpawn())
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
    
        private void PickRandomDoor()
        {
            if (availableDoor.Count == 0) return;
            
            int index = Random.Range(0, availableDoor.Count);
        
            Door door = DoorList[availableDoor[index]];
            
            EnnemyBehaviour nmi = SpawnEnemy(door.doorReference.transform);
            nmi.SetParametersOnSpawn(this, availableDoor[index],playerTransform);
            availableDoor.RemoveAt(index);
        }

        private EnnemyBehaviour SpawnEnemy(Transform doorTransform)
        {
            GameObject nmi = Instantiate(enemyPrefabs, doorTransform.position, Quaternion.identity);
            EventManager.EnemySpawn();
            // spawn
            return nmi.GetComponent<EnnemyBehaviour>();
        }
    
    }

    [Serializable]
    public class Door
    {
        public GameObject doorReference;
        [HideInInspector] public bool isAvalaible;
    }
}