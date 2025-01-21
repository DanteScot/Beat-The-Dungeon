using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeratore che definisce il gruppo per la generazione dei nemici in modo casuale
/// </summary>
public enum GenerationGroup
{
    first,
    second,
    third
}

public class EnemySpawpoint : MonoBehaviour
{
    public GenerationGroup generationGroup;

    private void OnValidate() {
        if (generationGroup == GenerationGroup.first) {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        } else if (generationGroup == GenerationGroup.second) {
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        } else if (generationGroup == GenerationGroup.third) {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void Start() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
