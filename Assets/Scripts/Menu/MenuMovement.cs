using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MenuMovement : MonoBehaviour
// {
//     [SerializeField] private GameObject background;

//     GameObject previousBackground;
//     GameObject currentBackground;
//     GameObject nextBackground;

//     private int step = 16;

//     private int lastSpawned = -1;

//     void Start()
//     {
//         currentBackground=Instantiate(background, new Vector3(0, 0, 0), Quaternion.identity);
//         foreach (var tmp in currentBackground.GetComponentsInChildren<SpriteRenderer>()){
//             tmp.sortingOrder += 1;
//         }
//         nextBackground=Instantiate(background, new Vector3(step, 0, 0), Quaternion.identity);
//         lastSpawned = step;
//     }

//     void FixedUpdate()
//     {
//         transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);

//         if(transform.position.x >= lastSpawned)
//         {
//             lastSpawned+=step;
//             Destroy(previousBackground);
//             previousBackground = currentBackground;
//             foreach (var tmp in previousBackground.GetComponentsInChildren<SpriteRenderer>()){
//                 tmp.sortingOrder -= 1;
//             }
//             currentBackground = nextBackground;
//             foreach (var tmp in currentBackground.GetComponentsInChildren<SpriteRenderer>()){
//                 tmp.sortingOrder += 1;
//             }
//             nextBackground=Instantiate(background, new Vector3(lastSpawned, 0, 0), Quaternion.identity);
//             foreach (var tmp in nextBackground.GetComponentsInChildren<SpriteRenderer>()){
//                 tmp.sortingOrder -= 1;
//             }
//         }
//     }
// }
