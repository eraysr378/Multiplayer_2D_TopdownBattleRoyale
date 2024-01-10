using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Weapon
{
    public Sprite GetWeaponSprite();
    public bool Fire();
    public Vector3 GetFirePoint();

}

