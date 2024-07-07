using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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
            warmthLossTimer -= warmthLossTimeInterval;
        }

        // Update UI
        warmthBar.SetWarmth((int) warmth);

        // Update timer
        warmthLossTimer += Time.deltaTime;
    }

    public void incrementWarmth(float incrementWarmth)
    {
        warmth += incrementWarmth;

        if (warmth > MAX_WARMTH)
        {
            warmth = MAX_WARMTH;
        }
    }
}
