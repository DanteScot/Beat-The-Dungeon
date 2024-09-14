using System.Collections;
using TMPro;
using UnityEngine;

// Classe che gestisce la UI degli oggetti raccolti
public class ItemUIController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake() {
        Messenger<string>.AddListener(GameEvent.ITEM_PICKED, OnItemPicked);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Quando un oggetto viene raccolto, mostra la UI dell'oggetto
    private void OnItemPicked(string name)
    {
        StopCoroutine(ShowItemUI());

        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Power.GetDescription(name);

        StartCoroutine(ShowItemUI());
    }

    IEnumerator ShowItemUI()
    {
        canvasGroup.alpha = 1;
        yield return new WaitForSeconds(2);
        
        // Fade out
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDestroy() {
        Messenger<string>.RemoveListener(GameEvent.ITEM_PICKED, OnItemPicked);
    }
}
