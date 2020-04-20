using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIBlood : MonoBehaviour
{
    public Mask mask;
    public float value;
    private void Start()
    {
        SetValue(1);
    }
    private void Update()
    {
        SetValue(value);
    }
    public void SetValue(float value)
    {
        this.value = value;
        mask.rectTransform.sizeDelta = new Vector2(64, 106 * value);
    }
}
