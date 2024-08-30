using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float moveSpeed;
    public float maxHealth;
    public float currentHealth;
    public float luck;
    public float baseAttackDamage;
    public float attackSpeed;
    public float attackRange;

    public GameData()
    {
        moveSpeed = 4f;
        maxHealth = 4f;
        currentHealth = 4f;
        luck = 1f;
        baseAttackDamage = 2.5f;
        attackSpeed = 3f;
        attackRange = 2f;
    }

    public GameData(PlayerManager playerManager)
    {
        moveSpeed = playerManager.MoveSpeed;
        maxHealth = playerManager.MaxHealth;
        currentHealth = playerManager.CurrentHealth;
        luck = playerManager.Luck;
        baseAttackDamage = playerManager.BaseAttackDamage;
        attackSpeed = playerManager.AttackSpeed;
        attackRange = playerManager.AttackRange;
    }
}