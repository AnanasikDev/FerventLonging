using NaughtyAttributes;
using UnityEngine;

public class HeaterRenderer : MonoBehaviour
{
    [ShowNativeProperty] public float angleToVertical { get; private set; }

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private new SpriteRenderer renderer;

    private float step;

    private void Start()
    {
        step = 360f / sprites.Length;
    }

    private void Update()
    {
        LookAt(Scripts.Player);
    }

    public void LookAt(Transform target)
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector3 direction = target.position - transform.position;

            // Calculate the angle between the current direction and the target direction
            angleToVertical = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + step;

            renderer.sprite = sprites[GetSpriteIndex(angleToVertical)];
        }
    }

    private int GetSpriteIndex(float angle)
    {
        return (int)Mathf.Repeat(angle / -step, sprites.Length);
    }
}
