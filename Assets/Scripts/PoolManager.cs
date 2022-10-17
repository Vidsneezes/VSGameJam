using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public string prefab;
    public List<Poolable> alivePool;
    public List<Poolable> pool;
    public int StartingPoolSize = 60;

    protected Poolable _poolablePrefab;
    private List<Poolable> returnToPool = new List<Poolable>();
    // Start is called before the first frame update
    void Start()
    {
        _poolablePrefab = Resources.Load<Poolable>(prefab);
        GeneratePool();
    }

    public virtual void GeneratePool()
    {
        for (int i = 0; i < StartingPoolSize; i++)
        {
            Poolable instance = Instantiate(_poolablePrefab, Vector3.zero, Quaternion.identity, transform);
            instance.gameObject.SetActive(false);
            instance.SetUp();
            pool.Add(instance);
        }
    }

    public virtual void Spawn(int amount)
    {

    }

    public virtual Poolable SpawnOne(Vector3 position)
    {
        Poolable instance = pool[0];
        pool.RemoveAt(0);
        instance.transform.position = position;
        instance.Revive();
        alivePool.Add(instance);
        return instance;
    }


    public virtual void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < alivePool.Count; i++)
        {
            alivePool[i].PreUpdate();
        }

        Runner();

        for (int i = 0; i < alivePool.Count; i++)
        {
            if (!alivePool[i].IsAlive)
            {
                returnToPool.Add(alivePool[i]);
                continue;
            }

            alivePool[i].OnUpdate(deltaTime);
        }

        for (int i = 0; i < returnToPool.Count; i++)
        {
            alivePool.Remove(returnToPool[i]);
            pool.Add(returnToPool[i]);
            returnToPool[i].gameObject.SetActive(false);
        }
        returnToPool.Clear();
        EndOfCycle();
    }

    public virtual void Runner()
    {
    }

    public virtual void EndOfCycle()
    {

    }
}