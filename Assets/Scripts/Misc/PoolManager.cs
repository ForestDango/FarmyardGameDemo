using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ع������ű����̳е���ģʽ������Ǽǲ��Ҽ���Ƶ��ʹ����Ʒ���ڴ濪���Լ��������ջ���
/// </summary>
public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>(); //���ö����Ƚ���������ݴ洢����
    [SerializeField] private Pool[] pool; //Pool��ṹ������
    [SerializeField] private Transform objectPoolTransform;

    [Serializable]
    public struct Pool
    {
        public int poolSize; //����ش�С
        public GameObject prefab; //����صǼǵ��ض���Ϸ����Ԥ����
    }

    private void Start()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    private void CreatePool(GameObject prefab,int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "Anchor");

        parentGameObject.transform.SetParent(objectPoolTransform); //���ɵĶ���صĸ�����Ĭ��ΪPoolManager

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            for (int i = 0; i < poolSize; i++)
            {
                //��������Ʒ��������Ϊ�Ǽ���״̬���������
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false); 
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }

    }

    /// <summary>
    /// ȡ������ص���Ʒ
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
    /// ���ö������Ʒ��λ��
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
    /// �Ӷ������ȡ����Ʒ
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
