using NaughtyAttributes;
using UnityEngine;

public class WarmthBar : MonoBehaviour
{
    [SerializeField] private Transform pointer;
    [SerializeField] private float maxShift;

    public void SetWarmth(float rel)
    {
        float x = maxShift * (1 - rel);
        pointer.transform.localPosition = pointer.transform.localPosition.SetX(x);
    }
}
