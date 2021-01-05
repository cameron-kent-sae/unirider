﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicycleMovement : MonoBehaviour
{
    public float movementSpeed = 10;
    public float rotationSpeed = 10;
    public float jumpForce = 10;
    public float gravity = -9.8f;
    private float groundCheckDistance = 0.5f;

    public int numberOfJumps = 2;
    public int currentNumberOfJumps;

    public bool rotateTowardsGround = false;
    private bool isGrounded;

    public Transform groundCheckPoint;

    private Rigidbody rb;

    void Start()
    {
        if (gameObject.GetComponent<Rigidbody>())
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        GroundRotate();
        Movement();
        Jumping();
    }

    void Jumping()
    {
        if(currentNumberOfJumps > 0)
        {
            if (Input.GetButtonDown("Jump") && rb)
            {
                rb.AddForce(transform.right * jumpForce);

                currentNumberOfJumps--;
            }
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //gameObject.transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);

            if (rb)
            {
                rb.velocity.Set(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * movementSpeed);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Rotate(-Vector3.right * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }

    void GroundRotate()
    {
        RaycastHit hit;
        Physics.SphereCast(groundCheckPoint.position, 0.1f, -groundCheckPoint.up, out hit, groundCheckDistance);

        if(hit.collider)
        {
            Debug.DrawLine(groundCheckPoint.position, hit.point, Color.green);

            isGrounded = true;

            Debug.Log("isGrounded is true");

            if (currentNumberOfJumps != numberOfJumps)
            {
                currentNumberOfJumps = numberOfJumps;
            }

            if (rotateTowardsGround)
            {
                transform.rotation = Quaternion.LookRotation(hit.point, hit.normal);
            }
        }

        if(hit.collider == null)
        {
            isGrounded = false;

            Debug.Log("isGrounded is false");

            rb.AddForce(transform.right * gravity);

            Debug.DrawLine(groundCheckPoint.position, new Vector3(groundCheckPoint.position.x, groundCheckPoint.position.y - groundCheckDistance, groundCheckPoint.position.z), Color.green);
        }
    }
}
