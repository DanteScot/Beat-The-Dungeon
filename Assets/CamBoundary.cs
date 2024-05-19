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

        var tmp = tilemapCollider.bounds.center;
        Vector2 mapCenter = new Vector2(tmp.x,tmp.y);

        Vector2 point1 = new Vector2(-x,y);
        Vector2 point2 = new Vector2(x,y);
        Vector2 point3 = new Vector2(x,-y);
        Vector2 point4 = new Vector2(-x,-y);

        polygonCollider.points = new[]{mapCenter+point1,mapCenter+point2,mapCenter+point3,mapCenter+point4};
    }
    void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();
        adapt_collider();
    }
}
