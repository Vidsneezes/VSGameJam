using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeaponData", menuName ="Data/Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponStyles
    {
        Direct,
        AreaDamage,
        Melee,
        Piercer,
        Radial,
        Falling,
        Calvary
    }

    public Sprite displayImage;
    public bool invincible;
    public int HP;
    public float acceleration;
    public float accelerationSlope;
    public float speed;
    public float rotationSpeed;
    public bool boundToHero;
    public bool aimDirection;
    public float spinSpeed;

    public float damage;
    public float cooldown;
    public float AliveTime;

    public WeaponStyles Style;

    public Vector2 MeleeDirection;
    public float radius;
}
