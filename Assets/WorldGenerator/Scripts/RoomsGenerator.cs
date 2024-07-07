using NaughtyAttributes;
using NavMeshPlus.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private List<Room> roomPrefabs;
    [SerializeField] private int numberOfRooms = 40;
    [ShowNonSerializedField][ReadOnly]
    private int generatedAmount = 0;

    [Tooltip("If true, children must be different from the parent")][SerializeField] 
    private bool doNotRepeateParent = true;

    [ReadOnly] public List<Room> generatedRooms = new List<Room>();

    [SerializeField] private Transform player;
    [SerializeField] public float distanceGenerateThreshold;
    [SerializeField] public float distanceDestroyThreshold;

    [SerializeField] private List<GameObject> propsPrefabs;
    [SerializeField] private List<Fuel> fuelPrefabs;
    [SerializeField] private List<EnemyController> enemyPrefabs;

    [SerializeField] private NavMeshPlus.Components.NavMeshSurface navMeshSurface;
    [SerializeField] private Transform referenceHandler;

    /// <summary>
    /// A dictionary holding prefab rooms as keys
    /// and their in-scene instances as values.
    /// Needed for quick physics calculations.
    /// </summary>
    public Dictionary<Room, Room> prefabToReference = new Dictionary<Room, Room>();

    public void Init()
    {
        if (!isEnabled) return;

        InitNavMesh();

        generatedAmount = 0;

        foreach (var room in roomPrefabs)
        {
            var reference = Instantiate(room, Vector2.one * 100, Quaternion.identity, referenceHandler);
            reference.gameObject.SetActive(false);
            reference.gameObject.layer = 0;
            prefabToReference.Add(room, reference);
        }

        Generate();

        //CalculateConnections();
    }

    private void InitNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    private void UpdateNavMesh()
    {
        //navMeshSurfaceCollider.offset = player.position;
        navMeshSurface.BuildNavMeshAsync();
    }

    private void FixedUpdate()
    {
        if (!isEnabled) return;
        UpdateProcedural();
    }

    private void UpdateProcedural()
    {
        int change = generatedAmount;

        var roomsToGenerate = generatedRooms.Where(room => (room.transform.position - player.transform.position).magnitude < distanceGenerateThreshold).ToList();

        foreach (var room in roomsToGenerate)
        {
            GenerateNeighbours(room);
        }

        change = generatedAmount - change;

        var roomsToDestroy = generatedRooms.Where(room => (room.transform.position - player.transform.position).magnitude > distanceDestroyThreshold).ToList();

        foreach (var room in roomsToDestroy)
        {
            generatedAmount--;
            generatedRooms.Remove(room);
            Destroy(room.gameObject);
        }

        if (change > 0)
            UpdateNavMesh();
    }

    public static bool DoBoundsIntersect(Bounds bounds1, Bounds bounds2)
    {
        // Check overlap in the x-axis
        bool xOverlap = bounds1.min.x < bounds2.max.x && bounds1.max.x > bounds2.min.x;

        // Check overlap in the y-axis
        bool yOverlap = bounds1.min.y < bounds2.max.y && bounds1.max.y > bounds2.min.y;

        // If there's overlap in all three axes, the bounds intersect
        return xOverlap && yOverlap;
    }

    private void Generate()
    {
        Queue<Room> queue = new Queue<Room>();

        // 0 - 2 for the reason of creating the
        // first room big enough for the heater to fit
        var first = Instantiate(roomPrefabs[Random.Range(0, 2)], Vector2.zero, Quaternion.identity);
        first.name += $" [{generatedAmount}]";
        first.Init();
        generatedRooms.Add(first);
        queue.Enqueue(first);

        generatedAmount = 1;

        gen(queue);
    }

    void gen(Queue<Room> queue)
    {
        if (generatedAmount >= numberOfRooms) return;

        if (queue.Count == 0) return;
        var room = queue.Dequeue();

        var newRooms = GenerateNeighbours(room);
        foreach (var newRoom in newRooms)
        {
            newRoom.name += $" [{generatedAmount}] (parent:{room.id})";
            generatedAmount++;
            queue.Enqueue(newRoom);
        }

        gen(queue);
    }

    private List<Room> GenerateNeighbours(Room parent)
    {
        List<Room> result = new List<Room>();

        foreach (RoomEntrance entrance in parent.entrances)
        {
            var newRoom = GenerateForEntrance(entrance, parent);
            if (newRoom != null)
            {
                newRoom.Init();
                result.Add(newRoom);
                generatedRooms.Add(newRoom);
                generatedAmount++;
                FillRoomAreas(newRoom);
            }
        }

        return result;
    }

    private Room GenerateForEntrance(RoomEntrance entrance, Room parent)
    {
        if (entrance.connectedEntrance != null) return null;

        var rooms = roomPrefabs.Shuffle();

        foreach (Room other in rooms)
        {
            foreach (RoomEntrance otherEntrance in other.entrances)
            {
                if (doNotRepeateParent && parent.prefabId == other.prefabId) continue; 

                if (otherEntrance.outDirection != -entrance.outDirection) continue;
                if (otherEntrance.width != entrance.width) continue;

                var reference = Scripts.RoomsGenerator.prefabToReference[other];

                Vector2 position = (parent.transform.position.ConvertTo2D() + entrance.localPosition) - otherEntrance.localPosition;

                bool free = IsSpaceFree(reference, position, parent.id);

                if (!free) continue;

                Room newRoom = Instantiate(other, transform);
                newRoom.transform.position = position;
                newRoom.id = generatedAmount;

                return newRoom;
            }
        }

        return null;
    }

    private bool IsSpaceFree(Room referenceRoom, Vector2 position, int parentid)
    {
        referenceRoom.transform.position = position;

        foreach (var room in generatedRooms)
        {
            if (DoBoundsIntersect(room.bounds, referenceRoom.bounds)) return false;
        }
        return true;
    }

    private void FillRoomAreas(Room room)
    {
        foreach (var area in room.fillinAreas)
        {
            if (!area.isEmpty) continue;

            if (area.areaType == AreaType.Props)
            {
                if (propsPrefabs.Count != 0)
                {
                    Vector2[] positions = area.bounds.GenerateRandomPositionsWithinBounds(0.15f);
                    foreach (var pos in positions)
                    {
                        var prop = Instantiate(propsPrefabs.GetRandom());
                        prop.transform.position = pos + room.transform.position.ConvertTo2D();
                        prop.transform.position = prop.transform.position.SetZ(prop.transform.position.y / 100f);
                        room.AddSpawnedObject(prop);
                    }
                }
            }

            if (area.areaType == AreaType.Enemies)
            {
                if (enemyPrefabs.Count != 0)
                {
                    Vector2 position = area.bounds.GenerateRandomPositionWithinBounds();
                    var enemy = Instantiate(enemyPrefabs.GetRandom());
                    enemy.transform.position = position + room.transform.position.ConvertTo2D();
                    enemy.Init();
                    room.AddSpawnedObject(enemy.gameObject);
                }
            }

            if (area.areaType == AreaType.Fuel)
            {
                if (fuelPrefabs.Count != 0)
                {
                    Vector2 position = area.bounds.GenerateRandomPositionWithinBounds();
                    var fuel = Instantiate(fuelPrefabs.GetRandom());
                    fuel.transform.position = position + room.transform.position.ConvertTo2D();
                    room.AddSpawnedObject(fuel.gameObject);
                    Fuel.fuels.Add(fuel.gameObject);
                }
            }

            area.isEmpty = false;
        }
    }

    private void CalculateConnections()
    {
        foreach (Room r1 in generatedRooms)
        {
            foreach (Room r2 in generatedRooms)
            {
                if (r1 == r2) continue;

                foreach (RoomEntrance e1 in r1.entrances)
                {
                    foreach (RoomEntrance e2 in r2.entrances)
                    {
                        if (e1.outDirection != -e2.outDirection) continue;
                        if (e1.width != e2.width) continue;
                        if (r1.transform.position.ConvertTo2D() + e1.localPosition != r2.transform.position.ConvertTo2D() + e2.localPosition) continue;

                        e1.connectedEntrance = e2;
                        e2.connectedEntrance = e1;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.transform.position, distanceGenerateThreshold);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.transform.position, distanceDestroyThreshold);
    }
}
