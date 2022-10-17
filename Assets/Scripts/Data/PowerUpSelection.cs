using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Power Up Selection", menuName ="Data/Power Up Selection")]
public class PowerUpSelection : ScriptableObject
{
    public string description;
    public WeaponConfiguration WeaponToAdd;
    public WeaponPowerUpData PowerUpData;
  
}
