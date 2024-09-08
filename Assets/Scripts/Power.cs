using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class Power
{
    public static void Init(string power, BulletScript bullet) {
        if (bullet == null) {
            Debug.LogError("Bullet object is null.");
            return;
        }

        MethodInfo method = typeof(Power).GetMethod(power + "_Init", BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null) {
            Debug.Log($"Method {power}_Init not found.");
            return;
        }
        method.Invoke(null, new object[] { bullet });
    }

    public static void OnHit(string power, BulletScript bullet, Enemy enemy) {
        if (bullet == null) {
            Debug.LogError("Bullet object is null.");
            return;
        }

        MethodInfo method = typeof(Power).GetMethod(power + "_OnHit", BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null) {
            Debug.Log($"Method {power}_OnHit not found.");
            return;
        }
        method.Invoke(null, new object[] { bullet, enemy });
    }

    public static bool CanDestroy(string power, BulletScript bullet) {
        if (bullet == null) {
            Debug.Log("Bullet object is null.");
            return true;
        }

        MethodInfo method = typeof(Power).GetMethod(power + "_CanDestroy", BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null) {
            Debug.Log($"Method {power}_CanDestroy not found.");
            return true;
        }
        return (bool)method.Invoke(null, new object[] { bullet });
    }

    private static void Isaac_Init(BulletScript bullet) {
        bullet.remainingBounces = 100;
        return;
    }

    private static void Isaac_OnHit(BulletScript bullet, Enemy enemy) {
        bullet.remainingBounces = 200;
        return;
    }


    private static void Jack_Init(BulletScript bullet){
        bullet.remainingBounces = 2;
    }
    private static void Jack_OnHit(BulletScript bullet, Enemy enemy) {
        var enemies = Physics2D.OverlapCircleAll(bullet.transform.position, bullet.range, LayerMask.GetMask("Enemy"));

        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var foe in enemies) {
            if(foe.transform == enemy.transform) continue;

            var tmp = foe.GetComponent<Enemy>();

            if (tmp == null) continue;

            if (closestEnemy == null) {
                closestEnemy = tmp;
                continue;
            }

            if (Vector2.Distance(bullet.transform.position, tmp.transform.position) < closestDistance) {
                closestEnemy = tmp;
                closestDistance = Vector2.Distance(bullet.transform.position, tmp.transform.position);
            }
        }

        bullet.remainingBounces--;
        bullet.closestEnemy = closestEnemy;
    }
    private static bool Jack_CanDestroy(BulletScript bullet) {
        if (bullet.remainingBounces > 0) return false;
        else return true;
    }
}