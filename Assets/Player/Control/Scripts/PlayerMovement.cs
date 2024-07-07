using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;

    public void UpdateMovement()
    {
        Vector2 translation = new Vector2(Scripts.Player.playerInput.v_horizonalAxis, Scripts.Player.playerInput.v_verticalAxis) * speed;

        gameObject.transform.position = gameObject.transform.position.ConvertTo2D() + translation;
    }
}
