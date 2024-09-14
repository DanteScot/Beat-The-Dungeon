using TMPro;
using UnityEngine;

// Classe che si occupa di mostrare un tooltip con la descrizione di un potenziamento all'incubatrice
public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;

    private TextMeshProUGUI text;
    private RectTransform rectTransform;

    private void Awake()
    {
        instance = this;
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.Find("Background").GetComponent<RectTransform>();
        HideTooltip();
    }

    // Aggiorna la posizione del tooltip in base alla posizione del mouse
    void Update()
    {
        if(!gameObject.activeSelf) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out Vector2 localPoint);
        transform.localPosition = localPoint;
    }

    // Restituisce il testo del tooltip in base al tipo di abilità
    private string GetTooltipText(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Health:
                return "Health: Increases the maximum health of the player.";
            case SkillType.Damage:
                return "Damage: Increases the damage of the player, both melee and ranged.";
            case SkillType.Speed:
                return "Speed: Increases the movement speed of the player.";
            case SkillType.Range:
                return "Range: Increases the range of the ranged attack.";
            case SkillType.AttackSpeed:
                return "Attack Speed: Increases the attack speed of the ranged attack.";
            case SkillType.Luck:
                return "Luck: Increases the chance of better drops,\nit also make some abilty's effects stronger.";
            default:
                return "";
        }
    }

    // Mostra il tooltip con la descrizione dell'abilità, setta la grandezza del background in base alla lunghezza del testo
    private void ShowTooltip(SkillType skillType)
    {
        gameObject.SetActive(true);
        text.text = GetTooltipText(skillType);
        
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(text.preferredWidth + textPaddingSize * 2, text.preferredHeight + textPaddingSize * 2);
        rectTransform.sizeDelta = backgroundSize;

    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    // Metodi statici per mostrare e nascondere il tooltip
    public static void ShowTooltip_Static(string skillTypeString)
    {
        if (System.Enum.TryParse(skillTypeString, true, out SkillType skillType))
        {
            instance.ShowTooltip(skillType);
        }
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
