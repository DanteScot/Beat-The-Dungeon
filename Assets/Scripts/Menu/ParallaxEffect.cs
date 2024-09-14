using UnityEngine;

// Effetto parallasse per lo sfondo
public class ParallaxEffect : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // In base alla posizione della camera, muove lo sfondo e dopo un certo punto lo riporta avanti
    void Update()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        float movement = cam.transform.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (movement > startPos + length)
            startPos += length;
        else if (movement < startPos - length)
            startPos -= length;
    }
}
