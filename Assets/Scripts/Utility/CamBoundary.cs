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
        Debug.Log("center: " + center);

        // Imposta l'offset del PolygonCollider2D al centro del TilemapCollider2D
        polygonCollider.offset = center-polygonCollider.bounds.center;

        Debug.Log("x: " + x);
        Debug.Log("y: " + y);

        // faccio -1 per "correggere" il fatto che non esiste un vero e proprio centro del tilemap
        Vector2 point1 = new Vector2(-x, y);
        Vector2 point2 = new Vector2(x, y);
        Vector2 point3 = new Vector2(x, -y);
        Vector2 point4 = new Vector2(-x, -y);

        Debug.Log("point1: " + point1);
        Debug.Log("point2: " + point2);
        Debug.Log("point3: " + point3);
        Debug.Log("point4: " + point4);

        polygonCollider.points = new[] { point1, point2, point3, point4 };
    }
}