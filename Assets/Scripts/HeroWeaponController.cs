using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroWeaponController : MonoBehaviour
{
    public HeroSheet heroSheet;

    public void SetHeroSheet(HeroSheet sheet)
    {
        heroSheet = sheet;
    }

    public void UpdateWeapon(WeaponConfiguration weapon, WeaponPowerUpData powerUpData)
    {
        heroSheet.UpdateWeaponConfig(weapon, powerUpData);
    }

    public WeaponConfiguration GetWeaponConfigInstance(WeaponConfiguration weapon)
    {
        for (int i = 0; i < heroSheet.weaponConfigs.Count; i++)
        {
            if(weapon.name == heroSheet.weaponConfigs[i].name)
            {
                return heroSheet.weaponConfigs[i];
            }
        }
        return null;
    }

    public void OnUpdate()
    {
        if (heroSheet.weaponConfigs != null && heroSheet.weaponConfigs.Count > 0)
        {
            for (int i = 0; i < heroSheet.weaponConfigs.Count; i++)
            {
                ProcessWeaponConfiguration(heroSheet.weaponConfigs[i]);
            }
        }
    }

    public void ProcessWeaponConfiguration(WeaponConfiguration weaponConfig)
    {

        if (GameController.g.elapsedTime - weaponConfig.weaponTimer > weaponConfig.CurrentWeapon.cooldown)
        {
            weaponConfig.Fire();
            weaponConfig.weaponTimer = GameController.g.elapsedTime;
        }
    }
}
