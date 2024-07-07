using NavMeshPlus.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    public int prefabId;

    public List<RoomEntrance> entrances;

    public Vector2 size;
    public Bounds bounds { get { return new Bounds(transform.position.NullZ(), size.ConvertTo3D()); } }

    [Tooltip("Areas where props, fuel and enemies will be spawned")] public Area[] fillinAreas;

    //[SerializeField] private GameObject collidersHandler;

    public int id;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void Init()
    {
        var obstacle = gameObject.AddComponent<NavMeshModifier>();
        obstacle.overrideArea = true;
        obstacle.area = NavMesh.GetAreaFromName("Not Walkable");
    }

    public void AddSpawnedObject(GameObject spawnedObject)
    {
        spawnedObjects.Add(spawnedObject);
        spawnedObject.transform.SetParent(transform);
    }

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

        if (fillinAreas != null)
        foreach (var area in fillinAreas)
        {
            if (area.areaType == AreaType.Fuel) Gizmos.color = Color.green;
            if (area.areaType == AreaType.Props) Gizmos.color = Color.cyan;
            if (area.areaType == AreaType.Enemies) Gizmos.color = new Color(255, 64, 0);
            Gizmos.DrawWireCube(area.bounds.center + transform.position, area.bounds.size);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i].gameObject.CompareTag("Fuel"))
                Fuel.fuels.Remove(spawnedObjects[i]);
            Destroy(spawnedObjects[i]);
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