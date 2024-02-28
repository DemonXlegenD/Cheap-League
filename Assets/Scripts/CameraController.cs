using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform target;
    [SerializeField] private Transform ball;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;

    private bool cameraLocked = false;
    private List<string> pressedKeys;

    [SerializeField, StringSelection("Camera 1", "Camera 2")] private string inputCameraName;


    private bool GetAxisDown(string input)
    {
        if (Input.GetAxis(input) != 0)
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
        pressedKeys = new List<string>();
    }

    private void FixedUpdate()
    {
        HandleCamera();
        HandleTranslation();
        HandleRotation();
    }

    private void HandleCamera()
    {
        if (GetAxisDown(inputCameraName))
        {
            if (cameraLocked == true)
            {
                cameraLocked = false;
            } else
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
            var targetDirection = target.position - ball.position;
            targetPosition = target.position - (targetDirection.normalized * offset.z) + new Vector3(0, offset.y, 0);
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