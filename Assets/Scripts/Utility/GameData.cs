// Classe di supporto per salvare i dati del gioco
[System.Serializable]
public class GameData
{
    public int moveSpeedLevel;
    public int maxHealthLevel;
    public int luckLevel;
    public int AttackDamageLevel;
    public int attackSpeedLevel;
    public int attackRangeLevel;
    public int gears;

    public GameData()
    {
        moveSpeedLevel = 0;
        maxHealthLevel = 0;
        luckLevel = 0;
        AttackDamageLevel = 0;
        attackSpeedLevel = 0;
        attackRangeLevel = 0;
        gears = 0;
    }

    public GameData(PlayerManager playerManager)
    {
        moveSpeedLevel = playerManager.MoveSpeedLevel;
        maxHealthLevel = playerManager.HealthLevel;
        luckLevel = playerManager.LuckLevel;
        AttackDamageLevel = playerManager.DamageLevel;
        attackSpeedLevel = playerManager.AttackSpeedLevel;
        attackRangeLevel = playerManager.AttackRangeLevel;
        gears = playerManager.Gears;
    }
}