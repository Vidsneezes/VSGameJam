using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : PoolManager
{
    private void Awake()
    {
        prefab = "Prefabs/Weapon";
    }

    public Weapon SpawnWeapon(Vector3 position, WeaponData weaponData)
    {
        Weapon instance = (Weapon)pool[0];
        pool.RemoveAt(0);
        instance.weaponData = weaponData;
        instance.Revive();
        instance.SetPosition(position);
        instance.Hero = GameController.g.Hero;
        alivePool.Add(instance);
        return instance;
    }

}
