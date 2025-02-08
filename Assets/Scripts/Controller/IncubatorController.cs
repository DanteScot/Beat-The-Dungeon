using System.Collections;
using TMPro;
using UnityEngine;

public class IncubatorController : Interactable
{
    [SerializeField] private GameObject incubatorUI;
    [SerializeField] private TextMeshProUGUI incubatorText;
    [SerializeField] protected intervalIndex index;

    private string playerName;


    public override void Interact()
    {
        isInteracting = true;
        StartCoroutine(InteractCoroutine());
    }

    // Prende il nome dell'utente che sta giocando
    void Awake()
    {
        string path = Application.persistentDataPath;
        string[] pathParts = path.Split('/');
        playerName = pathParts[2];
    }

    IEnumerator InteractCoroutine()
    {
        GameEvent.canMove = false;
        incubatorUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Coroutine tmp = StartCoroutine(AnalysisText());

        // Semplice transizione
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*2f;
            incubatorUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(1000, 0, t), 0);
            yield return null;
        }

        yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Escape));
        // Nell'eventualità che stia ancora analizzando
        StopCoroutine(tmp);

        // Semplice transizione
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime*2f;
            incubatorUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(0, 1000, t), 0);
            yield return null;
        }
        GameEvent.canMove = true;
        isInteracting = false;
        incubatorUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return null;
    }

    // Gestisce il testo in basso quando apri l'incubatrice
    IEnumerator AnalysisText()
    {
        string[] analysis = new string[]
        {
            "Analyzing...",
            "Analyzing... 10%",
            "Analyzing... 20%",
            "Analyzing... 30%",
            "Analyzing... 40%",
            "Analyzing... 50%",
            "Analyzing... 60%",
            "Analyzing... 70%",
            "Analyzing... 80%",
            "Analyzing... 90%",
            "Analyzing... 100%",
            "Analysis complete."
        };

        yield return new WaitForSeconds(0.5f);
        foreach (string s in analysis)
        {
            incubatorText.text = s;
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.75f);
        incubatorText.alpha = 0;
        yield return new WaitForSeconds(1);
        incubatorText.text = "subject identified\n";
        incubatorText.alpha = 1;



        string result = "Subject: " + playerName + "\n" +
                        "Status: OK\n" +
                        "Mט\u25A1ᶕλᶋo: ■░║╨◘♫";

        
        foreach (char c in result)
        {
            incubatorText.text += c;
            yield return new WaitForSeconds(0.15f);
        }

    }
}
