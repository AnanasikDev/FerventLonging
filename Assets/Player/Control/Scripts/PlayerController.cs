using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlayerWarmth playerWarmth { get; private set; }
    public PlayerInteraction playerInteraction { get; private set; }
    public PlayerAttack playerAttack { get; private set; }

    public void Init()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerWarmth = GetComponent<PlayerWarmth>();
        playerInteraction = GetComponent<PlayerInteraction>();
        playerAttack = GetComponent<PlayerAttack>();

        playerMovement.Init();
        playerWarmth.Init();
        playerAttack.Init();
    }

    private void Update()
    {
        playerInput.UpdateInput();
        playerWarmth.UpdateWarmth();
        playerInteraction.UpdateInteraction();
        playerAttack.UpdateAttack();
    }

    private void FixedUpdate()
    {
        playerMovement.UpdateMovement();
    }
}