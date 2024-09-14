using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Tipi di abilità che possono essere potenziate
[System.Serializable]
public enum SkillType
{
    Health,
    Damage,
    Speed,
    Range,
    AttackSpeed,
    Luck
}

// Gestisce l'upgrade delle abilità del giocatore
public class UpgradeSkillController : MonoBehaviour, Observer
{
    [Header("Controller Settings")]
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite fullSprite;


    [Header("Skill Stats")]
    [SerializeField] private SkillType skillType;
    [SerializeField] private int skillCost = 10;
    [SerializeField] private int skillLevel = 0;

    private TextMeshProUGUI skillCostText;
    private GameObject slider;

    void Awake()
    {
        PlayerManager.Instance.Attach(this);

        skillCostText = transform.Find("GearCost").Find("cost").GetComponent<TextMeshProUGUI>();

        transform.Find("StatImage").Find("Image").GetComponent<Image>().sprite = skillIcon;

        slider = transform.Find("Slider").gameObject;
    }

    void Start()
    {
        Notify();
    }

    void init()
    {
        skillCostText.text = skillCost.ToString();

        if(skillCost > PlayerManager.Instance.Gears) skillCostText.color = Color.red;
        else skillCostText.color = Color.green;

        // Riempie il numero di sprite corrispondente al livello dell'abilità
        for(int i = 0; i < skillLevel; i++)
        {
            slider.transform.GetChild(i).GetComponent<Image>().sprite = fullSprite;
        }
    }

    // Aggiorna il livello dell'abilità e i relativi costi e salva il gioco
    public void UpgradeSkill()
    {
        if(skillCost > PlayerManager.Instance.Gears || skillLevel == 4) return;

        Messenger.Broadcast(GameEvent.GEAR_PICKED);

        PlayerManager.Instance.Gears -= skillCost;
        skillLevel++;
        skillCost *= 2;

        switch (skillType)
        {
            case SkillType.Speed:
                PlayerManager.Instance.MoveSpeedLevel += 1;
                break;
            case SkillType.Range:
                PlayerManager.Instance.AttackRangeLevel += 1;
                break;
            case SkillType.AttackSpeed:
                PlayerManager.Instance.AttackSpeedLevel += 1;
                break;
            case SkillType.Luck:
                PlayerManager.Instance.LuckLevel += 1;
                break;
            case SkillType.Health:
                PlayerManager.Instance.HealthLevel += 1;
                PlayerManager.Instance.CurrentHealth = PlayerManager.Instance.MaxHealthLevelled;
                break;
            case SkillType.Damage:
                PlayerManager.Instance.DamageLevel += 1;
                break;
        }

        SaveSystem.SaveGame(new GameData(PlayerManager.Instance));
    }

    public void Notify()
    {
        switch (skillType)
        {
            case SkillType.Speed:
                skillLevel = PlayerManager.Instance.MoveSpeedLevel;
                break;
            case SkillType.Range:
                skillLevel = PlayerManager.Instance.AttackRangeLevel;
                break;
            case SkillType.AttackSpeed:
                skillLevel = PlayerManager.Instance.AttackSpeedLevel;
                break;
            case SkillType.Luck:
                skillLevel = PlayerManager.Instance.LuckLevel;
                break;
            case SkillType.Health:
                skillLevel = PlayerManager.Instance.HealthLevel;
                break;
            case SkillType.Damage:
                skillLevel = PlayerManager.Instance.DamageLevel;
                break;
        }

        skillCost = 10 * (int)Mathf.Pow(2, skillLevel);

        init();
    }

    public void OnDestroy()
    {
        PlayerManager.Instance.Detach(this);
    }
}
