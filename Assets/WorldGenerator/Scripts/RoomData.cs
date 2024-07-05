using UnityEngine;

[CreateAssetMenu(fileName="Room_", menuName="RoomData")]
public class RoomData : ScriptableObject
{
    [Tooltip("Sprites of rooms with same behaviour but different appearance")]
    public Sprite[] randomSprites;

    public RoomEntrance[] entrances;
}

[System.Serializable]
public struct RoomEntrance
{
    public Vector2 localPosition;
    public float width;
    public Vector2 outDirection;
}