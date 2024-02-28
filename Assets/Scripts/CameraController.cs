using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform target;
    [SerializeField] private Transform ball;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;

    private bool cameraLocked = false;
    private List<InputAction> pressedKeys;

    private Controls playerControls;

    private void Awake()
    {
        playerControls = new Controls();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Camera.Enable();
    }

    private void OnDisable()
    {
        playerControls.Gameplay.Camera.Disable();
    }

    private bool GetPressedKey(InputAction input)
    {
        if (input.ReadValue<float>() != 0)
        {
            if (pressedKeys.FindIndex(key => key == input) != -1)
                return false;
            pressedKeys.Add(input);
            return true;
        }
        else
            pressedKeys.Remove(input);
        return false;
    }

    private void Start()
    {
        pressedKeys = new List<InputAction>();
    }

    private void FixedUpdate()
    {
        //HandleCamera();
        HandleTranslation();
        HandleRotation();
    }

    public void HandleCamera(InputAction.CallbackContext context)
    {
        if (GetPressedKey(context.action))
        {
            if (cameraLocked == true)
            {
                cameraLocked = false;
            }
            else
            {
                cameraLocked = true;
            }
        }
    }

    private void HandleTranslation()
    {
        Vector3 targetPosition;
        if (cameraLocked)
        {
            var targetDirection = target.position - ball.position + new Vector3(0, offset.y, 0);
            targetPosition = target.position - (targetDirection.normalized * offset.z);
        } else
        {
            targetPosition = target.TransformPoint(offset);
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }
    private void HandleRotation()
    {
        var direction = target.position - transform.position;
        if (cameraLocked)
        {
            direction = ball.position - transform.position;
        }

        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}