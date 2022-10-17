using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLBloodFangs : IWeaponLogic
{
    public void Fire(WeaponConfiguration weaponConfig)
    {
        for (int i = 0; i < weaponConfig.amountToFire; i++)
        {
            Weapon instance = GameController.g.Weapons.SpawnWeapon(GameController.g.Hero.MidPoint, weaponConfig.CurrentWeapon);
            if (instance != null)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                instance.Launch(dir);
                instance.Delay = (float)i * 0.089f;
                instance.WeaponLogic = this;
            }
        }
    }

    public void OnUpdateWeapon(Weapon weapon)
    {
        WeaponData weaponData = weapon.weaponData;

        float accumulatedTime = (GameController.g.elapsedTime - weapon._timer);
        Vector2 center = weapon.origin;
        if (weaponData.boundToHero)
        {
            center = GameController.g.Hero.transform.position;
        }
        Vector2 position = Vector2.zero;

        weapon.kineticSpeed += weaponData.speed * Time.deltaTime * 15;
        position.x = Mathf.Sin(accumulatedTime * weaponData.speed) * weaponData.rotationSpeed;
        position.y = Mathf.Cos(accumulatedTime * weaponData.speed) * weaponData.rotationSpeed;
        weapon.SetPosition(Vector2.MoveTowards(weapon.LocalPosition, center + position, GameController.g.elapsedTime * 10));
    }
}
