using UnityEngine.UI;
using UnityEngine;

// Classe responsabile della gestione dell'UI della vita del giocatore
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
        maxHealth = PlayerManager.Instance.MaxHealthLevelled;
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
        // Cancella tutti i cuori
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Conta i cuori da mostrare e controllo se c'è un mezzo cuore
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

        // Mostra i cuori
        // Se i < heartCount, allora il cuore è pieno
        // Se i == heartCount, controllo se c'è un mezzo cuore e in caso lo mostro
        // Se i > heartCount, allora il cuore è vuoto
        for (int i=0; i<maxHealth/2;i++){
            if(i<heartCount){
                Instantiate(heartPrefab, transform).GetComponent<Image>().sprite = fullHeart;
            }else if(halfHeartFlag){
                Instantiate(heartPrefab, transform).GetComponent<Image>().sprite = halfHeart;
                halfHeartFlag = false;
            }else{
                Instantiate(heartPrefab, transform).GetComponent<Image>().sprite = emptyHeart;
            }
        }
    }

    public void OnDestroy()
    {
        PlayerManager.Instance.Detach(this);
    }
}
