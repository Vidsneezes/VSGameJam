using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponLogic 
{
    void Fire(WeaponConfiguration weaponConfig);
    void OnUpdateWeapon(Weapon weapon);


}
