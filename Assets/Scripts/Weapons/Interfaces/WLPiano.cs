using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLPiano : IWeaponLogic
{
    public void Fire(WeaponConfiguration weaponConfig)
    {
        for (int i = 0; i < weaponConfig.amountToFire; i++)
        {
            Weapon instance = GameController.g.Weapons.SpawnWeapon(GameController.g.Hero.MidPoint + Vector3.up * 3.4f, weaponConfig.CurrentWeapon);
            if (instance != null)
            {
                instance.SetDestination(GameController.g.Hero.MidPoint);
                instance.collisionOff = true;
                float horizontal = Random.Range(-0.3f, 0.3f);
                instance.Launch(Vector2.down + Vector2.right * horizontal);
                instance.MakeTransparent();
                instance.Delay = (float)i * 0.089f;
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

        if (weaponData.accelerationSlope > 0.01f)
        {
            weapon.velocity += Vector2.Lerp(Vector2.zero, weapon._acceleration, (GameController.g.elapsedTime - weapon._timer / weaponData.AliveTime) * 2f) * weaponData.acceleration * GameController.g.deltaTime;
        }
        else
        {
            weapon.velocity += weapon._acceleration * weaponData.acceleration * GameController.g.deltaTime;
        }

        weapon.velocity = Vector2.ClampMagnitude(weapon.velocity, weaponData.speed);
        localPosition = localPosition + weapon.velocity * GameController.g.deltaTime;
        weapon.UpdatePositionViaVelocity();

        if (weapon.collisionOff)
        {
            if (Vector2.Distance(weapon.transform.position, weapon.destination) < 1.7f)
            {
                weapon.MakeSolid();
                weapon.collisionOff = false;
            }
        }
    }
}
