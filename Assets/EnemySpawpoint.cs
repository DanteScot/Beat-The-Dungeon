using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenerationCategory
{
    first,
    second,
    third
}

public class EnemySpawpoint : MonoBehaviour
{
    public GenerationCategory generationCategory;

    private void OnValidate() {
        if (generationCategory == GenerationCategory.first) {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        } else if (generationCategory == GenerationCategory.second) {
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        } else if (generationCategory == GenerationCategory.third) {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void Start() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
