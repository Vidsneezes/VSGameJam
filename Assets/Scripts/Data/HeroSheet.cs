using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroSheet", menuName = "Data/Hero Sheet")]

public class HeroSheet : ScriptableObject
{
    public Sprite DisplaySprite;

    public WeaponConfiguration StarterWeaponConfiguration;

    public int Level;
    public float BaseHP;
    [HideInInspector]
    public float HP;
    public int XP;
    public int XPPool;

    public float BaseDamage = 0;
    public float Speed = 1;

    public AnimationCurve levelProgression;

    public List<WeaponConfiguration> weaponConfigs;

    public int RequiredXp
    {
        get
        {
            return 7 + 7 * Level - 2 * (Level + 1);
        }
    }

    public static HeroSheet GetInstanceHeroSheet(string sheetName)
    {
        HeroSheet sheet = Instantiate(Resources.Load<HeroSheet>($"GameData/HeroSheets/{sheetName}"));

        sheet.weaponConfigs = new List<WeaponConfiguration>();
        sheet.AddInstanceOfWeaponConfig(sheet.StarterWeaponConfiguration);
        sheet.Level = 1;
        return sheet;
    }

    public void AddInstanceOfWeaponConfig(WeaponConfiguration weapon)
    {
        WeaponConfiguration instance = Instantiate(weapon);
        instance.name = weapon.name;
        weaponConfigs.Add(instance);
    }

    public void UpdateHeroViaPowerData(HeroPowerUpData powerUpdata)
    {
        BaseHP *= powerUpdata.maxHp;
        BaseDamage += powerUpdata.damage;
        Speed *= powerUpdata.speed;
    }

    public void UpdateWeaponConfig(WeaponConfiguration weapon, WeaponPowerUpData powerUpData)
    {
        for (int i = 0; i < weaponConfigs.Count; i++)
        {
            if(weapon.name.Contains(weaponConfigs[i].name))
            {
                weaponConfigs[i].LevelUp(powerUpData);
                return;
            }
        }

        //Weapon config does not exist yet
        AddInstanceOfWeaponConfig(weapon);
    }

    public bool ReachedXpRequired()
    {
        return XP > RequiredXp;
    }


}
