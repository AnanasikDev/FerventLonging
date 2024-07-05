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
    public float width;
    public Vector2 outDirection;
    public bool isConnected = false;
}