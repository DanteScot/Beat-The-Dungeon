using System.Collections;
using UnityEngine;

// Gestisce i 3 pllini verdi nel tutorial
public class TutorialMovementController : MonoBehaviour
{
    private Vector3[] positions = new Vector3[3];
    private int currentPos = 0;

    void Start()
    {
        positions[0] = new Vector3(6, 3, 0);
        positions[1] = new Vector3(0, -2, 0);
        positions[2] = new Vector3(-3, 2, 0);

        StartCoroutine(Wait());
    }

    IEnumerator Wait(){
        yield return new WaitUntil(() => TutorialManager.Instance.started);
        transform.position = positions[currentPos];
    }

    // Una volta raggiunte tutte le posizioni, il tutorial è completato
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPos++;
            if (currentPos >= positions.Length) {
                TutorialManager.Instance.stepCompleted = true;
                TutorialManager.Instance.started = false;
                Destroy(gameObject);
            }
            else transform.position = positions[currentPos];
        }
    }
}
