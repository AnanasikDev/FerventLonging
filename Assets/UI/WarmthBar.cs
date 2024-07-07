using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarmthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxWarmth(int warmth)
    {
        slider.maxValue = warmth;
        slider.value = warmth;
    }

    public void SetWarmth(int warmth)
    {
        slider.value = warmth;
    }
}
