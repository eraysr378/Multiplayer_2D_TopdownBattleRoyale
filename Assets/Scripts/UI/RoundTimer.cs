using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    static public RoundTimer Instance { get; private set; }
    public event EventHandler OnRoundTimeIsUp;
    [SerializeField] private float roundTime;
    [SerializeField] private float timerCurrentValue;
    [SerializeField] private bool isStarted;
    [SerializeField] private TextMeshProUGUI roundTimeText;
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            if(timerCurrentValue > 0)
            {
                timerCurrentValue -= Time.deltaTime;
                int min = (int)(timerCurrentValue / 60);
                int sec = (int)(timerCurrentValue % 60);
                roundTimeText.text = min.ToString() + ":" + sec.ToString();
            }
          

        }
    }
    public void StartTimer(float time)
    {
        timerCurrentValue = time;
        isStarted = true;
    }
    public void ResumeTimer()
    {
        isStarted = true;
    }
    public void PauseTimer()
    {
        isStarted = false;
    }
    public void ResetTimer()
    {
        timerCurrentValue = roundTime;
    }

}
