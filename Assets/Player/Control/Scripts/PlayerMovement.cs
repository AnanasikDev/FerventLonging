using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed;
    public float pullingSpeed;
    [ReadOnly] public float currentSpeed;
    [ReadOnly] public Vector2 velocity;

    private Animator animator;

    public void Init()
    {
        animator = GetComponent<Animator>();
        SetSpeed(walkingSpeed);
    }
    public void UpdateMovement()
    {
        velocity = new Vector2(Scripts.Player.playerInput.v_horizonalAxis, 
                               Scripts.Player.playerInput.v_verticalAxis) * currentSpeed;

        if (Scripts.Player.playerInput.v_horizonalAxis == 0 && Scripts.Player.playerInput.v_verticalAxis == 0)
        {
            animator.SetBool("walking", false);
        }
        else
        {
            animator.SetBool("walking", true);
        }

        gameObject.transform.position = gameObject.transform.position.ConvertTo2D() + velocity;
    }

    public void SetSpeed(float _speed)
    {
        this.currentSpeed = _speed;
    }
}
