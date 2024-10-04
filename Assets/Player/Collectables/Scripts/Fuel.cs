using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour, PrefabID
{
    public static List<GameObject> fuels = new List<GameObject>();

    public static int totalFuelCollected = 0;

    public int id { get; set; }
}
