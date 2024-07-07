using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HeaterBehavior : MonoBehaviour
{

    // Warming effect
    public float warmthTimeInterval;
    private float warmthTimer = 0.0f;

    // Warmth params
    public float warmthStrength;
    public float heatRadius;

    // Fuel params
    public float MAX_FUEL;
    public float fuel;
    public float fuelBurnRate; // per sec

    // Refs
    public Player player;
    public GameObject WarmthBarFill;
    public HeaterFuelBar heaterFuelBar;

    private void Start()
    {
        fuel = MAX_FUEL;
        heaterFuelBar.SetMaxFuel((int) MAX_FUEL);
        fuel = 10;
    }

    // Update is called once per frame
    void Update()
    { 
        // Warm the player if in range
        if (isTimeToWarm())
        {
            if (playerInRange() && hasFuel() && Input.GetKey(KeyCode.Space))
            {
                player.incrementWarmth(warmthStrength);
            }

            warmthTimer -= warmthTimeInterval; // Resets timer
        }

        // Update warming timer
        warmthTimer += Time.deltaTime;

        if (playerInRange() && hasFuel() && Input.GetKey(KeyCode.Space))
        {
            enableBarWarmEffect();
        } else
        {
            disableBarWarmEffect();
        }

        // Deplete fuel
        depleteFuel();
    }

    // Make the warmth bar orange
    private void enableBarWarmEffect()
    {
        Image image = WarmthBarFill.GetComponent<Image>();
        image.color = new Color32(195, 100, 16, 255);
    }

    // Makes the warmth bar back to red
    private void disableBarWarmEffect()
    {
        Image image = WarmthBarFill.GetComponent<Image>();
        image.color = new Color32(176, 26, 26, 255);
    }

    private void depleteFuel()
    {
        fuel -= fuelBurnRate * Time.deltaTime;
        if (fuel < 0)
        {
            fuel = 0;
        }

        heaterFuelBar.SetFuel((int)fuel);
    }

    // It is time to warm when the timer has reached the time interval
    private bool isTimeToWarm()
    {
        return warmthTimer >= warmthTimeInterval;
    }

    // The player is in range when within the heat radius
    private bool playerInRange()
    {
        float distToPlayer = (player.transform.position - transform.position).magnitude;
        return distToPlayer < heatRadius;
    }

    public bool hasFuel()
    {
        return fuel > 0;
    }
}
