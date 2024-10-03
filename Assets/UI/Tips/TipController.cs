using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TipController : MonoBehaviour
{
    [ReadOnly][SerializeField] private List<Tip> tips = new List<Tip>();
    [SerializeField] private List<Sprite> sprites;

    [SerializeField] private Tip tipPrefab;

    public float tipSizeFactor = 0.4f;

    public void Init()
    {
        AddTip(tipType.collectFuel, sprites[0], 
            () =>
            {
                GameObject fuel = Fuel.fuels.FirstOrDefault(f => f && Scripts.Player.transform.position.SqrDistanceXY(f.transform.position) < 10f);
                if (fuel != null)
                {
                    return (true, WorldToCanvas(Vector2.Lerp(fuel.transform.position.WithZ(0), Scripts.Player.transform.position, 0.2f)));
                }
                return (false, Vector2.zero);
            }
            );

        AddTip(tipType.putFuel, sprites[1],
            () =>
            {
                if (PlayerInteraction.collectedFuel != 0 && Scripts.Heater.transform.position.SqrDistanceXY(Scripts.Player.transform.position) < 25)
                {
                    return (true, WorldToCanvas(Vector2.Lerp(Scripts.Heater.transform.position.WithZ(0), Scripts.Player.transform.position, 0.2f)));
                }
                return (false, Vector2.zero);
            }
            );

        AddTip(tipType.takeHeat, sprites[2],
            () =>
            {
                if (Scripts.Player.playerWarmth.relativeWarmth < 0.7f && Scripts.Heater.transform.position.SqrDistanceXY(Scripts.Player.transform.position) < 25)
                {
                    return (true, WorldToCanvas(Vector2.Lerp(Scripts.Heater.transform.position.WithZ(0), Scripts.Player.transform.position, 0.45f)));
                }
                return (false, Vector2.zero);
            }
            );
    }
    
    private void AddTip(tipType type, Sprite sprite, Func<(bool, Vector2)> stateFunction)
    {
        var tip = Instantiate(tipPrefab);
        tip.transform.SetParent(Scripts.Canvas.transform);
        tip.tipType = type;
        tip.stateFunction = stateFunction;
        tip.SetSprite(sprite);
        tips.Add(tip);
    }

    private Vector2 WorldToCanvas(Vector3 vec)
    {
        return Camera.main.WorldToScreenPoint(vec);
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