using NaughtyAttributes;
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
    [ReadOnly] public Bounds bounds;

    [Tooltip("Areas where props, fuel and enemies will be spawned")] public Area[] fillinAreas;

    //[SerializeField] private GameObject collidersHandler;

    public int id;
    public static int _id = 0;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<BoxCollider2D> gapsFillers = new List<BoxCollider2D>();

    public static event Action onAnyRoomDisabledEvent;
    public static event Action onAnyRoomEnabledEvent;

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
            filler.transform.position -= entrance.outDirection.ConvertTo3D() / 2f * entrance.depth / 32f;
            
            // set up collisions
            var collider = filler.AddComponent<BoxCollider2D>();
            Vector2 size = new Vector2(
                entrance.outDirection.x == 0 ? entrance.width : entrance.depth / 32f,
                entrance.outDirection.y == 0 ? entrance.width : entrance.depth / 32f
                );
            filler.transform.localScale = size.ConvertTo3D();
            collider.offset = Vector2.zero;

            // set up rendering
            var renderer = filler.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = -1;
            Texture2D tex = Texture2D.whiteTexture; // not black because alpha must be 1.0
            Sprite blankWhiteSquare = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4);
            renderer.sprite = blankWhiteSquare;
            renderer.color = Color.black;
            
            gapsFillers.Add(collider);
            collider.gameObject.SetActive(false);
        }
    }

    public void CalculateBounds()
    {
        bounds = new Bounds(transform.position.NullZ(), size.ConvertTo3D());
    }

    public void Enable()
    {
        CalculateBounds();
        gameObject.SetActive(true);
        onAnyRoomEnabledEvent += FillGaps;
        onAnyRoomDisabledEvent += FillGaps;
        FillGaps();
        foreach (var obj in spawnedObjects)
            obj.transform.position = obj.transform.position.WithZ(obj.transform.position.y / 100f);

        onAnyRoomEnabledEvent?.Invoke();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        onAnyRoomDisabledEvent -= FillGaps;
        onAnyRoomEnabledEvent -= FillGaps;
        gameObject.name = "spare in pool";
        foreach (var e in entrances)
        {
            if (e.connectedEntrance != null) e.connectedEntrance.connectedEntrance = null;
            e.connectedEntrance = null;
        }

        onAnyRoomDisabledEvent?.Invoke();
    }

    /// <summary>
    /// Fills all unconnected entrances with colliders
    /// </summary>
    private void FillGaps()
    {
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