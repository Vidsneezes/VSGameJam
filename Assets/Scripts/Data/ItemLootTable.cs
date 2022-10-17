using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Logic based on https://www.gamedeveloper.com/design/loot-drop-best-practices

[CreateAssetMenu(fileName = "ItemLootTable", menuName = "Data/Item Loot Table")]
public class ItemLootTable : ScriptableObject
{
    public List<ItemDropElement> itemDrops;

    public ItemData DropItem()
    {
        float maxProbability = 1;
        for (int i = 0; i < itemDrops.Count; i++)
        {
            itemDrops[i].lowRange = maxProbability;
            itemDrops[i].highRange = maxProbability + itemDrops[i].probability;

            maxProbability += itemDrops[i].probability;
        }
        float chance = Random.Range(1, maxProbability);
        for (int i = 0; i < itemDrops.Count; i++)
        {
            if(itemDrops[i].lowRange <= chance && chance < itemDrops[i].highRange)
            {
                return itemDrops[i].item;
            }
        }

        return null;
    }

    public List<float> GetPercentages()
    {
        List<float> percentages = new List<float>();

        float maxProbability = 1;
        for (int i = 0; i < itemDrops.Count; i++)
        {
            itemDrops[i].lowRange = maxProbability;
            itemDrops[i].highRange = maxProbability + itemDrops[i].probability;

            maxProbability += itemDrops[i].probability;
        }

        
        for (int i = 0; i < itemDrops.Count; i++)
        {
            float change = itemDrops[i].highRange - itemDrops[i].lowRange;
            percentages.Add(change / maxProbability);
        }

        return percentages;
    }
}

[System.Serializable]
public class ItemDropElement
{
    public ItemData item;
    public float probability;
    [HideInInspector]
    public float reduceProbability;
    [HideInInspector]
    public float lowRange;
    [HideInInspector]
    public float highRange;
}
