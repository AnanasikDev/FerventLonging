using System;
using UnityEngine;

public class PlayerWarmth : MonoBehaviour
{

    // Warmth
    public float MAX_WARMTH;
    private float warmth;

    // Warmth depletion
    private float warmthLossTimeInterval = 0.5f;
    private float warmthLossTimer = 0.0f;
    public float warmthLossRate; // per second

    // Refs
    public WarmthBar warmthBar;

    public event Action onPlayerDiedEvent;


    // Start is called before the first frame update
    public void Init()
    {
        // Init wamrth
        warmth = MAX_WARMTH;
    }

    // Update is called once per frame
    public void UpdateWarmth()
    {
        // Deplete warmth
        bool timeToLoseWarmth = warmthLossTimer >= warmthLossTimeInterval;
        if (timeToLoseWarmth)
        {
            decreaseWarmth(warmthLossRate);
            warmthLossTimer -= warmthLossTimeInterval;
        }

        // Update UI
        warmthBar.SetWarmth(warmth / MAX_WARMTH);

        // Update timer
        warmthLossTimer += Time.deltaTime;
    }

    public void increaseWarmth(float value)
    {
        warmth += value;

        if (warmth > MAX_WARMTH)
        {
            warmth = MAX_WARMTH;
        }
    }

    public void decreaseWarmth(float value)
    {
        warmth -= value;

        if (warmth < 0)
        {
            warmth = 0;
            onPlayerDiedEvent?.Invoke();
        }
    }
}
