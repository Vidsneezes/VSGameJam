using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroPowerUpData", menuName = "Data/Hero/HeroPowerUpData")]
public class HeroPowerUpData : BasePowerUpData
{
    public float speed;
    public float damage;
    public float maxHp;
    public float hpRecover;

    public override void SetUp()
    {
        SetDefaultOnZero();
    }

    public override void Execute()
    {
        GameController.g.Hero.UpgradeViaPowerData(this);
    }

    public void SetDefaultOnZero()
    {
        if (speed < 0.001f)
        {
            speed = 1;
        }
        if (damage < 0.001f)
        {
            damage = 1;
        }
        if(maxHp < 0.001f)
        {
            maxHp = 1;
        }
    }
}
