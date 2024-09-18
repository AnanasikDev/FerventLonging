using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName="Room_", menuName="RoomData")]
public class RoomData : ScriptableObject
{
    [Tooltip("Sprites of rooms with same behaviour but different appearance")]
    public Sprite[] randomSprites;

    public List<RoomEntrance> entrances;
}

[System.Serializable]
public class RoomEntrance
{
    public Vector2 localPosition;
    [Tooltip("Width of the entrance, defines max width of object that can be fit into it. In Unity units.")] public float width;
    [Tooltip("Depth of the entrance, usually equals depth of walls to its sides. In pixels.")] public float depth;
    public Vector2 outDirection;
    public RoomEntrance connectedEntrance;
}
