using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Reposition Policy", menuName ="Data/Reposition Policy")]
public class RepositionPolicy : ScriptableObject
{
    public float Time;
    public int AmountMin;
    public int AmountMax;
    public int MonsterLimit;
    public int MonsterReplaceAmount;

    public float ScaleX;
    public float ScaleY;
    public float FuzzyX;
    public float FuzzyY;
    public float RangeMin;
    public float RangeMax;
    public bool Rapid;
    public bool CleanUp;
}
