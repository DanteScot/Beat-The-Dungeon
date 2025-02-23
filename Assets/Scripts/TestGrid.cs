using UnityEngine;

// NON UTILIZZATO, LO TENGO PER RICORDARMI DELLA POSSIBILITA' DI UTILIZZARE IL GRID IN MODO DINAMICO
public class TestScript : MonoBehaviour
{
    Grid grid;
    [SerializeField] GameObject sprite;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = grid.GetCellCenterWorld(new Vector3Int(0, 0, 0));

        Instantiate(sprite, pos, Quaternion.identity);
    }
}
