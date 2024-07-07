using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    [ReadOnly] public Vector2 velocity;

    public void UpdateMovement()
    {
        velocity = new Vector2(Scripts.Player.playerInput.v_horizonalAxis, Scripts.Player.playerInput.v_verticalAxis) * speed;

        gameObject.transform.position = gameObject.transform.position.ConvertTo2D() + velocity;
    }
}
