using NaughtyAttributes;
using UnityEngine;

public class HeaterRenderer : MonoBehaviour
{
    [ShowNativeProperty] public float angleToVertical { get; private set; }

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private new SpriteRenderer renderer;

    public Vector2 handlerPosition { get; private set; }

    public float step { get; private set; }
    public int numberOfSteps;

    private void Start()
    {
        numberOfSteps = sprites.Length;
        step = 360f / numberOfSteps;
    }

    private void Update()
    {
        LookAt(Scripts.Player.transform);
    }

    public void LookAt(Transform target)
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 direction = target.position - transform.position;

            // Calculate the angle between the current direction and the target direction
            angleToVertical = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

            renderer.sprite = sprites[GetSpriteIndex(angleToVertical)];

            handlerPosition = transform.position + direction.normalized * 2;
        }
    }

    private int GetSpriteIndex(float angle)
    {
        return (int)(Mathf.Repeat(angle / -step, numberOfSteps));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(handlerPosition, Vector3.one * 0.5f);
    }
}
