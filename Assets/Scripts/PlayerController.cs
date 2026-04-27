using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityOSC;

public class PlayerController : MonoBehaviour
{

    private float horizontal_movement;
    private float forward_movement;

    private Rigidbody rb;
    
    private bool walking = false;
    private Camera camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
        OSCHandler.Instance.Init ();
    }

    void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.right * forward_movement * 10f);
        rb.AddForce(-transform.forward * horizontal_movement * 10f);

        if (horizontal_movement != 0f || forward_movement != 0f) {
            if (walking == false) {
                walking = true;
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/walking", 1);
            }
        } else {
            if (walking == true) {
                walking = false;
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/walking", 0);
            }
        }
        
        OSCHandler.Instance.UpdateLogs();
		//*************
    }

    void OnMove(InputValue value) {
        Vector2 movement = value.Get<Vector2>();
        forward_movement = movement.y;
        horizontal_movement = movement.x;
    }

    void OnJump(InputValue value) {
        if (value.isPressed) {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    void OnLook(InputValue value) {
        Vector2 lookInput = value.Get<Vector2>();
        float mouseX = lookInput.x * 2f;
        float mouseY = lookInput.y * 2f;

        // Rotate the player horizontally based on mouse X movement
        transform.Rotate(Vector3.up, mouseX);

        // Rotate the camera vertically based on mouse Y movement
        camera.transform.Rotate(Vector3.right, -mouseY);
    }
}
