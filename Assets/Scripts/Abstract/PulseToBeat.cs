using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Questo script serve per far pulsare un oggetto al ritmo della musica
public class PulseToBeat : RythmedObject
{
    [SerializeField] float pulseSize = 1.15f;
    [SerializeField] float returnSpeed = 5f;

    private Vector3 startSize;

    public void Start()
    {
        startSize = transform.localScale;
    }

    // Si occupa di far tornare l'oggetto alla dimensione originale
    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
    }

    // Fa pulsare l'oggetto al ritmo della musica
    public override void Trigger()
    {
        transform.localScale = startSize * pulseSize;
    }
}
