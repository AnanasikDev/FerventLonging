using NaughtyAttributes;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField][ReadOnly] private bool isPullingCart = false;
    [SerializeField] private float pullingForce = 10;
    [SerializeField] private float pullingDistance = 15;

    private int collectedFuel = 0;
    [SerializeField] private float collectionFuelDistance = 4;
    [SerializeField] private float putFuelDistance = 4;
    [SerializeField] private int fuelWarmth = 7;

    [SerializeField] private TextMeshProUGUI collectedFuelText;
    [SerializeField] private string collectedFuelTextFormat;

    private float minClickDelay = 0.2f;
    private float lastTime;

    private Vector2 target;

    public void Init()
    {
        lastTime = Time.time;
    }
    
    public void UpdateInteraction()
    {
        PullHeater();
        CollectFuel();
        PutFuelIntoHeater();
    }

    private void PullHeater()
    {
        if (Scripts.Player.playerInput.isTogglePullCartPressed)
        {
            if (Time.time - lastTime > minClickDelay)
            {
                if (isPullingCart || (transform.position - Scripts.Heater.transform.position).magnitude >= pullingDistance)
                {
                    isPullingCart = false;
                    Scripts.Heater.rigidbody.velocity = Vector3.zero;
                    Scripts.Player.playerMovement.SetSpeed(Scripts.Player.playerMovement.walkingSpeed);
                }
            
                else if (!isPullingCart && (transform.position - Scripts.Heater.transform.position).magnitude < pullingDistance)
                {
                    isPullingCart = true;
                    Scripts.Player.playerMovement.SetSpeed(Scripts.Player.playerMovement.pullingSpeed);
                }

                lastTime = Time.time;
            }
        }

        if (isPullingCart)
        {
            target = transform.position - Scripts.Heater.heaterRenderer.handlerPosition.ConvertTo3D();
            Vector2 shift = Scripts.Heater.heaterRenderer.handlerPosition - Scripts.Heater.transform.position.ConvertTo2D();
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;

            float roundedAngle = 360 - Scripts.Heater.heaterRenderer.step * Mathf.Repeat(Mathf.RoundToInt(angle / -Scripts.Heater.heaterRenderer.step), Scripts.Heater.heaterRenderer.numberOfSteps);

            Vector2 relativeShift = new Vector2(Mathf.Cos(roundedAngle * Mathf.Deg2Rad), Mathf.Sin(roundedAngle * Mathf.Deg2Rad)) * shift.magnitude;

            Debug.Log(relativeShift);

            float d = (transform.position - Scripts.Heater.transform.position).magnitude;
            if (d < 6)
            {
                Scripts.Heater.transform.position += relativeShift.ConvertTo3D() * 0.0125f;
            }
        }
    }

    private void CollectFuel()
    {
        if (!Scripts.Player.playerInput.isCollectFuelPressed) return;

        GameObject fuel = Fuel.fuels.Where(f => f && (f.transform.position - transform.position).sqrMagnitude < collectionFuelDistance * collectionFuelDistance).FirstOrDefault();

        if (fuel != null)
        {
            Destroy(fuel.gameObject);
            collectedFuel++;
            collectedFuelText.text = string.Format(collectedFuelTextFormat, collectedFuel);
        }
    }
    private bool PutFuelIntoHeater()
    {
        if (!Scripts.Player.playerInput.isPutFuelPressed) return false;

        // too far away from the heater
        if ((transform.position - Scripts.Heater.transform.position).sqrMagnitude > putFuelDistance * putFuelDistance) return false;

        Scripts.Heater.AddFuel(collectedFuel * fuelWarmth);
        collectedFuel = 0;
        collectedFuelText.text = string.Format(collectedFuelTextFormat, collectedFuel);

        return true;
    }
}