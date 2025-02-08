using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// Classe che si occupa di gestire il boss Jerry e che mi ha divertito davvero tanto scriverla come vedrete dai commenti
// Questo boss ha TUTTE le animazioni/azioni sincronizzate con la musica
// Infatti non possiede un animator, ma si basa sulle coroutine e sulle immagini per fare tutto
public class JerryController : Enemy
{
    [Header("Jerry Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private Sprite[] reloadSprites;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private Sprite[] movingSprites;

    [Header("Audio")]
    [SerializeField] private AudioClip firstPhaseSong;
    [SerializeField] private int firstSongBPM;
    [SerializeField] private AudioClip secondPhaseSong;
    [SerializeField] private int secondSongBPM;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Other Things")]
    // Posizioni in cui Jerry può muoversi, rende il tutto più pulito
    [SerializeField] private Vector3[] possiblePositions;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Slider healthBar;

    private AudioClip oldSong;
    private float oldBPM;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private NavMeshAgent agent;
    private Transform player;
    private AudioSource audioSource;
    private CinemachineImpulseSource impulseSource;

    private bool isMoving = false, isReloading = false;

    [SerializeField] private bool isFirstPhase = true;
    private float halfLife;

    private new void Awake() {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
        audioSource = GetComponent<AudioSource>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        halfLife = health / 2;
    }

    private void Start() {
        // In awake a volte funziona a volte no, quindi lo metto qui
        player = PlayerManager.Instance.GetPlayer();

        spriteRenderer.sprite = idleSprites[0];
        healthBar.maxValue = health;
        healthBar.value = health;
        healthBar.gameObject.SetActive(false);
        StartCoroutine(JerrysLogic());
        StartCoroutine(ScondPhase());
    }

    // Aspetta che la vita di Jerry sia minore della metà per passare alla seconda fase
    private IEnumerator ScondPhase() {
        yield return new WaitUntil(() => health <= halfLife);
        isMoving = true;
        PrepareSecondPhase();
    }

    // Ferma tutto e fa partire il dialogo
    private void PrepareSecondPhase() {
        GameEvent.canMove = false;
        StopAllCoroutines();
        
        spriteRenderer.sprite = idleSprites[0];

        circleCollider.enabled = true;
    }

    // Quando il dialogo finisce, fa partire la seconda fase
    public void OnDialogueEnd() {
        circleCollider.enabled = false;
        StartCoroutine(StartSecondPhase());
    }

    // Fa partire la seconda fase, raddoppiando il danno, aumentando le dimensioni di jerry e cambiando la canzone
    private IEnumerator StartSecondPhase() {
        StartCoroutine(ChangeSong(secondPhaseSong));
        float time = 0;
        while (time < 1) {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, time);
            yield return null;
        }
        damage *= 2;
        yield return new WaitForSeconds(.5f);
        isFirstPhase = false;
        isMoving = false;
        isAttacking = false;
        isReloading = false;
        isDying = false;
        StartCoroutine(JerrysLogic());
    }

    // Per evitare bug o sfruttare momenti di transizione, quando cambio canzone disattivo il movimento a tutti
    private IEnumerator ChangeSong(AudioClip clip) {
        isMoving = true;
        GameEvent.canMove = false;
        // Fade out
        while (BeatManager.Instance.AudioSource.volume > 0) {
            BeatManager.Instance.AudioSource.volume -= .01f;
            yield return new WaitForSeconds(.025f);
        }
        BeatManager.Instance.AudioSource.volume = 0;

        // Cambio canzone e relativo BPM
        BeatManager.Instance.AudioSource.Stop();
        BeatManager.Instance.AudioSource.clip = clip;

        if(clip == firstPhaseSong) BeatManager.Instance.SetBPM(firstSongBPM);
        else BeatManager.Instance.SetBPM(secondSongBPM);

        yield return new WaitForSeconds(.25f);
        BeatManager.Instance.AudioSource.Play();

        isMoving = false;
        GameEvent.canMove = true;

        // Fade in
        while (BeatManager.Instance.AudioSource.volume < .5f) {
            BeatManager.Instance.AudioSource.volume += .01f;
            yield return new WaitForSeconds(.025f);
        }
        BeatManager.Instance.AudioSource.volume = .5f;
    }

    // Logica di Jerry, si muove, attacca e ricarica
    private IEnumerator JerrysLogic() {
        yield return new WaitUntil(() => isActive);
        healthBar.gameObject.SetActive(true);

        TilemapCollider2D tilemap = PlayerManager.Instance.currentRoom.TilemapCollider;
        int xPoints = (int)((tilemap.bounds.size.x-2) / 7) + 1;
        int yPoints = (int)((tilemap.bounds.size.y-2) / 3);

        possiblePositions = new Vector3[xPoints * yPoints];
        int index = 0;

        for (int i = 0; i < xPoints; i++) {
            for (int j = 0; j < yPoints; j++) {
                possiblePositions[index] = new Vector3(tilemap.bounds.min.x + 3 + i * 7, tilemap.bounds.min.y + 3 + j * 3, 0);
                index++;
            }
        }

        // La prima volta mette la prima canzone, la seconda non fa nulla
        if(BeatManager.Instance.AudioSource.clip != secondPhaseSong) {
            oldSong = BeatManager.Instance.AudioSource.clip;
            oldBPM = BeatManager.Instance.GetBPM();
            StartCoroutine(ChangeSong(firstPhaseSong));
        }

        while (health > 0) {
            while(isMoving || isAttacking || isReloading || isDying) yield return null;

            yield return StartCoroutine(Move());
            yield return StartCoroutine(Attack());
            yield return StartCoroutine(Reload());
        }
    }

    // Sceglie una posizione random e ci si muove, guardando verso quella posizione
    private IEnumerator Move() {
        isMoving = true;
        Vector3 targetPosition = possiblePositions[Random.Range(0, possiblePositions.Length)];
        agent.SetDestination(targetPosition);

        if(targetPosition.x < transform.position.x) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); 

        // Ha un massimo di 5 iterazioni per raggiungere la posizione, se non ci riesce si ferma (per evitare bug)
        int maxIterations = 5;
        while(Vector2.Distance(transform.position, targetPosition) > 1 && maxIterations > 0){
            for (int i = 0; i < movingSprites.Length; i++) {
                spriteRenderer.sprite = movingSprites[i];
                if(i%2!=0){
                    // Ogni volta che poggia il piede a terra, genera un'impulso attivando il camera shake per dare l'effetto di pesantezza
                    impulseSource.m_DefaultVelocity = new Vector3(Random.Range(-.05f,.05f)*damage, Random.Range(-.05f,.05f)*damage, 0);
                    impulseSource.GenerateImpulse();
                }

                yield return new WaitUntil(() => canAttack);
                canAttack = false;
                maxIterations--;
            }
        }

        spriteRenderer.sprite = idleSprites[0];
        yield return new WaitForSeconds(.25f);
        isMoving = false;
    }

