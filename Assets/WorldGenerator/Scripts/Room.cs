using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Tooltip("Sprites of rooms with same behaviour but different appearance")]
    public Sprite[] randomSprites;

    public List<RoomEntrance> entrances;

    public PolygonCollider2D shapeObject;
    public static readonly int roomsLayerIndex = 3;

    private bool isSpaceFree(PolygonCollider2D collider)
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        contactFilter.layerMask = 1 << roomsLayerIndex;

        List<Collider2D> results = new List<Collider2D>();

        int i = collider.OverlapCollider(contactFilter, results);

        Debug.Log(i);

        return i == 0;
    }

    public int CalculateNeighbours(Room other, out List<Vector2> positions, out List<RoomEntrance> thisEntrances, out List<RoomEntrance> otherEntrances)
    {
        positions = new List<Vector2>();
        thisEntrances = new List<RoomEntrance>();
        otherEntrances = new List<RoomEntrance>();

        var randomEntrances = this.entrances.OrderBy(x => Random.value).ToList();

        foreach (var entrance in randomEntrances)
        {
            foreach (var otherEntrance in other.entrances)
            {
                if (otherEntrance.outDirection != -entrance.outDirection) continue;
                if (otherEntrance.width != entrance.width) continue;

                var reference = Scripts.RoomsGenerator.prefabToReference[other];
                reference.transform.position = (transform.position.ConvertTo2D() + entrance.localPosition) - otherEntrance.localPosition;
                reference.gameObject.SetActive(true);
                bool free = isSpaceFree(reference.shapeObject);
                reference.gameObject.SetActive(false);

                if (!free) continue;

                positions.Add((transform.position.ConvertTo2D() + entrance.localPosition) - otherEntrance.localPosition);
                thisEntrances.Add(entrance);
                otherEntrances.Add(otherEntrance);
            }
        }

        return positions.Count;
    }

    public void UpdateEntrances(RoomEntrance[] toUpdate)
    {
        for (int i = 0; i < toUpdate.Length; i++)
        {
            var e = entrances.Find(e => e.localPosition == toUpdate[i].localPosition);
            if (e == null) continue;

            e = toUpdate[i];
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var entrance in entrances)
        {
            Gizmos.color = entrance.isConnected ? Color.yellow : Color.red;
            //Gizmos.DrawCube(transform.position + entry.localPosition.ConvertTo3D(), entry.width / 2f * Vector3.one);
            Gizmos.DrawWireCube(transform.position + entrance.localPosition.ConvertTo3D(), entrance.width * Vector3.one);
        }
    }
}
