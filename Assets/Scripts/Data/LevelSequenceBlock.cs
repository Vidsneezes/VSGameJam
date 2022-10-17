using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Level Sequence Block", menuName ="Data/Sequence Block")]
public class LevelSequenceBlock : ScriptableObject
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
            if (monsterOnTimes[i].hasBeenExecuted && monsterOnTimes[i].oneTime)
            {
                continue;
            }

            if (monsterOnTimes[i].CanExecute(_startTime))
            {
                monsterOnTimes[i].Execute(counterBucket);
            }
        }
    }
}
