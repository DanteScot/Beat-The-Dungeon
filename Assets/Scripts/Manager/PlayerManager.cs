using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct startingStats
{
    public float moveSpeed;
    public float maxHealth;
    public float luck;
    public float baseAttackDamage;
    public float attackSpeed;
    public float attackRange;

    public startingStats(float moveSpeed, float maxHealth, float luck, float baseAttackDamage, float attackSpeed, float attackRange)
    {
        this.moveSpeed = moveSpeed;
        this.maxHealth = maxHealth;
        this.luck = luck;
        this.baseAttackDamage = baseAttackDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
    }
}

public class PlayerManager : MonoBehaviour, Subject
{
    public static PlayerManager Instance { get; private set; }

    private startingStats startingStats;
    private List<Observer> observers = new List<Observer>();
    private Transform player;

    #region Stats

    [Header("Player Stats")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float maxHealth = 4f;
    [SerializeField] private float currentHealth = 4f;
    [SerializeField] private float luck = 1f;
    [SerializeField] private float baseAttackDamage = 2.5f;
    [SerializeField] private float attackSpeed = 3f;
    [SerializeField] private float attackRange = 2f;

    [Header("Player Stats Level")]
    [SerializeField] private int moveSpeedLevel = 0;
    [SerializeField] private int healthLevel = 0;
    [SerializeField] private int luckLevel = 0;
    [SerializeField] private int damageLevel = 0;
    [SerializeField] private int attackSpeedLevel = 0;
    [SerializeField] private int attackRangeLevel = 0;

    [Header("Other Stats")]
    [SerializeField] private int gears = 0;

    #endregion


    #region Inline Getters and Setters

    public float MoveSpeed { get => moveSpeed; set {moveSpeed = value; Notify();} }
    public float MaxHealth { get => maxHealth; set {maxHealth = value; Notify();} }
    public float CurrentHealth { get => currentHealth; set {currentHealth = value; Notify();} }
    public float Luck { get => luck; set {luck = value; Notify();} }
    public float BaseAttackDamage { get => baseAttackDamage; set {baseAttackDamage = value; Notify();} }
    public float AttackSpeed { get => attackSpeed; set {attackSpeed = value; Notify();} }
    public float AttackRange { get => attackRange; set {attackRange = value; Notify();} }

    public int MoveSpeedLevel { get => moveSpeedLevel; set {moveSpeedLevel = value; Notify();} }
    public int HealthLevel { get => healthLevel; set {healthLevel = value; Notify();} }
    public int LuckLevel { get => luckLevel; set {luckLevel = value; Notify();} }
    public int DamageLevel { get => damageLevel; set {damageLevel = value; Notify();} }
    public int AttackSpeedLevel { get => attackSpeedLevel; set {attackSpeedLevel = value; Notify();} }
    public int AttackRangeLevel { get => attackRangeLevel; set {attackRangeLevel = value; Notify();} }

    public float MoveSpeedLevelled { get => moveSpeed + moveSpeedLevel; }
    public float MaxHealthLevelled { get => maxHealth + (healthLevel*2); }
    public float LuckLevelled { get => luck + (luckLevel*2); }
    public float BaseAttackDamageLevelled { get => baseAttackDamage + (damageLevel*0.5f); }
    public float AttackSpeedLevelled { get => attackSpeed + (attackRangeLevel*1.5f); }
    public float AttackRangeLevelled { get => attackRange + (attackRangeLevel*2); }

    public int Gears { get => gears; set {gears = value; Notify();} }

    #endregion


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            startingStats = new startingStats(moveSpeed, maxHealth, luck, baseAttackDamage, attackSpeed, attackRange);
            // HO DOVUTO INIZIALIZZARE IL SEED PERCHE' NON FUNZIONAVA RANDOM
            // QUANTOMENO NON NELLO START DI Lobby808Controller, DAVA SEMPRE LO STESSO NUMERO
            Random.InitState(System.DateTime.Now.Millisecond);
        }
        else
        {
            Debug.LogWarning("PlayerManager already exists, destroying new one");
            Destroy(gameObject);
        }
    }

    public void EndGame()
    {
        moveSpeed = startingStats.moveSpeed;
        maxHealth = startingStats.maxHealth;
        luck = startingStats.luck;
        baseAttackDamage = startingStats.baseAttackDamage;
        attackSpeed = startingStats.attackSpeed;
        attackRange = startingStats.attackRange;

        Notify();
    }


    // TODO: Remove this Update method
    public void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            GameEvent.IsPaused = !GameEvent.IsPaused;
        }


        if(Input.GetKeyDown(KeyCode.F1)){
            MoveSpeed+=1;
        }
        if(Input.GetKeyDown(KeyCode.F2)){
            MoveSpeed-=1;
        }
        if(Input.GetKeyDown(KeyCode.F10)){
            Gears+=100;
        }
        if(Input.GetKeyDown(KeyCode.F12)){
            SaveSystem.SaveGame(new GameData(this));
        }
    }

    public void LoadPlayerStats(GameData data)
    {
        moveSpeedLevel = data.moveSpeed;
        healthLevel = data.maxHealth;
        luckLevel = data.luck;
        damageLevel = data.baseAttackDamage;
        attackSpeedLevel = data.attackSpeed;
        attackRangeLevel = data.attackRange;
        gears = data.gears;

        Notify();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            player.GetComponent<PlayerController>().StartCoroutine("Die");
        }
        Debug.Log(CurrentHealth);
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
