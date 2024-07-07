using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Handling all managers/controller in the game
/// with ease of static referencing.
/// </summary>
public class Scripts : MonoBehaviour
{
    [SerializeField][Required] private RoomsGenerator _RoomsGenerator;
    public static RoomsGenerator RoomsGenerator;

    [SerializeField][Required] private PlayerController _Player;
    public static PlayerController Player;

    [SerializeField][Required] private HeaterBehavior _Heater;
    public static HeaterBehavior Heater;

    private void Start()
    {
        Heater = _Heater;
        Player = _Player;
        RoomsGenerator = _RoomsGenerator;

        Player.Init();
        Heater.Init();
        RoomsGenerator.Init();
    }
}
