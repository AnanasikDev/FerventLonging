using NaughtyAttributes;
using UnityEngine;

public class WarmthBar : MonoBehaviour
{
    [SerializeField] private Transform pointer;
    [SerializeField] private Vector2 minMaxY;

    public void SetWarmth(float rel)
    {
        float y = minMaxY.x + rel * (minMaxY.y - minMaxY.x);
        pointer.transform.localPosition = pointer.transform.localPosition.SetY(y);
    }
}
