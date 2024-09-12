using System.Collections;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    private static Tooltip instance;

    private TextMeshProUGUI text;
    private RectTransform rectTransform;
    private  SkillType skillType;

    private void Awake()
    {
        instance = this;
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.Find("Background").GetComponent<RectTransform>();
        HideTooltip();
    }

    void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        transform.localPosition = localPoint;
    }

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
                return "Luck: Increases the chance of better drops.";
            default:
                return "";
        }
    }

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

    public static void ShowTooltip_Static(string skillType)
    {
        SkillType skillTypeString;
        if (System.Enum.TryParse(skillType, true, out skillTypeString))
        {
            instance.ShowTooltip(skillTypeString);
        }
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
