using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaterFuelBar : MonoBehaviour
{
    public Slider slider;
    public GameObject heater;
    public Vector2 offset;

    public void SetMaxFuel(int warmth)
    {
        slider.maxValue = warmth;
        slider.value = warmth;
    }

    public void SetFuel(int warmth)
    {
        slider.value = warmth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(heater.transform.position.ConvertTo2D() + offset);

    }
}
