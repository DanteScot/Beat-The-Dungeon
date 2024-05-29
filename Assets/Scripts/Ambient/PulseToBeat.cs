using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PulseToBeat : RythmedObject
{
    [SerializeField] float pulseSize = 1.15f;
    [SerializeField] float returnSpeed = 5f;

    private Vector3 startSize;

    public new void Start()
    {
        base.Start();
        startSize = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, startSize, Time.deltaTime * returnSpeed);
    }

    public void Pulse()
    {
        transform.localScale = startSize * pulseSize;
    }

    public override void Trigger()
    {
        Pulse();
    }
}
