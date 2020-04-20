using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour, IPerson
{
    public PeopleManager manager;
    public AnimationController animation;
    public int hp = 1;
    public bool alive = true;
    public bool stunned = false;

    public Vector2 Direction = Vector2.left;
    public Rigidbody2D rb;
    public Collider2D collider;
    public SpriteRenderer renderer;
    float t = -1;
    public float blood = 1;
    public MapTile tile;
    bool animating = false;
    Vector2 fearSource;

    public float fearRange = 6;
    public float calmRange = 10;
    public enum Status
    {
        Wandering, Scared
    }
    Status status;

    [Header("Audio")]
    public AudioClip killedAudioClip;
    public AudioSource audioSource;


    public bool Active
    {
        get
        {
            StopAllCoroutines();
            animating = false;
            return this.gameObject.activeSelf;
        }
        set
        {
            this.gameObject.SetActive(value);
        }
    }

    public bool Alive => alive;

    public Vector2 Position => rb.position;

    public bool Animating => animating;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Sense()
    {
        tile = Map.GetTile(rb.position);

        if (Player.main.walking)
        {
            fearSource = Player.main.position;
            var fdirection = this.rb.position - fearSource;
            if (fdirection.magnitude < fearRange)
            {
                status = Status.Scared;
            }
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
            if (t<0 || t > 1)
            {
                var cellNeighbor = Map.GetTileNeighbor4(rb.position);
                for (int i = 0; i < cellNeighbor.Length;++i)
                {
                    var cell = cellNeighbor[i];
                    if (cell)
                    {
                        if (cell.type == MapTile.Type.Walk || cell.type == MapTile.Type.StreetWalk)
                        {
                            Direction = (Vector2Int)Map.Directions[i];
                            Direction.y += Random.Range(-0.1f, 0.1f);
                            Direction.x += Random.Range(-0.1f, 0.1f);
                            break;
                        }
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
            t += Time.deltaTime;
        }
    }
    public void Act()
    {
        if (!alive || stunned)
        {
            return;
        }
        if (Direction.sqrMagnitude > 0)
        {
            this.rb.MovePosition(this.rb.position + Direction * (status == Status.Scared ? 2 : 1) * Time.deltaTime);
            animation.playAnimation(AnimationController.AnimationType.WALKING);
            transform.up = Direction;
        }
    }
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
    public float GetKissed()
    {
        stunned = true;
        StartCoroutine(DoGetKissed());
        float tblood = this.blood;
        this.blood = 0;
        return tblood;
    }
    IEnumerator DoGetKissed()
    {
        animation.LaunchAnimation(AnimationController.AnimationType.KISSED);
        yield return new WaitForSeconds(1);
        FXManager.instance.EmitBloodStain(rb.position);
        Die();
    }
    public void GetHurt(GameObject source, int amount = 1)
    {
        if (alive)
        {
            hp-= amount;
            FXManager.instance.EmitBlood(rb.position, this.transform.position-source.transform.position, 10);
            FXManager.instance.EmitBloodStain(rb.position);
            if (hp <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        rb.simulated = false;
        renderer.color = Color.gray;
        renderer.sortingOrder = -1;
        audioSource.pitch = 1f + Random.Range(-0.3f, 0.3f);
        audioSource.PlayOneShot(killedAudioClip);
        alive = false;
        animation.LaunchAnimation(AnimationController.AnimationType.DYING);
        manager.OnDied(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == Constants.LayerCar)
        {
            Car car = collision.gameObject.GetComponent<Car>();
            if (car)
            {
                if (car.currentSpeed > 2)
                {
                    GetHurt(collision.gameObject, 10);
                }
            }
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
