using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityOSC;

public class PlayerController : MonoBehaviour
{
    private float horizontal_movement;
    private float forward_movement;
    private float xRotation = 0f;

    private Rigidbody rb;
    private bool walking = false;
    private Camera playerCamera;

    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float moveSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        OSCHandler.Instance.Init();
        Cursor.lockState = CursorLockMode.Locked;
        startMusic();
    }

    void FixedUpdate()
    {
        Vector3 moveDir = (transform.forward * forward_movement) + (transform.right * horizontal_movement);
        
        if (moveDir.magnitude > 1f)
        {
            moveDir.Normalize();
        }

        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        bool isMoving = horizontal_movement != 0f || forward_movement != 0f;
        if (isMoving && !walking)
        {
            walking = true;
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/walking", 1);
        }
        else if (!isMoving && walking)
        {
            walking = false;
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/walking", 0);
        }

        OSCHandler.Instance.UpdateLogs();
    }

    void startMusic()
    {
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/playsong", 1);
        OSCHandler.Instance.UpdateLogs();
    }

    void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        forward_movement = movement.y;
        horizontal_movement = movement.x;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/jumping", 1);
        OSCHandler.Instance.UpdateLogs();
    }

    void OnLook(InputValue value)
    {
        Vector2 lookInput = value.Get<Vector2>();
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/landing", 1);
        OSCHandler.Instance.UpdateLogs();

    }
}