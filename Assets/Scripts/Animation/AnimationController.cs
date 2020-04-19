using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer sr;
    public AudioSource audioSource;
    public bool animating = false;
    public int animationIndex;
    private float animationTime;
    public AnimationType lastAnimation;

    //public Weapon weapon;
    [System.Serializable]
    public class Animation
    {
        public AnimationType type;
        public float time;
        public Sprite[] frames;
        public AudioClip[] sound;
        public bool looping = true;
    }
    [SerializeField] List<Animation> animations;

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
        HURT,
        DYING,
        BITING,
        KISSED
    }

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animationTime = 0.0f;
        animationIndex = 0;

        /*foreach(AnimationController ac in slaves)
        {
            ac.timeAttack = timeAttack;
            ac.timeDying = timeDying;
            ac.timeFalling = timeFalling;
            ac.timeIdle = timeIdle;
            ac.timeJumping = timeJumping;
            ac.timeWalking = timeWalking;
        }*/
    }

    public void LaunchAnimation(AnimationType animType, bool flip = false)
    {
        animating = true;
        CancelAnimation();
        StartCoroutine(DoAnimationBlocking(animType,flip));
    }
    public void CancelAnimation()
    {
        StopAllCoroutines();
    }
    IEnumerator DoAnimationBlocking(AnimationType animType, bool flip = false)
    {
        bool over = playAnimation(animType, flip,true);
        while (!over)
        {
            yield return new WaitForEndOfFrame();
            over = playAnimation(animType, flip);
        }
        animating = false;
    }

    Animation FindAnimation(AnimationType animType)
    {
        Animation animation = null;
        foreach (var a in animations)
        {
            if (a.type == animType)
            {
                animation = a;
                break;
            }
        }
        return animation;
    }
    public bool playAnimation(AnimationType animType, bool flipped = false, bool restart = false)
    {
        if (!sr) return true;

        foreach (AnimationController ac in slaves)
            ac.playAnimation(animType, flipped);


        Animation animation = FindAnimation(animType);
        if(animation==null) animation = FindAnimation(AnimationType.IDLE);

        if (animation.frames.Length == 0)
            return true;

        if (restart || animType != lastAnimation)
        {
            lastAnimation = animType;
            animationTime = 0.0f;
            animationIndex = 0;

            sr.sprite = animation.frames[animationIndex];
            sr.flipX = flipped;
        }

        if (animationTime >= animation.time)
        {
            bool loop = (animationIndex + 1) >= animation.frames.Length;
            animationTime -= animation.time;

            //if (animType != AnimationType.DYING)
            if(animation.looping)
                animationIndex = (animationIndex + 1) % animation.frames.Length;
            else
                animationIndex = Mathf.Min(animationIndex + 1, animation.frames.Length -1);

            sr.sprite = animation.frames[animationIndex];
            sr.flipX = flipped;


            if (audioSource != null && animation.sound != null && animation.sound.Length> animationIndex && animation.sound[animationIndex]!=null)
            {
                audioSource.PlayOneShot(animation.sound[animationIndex]);
            }
            return loop;
        }
        animationTime += Time.deltaTime;
        return false;
    }
}
