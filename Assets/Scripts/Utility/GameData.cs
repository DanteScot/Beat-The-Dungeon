using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int moveSpeed;
    public int maxHealth;
    public int luck;
    public int baseAttackDamage;
    public int attackSpeed;
    public int attackRange;
    public int gears;

    public GameData()
    {
        moveSpeed = 0;
        maxHealth = 0;
        luck = 0;
        baseAttackDamage = 0;
        attackSpeed = 0;
        attackRange = 0;
        gears = 0;
    }

    public GameData(PlayerManager playerManager)
    {
        moveSpeed = playerManager.MoveSpeedLevel;
        maxHealth = playerManager.HealthLevel;
        luck = playerManager.LuckLevel;
        baseAttackDamage = playerManager.DamageLevel;
        attackSpeed = playerManager.AttackSpeedLevel;
        attackRange = playerManager.AttackRangeLevel;
        gears = playerManager.Gears;
    }
}