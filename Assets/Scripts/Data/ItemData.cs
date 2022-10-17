using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public enum SpecialAbility
    {
        None,
        Eradicate
    }

    public Sprite displayImage;
    public float AmountXP;
    public float AmountHp;

    public SpecialAbility ability;
}

