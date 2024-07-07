using NaughtyAttributes;
using UnityEngine;

public class HeaterRenderer : MonoBehaviour
{
    [ShowNativeProperty] public float angleToVertical { get; private set; }

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private new SpriteRenderer renderer;

    public Vector2 handlerPosition { get; private set; }

    private float step;

    private void Start()
    {
        step = 360f / sprites.Length;
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
        return (int)Mathf.Repeat(angle / -step, sprites.Length);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(handlerPosition, Vector3.one * 0.5f);
    }
}
