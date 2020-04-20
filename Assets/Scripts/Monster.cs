using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
   
    public Player player;
    new public AnimationController animation;
    new public Collider2D collider;
    public Vector2 direction;
    public float directionSpeed = 0.7f;
    public float speed = 1f;
    public bool alive = true;

    public Rigidbody2D body;
    private Vector2 fixedUpdateDirection;
    private Vector2 lastNonZeroDirection;

    [Header("Controls settings")]
    public LayerMask ennemyMask;
    public float carEntryRadius = 3;
    public float ennemySearchRadius = 3;
    public Collider2D attackBox;
    public ContactFilter2D attackFilter;
    bool firstFrame = true;
    public bool animating = false;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackAudioClip;
    public AudioClip bittingAudioClip;
    public AudioClip dyingAudioClip;
    public AudioClip[] eatingAudioClips;
    private List<AudioClip> unusedEatingAudioClips = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.right;
        lastNonZeroDirection = direction;
        collider = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
    }


    private void OnEnable()
    {
        lastNonZeroDirection = direction;
        firstFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive || !player.Active)
        {
            return;
        }
        if (!animating)
        {
            Vector2 d = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            bool interacting = false;
            if (!firstFrame)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, ennemySearchRadius, transform.up, 3f, ennemyMask);
                    if (hit.collider != null)
                    {
                        Person person = hit.collider.gameObject.GetComponent<Person>();
                        if (person)
                        {
                            Dash(person);
                            interacting = true;
                        }
                    }
                    else if (Vector2.Distance(this.transform.position, player.carController.transform.position) < carEntryRadius)
                    {
                        player.EnterCar();
                        interacting = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftControl)){
                    interacting = true;
                    StartCoroutine(DoAttack());
                }
            }

            if (!interacting)
            {
               
                direction = PersoMoveToward(direction, d, directionSpeed * Time.deltaTime);
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
        
        //Debug.Log(fixedUpdateDirection * speed * Time.fixedDeltaTime);
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
        var attackable = new Collider2D[8];
        int count = Physics2D.OverlapCollider(attackBox, attackFilter, attackable);
        for (int c = 0; c < count; ++c)
        {
            // RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, ennemyAttackRadius, transform.up, ennemyAttackRadius, ennemyMask);
            //if (hit.collider != null)
            {
                Person person = attackable[c].gameObject.GetComponent<Person>();
                if (person)
                {
                    Attack(person);
                }
                else
                {
                    Ghoul monster = attackable[c].gameObject.GetComponent<Ghoul>();
                    if (monster)
                    {
                        Attack(monster);
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
        bool kill = target.GetHurt(this.gameObject,3);
        if (kill)
        {
            player.OnKillPerson(false);
        }
    }
    public void Attack(Ghoul target)
    {
        bool kill = target.GetHurt(this.gameObject, 3);
        if (kill)
        {
            player.OnKillPerson(false);
        }
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
                    GetHurt(collision.gameObject);
                }
            }
        }
    }
    public Transform ClosestPerson()
    {
        var sight = new Collider2D[8];

        int sightCount = 0;
        float radius = 2;
        while (sightCount==0 && radius < 32)
        {
            sightCount = Physics2D.OverlapCircleNonAlloc(this.transform.position, radius, sight, attackFilter.layerMask);
            radius *= 2;
        }
        Transform result = null;
        float d = float.MaxValue;
        for (int s = 0; s < sightCount; ++s)
        {
            if (sight[s].attachedRigidbody != this.body)
            {
                var candidate = sight[s].transform;
                float d2 = Vector2.Distance(this.transform.position, candidate.position);
                if (d2 < d)
                {
                    d = d2;
                    result = candidate.transform;
                }
            }
        }
        return result;
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
        audioSource.pitch = 1f;
        audioSource.Stop();
        audioSource.PlayOneShot(attackAudioClip);
        for (float t = 0; t < 1f; t += Time.deltaTime * 3)
        {

            Teleport(Vector3.Lerp(from, to, t));
            yield return new WaitForEndOfFrame();
        }
        Teleport(to);
       

        animation.LaunchAnimation(AnimationController.AnimationType.BITING);
        audioSource.pitch = 0.8f + Random.Range(-0.1f, 0.1f);
        audioSource.PlayOneShot(bittingAudioClip);

        player.uiBloodFrame.SetActive(true);
        target.transform.up = -direction;
        float blood = target.GetKissed(true);
        player.Feed(blood);
        player.OnEatPerson();
        for (float t = 0; t < 1f; t += Time.deltaTime )
        {
            FXManager.instance.EmitBlood(target.transform.position, direction, 15);
            yield return new WaitForEndOfFrame();
        }
        if (Random.Range(0,10) < 4)
        {
            if (unusedEatingAudioClips.Count == 0) {
                unusedEatingAudioClips.AddRange(eatingAudioClips);
            }

            int eatingAudioIndex = Random.Range(0, unusedEatingAudioClips.Count - 1);
            audioSource.pitch = 1;
            AudioClip usedAudioClip = unusedEatingAudioClips[eatingAudioIndex];
            unusedEatingAudioClips.Remove(usedAudioClip);
            audioSource.PlayOneShot(usedAudioClip);
        }
        player.uiBloodFrame.SetActive(false);
        //restore physics
       // this.body.isKinematic = false;
        this.collider.enabled = true;
        animating = false;
    }

    public void GetHurt(GameObject source)
    {
        if (alive)
        {
            --player.blood;
            FXManager.instance.EmitBlood(body.position, this.transform.position - source.transform.position, 10);
            if (player.blood <= 0)
            {
                Die();
            }
        }
    }
    public void Die()
    {
        if (alive)
        {
            body.simulated = false;
            collider.enabled = false;
            GetComponent<Renderer>().sortingOrder = -1;
            alive = false; 
            FXManager.instance.EmitBloodStain(body.position);
            animation.LaunchAnimation(AnimationController.AnimationType.DYING);
            audioSource.PlayOneShot(dyingAudioClip);
        }
    }
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, player.carController.transform.position);

#if UNITY_EDITOR
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, carEntryRadius);
#endif
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector2.one);
        Gizmos.DrawLine(transform.position, transform.position + ennemySearchRadius * transform.up);
    }
}
