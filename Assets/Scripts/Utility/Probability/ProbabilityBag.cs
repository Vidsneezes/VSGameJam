using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProbabilityBag : ScriptableObject
{
    public List<ProbabilityObject> probabilityObjects;

    public ScriptableObject PickOneObject()
    {
        float maxProbability = 1;
        for (int i = 0; i < probabilityObjects.Count; i++)
        {
            probabilityObjects[i].lowRange = maxProbability;
            probabilityObjects[i].highRange = maxProbability + probabilityObjects[i].probability;

            maxProbability += probabilityObjects[i].probability;
        }
        float chance = Random.Range(1, maxProbability);
        for (int i = 0; i < probabilityObjects.Count; i++)
        {
            if (probabilityObjects[i].lowRange <= chance && chance < probabilityObjects[i].highRange)
            {
                return probabilityObjects[i].selectionObject;
            }
        }

        return null;
    }

    public HashSet<ScriptableObject> PickTwoObjectsUnique()
    {
        HashSet<ScriptableObject> pickedObjects = new HashSet<ScriptableObject>();
        int iter = 2000;
        while(pickedObjects.Count < 2 && iter > 0)
        {
            iter -= 1;
            pickedObjects.Add(PickOneObject());
        }
        
        return pickedObjects;
    }

    public List<float> GetPercentages()
    {
        List<float> percentages = new List<float>();

        float maxProbability = 1;
 
        for (int i = 0; i < probabilityObjects.Count; i++)
        {
            probabilityObjects[i].lowRange = maxProbability;
            probabilityObjects[i].highRange = maxProbability + probabilityObjects[i].probability;

            maxProbability += probabilityObjects[i].probability;
        }


        for (int i = 0; i < probabilityObjects.Count; i++)
        {
            float change = probabilityObjects[i].highRange - probabilityObjects[i].lowRange;
            percentages.Add(change / maxProbability);
        }

        return percentages;
    }
}


