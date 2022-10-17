using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : PoolManager
{
    public List<MonsterData> monsters;
    public List<int> MonsterOutOfCamera;

    public List<SoundBankPlayer> sounds;

    public List<Vector3> ReusePositions;
    public LevelSequence currentLevelSequence;
    private float _repositionTimer;
    int state = 0;
    float _timer = 0;
    float _lastSound;

    private void Awake()
    {
        prefab = "Prefabs/Monster";
        _repositionTimer = -1;
    }

    public Monster SpawnMonster(Vector3 position)
    {
        Poolable instance = pool[0];
        pool.RemoveAt(0);
        instance.transform.position = position;
        ((Monster)instance).monsterData = monsters[Random.Range(0,monsters.Count)];
        ((Monster)instance).SetPosition(position);

        instance.Revive();
        alivePool.Add(instance);
        return (Monster)instance;
    }


    public Monster SpawnMonster(int levelSequenceId, MonsterData monsterData, Vector3 position)
    {
        if(pool.Count == 0 && MonsterOutOfCamera.Count == 0)
        {
            Debug.Log("Ran out of Monsters");
            return null;
        }

        Poolable instance;
        if (pool.Count == 0)
        {
            int monsterIndex = MonsterOutOfCamera[0];
            instance = alivePool[monsterIndex];
            MonsterOutOfCamera.RemoveAt(0);
        }
        else
        {
            instance = pool[0];
            pool.RemoveAt(0);
            alivePool.Add(instance);
        }

        instance.transform.position = position;
        ((Monster)instance).monsterData = monsterData;
        ((Monster)instance).SetPosition(position);
        instance.Revive();

        return (Monster)instance;
    }

    public void EradicateAbility()
    {
        for (int i = 0; i < alivePool.Count; i++)
        {
            if((alivePool[i].transform.position - GameController.g.Hero.transform.position).magnitude < 4)
            {
                alivePool[i].Kill();
            }
        }
    }

    public void EradicateAll()
    {
        for (int i = 0; i < alivePool.Count; i++)
        {
            alivePool[i].Kill();
        }
    }

    public Vector3 PickRandomEnemy()
    {
        if (alivePool.Count > 0)
        {
            int random = Random.Range(0, alivePool.Count);
            return alivePool[random].transform.position;
        }
        else
        {
            return GameController.g.Hero.gameController.transform.position + (Vector3)Random.insideUnitCircle * 2;
        }
    }

    public Vector3 FurthestMonsterDirection(Vector3 center)
    {
        if (alivePool == null || alivePool.Count == 0)
        {
            return Random.insideUnitCircle.normalized;
        }

        int furthest = -1;
        float furthestDistance = 0;
        Vector3 position;
        for (int i = 0; i < alivePool.Count; i++)
        {
            position = alivePool[i].transform.position;
            float distance = (center - position).sqrMagnitude;
            if (distance > furthestDistance)
            {
                furthest = i;
                furthestDistance = distance;
                if (furthestDistance > 10)
                {
                    break;
                }
            }
        }

        if (furthest == -1)
        {
            return Random.insideUnitCircle.normalized;
        }

        return (alivePool[furthest].transform.position - center).normalized;
    }

    public Vector3 ClosetMonsterDirection(Vector3 center)
    {
        if(alivePool == null || alivePool.Count == 0)
        {
            return Random.insideUnitCircle.normalized;
        }

        int closest = -1;
        float closestDistance = 9999;
        Vector3 position;
        for (int i = 0; i < alivePool.Count; i++)
        {
            position = alivePool[i].transform.position;
            float distance = (center - position).sqrMagnitude;
            if(distance < closestDistance)
            {
                closest = i;
                closestDistance = distance;
                if(closestDistance < 0.001f)
                {
                    break;
                }
            } 
        }

        if(closest == -1)
        {
            return Random.insideUnitCircle.normalized;
        }

        return (alivePool[closest].transform.position - center).normalized;
    }

    public void SpawnWithPolicy(int levelSequenceId, MonsterData monsterData, RepositionPolicy policy)
    {
        SpawnMonster(levelSequenceId, monsterData, GameController.g.GetPIPositionOutsideCamera(policy.ScaleX, policy.ScaleY, policy.FuzzyX, policy.FuzzyY, policy.RangeMin, policy.RangeMax));
    }

    public void SpawnAroundPlayer(int levelSequenceId, MonsterData monsterData)
    {
        SpawnMonster(levelSequenceId,monsterData, GameController.g.GetPIPositionOutsideCamera(1.3f,2, 1.4f,1.5f));
    }

    public bool IsPointFree(Vector2 position)
    {

        for (int i = 0; i < alivePool.Count; i++)
        {
            if((alivePool[i].LocalPosition - position).sqrMagnitude > 3)
            {
                continue;
            }
            if(Physics2D.OverlapCircle(alivePool[i].LocalPosition, 0.4f) != null)
            {
                return false;
            }
        }
        return true;
    }

    public void SeparateInstancesPolicyUpwards()
    {
        Vector2 size = new Vector2(100, 100);
        Rect viewPort = new Rect((Vector2)GameController.g.Hero.transform.position - size * 0.5f, size);
        List<Poolable> narrowPhase = new List<Poolable>();
        for (int i = 0; i < alivePool.Count; i++)
        {
            if (viewPort.Contains(alivePool[i].LocalPosition))
            {
                narrowPhase.Add(alivePool[i]);
            }
        }

        for (int i = 0; i < narrowPhase.Count; i++)
        {
            Monster monster = narrowPhase[i] as Monster;
            monster.ResetAvoidance();
            int closests = -1;
            float minDistance = 99999;
            Vector2 ahead = monster.LocalPosition + monster.GetVelocity().normalized * 0.25f;
            Vector2 ahead2 = monster.LocalPosition + monster.GetVelocity().normalized * 0.125f;
            for (int j = 0; j < narrowPhase.Count; j++)
            {
                if (narrowPhase[i] == narrowPhase[j])
                {
                    continue;
                }
                float distance = OverlapCircle(narrowPhase[i], narrowPhase[j]);
                bool intersects = LineIntersectCircle(ahead, ahead2, narrowPhase[j]);

                if(intersects && distance < minDistance)
                {
                    closests = j;
                    minDistance = distance;
                }
            }

            if(closests != -1)
            {
                ((Monster)narrowPhase[i]).SetAvoidance(narrowPhase[closests].LocalPosition);
            }
        }
    }

    public bool LineIntersectCircle(Vector2 ahead, Vector2 ahead2, Poolable obstacle)
    {
        return Vector2.Distance(obstacle.ColliderPosition, ahead) <= obstacle.radius || Vector2.Distance(obstacle.ColliderPosition, ahead2) <= obstacle.radius;
    }

    public float OverlapCircle(Poolable p1, Poolable p2)
    {
        float dx = p1.LocalPosition.x - p2.LocalPosition.x;
        float dy = p1.LocalPosition.y - p2.LocalPosition.y;

        float distance = Vector2.Distance(p1.LocalPosition, p2.LocalPosition);

        if (distance < 0.25f + 0.25f)
        {
            Vector2 dir = (p1.LocalPosition - p2.LocalPosition).normalized;

            p1.SetPosition(p1.LocalPosition + dir * distance * 0.09f);
            p2.SetPosition(p2.LocalPosition - dir * distance * 0.09f);
        }
        return distance;
    }

    public override void Runner()
    {
        SeparateInstancesPolicyUpwards();

        if (currentLevelSequence != null)
        {
            if(!currentLevelSequence.Initialized)
            {
                currentLevelSequence.BeginSequence();
            }
            else
            {
                currentLevelSequence.UpdateSequence();
                if(currentLevelSequence.IsDone)
                {
                    currentLevelSequence = null;
                    return;
                }
            }


            if (currentLevelSequence.CurrentRepositionPolicy != null)
            {
                RepositionPolicyCustom();
            }
            else
            {
                RepositionPolicyLite();
            }
        }

        if(GameController.g.elapsedTime - _lastSound > 0.037f && sounds != null && sounds.Count > 0)
        {
            StateManager.g.globalAudioSource.PlayOneShot(sounds[0].clip, Random.Range(0.2f,0.4f));
            sounds.RemoveAt(0);
            _lastSound = GameController.g.elapsedTime;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if(sounds == null)
        {
            sounds = new List<SoundBankPlayer>();
        }

        sounds.Add(new SoundBankPlayer(clip));
    }

    public void RepositionPolicyLite()
    {
        bool reposition = GameController.g.elapsedTime - _repositionTimer > 1.5f || _repositionTimer < 0;

        if (reposition)
        {
            int repositionAmount = Random.Range(6, 13);

            if (MonsterOutOfCamera.Count > 50)
            {
                repositionAmount = 28;
            }
            GetReusePositions(repositionAmount,1.2f,1.5f,1.6f,1.6f);
            _repositionTimer = GameController.g.elapsedTime;
        }

        for (int i = 0; i < MonsterOutOfCamera.Count; i++)
        {
            if (ReusePositions.Count > 0)
            {
                int index = Random.Range(0, ReusePositions.Count);
                alivePool[MonsterOutOfCamera[i]].SetPosition(ReusePositions[index]);
                ReusePositions.RemoveAt(index);
            }
            else
            {
                break;
            }
        }
    }

    public void RepositionPolicyCustom()
    {
        RepositionPolicy policy = currentLevelSequence.CurrentRepositionPolicy;
        bool reposition = GameController.g.elapsedTime - _repositionTimer > policy.Time || _repositionTimer < 0;

        if (reposition)
        {
            int repositionAmount = Random.Range(policy.AmountMin, policy.AmountMax);

            if (MonsterOutOfCamera.Count > policy.MonsterLimit)
            {
                repositionAmount = policy.MonsterReplaceAmount;
            }
            GetReusePositions(repositionAmount, policy.ScaleX, policy.ScaleY, policy.FuzzyX, policy.FuzzyY, policy.RangeMin, policy.RangeMax);
            _repositionTimer = GameController.g.elapsedTime;
        }

        for (int i = 0; i < MonsterOutOfCamera.Count; i++)
        {
            if (ReusePositions.Count > 0)
            {
                if (!policy.CleanUp)
                {
                    int index = Random.Range(0, ReusePositions.Count);
                    alivePool[MonsterOutOfCamera[i]].SetPosition(ReusePositions[index]);
                    ReusePositions.RemoveAt(index);
                }
                else
                {
                    ((Monster)alivePool[MonsterOutOfCamera[i]]).Retire();
                }
                if(!policy.Rapid)
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    public void GetReusePositions(int amount, float scaleX = 1, float scaleY = 1, float fuzzyX = 1, float fuzzyY = 1, float rangeMin = 0, float rangeMax = 1)
    {
        ReusePositions = new List<Vector3>();
       

        for (int i = 0; i < amount; i++)
        {
            ReusePositions.Add(GameController.g.GetPIPositionOutsideCamera(scaleX, scaleY, fuzzyX, fuzzyY, rangeMin, rangeMax));
        }
    }

    public override void EndOfCycle()
    {
        base.EndOfCycle();
        MonsterOutOfCamera.Clear();
      
        for (int i = 0; i < alivePool.Count; i++)
        {
            Monster m = alivePool[i] as Monster;
            if(m.CalculateOutOfCameraView())
            {
                MonsterOutOfCamera.Add(i);
            }
        }
    }
}

public class SoundBankPlayer
{
    public AudioClip clip;

    public SoundBankPlayer(AudioClip _clip)
    {
        clip = _clip;
    }
}
