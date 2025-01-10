using Cinemachine;
using UnityEngine;

public class OrthographicCameraAdapter : MonoBehaviour
{
    public CinemachineVirtualCamera orthoCamera;
    public float targetWidth = 12f;  // Larghezza di riferimento (ad esempio, 10 unità)
    
    private float referenceAspectRatio = 16f / 9f; // Aspect ratio di riferimento (16:9)

    void Start()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float widthFactor, heightFactor;
        

        // Calcola l'ortographic size in base all'altezza di riferimento
        heightFactor = targetWidth / 1.1f / referenceAspectRatio;


        // Calcola l'ortographic size in base alla larghezza di riferimento
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleFactor = windowAspect / referenceAspectRatio;

        widthFactor = targetWidth / 2f / scaleFactor;


        // Imposta l'ortographic size in base al fattore più grande
        orthoCamera.m_Lens.OrthographicSize = Mathf.Max(widthFactor, heightFactor);
    }

    void OnValidate()
    {
        if (orthoCamera == null)
            orthoCamera = GetComponent<CinemachineVirtualCamera>();
    }
}
