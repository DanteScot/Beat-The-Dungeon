using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private Transform player;

    public float moveSpeed = 1f;
    public float health = 3f;
    public float luck = 1f;
    public float baseAttackDamage = 3.5f;
    public float critMultiplier = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
    public int critFrameWindow = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("PlayerManager already exists, destroying new one");
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            player.GetComponent<PlayerController>().StartCoroutine("Die");
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public Transform GetPlayer()
    {
        return player;
    }
}
