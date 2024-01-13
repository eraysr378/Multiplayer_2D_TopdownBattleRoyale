using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageCircleTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shrinkTimeText;
    private float timer;
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float shrinkTimer = DamageCircle.Instance.GetShrinkTimer();
        if (shrinkTimer > 0)
        {
            shrinkTimeText.text = "Circle will start shrinking in " + shrinkTimer.ToString("0") + " secs";
        }
        else
        {
            shrinkTimeText.text = "Circle is shrinking...";
        }




        timer -= Time.deltaTime;
    }
    public void SetTimer(float time)
    {
        timer = time;
    }


}
