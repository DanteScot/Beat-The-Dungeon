using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AttackDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class PlayerManager : RythmedObject
{
    [Header("Player Settings")]
    [SerializeField] private GameObject bulletPrefab;
    public float critWindowTime = 0.1f;
    

    [Space(10)]

    [Header("Player Stats")]
    public float baseAttackDamage = 10f;
    public float damageMultiplier = 1f;
    public float critMultiplier = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;
    public float moveSpeed = 1f;
    public float health = 3f;
    public float luck = 1f;

    private bool canAttack = false;
    // private bool wantToAttack = false;
    private AttackDirection attackDirection;
    private bool isCrit = false;

    public float attackDamage
    {
        get
        {
            return baseAttackDamage * damageMultiplier;
        }
    }


    new void Start()
    {
        base.Start();
        attackDirection = AttackDirection.None;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            StartCoroutine(PrepareAttack(AttackDirection.Up));
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            StartCoroutine(PrepareAttack(AttackDirection.Down));
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            StartCoroutine(PrepareAttack(AttackDirection.Left));
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
            StartCoroutine(PrepareAttack(AttackDirection.Right));
        }
    }

    IEnumerator PrepareAttack(AttackDirection direction)
    {
        if(canAttack){
            isCrit=true;
        }else{
            isCrit=false;
            yield return new WaitUntil(() => canAttack);
        }
        attackDirection = direction;
        Attack();
    }


    public override void Trigger()
    {
        StopCoroutine(critWindow());
        StartCoroutine(critWindow());
    }

    private IEnumerator critWindow()
    {
        canAttack = true;
        yield return new WaitForSeconds(critWindowTime);
        canAttack = false;
    }


    void Attack(){
        Debug.Log("Direction: " + attackDirection + "Is Crit: " + isCrit);
        // Attack
        Instantiate(bulletPrefab, transform.position, transform.rotation).GetComponent<BulletScript>().SetBullet(attackSpeed, attackDamage, attackRange);

        // Reset attack
        attackDirection = AttackDirection.None;
        isCrit = false;
    }
}
