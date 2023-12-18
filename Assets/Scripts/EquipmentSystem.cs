using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Equipment
{
    Vest,
    Boots,
    RifleMag,
    PistolMag,
    Grenade


}
public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] private Player player;
    private List<Equipment> equippedEquipmentList = new List<Equipment>();
    public void EquipVest(float increaseAmount)
    {
        player.GetHealthSystem().IncreaseMaxHealth(increaseAmount);
    }
    public void EquipBoots(float increaseAmount)
    {
        player.IncreaseMoveSpeed(increaseAmount);
    }
    public void EquipRifleMag(float increaseAmount)
    {
        player.GetWeaponController().GetRifle().IncreaseMagCapacity(increaseAmount);
    }
    public void EquipPistolMag(float increaseAmount)
    {
        player.GetWeaponController().GetPistol().IncreaseMagCapacity(increaseAmount);
    }
    public void EquipGrenade(int increaseAmount)
    {
        player.GetThrowSystem().IncreaseGrenadeAmount(increaseAmount);
    }
    public bool EquipGiven(EquipmentOnGround equipmentOnGround)
    {
        switch (equipmentOnGround.GetEquipment())
        {
            case Equipment.Boots:
                if (!equippedEquipmentList.Contains(Equipment.Boots))
                {
                    equippedEquipmentList.Add(Equipment.Boots);
                    EquipBoots(3);
                    return true;
                }
                break;
            case Equipment.Vest:
                if (!equippedEquipmentList.Contains(Equipment.Vest))
                {
                    equippedEquipmentList.Add(Equipment.Vest);
                    EquipVest(50);
                    return true;
                }
                break;
            case Equipment.RifleMag:
                if (!equippedEquipmentList.Contains(Equipment.RifleMag))
                {
                    equippedEquipmentList.Add(Equipment.RifleMag);
                    EquipRifleMag(5);
                    return true;

                }
                break;
            case Equipment.PistolMag:
                if (!equippedEquipmentList.Contains(Equipment.PistolMag))
                {
                    equippedEquipmentList.Add(Equipment.PistolMag);
                    EquipPistolMag(3);
                    return true;
                }
                break;
            case Equipment.Grenade:
                EquipGrenade(1);
                    return true;
          
        }
        return false;
    }
}
