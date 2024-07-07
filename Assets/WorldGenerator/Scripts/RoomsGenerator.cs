using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private List<Room> roomPrefabs;
    [SerializeField] private int numberOfRooms = 40;
    [ShowNonSerializedField][ReadOnly]
    private int generatedAmount = 0;
    [SerializeField] private bool isEnabled = true;

    [Tooltip("If true, children must be different from the parent")][SerializeField] 
    private bool doNotRepeateParent = true;

    [ReadOnly] public List<Room> generatedRooms = new List<Room>();

    [SerializeField] private Transform player;
    [SerializeField] public float distanceGenerateThreshold;
    [SerializeField] public float distanceDestroyThreshold;

    /// <summary>
    /// A dictionary holding prefab rooms as keys
    /// and their in-scene instances as values.
    /// Needed for quick physics calculations.
    /// </summary>
    public Dictionary<Room, Room> prefabToReference = new Dictionary<Room, Room>();

    public void Init()
    {
        if (!isEnabled) return;

        generatedAmount = 0;

        foreach (var room in roomPrefabs)
        {
            var reference = Instantiate(room, Vector2.zero, Quaternion.identity, transform);
            reference.gameObject.SetActive(false);
            prefabToReference.Add(room, reference);
        }

        Generate();

        CalculateConnections();
    }

    private void FixedUpdate()
    {
        if (!isEnabled) return;
        UpdateProcedural();
    }

    private void UpdateProcedural()
    {
        var roomsToGenerate = generatedRooms.Where(room => (room.transform.position - player.transform.position).magnitude < distanceGenerateThreshold).ToList();

        foreach (var room in roomsToGenerate)
        {
            GenerateNeighbours(room);
        }

        var roomsToDestroy = generatedRooms.Where(room => (room.transform.position - player.transform.position).magnitude > distanceDestroyThreshold).ToList();

        foreach (var room in roomsToDestroy)
        {
            generatedAmount--;
            generatedRooms.Remove(room);
            Destroy(room.gameObject);
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

    private void Generate()
    {
        Queue<Room> queue = new Queue<Room>();
        var first = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], Vector2.zero, Quaternion.identity);
        first.name += $" [{generatedAmount}]";
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
                result.Add(newRoom);
                generatedRooms.Add(newRoom);
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

                Room newRoom = Instantiate(other);
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
