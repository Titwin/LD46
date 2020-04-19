using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player player;
    public AnimationController animation;
    public Collider2D collider;
    public Vector2 direction;
    public float directionSpeed = 0.7f;
    public float speed = 1f;

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
    bool animating = false;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackAudioClip;

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
        var results = new Collider2D[8];
        int count = Physics2D.OverlapCollider(attackBox, attackFilter, results);
        for (int c = 0; c < count; ++c)
        {
            // RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, ennemyAttackRadius, transform.up, ennemyAttackRadius, ennemyMask);
            //if (hit.collider != null)
            {
                Person person = results[c].gameObject.GetComponent<Person>();
                if (person)
                {
                    Attack(person);
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
        target.Hurt(this.gameObject);
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
        player.uiBloodFrame.SetActive(true);
        audioSource.pitch = 1f + Random.Range(-0.2f, 0.2f);
        audioSource.Stop();
        audioSource.PlayOneShot(attackAudioClip);
        target.transform.up = -direction;
        float blood = target.GetKissed();
        player.Feed(blood);
        for (float t = 0; t < 1f; t += Time.deltaTime )
        {
            FXManager.instance.EmitBlood(target.transform.position, direction, 15);
            yield return new WaitForEndOfFrame();
        }
        player.uiBloodFrame.SetActive(false);
        //restore physics
       // this.body.isKinematic = false;
        this.collider.enabled = true;
        animating = false;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, player.carController.transform.position);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, carEntryRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector2.one);
        Gizmos.DrawLine(transform.position, transform.position + ennemySearchRadius * transform.up);

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, ennemySearchRadius, transform.up, 3f, ennemyMask);
        if (hit.collider != null)
        {
            Person person = hit.collider.gameObject.GetComponent<Person>();
            Gizmos.DrawLine(this.transform.position, person.transform.position);
        }
    }
}
