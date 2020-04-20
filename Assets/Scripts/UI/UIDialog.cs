using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class UIDialog : MonoBehaviour
{
    public Image avatar;
    public Text text;
    public RectTransform container;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowText(string value)
    {
        this.gameObject.SetActive(true);
        text.text = value;
    }
    // Update is called once per frame
    void Update()
    {
        avatar.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time*360/120)*5);
    }

    void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
