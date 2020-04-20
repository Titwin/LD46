using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class UIScore : MonoBehaviour
{
    [SerializeField] Color baseColor;
    [SerializeField] Text score;
    [SerializeField] Text modifier;

    private void Start()
    {
        modifier.color = new Color(0, 0, 0, 0);
    }
    int value = 0;
    public void AddScore(int delta)
    {
        value += delta;
        score.text = "SCORE:" + value.ToString("0000000");
        modifier.text = "+" + delta;

        FadeStart(delta);
    }

    void FadeStart(int delta)
    {
        float duration = Mathf.Lerp(0.15f, 0.25f, delta / 100);
        LeanTween.value(gameObject, baseColor, Color.white, duration).setOnUpdate(
            (Color val) => {
                score.color = val;
            }
        ).setOnComplete(FadeFinished);

        LeanTween.value(gameObject, new Color(0,0,0,0), Color.white, duration).setOnUpdate(
           (Color val) => {
               modifier.color = val;
           }
       );
        
        score.rectTransform.LeanScale(new Vector3(1, 1, 1)*Mathf.Lerp(1.1f,1.3f,delta/100), duration );
       // score.rectTransform.LeanRotate(new Vector3(2, 2, 2), 0.15f);

        modifier.rectTransform.LeanScale(new Vector3(1f, 1f, 1) * Mathf.Lerp(1.1f, 1.5f, delta / 100), duration);
        //modifier.rectTransform.LeanRotate(new Vector3(3, 3, 3), 0.15f);
    }
    void FadeFinished()
    {
        LeanTween.value(gameObject, Color.white, baseColor, 0.25f).setOnUpdate(
             (Color val) => {
                 score.color = val;
             }
         );
        LeanTween.value(gameObject, Color.white, new Color(0, 0, 0, 0), 0.25f).setOnUpdate(
             (Color val) => {
                 modifier.color = val;
             }
         );
        score.rectTransform.LeanScale(new Vector3(1,1, 1), 0.25f);
        //score.rectTransform.LeanRotate(new Vector3(-2, -2, -2), 0.15f);
        modifier.rectTransform.LeanScale(new Vector3(1, 1, 1), 0.25f);
        //modifier.rectTransform.LeanRotate(new Vector3(-3, -3, -3), 0.25f);
    }
}
