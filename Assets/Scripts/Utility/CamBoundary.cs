using UnityEngine;
using UnityEngine.Tilemaps;

// Questo script serve per adattare il collider del boundary della camera al collider del tilemap
// In questo modo la camera non si sposterà mai fuori dalla mappa
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

    // Prende la dimensione del collider del tilemap e setta i punti del collider
    void adapt_collider(){
        float x = tilemapCollider.bounds.size.x / 2;
        float y = tilemapCollider.bounds.size.y / 2;

        Vector3 center = tilemapCollider.bounds.center;

        polygonCollider.offset = center-polygonCollider.bounds.center;

        Vector2 point1 = new(-x, y);
        Vector2 point2 = new(x, y);
        Vector2 point3 = new(x, -y);
        Vector2 point4 = new(-x, -y);

        polygonCollider.points = new[] { point1, point2, point3, point4 };
    }
}