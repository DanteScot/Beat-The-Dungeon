using UnityEngine;

// Semplice classe che muove il "player" verso destra
public class MenuMovement : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", speed);
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.right);
    }
}