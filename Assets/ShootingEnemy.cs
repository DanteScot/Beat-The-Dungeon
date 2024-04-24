using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public static class DirectionExtensions
{
    public static Vector2 ToVector2(this Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                return new Vector2(0, 1);
            case Direction.DOWN:
                return new Vector2(0, -1);
            case Direction.LEFT:
                return new Vector2(-1, 0);
            case Direction.RIGHT:
                return new Vector2(1, 0);
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}

public class ShootingEnemy : Enemy
{
    [Header("ShootingEnemy Stats")]
    [SerializeField] private float attackSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;

    //verrà usato per dire dove si muove/attacca
    Direction direction;


    new void Start()
    {
        base.Start();
        ChangeDirection();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction.ToVector2() * speed * Time.deltaTime);
        
        //check if it is going to hit the wall
        Vector2 raycastStartPoint = new Vector2(transform.position.x, transform.position.y) + direction.ToVector2();
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, direction.ToVector2(), 0.1f);
        if(hit.collider != null){
            ChangeDirection();
        }
    }

    private void ChangeDirection(){
        int randomDirection = Random.Range(0, 4);

        switch(randomDirection){
            case 0:
                direction = Direction.UP;
                break;
            case 1:
                direction = Direction.DOWN;
                break;
            case 2:
                direction = Direction.LEFT;
                break;
            case 3:
                direction = Direction.RIGHT;
                break;
        }
    }

    public void OnGizmosDraw()
    {
        Gizmos.color = Color.green;
        //draw raycast
        Gizmos.DrawRay(transform.position, direction.ToVector2());
    }
}
