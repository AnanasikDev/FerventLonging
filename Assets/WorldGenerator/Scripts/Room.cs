using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Tooltip("Sprites of rooms with same behaviour but different appearance")]
    public Sprite[] randomSprites;

    public List<RoomEntrance> entrances;

    public PolygonCollider2D shapeObject;

    private void OnDrawGizmos()
    {
        foreach (var entrance in entrances)
        {
            Gizmos.color = entrance.isConnected ? Color.yellow : Color.red;

            Gizmos.DrawWireCube(transform.position + entrance.localPosition.ConvertTo3D(), entrance.width * Vector3.one);
        }
    }
}
