using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TipController : MonoBehaviour
{
    [ReadOnly][SerializeField] private List<Tip> tips = new List<Tip>();
    [SerializeField] private List<Sprite> sprites;

    [SerializeField] private Tip tipPrefab;

    public float tipSizeFactor = 0.4f;

    public void Init()
    {
        // Collect fuel tip
        AddTip(tipType.collectFuel, sprites[0], 
            () =>
            {
                GameObject fuel = Fuel.fuels.FirstOrDefault(f => f && Scripts.Player.transform.position.DistanceXY(f.transform.position) < Scripts.Player.playerInteraction.collectionFuelDistance);
                if (fuel != null)
                {
                    return (true, Vector2.Lerp(fuel.transform.position.WithZ(0), Scripts.Player.transform.position, 0.2f));
                }
                return (false, Vector2.zero);
            }
            );

        // Put fuel tip
        AddTip(tipType.putFuel, sprites[1],
            () =>
            {
                if (Scripts.Player.playerInteraction.collectedFuel != 0 && Scripts.Heater.transform.position.DistanceXY(Scripts.Player.transform.position) < Scripts.Player.playerInteraction.putFuelDistance)
                {
                    return (true, Vector2.Lerp(Scripts.Heater.transform.position.WithZ(0), Scripts.Player.transform.position, 0.2f));
                }
                return (false, Vector2.zero);
            }
            );

        // Take heat tip
        AddTip(tipType.takeHeat, sprites[2],
            () =>
            {
                if (Scripts.Player.playerWarmth.relativeWarmth < 0.7f && Scripts.Heater.transform.position.DistanceXY(Scripts.Player.transform.position) < Scripts.Heater.heatRadius)
                {
                    return (true, Vector2.Lerp(Scripts.Heater.transform.position.WithZ(0), Scripts.Player.transform.position, 0.45f));
                }
                return (false, Vector2.zero);
            }
            );
    }
    
    private void AddTip(tipType type, Sprite sprite, Func<(bool, Vector2)> stateFunction)
    {
        var tip = Instantiate(tipPrefab);
        tip.transform.SetParent(transform);
        tip.tipType = type;
        tip.stateFunction = stateFunction;
        tip.SetSprite(sprite);
        tip.Disable();
        tips.Add(tip);
    }

    private void Update()
    {
        foreach (var tip in tips)
        {
            tip.UpdateTip();
        }
    }
}

public enum tipType
{
    collectFuel,
    putFuel,
    takeHeat,
    emitHeat,
    pullCart
}

class TipData
{
    public tipType tipType;
    public Sprite sprite;
}