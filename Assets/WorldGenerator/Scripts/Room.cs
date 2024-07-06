using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int prefabId;

    [Tooltip("Sprites of rooms with same behaviour but different appearance")]
    public Sprite[] randomSprites;

    public List<RoomEntrance> entrances;

    public Vector2 size;
    public Bounds bounds { get { return new Bounds(transform.position.NullZ(), size.ConvertTo3D()); } }

    public int id;

    private void OnDrawGizmos()
    {
        foreach (var entrance in entrances)
        {
            Gizmos.color = entrance.connectedEntrance != null ? Color.yellow : Color.red;

            Gizmos.DrawWireCube(transform.position + entrance.localPosition.ConvertTo3D(), entrance.width * Vector3.one);
            Gizmos.DrawLine(transform.position + entrance.localPosition.ConvertTo3D(), transform.position + entrance.localPosition.ConvertTo3D() + entrance.outDirection.ConvertTo3D());

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
