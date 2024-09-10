using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public static class Power
{
    private static string basePath = "Prefabs/";
    private static int luck;
    private static Color bulletColor = new Color(0,0,0);

    private static Dictionary<string, string> powerDescriptions = new Dictionary<string, string>
    {
        { "Isaac", "Justice for Meat Boy" },
        { "Jack", "Share the sound with your friend!" },
        { "FireFlower", "make it bun dem!" }
    };



    public static void OneTimeInit(string power) {
        MethodInfo method = typeof(Power).GetMethod(power + "_OneTimeInit", BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null) return;

        method.Invoke(null, null);
    }

    public static void Init(string power, BulletScript bullet) {
        MethodInfo method = typeof(Power).GetMethod(power + "_Init", BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null) return;

        luck = (int)PlayerManager.Instance.LuckLevelled;

        method.Invoke(null, new object[] { bullet });
    }

    public static void OnHit(string power, BulletScript bullet, Enemy enemy) {
        MethodInfo method = typeof(Power).GetMethod(power + "_OnHit", BindingFlags.NonPublic | BindingFlags.Static);
        
        if (method == null) return;

        method.Invoke(null, new object[] { bullet, enemy });
    }

    public static bool CanDestroy(string power, BulletScript bullet) {
        MethodInfo method = typeof(Power).GetMethod(power + "_CanDestroy", BindingFlags.NonPublic | BindingFlags.Static);

        if (method == null) return true;
        
        return (bool)method.Invoke(null, new object[] { bullet });
    }

    public static string GetDescription(string power) {
        if (powerDescriptions.ContainsKey(power)) {
            return powerDescriptions[power];
        }
        return "No description available.";
    }




    private static void Isaac_OneTimeInit() {
        PlayerManager.Instance.InstantiatePrefab(basePath + "Isaac");
    }


    private static void Jack_Init(BulletScript bullet){
        bullet.remainingBounces = 1 + luck;
    }
    private static void Jack_OnHit(BulletScript bullet, Enemy enemy) {
        var enemies = PlayerManager.Instance.currentRoom.enemies;

        Enemy closestEnemy = enemy;
        float closestDistance = float.MaxValue;

        foreach (Collider2D foe in enemies) {
            Enemy tmp = foe.GetComponent<Enemy>();

            if(tmp.Id == closestEnemy.Id || tmp.Id == enemy.Id) continue;

            if (Vector2.Distance(bullet.transform.position, tmp.transform.position) < closestDistance) {
                closestEnemy = tmp;
                closestDistance = Vector2.Distance(bullet.transform.position, tmp.transform.position);
            }
        }

        bullet.remainingBounces--;

        if (closestEnemy == enemy) bullet.closestEnemy = null;
        else bullet.closestEnemy = closestEnemy;
    }
    private static bool Jack_CanDestroy(BulletScript bullet) {
        if (bullet.remainingBounces > 0) return false;
        else return true;
    }


    private static void FireFlower_Init(BulletScript bullet) {
        if (bulletColor.Equals(new Color(0,0,0))) bulletColor = new Color(255,0,0);
        else bulletColor = (bulletColor + new Color(255,0,0))/2;

        bullet.gameObject.GetComponent<SpriteRenderer>().color = bulletColor;
    }
    private static async void FireFlower_OnHit(BulletScript bullet, Enemy enemy) {
        await FireFlower_FireOnHit(bullet.damage, enemy);
    }
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