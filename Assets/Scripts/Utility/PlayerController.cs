using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

public class PlayerController : RythmedObject, Observer
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject meleeCheck;
    [SerializeField] private Animator animator;

    private int critFrameWindow;
    private float attackSpeed;
    private float baseAttackDamage;
    private float finalDamage;
    private float attackRange;
    private float moveSpeed;

    private int lowerCritFrameWindow;
    private bool canAttack = false;
    private bool isCrit = false;
    private bool isAttacking = false;
    private AttackDirection attackDirection;

    private Vector2 movement;


    public void Awake()
    {
        PlayerManager.Instance.SetPlayer(transform);
    }

    new void Start()
    {
        base.Start();
        Notify();
        PlayerManager.Instance.Attach(this);
        attackDirection = AttackDirection.NONE;
        lowerCritFrameWindow = Mathf.RoundToInt(critFrameWindow/2);
    }

    public void Notify()
    {
        critFrameWindow = PlayerManager.Instance.CritFrameWindow;
        attackSpeed = PlayerManager.Instance.AttackSpeed;
        baseAttackDamage = PlayerManager.Instance.BaseAttackDamage;
        attackRange = PlayerManager.Instance.AttackRange;
        moveSpeed = PlayerManager.Instance.MoveSpeed;

        finalDamage=(float)(3.5*Mathf.Sqrt(1+baseAttackDamage));
    }

    void Update()
    {
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed;
        GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(movement, moveSpeed);
        animator.SetFloat("Speed", movement.magnitude);

        if(!animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("player_attack")){
            if(Input.GetAxis("Horizontal")<0)   transform.localScale = new Vector3(-1, 1, 1);
            else if(Input.GetAxis("Horizontal")>0)  transform.localScale = new Vector3(1, 1, 1);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))           attackDirection = AttackDirection.UP;
        else if(Input.GetKeyDown(KeyCode.DownArrow))    attackDirection = AttackDirection.DOWN;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))    attackDirection = AttackDirection.LEFT;
        else if(Input.GetKeyDown(KeyCode.RightArrow))   attackDirection = AttackDirection.RIGHT;
        
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
        if(attackDirection == AttackDirection.LEFT) transform.localScale = new Vector3(-1, 1, 1);
        else if(attackDirection == AttackDirection.RIGHT) transform.localScale = new Vector3(1, 1, 1);

        StartCoroutine(CheckMelee());
        animator.SetTrigger("Attack");

        if(isCrit){
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,(int)attackDirection)).GetComponent<BulletScript>().SetBullet(attackSpeed, finalDamage, attackRange, movement.normalized);
        }

        // Reset attack
        attackDirection = AttackDirection.NONE;
        isAttacking = false;
        isCrit = false;
    }

    IEnumerator CheckMelee(){
        Vector3 pos;
        switch(attackDirection){
            case AttackDirection.UP:
                pos=new Vector3(0,1,0);
                break;
            case AttackDirection.RIGHT:
                pos=new Vector3(1,0,0);
                break;
            case AttackDirection.DOWN:
                pos=new Vector3(0,-1,0);
                break;
            case AttackDirection.LEFT:
                pos=new Vector3(-1,0,0);
                break;
            default:
                pos=new Vector3(0,0,0);
                break;
        }

        meleeCheck.GetComponent<MeleeCheck>().damage=finalDamage;
        meleeCheck.transform.position=transform.position+pos;
        meleeCheck.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        meleeCheck.SetActive(false);
    }

    public void SetCurrentRoom(RoomManager roomManager)
    {
        var tmp=GetComponentsInChildren<CinemachineConfiner2D>()[0];
        tmp.m_BoundingShape2D = roomManager.GetComponent<PolygonCollider2D>();
        tmp.InvalidateCache();
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Hurt");
        PlayerManager.Instance.TakeDamage(damage);
    }

    public IEnumerator Die()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1);
        PlayerManager.Instance.Detach(this);
        Time.timeScale = 0;
    }
}
