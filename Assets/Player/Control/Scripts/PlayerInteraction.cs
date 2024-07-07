using NaughtyAttributes;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField][ReadOnly] private bool isPullingCart = false;
    [SerializeField] private float pullingForce = 10;
    [SerializeField] private float pullingDistance = 15;
    
    public void UpdateInteraction()
    {
        if (Scripts.Player.playerInput.isTogglePullCartPressed)
        {
            if (!isPullingCart && (transform.position - Scripts.Heater.transform.position).magnitude < pullingDistance)
            {
                isPullingCart = true;
            }

            else if (isPullingCart || (transform.position - Scripts.Heater.transform.position).magnitude >= pullingDistance)
            {
                isPullingCart = false;
                Scripts.Heater.rigidbody.velocity = Vector3.zero;
            }
        }

        if (isPullingCart)
        {
            //Scripts.Heater.rigidbody.AddForceAtPosition(Scripts.Player.playerMovement.velocity * 20, Scripts.Heater.heaterRenderer.handlerPosition);

            Vector2 movement = Scripts.Player.playerMovement.velocity.normalized;
            Vector2 look = (transform.position - Scripts.Heater.transform.position).normalized;

            if (Vector3.Dot(movement, look) > 0.6f)// && (transform.position - Scripts.Heater.transform.position).magnitude > 8)
            {
                Scripts.Heater.rigidbody.velocity = Scripts.Player.playerMovement.velocity * pullingForce;
            }
        }
    }
}