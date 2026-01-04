using System.Collections.Generic;
using UnityEngine;

namespace Script.Utility
{
    public static class ObjectPooler
    { 
        public static Dictionary<string, Component> PoolLookUp = new Dictionary<string, Component>();
        public static Dictionary<string, Queue<Component>> PoolDictionary = new Dictionary<string, Queue<Component>>();
       
        public static void EnqueueObject<T>(T item, string name) where T : Component
        {
            if (!item.gameObject.activeSelf) return;

            item.transform.position = Vector3.zero;
            PoolDictionary[name].Enqueue(item);
            item.gameObject.SetActive(false);
        }
        
        public static T DequeueObject<T>(string key) where T : Component
        {
            //return (T)PoolDictionary[key].Dequeue();

            if (PoolDictionary[key].TryDequeue(out var item))
            {
                return (T)item;
            }

            return (T)EnqueueNewInstance(PoolLookUp[key], key);
        }

        public static T EnqueueNewInstance<T>(T item, string key) where T : Component
        {
            T newInstance = Object.Instantiate(item);
            newInstance.gameObject.SetActive(false);
            newInstance.transform.position = Vector3.zero;
            PoolDictionary[key].Enqueue(newInstance);
            return newInstance;
        }
        
        public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry) where T : Component
        {
            ResetPool();
            
            PoolDictionary.Add(dictionaryEntry,new Queue<Component>());
            PoolLookUp.Add(dictionaryEntry,pooledItemPrefab);
            
            for (int i = 0; i < poolSize; i++)
            {
                T pooledInstance = Object.Instantiate(pooledItemPrefab);
                pooledInstance.gameObject.SetActive(false);
                PoolDictionary[dictionaryEntry].Enqueue((T)pooledInstance);
            }
        }

        private static void ResetPool()
        {
            PoolDictionary.Clear();
            PoolLookUp.Clear();
        }
    }
}