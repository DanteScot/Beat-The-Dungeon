using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


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
    [SerializeField] private Transform[] possiblePositions;
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

    private IEnumerator ScondPhase() {
        yield return new WaitUntil(() => health <= halfLife);
        isMoving = true;
        PrepareSecondPhase();
    }

    private void PrepareSecondPhase() {
        StopAllCoroutines();
        
        spriteRenderer.sprite = idleSprites[0];

        circleCollider.enabled = true;
    }

    public void OnDialogueEnd() {
        circleCollider.enabled = false;
        StartCoroutine(StartSecondPhase());
    }

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

    private IEnumerator ChangeSong(AudioClip clip) {
        isMoving = true;
        GameEvent.canMove = false;
        while (BeatManager.Instance.audioSource.volume > 0) {
            BeatManager.Instance.audioSource.volume -= .01f;
            yield return new WaitForSeconds(.025f);
        }
        BeatManager.Instance.audioSource.volume = 0;

        BeatManager.Instance.audioSource.Stop();
        BeatManager.Instance.audioSource.clip = clip;

        if(clip == firstPhaseSong) BeatManager.Instance.SetBPM(firstSongBPM);
        else BeatManager.Instance.SetBPM(secondSongBPM);

        yield return new WaitForSeconds(.25f);
        BeatManager.Instance.audioSource.Play();

        isMoving = false;
        GameEvent.canMove = true;

        while (BeatManager.Instance.audioSource.volume < .5f) {
            BeatManager.Instance.audioSource.volume += .01f;
            yield return new WaitForSeconds(.025f);
        }
        BeatManager.Instance.audioSource.volume = .5f;
    }

    private IEnumerator JerrysLogic() {
        yield return new WaitUntil(() => isActive);
        healthBar.gameObject.SetActive(true);

        if(BeatManager.Instance.audioSource.clip != secondPhaseSong) {
            oldSong = BeatManager.Instance.audioSource.clip;
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

    private IEnumerator Move() {
        isMoving = true;
        Vector3 targetPosition = possiblePositions[Random.Range(0, possiblePositions.Length)].position;
        agent.SetDestination(targetPosition);

        if(targetPosition.x < transform.position.x) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); 

        int maxIterations = 5;
        while(Vector2.Distance(transform.position, targetPosition) > 1 && maxIterations > 0){
            for (int i = 0; i < movingSprites.Length; i++) {
                spriteRenderer.sprite = movingSprites[i];
                if(i%2!=0){
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

    private IEnumerator Attack() {
        isAttacking = true;
        if(player.position.x < transform.position.x)    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else                                            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        yield return new WaitUntil(() => canAttack);
        canAttack = false;

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

    private IEnumerator Death() {
        isDying = true;

        for(int i=0; i<deathSprites.Length; i++){
            spriteRenderer.sprite = deathSprites[i];
            audioSource.volume = 1 - (i / deathSprites.Length);
            audioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(.25f);
        }

        spriteRenderer.sprite = null;
        yield return StartCoroutine(ChangeSong(oldSong));
        BeatManager.Instance.SetBPM(oldBPM);
        base.Die();
    }

    public override void TakeDamage(float damage){
        health-=damage;
        if(health<=0) health = 0;
        healthBar.value = health;
        base.TakeDamage(0);
    }

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
}