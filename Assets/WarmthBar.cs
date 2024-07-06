using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarmthBar : MonoBehaviour
{
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
