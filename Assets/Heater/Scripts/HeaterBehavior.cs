using UnityEngine;
using UnityEngine.UI;

public class HeaterBehavior : MonoBehaviour
{
    public HeaterRenderer heaterRenderer { get; private set; }

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
    public PlayerWarmth player;
    public GameObject WarmthBarFill;
    public HeaterFuelBar heaterFuelBar;

    [HideInInspector] public new Rigidbody2D rigidbody;

    public void Init()
    {
        heaterFuelBar.SetMaxFuel((int) MAX_FUEL);
        rigidbody = GetComponent<Rigidbody2D>();
        heaterRenderer = GetComponent<HeaterRenderer>();
    }

    // Update is called once per frame
    void Update()
    { 
        // Warm the player if in range
        if (isTimeToWarm())
        {
            if (playerInRange() && hasFuel() && Input.GetKey(KeyCode.Space))
            {
                player.increaseWarmth(warmthStrength);
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

        UpdateFuel();
    }

    private void UpdateFuel()
    {
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

    public void AddFuel(int f)
    {
        fuel += f;
        heaterFuelBar.SetFuel((int)fuel);
    }

    public bool hasFuel()
    {
        return fuel > 0;
    }
}
