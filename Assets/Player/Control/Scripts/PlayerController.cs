using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlayerWarmth playerWarmth { get; private set; }

    public void Init()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWarmth = GetComponent<PlayerWarmth>();

        playerWarmth.Init();
    }

    private void Update()
    {
        playerInput.UpdateInput();
        playerWarmth.UpdateWarmth();
    }

    private void FixedUpdate()
    {
        playerMovement.UpdateMovement();
    }
}