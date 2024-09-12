using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interfaccia per il pattern Observer
public interface Subject
{
    void Attach(Observer observer);
    void Detach(Observer observer);
    void Notify();
}