using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    private float h;
    private float v;

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Vector2 translation = new Vector2(h, v) * speed;

        gameObject.transform.position = gameObject.transform.position.ConvertTo2D() + translation;
    }
}
