using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class TimeManager : MonoBehaviour
{
    public Text timeText;
    public double time;
    public float baseTimeScale = 60;
    public float timeScale;
    public int minuteMultiplier = 5;
    public int day;
    public int night;
    public int hours;
    public int minutes;
    public float seconds;

    public bool isNight {
        get {
            return hours > 20 || hours < 6;
        }
    }
    public bool paused;

    public void Start()
    {
        SetTime(day, hours, minutes, seconds);
    }
    public void SetTime(int day, int hour, int minutes, float seconds = 0)
    {
        this.day = day;
        this.hours = hour;
        this.minutes = minutes;
        this.seconds = seconds;
        time = day * (hour + (minutes + seconds / 60.0) / 60.0) / 24.0;
    }
    private void Update()
    {
        bool wasNight = isNight;
        int intensity = 0;
        if (!paused)
        {
            int prevSeconds = (int)seconds;
            seconds += Time.deltaTime * timeScale* baseTimeScale;
            if (prevSeconds < (int)seconds)
            {
                //intensity += 1;
                if (seconds >= 60)
                {
                    seconds -= 60;
                    minutes+= minuteMultiplier;
                    intensity += 5;
                }
                if (minutes >= 60)
                {
                    minutes -= 60;
                    ++hours;
                    intensity += 20;
                }
                if (hours >=24)
                {
                    hours -= 24;
                    ++day;
                    intensity += 50;
                }
            }
        }
        if (intensity > 0)
        {
            FadeStart(intensity);
        }
        timeText.text = hours.ToString("00") + ":" + minutes.ToString("00");
       
        if(!wasNight && isNight)
        {
            Debug.Log("night started");
        }
        else if (wasNight && !isNight)
        {
            Debug.Log("night ended");
        }
    }

    public Color baseColor;
    void FadeStart(float delta)
    {
        float t = delta / 100;
        float duration = Mathf.Lerp(0.05f, 0.25f, t);
        LeanTween.value(gameObject, baseColor, Color.Lerp(baseColor,Color.white,t), duration).setOnUpdate(
            (Color val) => {
                timeText.color = val;
            }
        ).setOnComplete(FadeFinished);

        timeText.rectTransform.LeanScale(new Vector3(1, 1, 1) * Mathf.Lerp(1.0f, 1.5f, t), duration);
    }
    void FadeFinished()
    {
        LeanTween.value(gameObject, Color.white, baseColor, 0.25f).setOnUpdate(
             (Color val) => {
                 timeText.color = val;
             }
         );

        timeText.rectTransform.LeanScale(new Vector3(1, 1, 1), 0.25f);
    }
}
