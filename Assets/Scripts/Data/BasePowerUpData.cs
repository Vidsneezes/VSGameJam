using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePowerUpData : ScriptableObject
{
    public string description;
    public abstract void SetUp();

    public abstract void Execute();
}
