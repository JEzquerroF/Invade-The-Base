using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    private CharacterController _characterController;
    [SerializeField] private Camera _camera;

    [SerializeField] private float mouseSensitivity = 2.0f;
    private float mouseX;
    private float mouseY;
    private float rotationX = 0.0f;

    private Vector3 _movement = Vector3.zero;
    private float moveHorizontal = 0.0f;
    private float moveVertical = 0.0f;

    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 100.0f;   

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

    }

    void Update()
    {
        // Movement Camera 

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0.0f, mouseX, 0.0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -40.0f, 60.0f);
        _camera.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);

        //Movement Player

        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        Vector3 _move = transform.forward * moveVertical + transform.right * moveHorizontal;
        _movement = _move * speed;

        if (Input.GetKey(KeyCode.LeftShift)) { speed = 12.0f; }
        else { speed = 6; }


        Debug.Log("Speed: " + speed);
    }

    private void FixedUpdate()
    {
        _movement.y -= gravity * Time.fixedDeltaTime;
        _characterController.Move(_movement * Time.fixedDeltaTime);
    }
}