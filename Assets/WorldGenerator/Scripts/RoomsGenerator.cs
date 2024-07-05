using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsGenerator : MonoBehaviour
{
    [SerializeField] private List<Room> roomPrefabs;
    [SerializeField] private int numberOfRooms = 40;
    
    [ReadOnly] public List<Room> generatedRooms = new List<Room>();

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

        int generatedNumber = 1;

        while (true)
        {
            if (generatedNumber >= numberOfRooms) break;

            if (queue.Count == 0) break;

            var room = queue.Dequeue();

            var randomRooms = roomPrefabs.OrderBy(x => Random.value).ToArray();

            foreach (var otherRoom in randomRooms)
            {
                int neighbours = room.CalculateNeighbours(otherRoom, out List<Vector2> positions, out List<RoomEntrance> thisEntrances, out List<RoomEntrance> otherEntrances);
                
                if (neighbours == 0) continue;

                for (int i = 0; i < neighbours; i++)
                {
                    var newRoom = Instantiate(otherRoom, positions[i].ConvertTo3D(), Quaternion.identity);
                    thisEntrances[i].isConnected = true;
                    otherEntrances[i].isConnected = true;
                    newRoom.UpdateEntrances(otherEntrances.ToArray());
                    room.UpdateEntrances(thisEntrances.ToArray());
                    
                    queue.Enqueue(newRoom);
                    generatedNumber++;
                }
            }
        }
    }
}
