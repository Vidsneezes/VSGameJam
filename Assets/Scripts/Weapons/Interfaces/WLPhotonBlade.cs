using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLPhotonBlade : IWeaponLogic
{
    public void Fire(WeaponConfiguration weaponConfig)
    {
        for (int i = 0; i < weaponConfig.amountToFire; i++)
        {
            Weapon instance = GameController.g.Weapons.SpawnWeapon(GameController.g.Hero.MidPoint, weaponConfig.CurrentWeapon);
            if (instance != null)
            {
                Vector2 dir = (GameController.g.Monsters.PickRandomEnemy() - GameController.g.Hero.transform.position).normalized;
                instance.Launch(dir);
                instance.Delay = (float)i * 0.02f;
                instance.WeaponLogic = this;

            }
        }
    }

    public void OnUpdateWeapon(Weapon weapon)
    {
        WeaponData weaponData = weapon.weaponData;

        if (weaponData.accelerationSlope > 0.01f)
        {
            weapon.velocity += Vector2.Lerp(Vector2.zero, weapon._acceleration, (GameController.g.elapsedTime - weapon._timer / weaponData.AliveTime) * 2f) * weaponData.acceleration * GameController.g.deltaTime;
        }
        else
        {
            weapon.velocity += weapon._acceleration * weaponData.acceleration * GameController.g.deltaTime;
        }

        weapon.velocity = Vector2.ClampMagnitude(weapon.velocity, weaponData.speed);
        weapon.UpdatePositionViaVelocity();
    }
}
