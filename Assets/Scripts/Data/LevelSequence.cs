using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Sequence", menuName = "Data/Level Sequence")]

public class LevelSequence : ScriptableObject
{
    public List<MonsterWaveData> waveSequence;
    public List<LevelSequenceBlock> sequenceTemplate;

    public bool Initialized;
    public bool IsDone;

    private List<LevelSequenceBlock> _instanceSequence;

    public RepositionPolicy CurrentRepositionPolicy
    {
        get
        {
            if(_sequenceIndex < 0 || _sequenceIndex >= _instanceSequence.Count)
            {
                return null;
            }
            return _instanceSequence[_sequenceIndex].repositionPolicy;
        }
    }
    private int _sequenceIndex;

    public void BeginSequence()
    {
        Initialized = true;

        _instanceSequence = new List<LevelSequenceBlock>();
        for (int i = 0; i < sequenceTemplate.Count; i++)
        {
            _instanceSequence.Add(LevelSequenceBlock.Instantiate(sequenceTemplate[i]));
        }

        _sequenceIndex = 0;
        _instanceSequence[_sequenceIndex].OnStart();
    }

    public void UpdateSequence()
    {
        if(_sequenceIndex >= _instanceSequence.Count)
        {
            return;
        }

        if(_instanceSequence[_sequenceIndex].IsDone)
        {
            IncrementWave();
        }
        else
        {
            _instanceSequence[_sequenceIndex].OnUpdate();
        }
    }

    public void IncrementWave()
    {
        if(IsDone)
        {
            return;
        }

        _sequenceIndex += 1;
        if(_sequenceIndex >= _instanceSequence.Count)
        {
            _sequenceIndex = _instanceSequence.Count;
            GameController.g.Monsters.EradicateAll();
            GameController.g.GameClearTime = GameController.g.elapsedTime;
            IsDone = true;
        }
        else
        {
            _instanceSequence[_sequenceIndex].OnStart();
        }
    }
}

[UnityEngine.Scripting.Preserve]
[System.Serializable]
public class MonsterWaveData
{
    public List<MonsterOnTime> monsterOnTimes; 
    public float duration;
    public int counterBucket;
    public RepositionPolicy repositionPolicy;
    public bool IsDone
    {
        get
        {
            return GameController.g.elapsedTime - _startTime > duration;
        }
    }

    private float _startTime;

    public void OnStart()
    {
        _startTime = GameController.g.elapsedTime;
    }

    public void OnUpdate()
    {
        for (int i = 0; i < monsterOnTimes.Count; i++)
        {
            if(monsterOnTimes[i].hasBeenExecuted && monsterOnTimes[i].oneTime)
            {
                continue;
            }

            if(monsterOnTimes[i].CanExecute(_startTime))
            {
                monsterOnTimes[i].Execute(counterBucket);
            }
        }
    }
}

[UnityEngine.Scripting.Preserve]
[System.Serializable]
public class MonsterProbability
{
    public float probability;
    public MonsterData monster;
}

[UnityEngine.Scripting.Preserve]
[System.Serializable]
public class MonsterOnTime
{
    public float FirtsTime;
    public float RepeatTime;
    public RepositionPolicy SpawnPolicy;
    public MonsterData monster;
    public List<MonsterProbability> monsterProbabilities;
    public int amount;
    public bool useSingleMonster;

    public bool hasBeenExecuted;
    public bool oneTime;

    public bool CanExecute(float startTime)
    {
        if(!hasBeenExecuted)
        {
            return GameController.g.elapsedTime - startTime > FirtsTime;
        }
        else
        {
            return GameController.g.elapsedTime - _lastExecuted > RepeatTime;
        }
    }

    private float _lastExecuted = 0;

    public void Execute(int counter)
    {
        hasBeenExecuted = true;

        for (int i = 0; i < amount; i++)
        {
            if(SpawnPolicy != null)
            {
                GameController.g.Monsters.SpawnWithPolicy(counter, monster, SpawnPolicy);
            }
            else
            {
                GameController.g.Monsters.SpawnAroundPlayer(counter, monster);
            }
        }

        _lastExecuted = GameController.g.elapsedTime;
    }
}