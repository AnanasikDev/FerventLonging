using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int prefabId;

    public List<RoomEntrance> entrances;

    public Vector2 size;
    public Bounds bounds { get { return new Bounds(transform.position.NullZ(), size.ConvertTo3D()); } }

    [Tooltip("Areas where props, fuel and enemies will be spawned")] public Area[] fillinAreas;

    public int id;

    private void OnDrawGizmos()
    {
        foreach (var entrance in entrances)
        {
            Gizmos.color = entrance.connectedEntrance != null ? Color.yellow : Color.red;

            Gizmos.DrawWireCube(transform.position + entrance.localPosition.ConvertTo3D(), entrance.width * Vector3.one);
            Gizmos.DrawLine(transform.position + entrance.localPosition.ConvertTo3D(), transform.position + entrance.localPosition.ConvertTo3D() + entrance.outDirection.ConvertTo3D());

        }
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        foreach (var area in fillinAreas)
        {
            if (area.areaType == AreaType.Fuel) Gizmos.color = Color.green;
            if (area.areaType == AreaType.Props) Gizmos.color = Color.cyan;
            if (area.areaType == AreaType.Enemies) Gizmos.color = new Color(255, 64, 0);
            Gizmos.DrawWireCube(area.bounds.center + transform.position, area.bounds.size);
        }
    }
}

[Serializable]
public class Area
{
    public Bounds bounds;
    public AreaType areaType;
    public bool isEmpty = true;
}

public enum AreaType
{
    Props,
    Fuel,
    Enemies
}