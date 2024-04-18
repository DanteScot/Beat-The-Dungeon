using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum AttackDirection
{
    NONE=-1,
    UP=0,
    LEFT=90,
    DOWN=180,
    RIGHT=270
}

public class ShootingScript : RythmedObject
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private int critFrameWindow = 4;
    private int lowerCritFrameWindow;
    private bool canAttack = false;
    private bool isCrit = false;
    private bool isAttacking = false;
    private AttackDirection attackDirection;

    public float baseAttackDamage = 3.5f;
    public float critMultiplier = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 1f;


    new void Start()
    {
        base.Start();
        attackDirection = AttackDirection.NONE;
        lowerCritFrameWindow = Mathf.RoundToInt(critFrameWindow/2);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            attackDirection = AttackDirection.UP;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            attackDirection = AttackDirection.DOWN;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            attackDirection = AttackDirection.LEFT;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            attackDirection = AttackDirection.RIGHT;
        }
        
        if(attackDirection != AttackDirection.NONE && !isAttacking)
        {
            StartCoroutine(PrepareAttack());
        }
    }

    IEnumerator PrepareAttack()
    {
        isAttacking = true;

        if(canAttack){
            isCrit=true;
        }else{
            isCrit=false;

            for(int i=0; i<lowerCritFrameWindow; i++)
            {
                yield return new WaitForEndOfFrame();
                if(canAttack)
                {
                    isCrit=true;
                    break;
                }
            }
            
            if(!isCrit)
                yield return new WaitUntil(() => canAttack);
        }

        Attack();
    }


    public override void Trigger()
    {
        StartCoroutine(critWindow());
    }

    private IEnumerator critWindow()
    {
        canAttack = true;
        for(int i=0; i<critFrameWindow; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        canAttack = false;
    }


    void Attack(){
        // Attack
        var dmg=(float)(3.5*Mathf.Sqrt(1+baseAttackDamage));

        if(isCrit)
            dmg*=critMultiplier;
        
        var vel=gameObject.GetComponent<Rigidbody2D>().velocity;

        Debug.Log("player " + vel);

        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,(int)attackDirection)).GetComponent<BulletScript>().SetBullet(attackSpeed, dmg, attackRange, false, vel.x, vel.y);

        // Reset attack
        attackDirection = AttackDirection.NONE;
        isAttacking = false;
        isCrit = false;
    }
}
