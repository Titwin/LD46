using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public PeopleManager manager;

    public int hp = 1;
    public bool alive = true;

    public Vector2 Direction = Vector2.left;
    public Rigidbody2D rb;
    public Collider2D collider;
    public SpriteRenderer renderer;
    float t = 0;

    public MapTile tile;

    Vector2 fearSource;

    static float fearRange = 6;
    static float calmRange = 10;
    public enum Status
    {
        Wandering, Scared
    }
    Status status;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Sense()
    {
        tile = Map.GetTile(rb.position);
        fearSource = Player.main.position;
        var fdirection = this.rb.position - fearSource;
        if (fdirection.magnitude < fearRange)
        {
            status = Status.Scared;
        }
    }
    public void Think()
    {
        if (!alive)
        {
            return;
        }
        if (status == Status.Scared)
        {
            Direction = this.rb.position - fearSource;
            if (Direction.magnitude > calmRange)
            {
                status = Status.Wandering;
                Direction.Normalize();
            }
            else
            {
                Direction.Normalize();
            }
        }
        if (status == Status.Wandering)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                var cellNeighbor = Map.GetTileNeighbor(rb.position);
                for (int i = 0; i < cellNeighbor.Length;++i)
                {
                    var cell = cellNeighbor[i];
                    if (cell.type == MapTile.Type.Walk)
                    {
                        Direction = (Vector2Int)Map.Directions[i];
                        break;
                    }
                }
                t -= 1;
                float r = Random.value;
                /*
                if (r < 1 / 4f)
                {
                    Direction = new Vector2(-1, 0);
                }
                else if (r < 2 / 4f)
                {
                    Direction = new Vector2(1, 0);
                }
                else if (r < 3 / 4f)
                {
                    Direction = new Vector2(0, -1);
                }
                else if (r < 4 / 4f)
                {
                    Direction = new Vector2(0, 1);
                }*/
            }
        }
    }
    public void Act()
    {
        if (!alive)
        {
            return;
        }
        this.rb.MovePosition(this.rb.position + Direction *(status == Status.Scared?2:1)* Time.deltaTime);
    }

    public void Hurt(GameObject source)
    {
        if (alive)
        {
            --hp;
            FXManager.instance.EmitBlood(rb.position, this.transform.position-source.transform.position, 1);
            if (hp <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        rb.simulated = false;
        renderer.color = Color.red;
        alive = false;
        manager.OnDied(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == Constants.LayerCar)
        {
            Hurt(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawLine(this.rb.position, fearSource);
#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1, 0, 0, 0.2f);
        UnityEditor.Handles.DrawWireDisc(this.rb.position, Vector3.back, calmRange);
#endif
    }
}
