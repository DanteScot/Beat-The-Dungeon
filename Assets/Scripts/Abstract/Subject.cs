using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Subject
{
    void Attach(Observer observer);
    void Detach(Observer observer);
    void Notify();
}