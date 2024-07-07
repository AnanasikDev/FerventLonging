using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SnowShaderController : MonoBehaviour
{
    [SerializeField] private Transform heater;

    //[SerializeField] private Material material;

    [SerializeField] private Material material;

    [SerializeField][ReadOnly] private List<Vector2> positions = new List<Vector2>();

    [SerializeField] private float delay = 0.2f;

    private void Start()
    {
        InvokeRepeating("CalculatePositions", 0, delay);
    }

    private void CalculatePositions()
    {
        positions.Add(heater.transform.position);
        if (positions.Count > 3)
        {
            positions.RemoveAt(0);
        }
    }

    private void Update()
    {
        if (positions.Count > 0) material.SetVector("_heaterPos1", positions[0]);
        if (positions.Count > 1) material.SetVector("_heaterPos2", positions[1]);
        if (positions.Count > 2) material.SetVector("_heaterPos3", positions[2]);
    }
}
