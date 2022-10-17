using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfiguration", menuName = "Data/WeaponConfiguration")]
public class WeaponConfiguration : ScriptableObject
{
    public WeaponData BaseData;

    public int level;
    public float weaponTimer;
    public int amountToFire;

    private IWeaponLogic WeaponLogic;
    private WeaponData _instanceWeaponData;

    public WeaponData CurrentWeapon
    {
        get
        {
            if(_instanceWeaponData == null)
            {
                _instanceWeaponData = Instantiate(BaseData);
                if(WeaponLogic == null)
                {
                    string[] logicalName = name.Split('_');
                    System.Type stateType = System.Type.GetType("WL"+logicalName[1]);
                    Debug.Log(logicalName[1]);
                    
                    object _WeaponLogic = System.Activator.CreateInstance(stateType);
                    WeaponLogic = _WeaponLogic as IWeaponLogic;
                }

            }
            return _instanceWeaponData;
        }
    }


    public void LevelUp(WeaponPowerUpData powerData)
    {
        int previousLevel = level;
        level += 1;
        powerData.SetDefaultOnZero();
        amountToFire += powerData.amountFired;
        CurrentWeapon.speed *= powerData.speed;
        CurrentWeapon.damage *= powerData.damage;
        CurrentWeapon.AliveTime *= powerData.aliveTime;
        CurrentWeapon.cooldown *= powerData.coolDown;
    }

    public void Fire()
    {
        if(WeaponLogic != null)
        {
            WeaponLogic.Fire(this);
        }
    }
}


