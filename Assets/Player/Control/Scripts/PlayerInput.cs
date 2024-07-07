using NaughtyAttributes;
using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private KeyCode commitWarmth = KeyCode.Space;
    [SerializeField] private KeyCode togglePullCart = KeyCode.Mouse0;
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    [ReadOnly] public float v_horizonalAxis;
    [ReadOnly] public float v_verticalAxis;
    [ReadOnly] public bool isCommitWarmthPressed;
    [ReadOnly] public bool isTogglePullCartPressed;

    public void UpdateInput()
    {
        v_horizonalAxis = Input.GetAxis(horizontalAxis);
        v_verticalAxis = Input.GetAxis(verticalAxis);
        isCommitWarmthPressed = Input.GetKey(commitWarmth);
        isTogglePullCartPressed = Input.GetKey(togglePullCart);
    }
}
