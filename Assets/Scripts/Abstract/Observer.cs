using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interfaccia per gli osservatori
public interface Observer
{
    void Notify();
    void OnDestroy();
}
