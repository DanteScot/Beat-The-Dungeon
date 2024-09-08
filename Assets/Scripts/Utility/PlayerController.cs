using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    private float critTimeWindow;
    private float attackSpeed;
    private float baseAttackDamage;
    private float finalDamage;
    private float attackRange;
    private float moveSpeed;

    private bool canAttack = false;
    private bool isCrit = false;
    private bool isAttacking = false;
    private AttackDirection attackDirection;

    public List<string> powers = new List<string>();
    private Vector2 movement;


    public new void Awake()
    {
        base.Awake();
        PlayerManager.Instance.SetPlayer(transform);
    }

    void Start()
    {
        Notify();
        PlayerManager.Instance.Attach(this);
        attackDirection = AttackDirection.NONE;
    }

    public void Notify()
    {
        attackSpeed = PlayerManager.Instance.AttackSpeedLevelled;
        baseAttackDamage = PlayerManager.Instance.BaseAttackDamageLevelled;
        attackRange = PlayerManager.Instance.AttackRangeLevelled;
        moveSpeed = PlayerManager.Instance.MoveSpeedLevelled;

        finalDamage = (float)System.Math.Round(Mathf.Sqrt(1 + baseAttackDamage), 2);
    }

    void Update()
    {
        if(!GameEvent.canMove){
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

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
        critTimeWindow = BeatManager.Instance.TimeBetweenBeats / 3;

        float waitTime = critTimeWindow / 4;
        // Debug.Log(waitTime);

        if(canAttack){
            isCrit=true;
        }else{
            isCrit=false;

            var start=Time.time;
            yield return new WaitUntil(() => canAttack || Time.time-start>critTimeWindow);
            
            if(canAttack) isCrit=true;
            else yield return new WaitUntil(() => canAttack);
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

        // Debug.Log(critTimeWindow / 2);
        yield return new WaitForSeconds(critTimeWindow / 2);

        canAttack = false;
    }

    void Attack(){
        // Attack
        if(attackDirection == AttackDirection.LEFT) transform.localScale = new Vector3(-1, 1, 1);
        else if(attackDirection == AttackDirection.RIGHT) transform.localScale = new Vector3(1, 1, 1);

        StartCoroutine(CheckMelee());
        animator.SetTrigger("Attack");

        if(isCrit){
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,(int)attackDirection)).GetComponent<BulletScript>().SetBullet(attackSpeed, finalDamage, attackRange, movement.normalized, powers);
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
        StartCoroutine(Blink());
        PlayerManager.Instance.TakeDamage(damage);
    }

    IEnumerator Blink(){
        for(int i=0; i<3; i++){
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator Die()
    {
        GameEvent.canMove = false;
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1);
        PlayerManager.Instance.Detach(this);
        Time.timeScale = 0;
    }

    public void OnDestory()
    {
        PlayerManager.Instance.Detach(this);
    }
}
