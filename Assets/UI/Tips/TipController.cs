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

    public void Init()
    {
        //Assert.AreEqual(sprites.Count, Enum.GetNames(typeof(tipType)).Length, "Number of sprites is not equal the number of defined types of tips.");

        AddTip(tipType.collectFuel, sprites[0], 
            () =>
            {
                GameObject fuel = Fuel.fuels.FirstOrDefault(f => Scripts.Player.transform.position.SqrDistanceXY(f.transform.position) < 10f);
                if (fuel != null)
                {
                    return (true, WorldToCanvas(fuel.transform.position.WithZ(0)));
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
    pullCart,
    takeHeat,
    emitHeat
}

class TipData
{
    public tipType tipType;
    public Sprite sprite;
}