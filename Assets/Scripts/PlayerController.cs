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

public class PlayerController : RythmedObject
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Animator animator;

    private int critFrameWindow;
    private int lowerCritFrameWindow;
    private bool canAttack = false;
    private bool isCrit = false;
    private bool isAttacking = false;
    private AttackDirection attackDirection;

    


    new void Start()
    {
        base.Start();
        attackDirection = AttackDirection.NONE;
        critFrameWindow = PlayerManager.Instance.critFrameWindow;
        lowerCritFrameWindow = Mathf.RoundToInt(critFrameWindow/2);
    }

    void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 3;

        GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(movement, 3);
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            attackDirection = AttackDirection.UP;
            animator.SetBool("att1", true);
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
        var dmg=(float)(3.5*Mathf.Sqrt(1+PlayerManager.Instance.baseAttackDamage));

        if(isCrit){
            // dmg*=PlayerManager.Instance.critMultiplier;
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,(int)attackDirection)).GetComponent<BulletScript>().SetBullet(PlayerManager.Instance.attackSpeed, dmg, PlayerManager.Instance.attackRange, true);
        }


        // Reset attack
        attackDirection = AttackDirection.NONE;
        isAttacking = false;
        isCrit = false;
    }
}
