using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public int hp = 1;
    public bool alive = true;

    public Rigidbody2D rb;
    public Collider2D collider;
    public SpriteRenderer renderer;
    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }
    public void Hurt()
    {
        if (alive)
        {
            --hp;
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
