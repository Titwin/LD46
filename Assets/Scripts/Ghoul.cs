using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul : MonoBehaviour,IPerson
{
    public PeopleManager manager;
    new public AnimationController animation;
    new public Collider2D collider;
    public Vector2 direction;
    public float directionSpeed = 0.7f;
    public float speed = 1f;
    public bool alive = true;
    public int hp = 1;
    public Rigidbody2D body;
    private Vector2 fixedUpdateDirection;
    private Vector2 lastNonZeroDirection;

    [Header("Controls settings")]
    public LayerMask sightMask;
    public LayerMask ennemyMask;
    public float sightRadius = 3;
    public float ennemySearchRadius = 3;
    public float dashRange = 0.5f;
    public Collider2D attackBox;
    public ContactFilter2D attackFilter;
    bool firstFrame = true;
    bool animating = false;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackAudioClip;
    public AudioClip[] spawningAudioClips;
    public AudioClip[] dyingAudioClips;

    float eatCooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.right;
        lastNonZeroDirection = direction;
        collider = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
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
                    GetHurt(collision.gameObject,10);
                }
            }
        }
    }

    private void OnEnable()
    {
        lastNonZeroDirection = direction;
        firstFrame = true;

        int spawningAudioIndex = Random.Range(0, spawningAudioClips.Length - 1);
        audioSource.PlayOneShot(spawningAudioClips[spawningAudioIndex]);
    }

    void Teleport(Vector2 position)
    {
        body.MovePosition(position);
    }

    private Vector2 PersoMoveToward(Vector2 current, Vector2 target, float delta)
    {
        Vector2 v = target - current;
        float d = v.magnitude;
        if (delta > d)
            return target;
        else
            return current + delta * v.normalized;
    }

    public void Dash(Person target)
    {
        StartCoroutine(DoDash(target));
    }
    bool attackleft = false;
    IEnumerator DoAttack()
    {
        animating = true;
        animation.LaunchAnimation(AnimationController.AnimationType.ATTACK, attackleft);
        attackleft = !attackleft;
        var attackFilter = new ContactFilter2D();
        attackFilter.layerMask = ennemyMask;

        audioSource.PlayOneShot(attackAudioClip);

        for (int c = 0; c < attackableCount; ++c)
        {
            // RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, ennemyAttackRadius, transform.up, ennemyAttackRadius, ennemyMask);
            if (attackable[c].gameObject != this.gameObject)
            {
                Person person = attackable[c].gameObject.GetComponent<Person>();
                if (person)
                {
                    Attack(person);
                }
                else
                {
                    Monster monster = attackable[c].gameObject.GetComponent<Monster>();
                    if (monster)
                    {
                        Attack(monster);
                    }
                    else
                    {
                        Ghoul other = attackable[c].gameObject.GetComponent<Ghoul>();
                        if (other)
                        {
                            Attack(other);
                        }
                    }
                }
            }
        }
        while (animation.animating)
        {
            yield return new WaitForEndOfFrame();
        }
        animating = false;
    }
    public void Attack(Person target)
    {
        target.GetHurt(this.gameObject);
    }
    public void Attack(Ghoul target)
    {
        target.GetHurt(this.gameObject);
    }
    public void Attack(Monster target)
    {
        target.GetHurt(this.gameObject);
    }
    IEnumerator DoDash(Person target)
    {
        animating = true;
        //disable physics
        this.collider.enabled = false;
        //this.body.isKinematic = true;

        //jump animation
        Vector3 direction = target.transform.position - this.transform.position;
        transform.up = direction.normalized;
        animation.LaunchAnimation(AnimationController.AnimationType.JUMPING);

        //stun target
        target.collider.enabled = false;
        target.stunned = true;
        target.rb.simulated = false;

        yield return new WaitForSeconds(0.25f);

        Vector3 from = this.transform.position;
        Vector3 to = target.transform.position - direction.normalized * 7 * Constants.pixelToMeters;
        for (float t = 0; t < 1f; t += Time.deltaTime * 3)
        {

            Teleport(Vector3.Lerp(from, to, t));
            yield return new WaitForEndOfFrame();
        }
        Teleport(to);


        animation.LaunchAnimation(AnimationController.AnimationType.BITING);
        //player.uiBloodFrame.SetActive(true);
        audioSource.pitch = 1f + Random.Range(-0.2f, 0.2f);
        audioSource.Stop();
        audioSource.PlayOneShot(attackAudioClip);
        target.transform.up = -direction;
        float blood = target.GetKissed();
       // player.Feed(blood);
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            FXManager.instance.EmitBlood(target.transform.position, direction, 15);
            yield return new WaitForEndOfFrame();
        }
       // player.uiBloodFrame.SetActive(false);
        //restore physics
        // this.body.isKinematic = false;
        this.collider.enabled = true;
        animating = false;
    }


    public void GetHurt(GameObject source, int amount = 1)
    {
        if (alive)
        {
            --hp;
            FXManager.instance.EmitBlood(body.position, this.transform.position - source.transform.position, 10);
            if (hp <= 0)
            {
                Die();
            }
        }
    }
    public void Die()
    {
        if (alive)
        {
            alive = false;
            body.simulated = false;
            collider.enabled = false;
            GetComponent<SpriteRenderer>().color = Color.gray;
            GetComponent<SpriteRenderer>().sortingOrder = -1;
            FXManager.instance.EmitBloodStain(body.position);
            animation.LaunchAnimation(AnimationController.AnimationType.DYING);

            int dyingSoundIndex = Random.Range(0, dyingAudioClips.Length - 1);
            audioSource.PlayOneShot(dyingAudioClips[dyingSoundIndex]);
        }
    }

    bool bEat = false;
    bool bAttack = false;
    Vector2 inputDirection;
    Collider2D[] sight = new Collider2D[8];
    Collider2D[] attackable = new Collider2D[2];
    int attackableCount;
    int sightCount;
    RaycastHit2D dashable;
    public Transform target;
    public bool Active { 
        get {
            StopAllCoroutines();
            animating = false;
            return this.gameObject.activeSelf; 
        }
        set {
            this.gameObject.SetActive(value);
        }
    }

    public bool Alive => alive;

    public Vector2 Position => body.position;

    public bool Animating => animating;

    public void Sense()
    {
        attackableCount = Physics2D.OverlapCollider(attackBox, attackFilter, attackable);
        dashable = Physics2D.BoxCast(transform.position, Vector2.one, ennemySearchRadius, transform.up, dashRange, ennemyMask);
        sightCount = Physics2D.OverlapCircleNonAlloc(this.transform.position, sightRadius, sight,sightMask);
    }

    public void Think()
    {
        bAttack = false;
        bEat = false;
        eatCooldown -= Time.deltaTime;
        if (attackableCount > 1)
        {
            bAttack = true;
        }else if (eatCooldown<=0 && dashable.collider != null)
        {
            bEat = true;
        } 
        else if(sightCount >0)
        {
            target= null;
            float d = float.MaxValue;
            for(int s = 0; s < sightCount; ++s)
            {
                if (sight[s].attachedRigidbody !=this.body)
                {
                    var candidate = sight[s].transform;
                    float d2 = Vector2.Distance(this.transform.position, candidate.position);
                    if (d2 < d)
                    {
                        d = d2;
                        target = candidate.transform;
                    }
                }
            }
            if (target!=null) {

                inputDirection = (target.position-this.transform.position).normalized;
            }
        }
    }

    public void Act()
    {
        if (!alive)
        {
            return;
        }
        if (!animating)
        {
            
            bool interacting = false;
            if (!firstFrame)
            {
                if (bEat)
                {
                    if (dashable.collider != null)
                    {
                        Person person = dashable.collider.gameObject.GetComponent<Person>();
                        if (person)
                        {
                            eatCooldown = 2;
                            Dash(person);
                            interacting = true;
                        }
                    }
                    /*else if (Vector2.Distance(this.transform.position, player.carController.transform.position) < carEntryRadius)
                    {
                        player.EnterCar();
                        interacting = true;
                    }*/
                }
                else if (bAttack)
                {
                    interacting = true;
                    StartCoroutine(DoAttack());
                }
            }

            if (!interacting)
            {

                direction = PersoMoveToward(direction, inputDirection, directionSpeed * Time.deltaTime);
                if (direction != Vector2.zero)
                {
                    animation.CancelAnimation();

                    lastNonZeroDirection = direction;
                    animation.playAnimation(AnimationController.AnimationType.WALKING);
                }
                transform.up = lastNonZeroDirection;
                fixedUpdateDirection = direction;

                Teleport(body.position + fixedUpdateDirection * speed * Time.fixedDeltaTime);
            }
        }
        firstFrame = false;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
