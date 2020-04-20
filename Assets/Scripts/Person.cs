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
    Collider2D[] fearSources = new Collider2D[8];
    Vector2 fearSource;
    public LayerMask fearMask;
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
            
            return this.gameObject.activeSelf;
        }
        set
        {
            if (!value)
            {
                StopAllCoroutines();
                animating = false;
            }
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
        if (!alive || stunned)
        {
            return;
        }
        tile = Map.GetTile(rb.position);

        //if (status == Status.Wandering)
        {
            int fearCount = Physics2D.OverlapCircleNonAlloc(this.transform.position, fearRange, fearSources, fearMask);
            var fearSource = Vector2.zero;
            if (fearCount > 0)
            {
                for (int f = 0; f < fearCount; ++f) {
                    fearSource += (Vector2)(this.transform.position - fearSources[f].transform.position);
                }
                status = Status.Scared;
                fearSource /= fearCount;
            }
            else
            {
                status = Status.Wandering;
            }
           
        }
    }
    public void Think()
    {
        if (!alive || stunned)
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
            if (t<0)
            {
                var cellNeighbor = Map.GetTileNeighbor4(rb.position);
                List<int> options = new List<int>();
                for (int i = 0; i < cellNeighbor.Length;++i)
                {
                    var cell = cellNeighbor[i];
                    
                    if (cell)
                    {
                        if (cell.type == MapTile.Type.Walk || cell.type == MapTile.Type.StreetWalk || cell.type == MapTile.Type.Vegetation)
                        {
                            options.Add(i);
                        }
                    }
                }
                if (options.Count > 0)
                {
                    int i = Random.Range(0, options.Count);
                    Direction = (Vector2Int)Map.Directions[options[i]];
                }
                else
                {
                    int i = Random.Range(0, cellNeighbor.Length);
                    Direction = (Vector2Int)Map.Directions[i];
                }
                Direction.y += Random.Range(-0.25f, 0.25f);
                Direction.x += Random.Range(-0.25f, 0.25f);
                t = Random.Range(1,3);
            }
            t -= Time.deltaTime;
        }
    }
    Vector2 currentDirection;
    public void Act()
    {
        if (!alive || stunned)
        {
            return;
        }
        if (Direction.sqrMagnitude > 0)
        {
            currentDirection = Vector2.MoveTowards(currentDirection, Direction, Time.deltaTime);
            this.rb.MovePosition(this.rb.position + currentDirection * (status == Status.Scared ? 2 : 1) * Time.deltaTime);
            animation.playAnimation(AnimationController.AnimationType.WALKING);
            transform.up = currentDirection;
        }
    }
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
    public float GetKissed()
    {
        stunned = true; 
        float tblood = this.blood;
        this.blood = 0;
        if (Active)
        {
            StartCoroutine(DoGetKissed());
        }
        else
        {
            Die();
        }
       
        return tblood;
    }
    IEnumerator DoGetKissed()
    {
        animation.LaunchAnimation(AnimationController.AnimationType.KISSED);
        yield return new WaitForSeconds(1);
        FXManager.instance.EmitBloodStain(rb.position);
        Die(true);
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

    void Die(bool respawnAsGhoul = false)
    {
        rb.simulated = false;
        renderer.color = Color.gray;
        renderer.sortingOrder = -1;
        audioSource.pitch = 1f + Random.Range(-0.3f, 0.3f);
        audioSource.PlayOneShot(killedAudioClip);
        alive = false;
        if(Active)
            animation.LaunchAnimation(AnimationController.AnimationType.DYING);
        manager.OnDied(this);
        animating = false;
        
        if (respawnAsGhoul)
        {
            StartCoroutine(DoRespawnAsGhoul());
        }
        else
        {
            manager.AddGhoul(this.Position);
        }
    }

    IEnumerator DoRespawnAsGhoul()
    {
        yield return new WaitForSeconds(3);
        manager.AddGhoul(this.Position);
        this.Active = false;
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
