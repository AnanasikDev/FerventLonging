using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private List<Room> roomPrefabs;
    [SerializeField] private int numberOfRooms = 40;
    [ShowNonSerializedField][ReadOnly]
    private int generatedAmount = 0;

    [ReadOnly] public List<Room> generatedRooms = new List<Room>();

    public static readonly int roomsLayerIndex = 3;


    public bool intersect = false;
    public Room r1;
    public Room r2;

    /// <summary>
    /// A dictionary holding prefab rooms as keys
    /// and their in-scene instances as values.
    /// Needed for quick physics calculations.
    /// </summary>
    public Dictionary<Room, Room> prefabToReference = new Dictionary<Room, Room>();

    public void Init()
    {
        generatedAmount = 0;

        foreach (var room in roomPrefabs)
        {
            var reference = Instantiate(room, Vector2.zero, Quaternion.identity, transform);
            reference.gameObject.SetActive(false);
            prefabToReference.Add(room, reference);
        }

        //Instantiate(roomPrefabs[0]);
        //Debug.Log(IsSpaceFree(prefabToReference[roomPrefabs[1]].shapeObject, Vector2.zero, 0));



        Generate();

        CalculateConnections();
    }

    private void Update()
    {
        if (r1 == null || r2 == null) return;
        Debug.Log("Internal: " + r1.boundaries.bounds.Intersects(r2.boundaries.bounds));
        intersect = DoBoundsIntersect(r1.boundaries.bounds, r2.boundaries.bounds);
        Debug.Log("Custom: " + intersect);
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

        StartCoroutine(gen(first, queue));
    }

    IEnumerator gen(Room room, Queue<Room> queue)
    {
        if (generatedAmount >= numberOfRooms) yield break;

        if (queue.Count == 0) yield break;

        var newRooms = GenerateNeighbours(room);
        foreach (var newRoom in newRooms)
        {
            newRoom.name += $" [{generatedAmount}] (parent:{room.id})";
            generatedAmount++;
            queue.Enqueue(newRoom);
        }

        yield return new WaitForFixedUpdate(); //new WaitForSeconds(2);
        var nextRoom = queue.Dequeue();
        yield return gen(nextRoom, queue);
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
                if (otherEntrance.outDirection != -entrance.outDirection) continue;
                if (otherEntrance.width != entrance.width) continue;

                var reference = Scripts.RoomsGenerator.prefabToReference[other];

                //Debug.Log(entrance.localPosition.ToString() + "  /  " + otherEntrance.localPosition);

                Vector2 position = (parent.transform.position.ConvertTo2D() + entrance.localPosition) - otherEntrance.localPosition;

                bool free = IsSpaceFree(reference, position, parent.id);

                if (!free) continue;

                Room newRoom = Instantiate(other);
                newRoom.transform.position = position;
                newRoom.id = generatedAmount;
                //newRoom.entrances.Find(e => e.localPosition == otherEntrance.localPosition).isConnected = true;
                //entrance.connectedEntrance = true;

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
            if (room == referenceRoom) continue;

            //if (room.boundaries.bounds.Intersects(referenceRoom.boundaries.bounds)) return false;
            if (DoBoundsIntersect(room.boundaries.bounds, referenceRoom.boundaries.bounds)) return false;
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
}
