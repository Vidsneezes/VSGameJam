using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLSlash : IWeaponLogic
{
    public void Fire(WeaponConfiguration weaponConfig)
    {
        for (int i = 0; i < weaponConfig.amountToFire; i++)
        {
            Weapon instance = GameController.g.Weapons.SpawnWeapon(GameController.g.Hero.MidPoint, weaponConfig.CurrentWeapon);
            if (instance != null)
            {
                int facing = GameController.g.Hero.Facing;
                instance.MeleeLaunch(weaponConfig.CurrentWeapon.MeleeDirection * facing);
                instance.Delay = (float)i * 0.001f;
                instance.WeaponLogic = this;

            }
        }
    }

    public void OnUpdateWeapon(Weapon weapon)
    {
        WeaponData weaponData = weapon.weaponData;

        Vector2 localPosition = weapon.LocalPosition;

        if (weaponData.boundToHero)
        {
            Vector2 heroPosition = GameController.g.Hero.transform.position;
            localPosition += (localPosition - heroPosition);
        }
        weapon.UpdatePositionViaVelocity();
    }
}
