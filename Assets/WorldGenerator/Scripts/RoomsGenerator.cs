using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private List<Room> roomPrefabs;
    private int roomPrefabNumber;
    [SerializeField] private int numberOfRooms = 40;
    [ShowNonSerializedField][ReadOnly]
    private int generatedAmount = 0;
    private Pool<Room> roomPool;

    [Tooltip("If true, children must be different from the parent")][SerializeField] 
    private bool doNotRepeateParent = true;

    [ReadOnly] public List<Room> generatedRooms = new List<Room>();

    [SerializeField] private Transform player;
    [SerializeField] public float distanceGenerateThreshold;
    [SerializeField] public float distanceDestroyThreshold;

    [SerializeField] private List<Prop> propsPrefabs;
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

    public static event System.Action onNavMeshUpdatedEvent;

    public void Init()
    {
        if (!isEnabled) return;

        roomPrefabNumber = roomPrefabs.Count;
        roomPool = new Pool<Room>(r => r.gameObject.activeSelf);

        InitNavMesh();

        generatedAmount = 0;

        for (int i = 0; i < propsPrefabs.Count; i++)
            propsPrefabs[i].id = i;

        for (int i = 0; i < fuelPrefabs.Count; i++)
            fuelPrefabs[i].id = i;

        for (int i = 0; i < enemyPrefabs.Count; i++)
            enemyPrefabs[i].id = i;

        foreach (var prefab in roomPrefabs)
        {
            var reference = Instantiate(prefab, Vector2.one * 100, Quaternion.identity, referenceHandler);
            reference.gameObject.SetActive(false);
            reference.gameObject.layer = 0;
            prefabToReference.Add(prefab, reference);
        }

        Generate();
        foreach (var room in roomPool.objects)
        {
            if (room.gameObject.activeSelf) room.FillGaps();
        }
    }
    private void InitNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    void generateOnInit(Queue<Room> queue)
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

        generateOnInit(queue);
    }

    private void FixedUpdate()
    {
        if (!isEnabled) return;
        UpdateProcedural();
    }
    private void UpdateNavMesh()
    {
        //navMeshSurfaceCollider.offset = player.position;
        AsyncOperation op = navMeshSurface.BuildNavMeshAsync();
        op.completed += (AsyncOperation asOp) => onNavMeshUpdatedEvent?.Invoke();
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
            room.Disable();
            //Destroy(room.gameObject);
        }

        if (change > 0 || change - generatedAmount > 0)
            UpdateNavMesh();
    }

    private void Generate()
    {
        Queue<Room> queue = new Queue<Room>();

        // 0 - 2 for the reason of creating the
        // first room big enough for the heater to fit
        Room first = Instantiate(roomPrefabs[Random.Range(0, 2)], Vector2.zero, Quaternion.identity);
        first.name += $" [INITIAL]";
        first.transform.SetParent(transform);
        generatedAmount++;
        first.Init();
        roomPool.RecordNew(first);
        first.Enable();
        generatedRooms.Add(first);
        queue.Enqueue(first);

        generatedAmount = 1;

        generateOnInit(queue);
    }
    private List<Room> GenerateNeighbours(Room parent)
    {
        List<Room> result = new List<Room>();

        foreach (RoomEntrance entrance in parent.entrances)
        {
            var newRoom = GenerateForEntrance(entrance, parent);
            if (newRoom != null)
            {
                newRoom.Enable();
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

        int i = 0;
        while (true)
        {
            Room prefab = roomPrefabs.RandomElement();
            foreach (RoomEntrance otherEntrance in prefab.entrances)
            {
                if (doNotRepeateParent && parent.prefabId == prefab.prefabId) continue; 

                if (otherEntrance.outDirection != -entrance.outDirection) continue;
                if (otherEntrance.width != entrance.width) continue;

                var reference = Scripts.RoomsGenerator.prefabToReference[prefab];

                Vector2 position = (parent.transform.position.ConvertTo2D() + entrance.localPosition) - otherEntrance.localPosition;

                bool free = IsSpaceFree(reference, position);

                if (!free) continue;

                Room newRoom;
                if (!roomPool.TryTakeInactive(out newRoom, r => r.prefabId == prefab.prefabId))
                {
                    newRoom = Instantiate(prefab, transform);
                    newRoom.prefabId = prefab.prefabId;
                    newRoom.Init();
                    roomPool.RecordNew(newRoom);
                }
                newRoom.transform.position = position;
                newRoom.id = generatedAmount;

                var newOtherEntrance = newRoom.entrances.Find(e => e.localPosition == otherEntrance.localPosition);

                parent.SetEntranceConnection(entrance, newOtherEntrance);
                newRoom.SetEntranceConnection(newOtherEntrance, entrance);

                return newRoom;
            }
            i++;
            if (i > roomPrefabNumber - 1) break;
        }

        return null;
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
                        var propPrefab = propsPrefabs.RandomElement();

                        // try pull out of the pool
                        var prop = World.props.FirstOrDefault(e => e.gameObject.activeSelf && e.id == propPrefab.id); // if needed can be checked to match desired prefab
                        if (prop == null)
                            prop = Instantiate(propPrefab);

                        prop.transform.position = pos + room.transform.position.ConvertTo2D();
                        prop.transform.position = prop.transform.position.WithZ(prop.transform.position.y / 100f);
                        room.AddSpawnedObject(prop.gameObject);
                    }
                }
            }

            if (area.areaType == AreaType.Enemies)
            {
                if (enemyPrefabs.Count != 0)
                {
                    Vector2 position = area.bounds.GenerateRandomPositionWithinBounds();

                    // try pull out of the pool
                    var enemy = World.enemies.FirstOrDefault(e => e.gameObject.activeSelf); // if needed can be checked to match desired prefab
                    if (enemy == null) 
                        enemy = Instantiate(enemyPrefabs.RandomElement());

                    enemy.transform.position = position + room.transform.position.ConvertTo2D();
                    enemy.transform.position = enemy.transform.position.WithZ(enemy.transform.position.y / 100f);
                    enemy.Init();
                    room.AddSpawnedObject(enemy.gameObject);
                }
            }

            if (area.areaType == AreaType.Fuel)
            {
                if (fuelPrefabs.Count != 0)
                {
                    Vector2 position = area.bounds.GenerateRandomPositionWithinBounds();

                    // try pull out of the pool
                    var fuel = World.fuels.FirstOrDefault(f => f.gameObject.activeSelf); // can be chosed fuel of the same type or prefab
                    if (fuel == null) fuel = Instantiate(fuelPrefabs.RandomElement());

                    fuel.transform.position = position + room.transform.position.ConvertTo2D();
                    fuel.transform.position = fuel.transform.position.WithZ(fuel.transform.position.y / 100f);
                    room.AddSpawnedObject(fuel.gameObject);
                    Fuel.fuels.Add(fuel.gameObject);
                }
            }

            area.isEmpty = false;
        }
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
    private bool IsSpaceFree(Room referenceRoom, Vector2 position)
    {
        referenceRoom.transform.position = position;
        referenceRoom.CalculateBounds();

        foreach (var room in generatedRooms)
        {
            if (DoBoundsIntersect(room.bounds, referenceRoom.bounds)) return false;
        }
        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.transform.position, distanceGenerateThreshold);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.transform.position, distanceDestroyThreshold);
    }
}
