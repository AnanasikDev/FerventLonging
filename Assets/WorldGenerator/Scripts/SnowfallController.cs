using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowfallController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float scale = 1;
    [SerializeField] private float magnitude = 1;

    public void Update()
    {
        ParticleSystem.NoiseModule noise = particles.noise;
        noise.strength = Mathf.PerlinNoise1D(Time.time * scale) * magnitude;
    }
}
