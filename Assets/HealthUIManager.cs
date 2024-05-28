using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour, Observer
{
    private float maxHealth;
    private float currentHealth;


    public void Notify()
    {
        maxHealth = PlayerManager.Instance.MaxHealth;
        currentHealth = PlayerManager.Instance.CurrentHealth;
    }

    void Start()
    {
        PlayerManager.Instance.Attach(this);
    }

    void Update()
    {
        
    }

    public void OnDestory()
    {
        PlayerManager.Instance.Detach(this);
    }
}