    // Guarda il player e spara
    private IEnumerator Attack() {
        isAttacking = true;
        if(player.position.x < transform.position.x)    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else                                            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        yield return new WaitUntil(() => canAttack);
        canAttack = false;

        // Se non è la prima fase spara 2 colpi extra (3 proiettili in totale) in rapida successione
        if(!isFirstPhase){
            for(int i=0;i<4;i++){
                spriteRenderer.sprite = attackSprites[i%2];

                if(i%2==0){
                    audioSource.PlayOneShot(attackSound);
                    Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBulletController>().SetBullet(3*damage, damage, 1.5f, player.position);
                }

                yield return new WaitUntil(() => canAttack);
                canAttack = false;
            }
        }

        // Sparo standard con conclusione animazione
        for(int i=0; i<attackSprites.Length; i++){
            spriteRenderer.sprite = attackSprites[i];

            if(i==0){
                audioSource.PlayOneShot(attackSound);
                Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBulletController>().SetBullet(3*damage, damage, 1.5f, player.position);

                /*
                SALVE PROF, SE STATE LEGGENO QUESTO COMMENTO, VOLEVO CHIEDERVI UNA COSA:
                AVETE LA PIU' PALLIDA IDEA DEL PERCHE' SE GLI METTO COME PARENT JERRY QUANDO COLPISCE IL PLAYER MAGICAMENTE IL SISTEMA DI DIALOGO
                RILEVA COME SE IL PLAYER FOSSE USCITO DAL RAGGIO E ATTIVA IL DIALOGO
                (COSA CHE NON DOVREBBE SUCCEDERE ESSENDO IL COLLIDER DEL DIALOGO SPENTO E LA FUNZIONE DEL DIALOGO DOVREBBE DISATTIVARE IL TUTTO QUANDO ESCI E NON ATTIVARLO)
                PERCHE' A PRIORI DI TUTTO NON VEDO COME IL FATTO DEL PARENT DEL BULLET POSSA INFLUIRE SULLE COSE DEL PLAYER
                */
                // Instantiate(bulletPrefab, transform.position, Quaternion.identity, transform).GetComponent<EnemyBulletController>().SetBullet(4*damage, damage, player.position);
            }

            yield return new WaitUntil(() => canAttack);
            canAttack = false;
        }

        isAttacking = false;
    }

    // Ricarica l'arma
    private IEnumerator Reload() {
        isReloading = true;
        yield return new WaitUntil(() => canAttack);
        canAttack = false;

        for(int i=0; i<reloadSprites.Length; i++){
            spriteRenderer.sprite = reloadSprites[i];
            yield return new WaitUntil(() => canAttack);
            canAttack = false;
        }

        spriteRenderer.sprite = idleSprites[0];
        yield return new WaitForSeconds(.25f);
        isReloading = false;
    }

    // Coroutine che gestisce la morte di Jerry
    private IEnumerator Death() {
        isDying = true;
        impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Explosion;

        // Fade in del volume per la morte riproducendo il suono più volte con anche il camera shake
        for(int i=0; i<deathSprites.Length; i++){
            spriteRenderer.sprite = deathSprites[i];
            audioSource.volume = i / deathSprites.Length;
            audioSource.PlayOneShot(deathSound);
            impulseSource.m_DefaultVelocity = new Vector3(Random.Range(-.05f,.05f)*damage, Random.Range(-.05f,.05f)*damage, 0);
            impulseSource.GenerateImpulse();
            yield return new WaitForSeconds(.25f);
        }

        PlayerManager.Instance.currentRoom.GenerateExit();

        spriteRenderer.sprite = null;
        yield return StartCoroutine(ChangeSong(oldSong));
        BeatManager.Instance.SetBPM(oldBPM);
        impulseSource.m_ImpulseDefinition.m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Rumble;
        GameEvent.canMove = true;
        base.Die();
    }

    // Override della funzione TakeDamage per aggiornare la barra della vita
    public override void TakeDamage(float damage){
        health-=damage;
        if(health<=0) health = 0;
        healthBar.value = health;
        base.TakeDamage(0);
    }

    // Override della funzione Die per far partire la coroutine della morte
    public override void Die(){
        StopAllCoroutines();
        StartCoroutine(Death());
        return;
    }


    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmos() {
        try{
            foreach (var pos in possiblePositions)
            {
                Gizmos.DrawWireSphere(pos, .5f);
            }
        } catch {}
    }
}