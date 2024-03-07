using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理器脚本，继承单例模式，负责登记并且减少频繁使用物品的内存开销以及垃圾回收机制
/// </summary>
public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>(); //利用队列先进后出的数据存储特性
    [SerializeField] private Pool[] pool; //Pool类结构体数组
    [SerializeField] private Transform objectPoolTransform;

    [Serializable]
    public struct Pool
    {
        public int poolSize; //对象池大小
        public GameObject prefab; //对象池登记的特定游戏对象预制体
    }

    private void Start()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    private void CreatePool(GameObject prefab,int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "Anchor");

        parentGameObject.transform.SetParent(objectPoolTransform); //生成的对象池的父对象默认为PoolManager

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            for (int i = 0; i < poolSize; i++)
            {
                //生成新物品，并设置为非激活状态，再入队列
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false); 
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }

    }

    /// <summary>
    /// 取出对象池的物品
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject ReuseObject(GameObject prefab,Vector3 position,Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToReuse = GetObjectFromPool(poolKey);
            ResetObject(position, rotation, objectToReuse, prefab);

            return objectToReuse;
        }
        else
        {
            Debug.Log("No object tool for" + prefab);
            return null;
        }
    }

    /// <summary>
    /// 设置对象池物品的位置
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="objectToReuse"></param>
    /// <param name="prefab"></param>
    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        objectToReuse.transform.localScale = prefab.transform.localScale;
    }

    /// <summary>
    /// 从对象池中取出物品
    /// </summary>
    /// <param name="poolKey"></param>
    /// <returns></returns>
    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(objectToReuse);

        if(objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }
}
