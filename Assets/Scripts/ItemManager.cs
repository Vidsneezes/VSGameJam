using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : PoolManager
{
    public List<ItemLootTable> lootTables;

    private Dictionary<string, ItemLootTable> _instanceLootTables; 

    private void Awake()
    {
        prefab = "Prefabs/Item";

        _instanceLootTables = new Dictionary<string, ItemLootTable>();
        for (int i = 0; i < lootTables.Count; i++)
        {
            _instanceLootTables.Add(lootTables[i].name, Instantiate(lootTables[i]));
        }
    }

    public void DropItem(Vector3 position, ItemData itemData)
    {
        Poolable instance = pool[0];
        pool.RemoveAt(0);
        instance.transform.position = position;
        ((Item)instance).itemData = itemData;
        instance.Revive();
        alivePool.Add(instance);
    }

    public void DropItemFromLootTable(ItemLootTable lootTable, Vector3 position)
    {
        ItemLootTable lt;
        if(_instanceLootTables.TryGetValue(lootTable.name, out lt))
        {
            ItemData item = lt.DropItem();
            if(item == null)
            {
                Debug.LogError("The item should not be null, check probability");
                return;
            }

            if(item.name.Contains("Null"))
            {
                return;
            }
            DropItem(position, item);
        }
    }

    public void DropSmallCoin(Vector3 position)
    {
    }
}