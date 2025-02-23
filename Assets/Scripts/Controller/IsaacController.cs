using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Classe che gestisce il movimento e l'attacco di Isaac (Minion)
public class IsaacController : MonoBehaviour, Observer, Minion
{
    private NavMeshAgent agent;

    [SerializeField] private float speed;
    private float damage;

    private bool canAttack = false;

    private RequiredNavMesh requiredNavMesh = RequiredNavMesh.GROUND_SMALL;
    public RequiredNavMesh RequiredNavMesh { get => requiredNavMesh; set => requiredNavMesh=value; }

    private void Awake() {
        PlayerManager.Instance.Attach(this);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;
        damage = PlayerManager.Instance.BaseAttackDamageLevelled/4;

        StartCoroutine(FakeUpdate());
    }

    // Ogni 0.5 secondi si muove verso il nemico più vicino
    // Se non ci sono nemici si ferma
    IEnumerator FakeUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            canAttack = true;

            Enemy[] enemies = PlayerManager.Instance.currentRoom.Enemies;

            Enemy closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (Enemy foe in enemies) {
                if (closestEnemy == null) {
                    closestEnemy = foe;
                    continue;
                }

                if (Vector2.Distance(transform.position, foe.transform.position) < closestDistance) {
                    closestEnemy = foe;
                    closestDistance = Vector2.Distance(transform.position, foe.transform.position);
                }
            }

            if (closestEnemy != null) agent.SetDestination(closestEnemy.transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (canAttack && other.CompareTag("Enemy")) {
            other.GetComponent<Enemy>().TakeDamage(damage);
            canAttack = false;
        }
    }

    public void Notify()
    {
        damage = PlayerManager.Instance.BaseAttackDamageLevelled/4;
    }

    public void OnDestroy()
    {
        PlayerManager.Instance.Detach(this);
    }
}
