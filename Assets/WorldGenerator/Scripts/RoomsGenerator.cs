using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private List<Room> roomPrefabs;
    [SerializeField] private int numberOfRooms = 40;
    
    [ReadOnly] public List<Room> generatedRooms = new List<Room>();

    public static readonly int roomsLayerIndex = 3;

    /// <summary>
    /// A dictionary holding prefab rooms as keys
    /// and their in-scene instances as values.
    /// Needed for quick physics calculations.
    /// </summary>
    public Dictionary<Room, Room> prefabToReference = new Dictionary<Room, Room>();

    public void Init()
    {
        foreach (var room in roomPrefabs)
        {
            var reference = Instantiate(room, Vector2.zero, Quaternion.identity, transform);
            reference.gameObject.SetActive(false);
            prefabToReference.Add(room, reference);
        }

        Generate();
    }

    private void Generate()
    {
        Queue<Room> queue = new Queue<Room>();
        var first = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], Vector2.zero, Quaternion.identity);
        queue.Enqueue(first);

        int number = 1;

        while (true)
        {
            if (number >= numberOfRooms) break;

            if (queue.Count == 0) break;

            var room = queue.Dequeue();

            var newRooms = GenerateNeighbours(room);
            foreach (var newRoom in newRooms)
            {
                number++;
                queue.Enqueue(newRoom);
            }
        }
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
            }
        }

        return result;
    }

    private Room GenerateForEntrance(RoomEntrance entrance, Room parent)
    {
        if (entrance.isConnected) return null;

        var rooms = roomPrefabs.Shuffle();

        foreach (Room other in rooms)
        {
            foreach (RoomEntrance otherEntrance in other.entrances)
            {
                if (otherEntrance.outDirection != -entrance.outDirection) continue;
                if (otherEntrance.width != entrance.width) continue;

                var reference = Scripts.RoomsGenerator.prefabToReference[other];

                Vector2 position = (parent.transform.position.ConvertTo2D() + entrance.localPosition) - otherEntrance.localPosition;

                bool free = IsSpaceFree(reference.shapeObject, position);

                if (!free) continue;

                Room newRoom = Instantiate(other, position, Quaternion.identity);
                newRoom.entrances.Find(e => e.localPosition == otherEntrance.localPosition).isConnected = true;
                entrance.isConnected = true;

                return newRoom;
            }
        }

        return null;
    }

    private bool IsSpaceFree(PolygonCollider2D collider, Vector2 position)
    {
        collider.gameObject.SetActive(true);
        collider.transform.parent.gameObject.SetActive(true);
        collider.transform.position = position;
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        contactFilter.layerMask = 1 << roomsLayerIndex;

        List<Collider2D> results = new List<Collider2D>();

        int i = collider.OverlapCollider(contactFilter, results);
        collider.gameObject.SetActive(false);
        collider.transform.parent.gameObject.SetActive(false);

        Debug.Log(i);

        return i == 0;
    }
}
