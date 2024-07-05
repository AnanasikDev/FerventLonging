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

    private void Start()
    {
        RoomsGenerator = _RoomsGenerator;
        RoomsGenerator.Init();
    }
}
