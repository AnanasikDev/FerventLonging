using NaughtyAttributes;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField][ReadOnly] private bool isPullingCart = false;
    [SerializeField] private float pullingForce = 10;
    [SerializeField] private float pullingDistance = 15;

    private Vector2 target;
    
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

            /*Vector2 movement = Scripts.Player.playerMovement.velocity.normalized;
            Vector2 look = (transform.position - Scripts.Heater.transform.position).normalized;

            if (Vector3.Dot(movement, look) > 0.6f)// && (transform.position - Scripts.Heater.transform.position).magnitude > 8)
            {
                Scripts.Heater.rigidbody.velocity = Scripts.Player.playerMovement.velocity * pullingForce;
            }*/

            target = transform.position - Scripts.Heater.heaterRenderer.handlerPosition.ConvertTo3D();
            Vector2 shift = Scripts.Heater.heaterRenderer.handlerPosition - Scripts.Heater.transform.position.ConvertTo2D();
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;

            float roundedAngle = 360 - Scripts.Heater.heaterRenderer.step * Mathf.Repeat(Mathf.RoundToInt(angle / -Scripts.Heater.heaterRenderer.step), Scripts.Heater.heaterRenderer.numberOfSteps);

            Vector2 relativeShift = new Vector2(Mathf.Cos(roundedAngle * Mathf.Deg2Rad), Mathf.Sin(roundedAngle * Mathf.Deg2Rad)) * shift.magnitude;

            Debug.Log(relativeShift);
            
            float d = (transform.position - Scripts.Heater.transform.position).magnitude;
            if (d < 6)
            {
                Scripts.Heater.transform.position += relativeShift.ConvertTo3D() * 0.01f; //Vector3.Lerp(Scripts.Heater.transform.position, target, d / 2f);
            }
        }
    }
}