using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

// Classe di supporto per i poteri
// Questa classe contiene tutti i metodi per gestire i poteri e li chiama tramite reflection
// Ci sono 4 funzioni per ogni potere:
// - OneTimeInit: chiamata una sola volta appena ottieni il potere
// - Init: chiamata ogni volta che spari un proiettile
// - OnHit: chiamata ogni volta che un proiettile colpisce un nemico
// - CanDestroy: chiamata ogni volta che un proiettile colpisce un nemico per decidere se distruggerlo o meno
// 
// Ho usato reflection per evitare di creare un enorme quantità di oggetti con script diversi
// Se ad un potere non serve una delle funzioni, non c'è bisogno di implementarla
public static class Power
{
    private static string basePath = "Prefabs/";
    private static int luck;

    // Dizionario per le descrizioni dei poteri
    private static Dictionary<string, string> powerDescriptions = new()
    {
        { "Isaac", "Justice for Meat Boy" },
        { "Jack", "Share the sound with your friend!" },
        { "FireFlower", "make it bun dem!" }
    };


    // Metodi per chiamare i metodi privati tramite reflection (descrizione generale in cima)

    public static void OneTimeInit(string power) {
        MethodInfo method = typeof(Power).GetMethod(power + "_OneTimeInit", BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null) return;

        method.Invoke(null, null);
    }

    public static void Init(string power, BulletController bullet) {
        MethodInfo method = typeof(Power).GetMethod(power + "_Init", BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null) return;

        luck = (int)PlayerManager.Instance.LuckLevelled;

        method.Invoke(null, new object[] { bullet });
    }

    public static void OnHit(string power, BulletController bullet, Enemy enemy) {
        MethodInfo method = typeof(Power).GetMethod(power + "_OnHit", BindingFlags.NonPublic | BindingFlags.Static);
        
        if (method == null) return;

        method.Invoke(null, new object[] { bullet, enemy });
    }

    public static bool CanDestroy(string power, BulletController bullet) {
        MethodInfo method = typeof(Power).GetMethod(power + "_CanDestroy", BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null) return true;
        
        return (bool)method.Invoke(null, new object[] { bullet });
    }


    // Ritora la descrizione di un potere
    public static string GetDescription(string power) {
        if (powerDescriptions.ContainsKey(power)) {
            return powerDescriptions[power];
        }
        return "No description available.";
    }



    // Isaac deve solo istanziare il prefab
    private static void Isaac_OneTimeInit() {
        PlayerManager.Instance.InstantiatePrefab(basePath + "Isaac");
    }


    // Jack fa rimbalzare i proiettili su un altro nemico
    private static void Jack_Init(BulletController bullet){
        bullet.remainingBounces = 1 + luck;
    }
    // Setta closestEnemy sul proiettile in base al nemico più vicino al momento dell'impatto
    private static void Jack_OnHit(BulletController bullet, Enemy enemy) {
        Enemy[] enemies = PlayerManager.Instance.currentRoom.Enemies;

        Enemy closestEnemy = enemy;
        float closestDistance = float.MaxValue;

        foreach (Enemy foe in enemies) {
            if(foe.Id == closestEnemy.Id || foe.Id == enemy.Id) continue;

            if (Vector2.Distance(bullet.transform.position, foe.transform.position) < closestDistance) {
                closestEnemy = foe;
                closestDistance = Vector2.Distance(bullet.transform.position, foe.transform.position);
            }
        }

        bullet.remainingBounces--;

        if (closestEnemy == enemy) bullet.closestEnemy = null;
        else bullet.closestEnemy = closestEnemy;
    }
    // Il proiettile non viene distrutto se ha ancora rimbalzi
    private static bool Jack_CanDestroy(BulletController bullet) {
        if (bullet.remainingBounces > 0) return false;
        else return true;
    }

    // FireFlower fa bruciare i nemici, cambia il colore del proiettile e infligge danno continuo
    private static void FireFlower_Init(BulletController bullet) {
        if (bullet.bulletColor.Equals(new Color(0,0,0))) bullet.bulletColor = new Color(255,0,0);
        else bullet.bulletColor = (bullet.bulletColor + new Color(255,0,0))/2;

        bullet.gameObject.GetComponent<SpriteRenderer>().color = bullet.bulletColor;
    }
    private static async void FireFlower_OnHit(BulletController bullet, Enemy enemy) {
        await FireFlower_FireOnHit(bullet.damage/2, enemy);
    }
    // Infligge damge continuo ogni secondo per un totale di 1 + luck secondi
    static async Task<bool> FireFlower_FireOnHit(float damage, Enemy enemy) {
        enemy.IsOnFire = true;
        for (int i = 0; i < 1 + PlayerManager.Instance.LuckLevel; i++){
            await Task.Delay(1000);
            enemy.TakeDamage(damage);
        }
        enemy.IsOnFire = false;
        return true;
    }
}