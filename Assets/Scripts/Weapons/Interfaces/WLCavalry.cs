using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WLCavalry : IWeaponLogic
{
    private List<Vector2> _calvaryLaunchPoints = new List<Vector2>()
    {
        new Vector2(-6.5f,-4.8f),
        new Vector2(6.5f,-4.8f),
        new Vector2(-6.5f,4.8f),
        new Vector2(6.5f,4.8f),
        new Vector2(-6.5f,0),
        new Vector2(6.5f,0),
    };

    public void Fire(WeaponConfiguration weaponConfig)
    {
        if (GameController.g.Weapons.pool.Count > 20)
        {
            int randomIndex = Random.Range(0, _calvaryLaunchPoints.Count);
            Vector3 instancePoint = _calvaryLaunchPoints[randomIndex];
            for (int i = 0; i < 7 * weaponConfig.amountToFire; i++)
            {
                instancePoint += Random.onUnitSphere * 0.9f;
                instancePoint.z = 0;
                Weapon instance = GameController.g.Weapons.SpawnWeapon(instancePoint, weaponConfig.CurrentWeapon);
                instance.LaunchWithFacing(-_calvaryLaunchPoints[randomIndex].normalized);
                instance.Delay = -1f;
                instance.WeaponLogic = this;
            }
        }

    }

    public void OnUpdateWeapon(Weapon weapon)
    {
        WeaponData weaponData = weapon.weaponData;
        Vector2 heroPosition = GameController.g.Hero.transform.position;
        weapon.LocalPosition += weapon.velocity * GameController.g.deltaTime;
        weapon.transform.position = heroPosition + weapon.LocalPosition;

        weapon._spriteRenderer.material.SetFloat("_BounceTime", Mathf.Cos(61f * GameController.g.elapsedTime));
    }
}
