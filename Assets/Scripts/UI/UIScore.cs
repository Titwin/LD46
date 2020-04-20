using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class UIScore : MonoBehaviour
{
    [SerializeField] Text score;
    [SerializeField] Text modifier;

    int value = 0;
    public void AddScore(int delta)
    {
        value += delta;
        score.text = "SCORE:" + value.ToString("0000000");
        modifier.text = "+" + delta;
    }
}
