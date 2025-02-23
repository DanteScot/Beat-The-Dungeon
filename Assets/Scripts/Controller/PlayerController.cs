using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

// Utilizzato per gestire la rotazione del colpo e sapere se effettivamente si sta attaccando
enum AttackDirection
{
    NONE=-1,
    UP=0,
    LEFT=90,
    DOWN=180,
    RIGHT=270
}

// Controller del giocatore
public class PlayerController : RythmedObject, Observer
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject meleeCheck;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject minions;

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

    private List<string> powers = new();

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
        PlayerManager.Instance.CurrentHealth = PlayerManager.Instance.MaxHealthLevelled;
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
        // Se non si può muovere, ferma il giocatore
        if(!GameEvent.canMove){
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        // Movimento + animazione
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed;
        GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(movement, moveSpeed);
        animator.SetFloat("Speed", movement.magnitude);

        // Aggiorna la rotazione del giocatore solo se non sta attaccando
        // In questo modo il giocatore guarderà nella direzione in cui sta attaccando fino alla fine dell'animazione
        if(!animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("player_attack")){
            if(Input.GetAxis("Horizontal")<0)   transform.localScale = new Vector3(-1, 1, 1);
            else if(Input.GetAxis("Horizontal")>0)  transform.localScale = new Vector3(1, 1, 1);
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))           attackDirection = AttackDirection.UP;
        else if(Input.GetKeyDown(KeyCode.DownArrow))    attackDirection = AttackDirection.DOWN;
        else if(Input.GetKeyDown(KeyCode.LeftArrow))    attackDirection = AttackDirection.LEFT;
        else if(Input.GetKeyDown(KeyCode.RightArrow))   attackDirection = AttackDirection.RIGHT;
        
        // Se si preme il tasto di attacco e non si sta già attaccando, prepara l'attacco
        if(attackDirection != AttackDirection.NONE && !isAttacking)
        {
            StartCoroutine(PrepareAttack());
        }
    }

    // Aggiunge un potere al giocatore ed esegue l'eventuale effetto da attivare appena preso
    public void AddPower(string power)
    {
        powers.Add(power);
        Power.OneTimeInit(power);
    }

    // Si assicura che l'attacco venga eseguito solo al ritmo della musica
    IEnumerator PrepareAttack()
    {
        isAttacking = true;
        // Finestra di tempo in cui si può effettuare un colpo critico PRIMA del beat
        critTimeWindow = BeatManager.Instance.TimeBetweenBeats / 3;

        // Se si preme il tasto di attacco al ritmo della musica, il colpo sarà critico
        if(canAttack){
            isCrit=true;
        }else{
            // Se si preme il tasto di attacco fuori dal ritmo della musica ha un tempo limitato
            // nel quale può diventare critico
            // In poche parole, se si preme il tasto di attacco entro critTimeWindow secondi prima del ritmo, il colpo sarà comunque critico

            isCrit=false;

            // Aspetta che si possa attaccare o che critTimeWindow secondi siano passati
            float start =Time.time;
            yield return new WaitUntil(() => canAttack || Time.time-start>critTimeWindow);
            
            // Se si può attaccare vuol dire che rientra nel tempo limite per il colpo critico
            // Altrimenti aspetta che si possa attaccare 
            if(canAttack) isCrit=true;
            else yield return new WaitUntil(() => canAttack);
        }

        Attack();
    }

    // Chiamata ogni beat
    public override void Trigger()
    {
        StartCoroutine(critWindow());
    }

    // Finestra di tempo in cui si può effettuare un colpo critico DOPO il beat
    private IEnumerator critWindow()
    {
        canAttack = true;

        yield return new WaitForSeconds(critTimeWindow / 2);

        canAttack = false;
    }

    void Attack(){
        // Cambia la rotazione del giocatore in base alla direzione dell'attacco
        if(attackDirection == AttackDirection.LEFT) transform.localScale = new Vector3(-1, 1, 1);
        else if(attackDirection == AttackDirection.RIGHT) transform.localScale = new Vector3(1, 1, 1);

        // Attacco corpo a corpo
        StartCoroutine(CheckMelee());
        animator.SetTrigger("Attack");

        // Se il colpo è critico, spara il proiettile
        if(isCrit){
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0,0,(int)attackDirection)).GetComponent<BulletController>().SetBullet(attackSpeed, finalDamage, attackRange, movement.normalized, powers);
        }

        // Resetta le variabili dell'attacco
        attackDirection = AttackDirection.NONE;
        isAttacking = false;
        isCrit = false;
    }

    // Attacco corpo a corpo
    // In base alla direzione dell'attacco, posiziona il collider del corpo a corpo in quella direzione
    // e lo attiva per 0.1 secondi
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

        // Aspetta che l'animazione di attacco sia quasi finita (è lì il frame dove "colpisce")
        yield return new WaitForSeconds(0.1f);
        meleeCheck.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        meleeCheck.SetActive(false);
    }

    // Resetta i bordi della camera in base alla stanza in cui si trova il giocatore
    // Senza questo metodo la camera rimarrebbe bloccata nei confini della stanza precedente
    public void SetCurrentRoom(MonoBehaviour roomManager)
    {
        CinemachineConfiner2D tmp =GetComponentsInChildren<CinemachineConfiner2D>()[0];
        tmp.m_BoundingShape2D = roomManager.GetComponent<PolygonCollider2D>();
        tmp.InvalidateCache();
    }

    public void TakeDamage(float damage)
    {
        Messenger.Broadcast(GameEvent.PLAYER_HIT);
        animator.SetTrigger("Hurt");
        StartCoroutine(Blink());
        PlayerManager.Instance.CurrentHealth -= damage;
        if (PlayerManager.Instance.CurrentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }
    // Quando viene colpito prende danno, manda l'evento PLAYER_HIT e fa lampeggiare il giocatore
    IEnumerator Blink(){
        for(int i=0; i<3; i++){
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Animazione di morte + funzioni di fine partita
    public IEnumerator Die()
    {
        GameEvent.canMove = false;
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(1);
        PlayerManager.Instance.Detach(this);
        Time.timeScale = 0;
        GameEvent.canMove = true;
        PlayerManager.Instance.EndGame();
        GameManager.Instance.LoadLobby();
    }

    public void OnDestory()
    {
        PlayerManager.Instance.Detach(this);
    }
}
