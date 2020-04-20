using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class UIDialog : MonoBehaviour
{
    public Image avatar;
    public Text text;
    public RectTransform container;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip talkingAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowText(string value)
    {
        this.gameObject.SetActive(true);
        text.text = value;
        if (value.Length > 0)
        {
            audioSource.PlayOneShot(talkingAudioClip);
        }
    }
    // Update is called once per frame
    void Update()
    {
        avatar.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time*360/120)*5);
        text.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 360 / 120));
    }

    void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
