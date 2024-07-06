using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{

    // Warmth
    public float MAX_WARMTH;
    private float warmth;

    // Warmth depletion
    private float warmthLossTimeInterval = 1.0f;
    private float warmthLossTimer = 0.0f;


    public WarmthBar warmthBar;
    public float warmthLossRate; // per second

    // Start is called before the first frame update
    void Start()
    {
        // Init wamrth
        warmth = MAX_WARMTH;
        warmthBar.SetMaxWarmth((int) MAX_WARMTH);
    }

    // Update is called once per frame
    void Update()
    {
        // Deplete warmth
        bool timeToLoseWarmth = warmthLossTimer >= warmthLossTimeInterval;
        if (timeToLoseWarmth)
        {
            warmth -= warmthLossRate;
            warmthLossTimer -= warmthLossRate;
        }

        // Update UI
        warmthBar.SetWarmth((int) warmth);

        // Update timer
        warmthLossTimer += Time.deltaTime;
    }
}
