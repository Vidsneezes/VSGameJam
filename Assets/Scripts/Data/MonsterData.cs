using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/Monster")]
[System.Serializable]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    public string internalName;
    [SerializeField]
    public Sprite displayImage;
    [SerializeField]
    public float HP;
    [SerializeField]
    public float defense;
    [SerializeField]
    public float speed;
    [SerializeField]
    public float damage;
    [SerializeField]
    public float moveTime;
    [SerializeField]
    public float holdTime;
    [SerializeField]
    public Vector2 colliderOffset;
    [SerializeField]
    public float radius;
    [SerializeField]
    public ItemLootTable itemLootTable;
}


