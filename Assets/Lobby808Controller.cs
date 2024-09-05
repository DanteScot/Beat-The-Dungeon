using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Lobby808Controller : MonoBehaviour
{
    public static Lobby808Controller Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private Vector3 targetPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool canMove = false;

    private bool isTalking = false;
    public bool IsTalking
    {
        get => isTalking;
        set {
            isTalking = value;
            animator.SetBool("isTalking", isTalking);
        }
    }

    [SerializeField] private float speed = 2;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        animator = GetComponent<Animator>();
        
        targetPosition = new Vector3(8f,-4,0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255,255,255,0);

    }
    
    public void StartAnimation()
    {
        StartCoroutine(FadeIn());
    }

    public void NoAnimation()
    {
        transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,1);
        transform.position = targetPosition;
        spriteRenderer.color = new Color(255,255,255,1);
    }

    void Update()
    {
        if(!canMove) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed*Time.deltaTime);

        if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,1);
            virtualCamera.Follow = PlayerManager.Instance.GetPlayer();
            canMove = false;
            GameEvent.canMove = true;
        }
    }

    IEnumerator FadeIn()
    {
        virtualCamera.Follow = transform;
        for(float i=0; i<=1; i+=0.01f)
        {
            spriteRenderer.color = new Color(255,255,255,i);
            yield return new WaitForSeconds(0.01f);
        }
        canMove = true;
    }
}
