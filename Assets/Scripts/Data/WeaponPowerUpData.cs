using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeaponPowerUpData", menuName ="Data/Weapons/PowerUpData")]
public class WeaponPowerUpData : BasePowerUpData
{
    public WeaponConfiguration weaponToModify;
    public float speed;
    public float damage;
    public float aliveTime;
    public float coolDown;
    public int amountFired;

    public override void SetUp()
    {
        string oldDescription = description + "!";
        SetDefaultOnZero();
        WeaponConfiguration weaponConfic = GameController.g.Hero.WeaponController.GetWeaponConfigInstance(weaponToModify);
        if (weaponConfic != null)
        {
            description = $"{name.Split('_')[0]} {oldDescription}";
        }
        else
        {
            description = $"GET {name.Split('_')[0]}!";
        }
    }

    public override void Execute()
    {
        WeaponConfiguration weaponConfic = GameController.g.Hero.WeaponController.GetWeaponConfigInstance(weaponToModify);
        GameController.g.Hero.WeaponController.UpdateWeapon(weaponToModify, this);
    }

    public void SetDefaultOnZero()
    {
        if(speed < 0.001f)
        {
            speed = 1;
        }
        if (damage < 0.001f)
        {
            damage = 1;
        }
        if (aliveTime < 0.001f)
        {
            aliveTime = 1;
        }
        if (coolDown < 0.001f)
        {
            coolDown = 1;
        }
    }
}
