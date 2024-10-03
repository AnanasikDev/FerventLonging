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

    [SerializeField][Required] private TipController _TipController;
    public static TipController TipController;

    [SerializeField][Required] private Canvas _Canvas;
    public static Canvas Canvas;

    private void Start()
    {
        Heater = _Heater;
        Player = _Player;
        RoomsGenerator = _RoomsGenerator;
        TipController = _TipController;
        Canvas = _Canvas;

        Player.Init();
        Heater.Init();
        RoomsGenerator.Init();
        TipController.Init();
    }
}
