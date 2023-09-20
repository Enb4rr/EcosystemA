using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
//using Unity.Android.Types;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // The object around which the camera will rotate.
    public float rotationSpeed = 3.0f; // Speed of camera rotation.
    public float movementSpeed = 5.0f; // Speed of camera movement.
    public float distanceFromTarget = 5.0f; // Initial distance from the target.

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public bool lockRotation = false;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        if (target == null)
        {
            Debug.LogError("Camera target is not set. Please assign a target GameObject.");
            enabled = false;
        }

        // Initialize camera position.
        Vector3 offset = -transform.forward * distanceFromTarget;
        transform.position = target.position + offset;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !lockRotation)
        {
            lockRotation = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetButtonDown("Fire1") && lockRotation)
        {
            lockRotation = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (lockRotation) return;
        // Camera rotation input.
        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationY = Mathf.Clamp(rotationY, -80.0f, 80.0f); // Limit vertical rotation.

        // Calculate rotation and position.
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 position = rotation * new Vector3(0, 0, -distanceFromTarget) + target.position;

        // Apply rotation and position to the camera.
        transform.rotation = rotation;
        //transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * movementSpeed);

        // Camera movement input (WASD keys).
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontalMovement, 0, verticalMovement);
        moveDirection.Normalize();
        //Translate camera position.
        gameObject.transform.Translate(moveDirection * movementSpeed * Time.deltaTime);
    }
}
