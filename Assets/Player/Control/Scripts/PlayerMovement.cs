using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed;
    public float pullingSpeed;
    [ReadOnly] public float currentSpeed;
    [ReadOnly] public Vector2 velocity;

    public void Init()
    {
        SetSpeed(walkingSpeed);
    }
    public void UpdateMovement()
    {
        velocity = new Vector2(Scripts.Player.playerInput.v_horizonalAxis, Scripts.Player.playerInput.v_verticalAxis) * currentSpeed;

        gameObject.transform.position = gameObject.transform.position.ConvertTo2D() + velocity;
    }

    public void SetSpeed(float _speed)
    {
        this.currentSpeed = _speed;
    }
}
