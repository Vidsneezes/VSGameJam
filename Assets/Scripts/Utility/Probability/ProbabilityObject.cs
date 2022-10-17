using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProbabilityObject : ScriptableObject
{
    public ScriptableObject selectionObject;
    public float probability;
    [HideInInspector]
    public float reduceProbability;
    [HideInInspector]
    public float lowRange;
    [HideInInspector]
    public float highRange;
}