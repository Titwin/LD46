using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLooking : MonoBehaviour
{
    public SpriteRenderer front;
    public SpriteRenderer back;

    public List<Sprite> frontTemplates;
    public List<Sprite> backTemplates;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        int index = Random.Range(0, frontTemplates.Count);
        front.sprite = frontTemplates[index];
        back.sprite = backTemplates[index];
    }
}
