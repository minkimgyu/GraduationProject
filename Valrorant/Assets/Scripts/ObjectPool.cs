using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PoolData
{
    [SerializeField] string _tag;
    public string Tag { get { return _tag; } }

    [SerializeField] GameObject _prefab;
    public GameObject Prefab { get { return _prefab; } }
    
    [SerializeField] int _size;
    public int Size { get { return _size; } }

    public PoolData(string tag, GameObject prefab, int size)
    {
        _tag = tag;
        _prefab = prefab;
        _size = size;
    }
}

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance = null;
    Dictionary<string, Queue<GameObject>> _poolItems;
    [SerializeField] List<PoolData> _poolDatas = new List<PoolData>();

    List<GameObject> _spawnedOjects;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawnedOjects = new List<GameObject>();
        _poolItems = new Dictionary<string, Queue<GameObject>>();

        // 여기서 오브젝트 생성
        foreach (PoolData pool in _poolDatas)
        {
            _poolItems.Add(pool.Tag, new Queue<GameObject>());
            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = ReturnObject(pool.Tag, pool.Prefab);
                SortPool(obj); // 정렬해줌
            }
        }
    }

    public static T Spawn<T>(string tag)
    {
        GameObject obj = _instance.SpawnGameObject(tag, Vector3.zero, Quaternion.identity);
        if (obj.TryGetComponent(out T component)) return component;
        else obj.SetActive(false);

        return default(T);
    }

    public static T Spawn<T>(string tag, Vector3 position)
    {
        GameObject obj = _instance.SpawnGameObject(tag, position, Quaternion.identity);
        if (obj.TryGetComponent(out T component)) return component;
        else obj.SetActive(false);

        return default(T);
    }

    public static T Spawn<T>(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject obj = _instance.SpawnGameObject(tag, position, rotation);
        if (obj.TryGetComponent(out T component)) return component;
        else obj.SetActive(false);

        return default(T);
    }

    GameObject SpawnGameObject(string tag, Vector3 position, Quaternion rotation)
    {
        Queue<GameObject> poolQueue = _poolItems[tag];
        if (poolQueue.Count <= 0)
        {
            PoolData pool = _poolDatas.Find(x => x.Tag == tag);
            GameObject obj = ReturnObject(pool.Tag, pool.Prefab);
            SortPool(obj);
        }

        // 큐에서 꺼내서 사용
        GameObject objectToSpawn = poolQueue.Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public static void ReturnGameObjectToPool(GameObject obj)
    {
        _instance._poolItems[obj.name].Enqueue(obj);
    }

    void SortPool(GameObject obj)
    {
        bool canFind = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1 || canFind)
            {
                obj.transform.SetSiblingIndex(i);
                _spawnedOjects.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
            {
                canFind = true;
            }
        }
    }

    GameObject ReturnObject(string tag, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = tag;
        obj.SetActive(false);
        return obj;
    }
}
