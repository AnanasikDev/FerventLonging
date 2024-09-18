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
    public static int _id = 0;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<BoxCollider2D> gapsFillers = new List<BoxCollider2D>();

    public static event Action onAnyRoomDestroyedEvent;

    public void Init()
    {
        _id++;

        var obstacle = gameObject.AddComponent<NavMeshModifier>();
        obstacle.overrideArea = true;
        obstacle.area = NavMesh.GetAreaFromName("Not Walkable");
        
        gapsFillers = new List<BoxCollider2D>();
        foreach (var entrance in entrances)
        {
            GameObject filler = new GameObject();
            filler.transform.SetParent(transform);
            filler.transform.position = transform.position + entrance.localPosition.ConvertTo3D();
            var collider = filler.AddComponent<BoxCollider2D>();
            Vector2 size = new Vector2(
                entrance.outDirection.x == 0 ? entrance.width : 0.1f,
                entrance.outDirection.y == 0 ? entrance.width : 0.1f
                );
            collider.size = size;
            collider.offset = Vector2.zero;
            gapsFillers.Add(collider);
            collider.gameObject.SetActive(false);
        }
        FillGaps();
        onAnyRoomDestroyedEvent += FillGaps;
    }

    /// <summary>
    /// Fills all unconnected entrances with colliders
    /// </summary>
    private void FillGaps()
    {
        // if one entrance is smaller than another but is completely inside it, then they should be connected

        for (int i = 0; i < gapsFillers.Count; i++)
        {
            if (entrances[i].connectedEntrance != null)
                gapsFillers[i].gameObject.SetActive(false);
            else
                gapsFillers[i].gameObject.SetActive(true);
        }
    }

    public void SetEntranceConnection(RoomEntrance entrance, RoomEntrance other)
    {
        entrance.connectedEntrance = other;
        FillGaps();
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
        onAnyRoomDestroyedEvent -= FillGaps;
        onAnyRoomDestroyedEvent?.Invoke();
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