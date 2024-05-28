using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, Subject
{
    public static PlayerManager Instance { get; private set; }

    private List<Observer> observers = new List<Observer>();

    private Transform player;

    private float moveSpeed = 1f;
    private float maxHealth = 3f;
    private float currentHealth = 3f;
    private float luck = 1f;
    private float baseAttackDamage = 3.5f;
    private float critMultiplier = 2f;
    private float attackSpeed = 1f;
    private float attackRange = 1f;
    private int critFrameWindow = 2;

    public float MoveSpeed { get => moveSpeed; set {moveSpeed = value; Notify();} }
    public float MaxHealth { get => maxHealth; set {maxHealth = value; Notify();} }
    public float CurrentHealth { get => currentHealth; set {currentHealth = value; Notify();} }
    public float Luck { get => luck; set {luck = value; Notify();} }
    public float BaseAttackDamage { get => baseAttackDamage; set {baseAttackDamage = value; Notify();} }
    public float CritMultiplier { get => critMultiplier; set {critMultiplier = value; Notify();} }
    public float AttackSpeed { get => attackSpeed; set {attackSpeed = value; Notify();} }
    public float AttackRange { get => attackRange; set {attackRange = value; Notify();} }
    public int CritFrameWindow { get => critFrameWindow; set {critFrameWindow = value; Notify();} }

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
        currentHealth -= damage;
        if (currentHealth <= 0)
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

    public void Attach(Observer observer)
    {
        observers.Add(observer);
    }

    public void Detach(Observer observer)
    {
        observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (Observer observer in observers)
        {
            observer.Notify();
        }
    }
}
