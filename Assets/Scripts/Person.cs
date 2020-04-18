using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public int hp = 1;
    public bool alive = true;

    public Vector2 Direction = Vector2.left;
    public Rigidbody2D rb;
    public Collider2D collider;
    public SpriteRenderer renderer;
    float t = 0;
    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Think()
    {
        if (!alive)
        {
            return;
        }
        t += Time.deltaTime;
        if (t > 1)
        {
            t -= 1;
            float r = Random.value;
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
                Direction = new Vector2(0,-1);
            }
            else if (r < 4 / 4f)
            {
                Direction = new Vector2(0, 1);
            }
        }
    }
    public void Act()
    {
        if (!alive)
        {
            return;
        }
        this.rb.MovePosition(this.rb.position + Direction * Time.deltaTime);
    }

    public void Hurt()
    {
        if (alive)
        {
            --hp;
            FXManager.instance.EmitBlood(rb.position, -rb.velocity, 1);
            if (hp <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        //rb.simulated = false;
        renderer.color = Color.red;
        alive = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == Constants.LayerCar)
        {
            Hurt();
        }
    }
}
