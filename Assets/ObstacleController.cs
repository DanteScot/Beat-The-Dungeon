using UnityEngine;

public class ObstacleController : PulseToBeat
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int hitPoints = 1;

    new void Awake()
    {
        int n = Random.Range(0, prefabs.Length);
        Instantiate(prefabs[n], transform.position, Quaternion.identity, transform);

        switch(n){
            case 0:
                waitBeat = beatIndex.FirstBeat;
                break;
            case 1:
                waitBeat = beatIndex.SecondBeat;
                break;
            default:
                break;
        }

        base.Awake();
    }

    public void Hit()
    {
        hitPoints--;
        if (hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    // public void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Bullet"))
    //     {
    //         Hit();
    //         Destroy(other.gameObject);
    //     }
    // }

    // public void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Bullet"))
    //     {
    //         Hit();
    //         Destroy(other.gameObject);
    //     }
    // }

    new void OnDestroy()
    {
        base.OnDestroy();
        PlayerManager.Instance.currentRoom.ObstacleDestroyed();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
