using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    
    public List<GameObject> inventory = new List<GameObject>();
    
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Wenn auf dem boden
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            currentSpeed = speed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 moveVector = transform.right * x + transform.forward * z;
        
        controller.Move(moveVector * (currentSpeed * Time.deltaTime));
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Aufhebbar"))
        {
            int itemID = hit.gameObject.GetInstanceID();
            bool alreadyAdded = inventory.Exists(item => item.GetInstanceID() == itemID);

            if (!alreadyAdded)
            {
                inventory.Add(hit.gameObject);
                hit.gameObject.SetActive(false);
            }
        }
    }


}