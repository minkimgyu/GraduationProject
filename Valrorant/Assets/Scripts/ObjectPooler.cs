using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Pool
{
    [SerializeField]
    string _tag;
    public string Tag { get { return _tag; } }

    [SerializeField]
    GameObject _prefab;
    public GameObject Prefab { get { return _prefab; } }

    [SerializeField]
    int _size;
    public int Size { get { return _size; } }

    public Pool(string tag, GameObject prefab, int size)
    {
        _tag = tag;
        _prefab = prefab;
        _size = size;
    }
}

public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler _instance = null;

    [SerializeField] List<Pool> _pools = new List<Pool>();

    List<GameObject> _spawnObjects;
    Dictionary<string, Queue<GameObject>> _poolDictionary;


    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        //if (InitAction != null) InitAction();

        _spawnObjects = new List<GameObject>();
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // �̸� ����
        foreach (Pool pool in _pools)
        {
            _poolDictionary.Add(pool.Tag, new Queue<GameObject>());
            for (int i = 0; i < pool.Size; i++)
            {
                var obj = CreateNewObject(pool.Tag, pool.Prefab);
                ArrangePool(obj);
            }

            // OnDisable�� ReturnToPool �������ο� �ߺ����� �˻�
            if (_poolDictionary[pool.Tag].Count <= 0)
                Debug.Log($"{pool.Tag} ObjectPooler.ReturnToPool(gameObject)�� �߰��ؾ���");
            else if (_poolDictionary[pool.Tag].Count != pool.Size)
                Debug.Log($"{pool.Tag}�� ReturnToPool�� �ߺ��˴ϴ�");
        }
    }

    public static void AddPool(Pool pool) => _instance._pools.Add(pool);

    public static T SpawnFromPool<T>(string tag)
    {
        GameObject obj = _instance.SpawnGameObjectFromPool(tag, Vector3.zero, Quaternion.identity);
        if (obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"�ش� ������Ʈ�� �������� ����");
        }
    }

    public static T SpawnFromPool<T>(string tag, Vector3 position)
    {
        GameObject obj = _instance.SpawnGameObjectFromPool(tag, position, Quaternion.identity);
        if (obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"�ش� ������Ʈ�� �������� ����");
        }
    }

    public static T SpawnFromPool<T>(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject obj = _instance.SpawnGameObjectFromPool(tag, position, rotation);
        if (obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"�ش� ������Ʈ�� �������� ����");
        }
    }

    GameObject SpawnGameObjectFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            throw new Exception($"{tag}�� ���� ������Ʈ�� �������� ����");
        }

        // ť�� ������ ���� �߰�
        Queue<GameObject> poolQueue = _poolDictionary[tag];
        if (poolQueue.Count <= 0)
        {
            Pool pool = _pools.Find(x => x.Tag == tag);
            GameObject obj = CreateNewObject(pool.Tag, pool.Prefab);
            ArrangePool(obj);
        }

        // ť���� ������ ���
        GameObject objectToSpawn = poolQueue.Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    public static void ReturnToPool(GameObject obj)
    {
        if (!_instance._poolDictionary.ContainsKey(obj.name))
            Debug.Log($"Ǯ�� {obj.name}�� �����ϴ�.");

        _instance._poolDictionary[obj.name].Enqueue(obj);
    }

    void ArrangePool(GameObject obj)
    {
        // �߰��� ������Ʈ ��� ����
        bool isFind = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1)
            {
                obj.transform.SetSiblingIndex(i);
                _spawnObjects.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
                isFind = true;
            else if (isFind)
            {
                obj.transform.SetSiblingIndex(i);
                _spawnObjects.Insert(i, obj);
                break;
            }
        }
    }

    GameObject CreateNewObject(string tag, GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.name = tag;
        obj.SetActive(false); // ��Ȱ��ȭ�� ReturnToPool�� �ϹǷ� Enqueue�� ��
        return obj;
    }
}
