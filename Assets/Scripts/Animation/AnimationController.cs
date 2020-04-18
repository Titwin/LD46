using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer sr;

    public int animationIndex;
    private float animationTime;
    public AnimationType lastAnimation;

    //public Weapon weapon;

    [Space(10)]
    public float timeIdle;
    public Sprite[] animationIdle;

    [Space(10)]
    public float timeWalking;
    public Sprite[] animationWalking;

    /*[Space(10)]
    public float timeDucking;
    public Sprite[] animationDucking;

    [Space(10)]
    public float timeJumpPrepare;
    public Sprite[] animationJumpPrepare;*/

    [Space(10)]
    public float timeJumping;
    public Sprite[] animationJumping;

    [Space(10)]
    public float timeFalling;
    public Sprite[] animationFalling;

    [Space(10)]
    public float timeAttack;
    public Sprite[] animationAttack;

    [Space(10)]
    public float timeDying;
    public Sprite[] animationDying;

    public List<AnimationController> slaves = new List<AnimationController>();
    
    public enum AnimationType
    {
        IDLE,
        WALKING,
        DUCKING,
        JUMPPREPARE,
        JUMPING,
        FALLING,
        ATTACK,
        DYING
    }

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animationTime = 0.0f;
        animationIndex = 0;

        foreach(AnimationController ac in slaves)
        {
            ac.timeAttack = timeAttack;
            ac.timeDying = timeDying;
            ac.timeFalling = timeFalling;
            ac.timeIdle = timeIdle;
            ac.timeJumping = timeJumping;
            ac.timeWalking = timeWalking;
        }
    }

    // Update is called once per frame
    public void playAnimation(AnimationType animType, bool flipped = false)
    {
        if (!sr) return;

        foreach (AnimationController ac in slaves)
            ac.playAnimation(animType, flipped);

        Sprite[] animation;
        float t;

        switch (animType)
        {
            case AnimationType.WALKING:
                animation = animationWalking;
                t = timeWalking;
                break;
            /*case AnimationType.DUCKING:
                animation = animationDucking;
                t = timeDucking;
                break;*/
            case AnimationType.ATTACK:
                animation = animationAttack;
                t = timeAttack;
                break;
            /*case AnimationType.JUMPPREPARE:
                animation = animationJumpPrepare;
                t = timeJumpPrepare;
                break;*/
            case AnimationType.JUMPING:
                animation = animationJumping;
                t = timeJumping;
                break;
            case AnimationType.FALLING:
                animation = animationFalling;
                t = timeFalling;
                break;
            case AnimationType.DYING:
                animation = animationDying;
                t = timeDying;
                break;
            default:
                animation = animationIdle;
                t = timeIdle;
                break;
        }



        if (animation.Length == 0)
            return;

        if (animType != lastAnimation)
        {
            lastAnimation = animType;
            animationTime = 0.0f;
            animationIndex = 0;

            sr.sprite = animation[animationIndex];
            sr.flipX = flipped;
        }


        if (animationTime >= t)
        {
            animationTime -= t;
            if (lastAnimation != AnimationType.DYING)
                animationIndex = (animationIndex + 1) % animation.Length;
            else
                animationIndex = Mathf.Min(animationIndex + 1, animation.Length -1);
            sr.sprite = animation[animationIndex];
            sr.flipX = flipped;
        }
        animationTime += Time.deltaTime;
    }
}
