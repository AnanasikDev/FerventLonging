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

    private void Start()
    {
        Player = _Player;
        Player.Init();

        RoomsGenerator = _RoomsGenerator;
        RoomsGenerator.Init();
    }
}
