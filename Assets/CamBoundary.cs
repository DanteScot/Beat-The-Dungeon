using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CamBoundary : MonoBehaviour
{
    private TilemapCollider2D tilemapCollider;
    private PolygonCollider2D polygonCollider;

    void adapt_collider(){
        var x=tilemapCollider.bounds.size.x/2;
        var y=tilemapCollider.bounds.size.y/2;

        // faccio -1 per "correggere" il fatto che non esiste un vero e proprio centro del tilemap
        Vector2 point1 = new Vector2(-x,y-1);
        Vector2 point2 = new Vector2(x,y-1);
        Vector2 point3 = new Vector2(x,-y-1);
        Vector2 point4 = new Vector2(-x,-y-1);

        polygonCollider.points = new[]{point1,point2,point3,point4};
    }
    void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();
        adapt_collider();
    }
}
