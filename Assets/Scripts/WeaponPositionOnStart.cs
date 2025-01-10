using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPositionOnStart : MonoBehaviour
{
    public Transform weapon;

    public Transform holster;

    bool oneSecondHasPassed = false;

    SwingDetection swingDetection;

    public WeaponStore weaponStoreLeft;

    bool weaponIsStored;

    private void Update()
    {
        if (!weaponStoreLeft.weaponInHolster && !weaponIsStored)
        {
            weapon.position = holster.position;
        }
        else
        {
            weaponIsStored = true;
        }
            
    }


}
