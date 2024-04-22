using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float health = 3f;
    public float luck = 1f;

    void FixedUpdate()
    {
        float deltaX = Input.GetAxis("Horizontal") * moveSpeed;
        float deltaY = Input.GetAxis("Vertical") * moveSpeed;

        Vector2 movement = new Vector2(deltaX, deltaY);
        movement = Vector2.ClampMagnitude(movement, moveSpeed);

        transform.Translate(movement * Time.deltaTime);
    }
}
