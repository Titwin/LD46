using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSounds : MonoBehaviour
{
    public AudioSource audiosource;
    public AnimationController animations;
    public List<AudioClip> combatSounds;

    private int lastIndex = 0;

    void Update()
    {
        if(animations.lastAnimation == AnimationController.AnimationType.ATTACK && animations.animationIndex == 1 && animations.animationIndex != lastIndex)
        {
            audiosource.clip = combatSounds[Random.Range(0, combatSounds.Count)];
            audiosource.Play();
        }
    }
}
