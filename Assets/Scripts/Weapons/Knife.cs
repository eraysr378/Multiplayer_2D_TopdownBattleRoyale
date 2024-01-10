using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knife : MonoBehaviour, Weapon
{
    [SerializeField] private Player player;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDamage;
    [SerializeField] private float neededTimeToHit;
    [SerializeField] private Sprite sprite;
    bool attack = true;
    float timer = 0;
    float hitTimer = 0;
    private void Start()
    {
        attackDamage = 50;
        neededTimeToHit = timeBetweenAttacks / 2;
    }
    private void Update()
    {
        if (!attack)
        {
            timer += Time.deltaTime;

            if (timer > timeBetweenAttacks)
            {
                attack = true;
                player.SetCanShoot(true);
                timer = 0;
            }
            hitTimer += Time.deltaTime;
            if (hitTimer > neededTimeToHit)
            {
                Hit();
                hitTimer = -999;
            }
        }
    }
    public bool Fire()
    {
        if (player.CanShoot() && attack)
        {
            player.SetCanShoot(false);
            attack = false;
            hitTimer = 0;

            return true;
        }
        return false;
    }
    public void Hit()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(attackPoint.position, attackRadius, Vector3.zero);
        foreach (RaycastHit2D hit in hits)
        {
            WeaponBox weaponBox = hit.transform.gameObject.GetComponent<WeaponBox>();
            Player enemy = hit.transform.gameObject.GetComponent<Player>();
            AbilityBox abilityBox = hit.transform.gameObject.GetComponent<AbilityBox>();
            EquipmentBox equipmentBox = hit.transform.gameObject.GetComponent<EquipmentBox>();

            if (weaponBox != null)
            {
                weaponBox.TakeDamage(attackDamage);
            }
            if (enemy != null && enemy != player)
            {
                enemy.TakeDamage(attackDamage);
            }
            if (abilityBox != null)
            {
                abilityBox.TakeDamage(attackDamage);
            }
            if (equipmentBox != null)
            {
                equipmentBox.TakeDamage(attackDamage);
            }
        }
    }
    public Vector3 GetFirePoint()
    {
        return attackPoint.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(0, 200, 0, 100);
        Gizmos.DrawSphere(attackPoint.position, attackRadius);
    }

    public Sprite GetWeaponSprite()
    {
        return sprite;
    }
}
