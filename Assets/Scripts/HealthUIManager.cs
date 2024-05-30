using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class HealthUIManager : MonoBehaviour, Observer
{
    private float maxHealth;
    private float currentHealth;

    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private GameObject heartPrefab;


    public void Notify()
    {
        maxHealth = PlayerManager.Instance.MaxHealth;
        currentHealth = PlayerManager.Instance.CurrentHealth;
        UpdateGui();
    }

    void Start()
    {
        PlayerManager.Instance.Attach(this);
        Notify();
        currentHealth=maxHealth;
    }

    void UpdateGui()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        bool halfHeartFlag = false;
        int heartCount;
        if (currentHealth % 2 == 0)
        {
            heartCount = (int)currentHealth / 2;
        }
        else
        {
            heartCount = (int)(currentHealth - 0.5f) / 2;
            halfHeartFlag = true;
        }

        for (int i=0; i<maxHealth/2;i++){
            if(i<heartCount){
                Instantiate(heartPrefab, transform).GetComponent<UnityEngine.UI.Image>().sprite = fullHeart;
            }else if(halfHeartFlag){
                Instantiate(heartPrefab, transform).GetComponent<UnityEngine.UI.Image>().sprite = halfHeart;
                halfHeartFlag = false;
            }else{
                Instantiate(heartPrefab, transform).GetComponent<UnityEngine.UI.Image>().sprite = emptyHeart;
            }
        }
    }

    public void OnDestory()
    {
        PlayerManager.Instance.Detach(this);
    }
}
