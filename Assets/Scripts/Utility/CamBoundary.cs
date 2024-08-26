using UnityEngine;
using UnityEngine.Tilemaps;

public class CamBoundary : MonoBehaviour
{
    private TilemapCollider2D tilemapCollider;
    private PolygonCollider2D polygonCollider;

    void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        tilemapCollider = transform.parent.GetComponentInChildren<TilemapCollider2D>();
        adapt_collider();
    }

    void adapt_collider(){
        var x = tilemapCollider.bounds.size.x / 2;
        var y = tilemapCollider.bounds.size.y / 2;

        var center = tilemapCollider.bounds.center;

        polygonCollider.offset = center-polygonCollider.bounds.center;

        Vector2 point1 = new Vector2(-x, y);
        Vector2 point2 = new Vector2(x, y);
        Vector2 point3 = new Vector2(x, -y);
        Vector2 point4 = new Vector2(-x, -y);

        polygonCollider.points = new[] { point1, point2, point3, point4 };
    }
}