using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicycleMovement : MonoBehaviour
{
    public float movementSpeed = 10;
    private float currentMovementSpeed;
    public float rotationSpeed = 10;
    public float jumpForce = 10;
    public float gravity = -9.8f;
    public float fallResetDistance = -50;
    private float groundCheckDistance = 0.5f;
    private float groundRotateMultiplier = 0.1f;

    public int numberOfJumps = 2;
    public int currentNumberOfJumps;

    public bool rotateTowardsGround = false;
    public bool autoCorrectRotation = true;
    public bool clampRotation = true;
    private bool isGrounded;
    private bool isStickey;
    private bool isMoving;
    private bool canStickey = true;
    private bool canUpdateHeightTracker;
    private bool flip;
    private bool hasCollided;

    public Transform groundCheckPoint;
    public Transform childCycle;
    public Transform startingLocation;

    private Rigidbody rb;

    private GameObject currentTrack;
    private GameObject levelGenerationManager;
    private GameObject heightTracker;

    private Animator animator;

    void Start()
    {
        if (gameObject.GetComponent<Rigidbody>())
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }

        currentMovementSpeed = movementSpeed;

        if (levelGenerationManager = GameObject.Find("LevelGenerationManager")) { }
        if (animator = gameObject.GetComponent<Animator>()) { }
    }

    void Update()
    {
        Movement();
        Jumping();
    }

    private void FixedUpdate()
    {
        CalculateHeight();

        GroundRotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasCollided = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        hasCollided = false;
    }

    void Jumping()
    {
        if(currentNumberOfJumps > 0)
        {
            if (Input.GetButtonDown("Jump") && rb)
            {
                Unstick();

                rb.AddForce(transform.up * jumpForce);

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
                //rb.velocity.Set(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(childCycle.forward * currentMovementSpeed * Time.deltaTime);
            }

            // Check if the player is moving and apply tilting animations
            if (isMoving == false)
            {
                isMoving = true;

                if (animator)
                {
                    animator.SetBool("isMoving", true);
                }
            }
        }
        else if (isMoving == true)
        {
            isMoving = false;

            if (animator)
            {
                animator.SetBool("isMoving", false);
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            childCycle.transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            childCycle.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        if (!isGrounded)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                //transform.Rotate(-Vector3.LerpUnclamped(childCycle.transform.right, -childCycle.transform.right, rotationSpeed * Time.deltaTime));
                //transform.RotateAroundLocal(-Vector3.LerpUnclamped(childCycle.transform.right, -childCycle.transform.right, rotationSpeed * Time.deltaTime));
                transform.Rotate(-childCycle.transform.right * rotationSpeed * Time.deltaTime);
                //Vector3.LerpUnclamped(childCycle.transform.right, -childCycle.transform.right, rotationSpeed);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                //transform.Rotate(Vector3.LerpUnclamped(childCycle.transform.right, -childCycle.transform.right, rotationSpeed * Time.deltaTime).normalized);
                transform.Rotate(childCycle.transform.right * rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //transform.Rotate(-Vector3.LerpUnclamped(childCycle.transform.forward, -childCycle.transform.forward, rotationSpeed * Time.deltaTime).normalized * 2);
                transform.Rotate(-childCycle.transform.forward * 2 * rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                //transform.Rotate(Vector3.LerpUnclamped(childCycle.transform.forward, -childCycle.transform.forward, rotationSpeed * Time.deltaTime).normalized * 2);
                transform.Rotate(childCycle.transform.forward * 2 * rotationSpeed * Time.deltaTime);
            }
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

            if (currentNumberOfJumps != numberOfJumps)
            {
                currentNumberOfJumps = numberOfJumps;
            }

            if (rotateTowardsGround)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, groundRotateMultiplier + (rb.velocity.magnitude / 1000));
            }

            if (currentTrack && currentTrack.gameObject.GetComponent<TrackPiece>())
            {
                if (currentTrack.gameObject.GetComponent<TrackPiece>().isStickey && isStickey == false && rb.velocity.magnitude > 5 && canStickey)
                {
                    Stick();
                }
                else if(!currentTrack.gameObject.GetComponent<TrackPiece>().isStickey)
                {
                    Unstick();
                }
            }
            
            if (isStickey)
            {
                //rb.AddForce(hit.normal * ((gravity / 2)) * (rb.velocity.magnitude / 4));
                transform.position = Vector3.Lerp(transform.position, hit.point, rotationSpeed);

                Debug.Log("rb.velocity.magnitude = " + rb.velocity.magnitude);

                if (rb.velocity.magnitude < 5)
                {
                    Unstick();
                }
            }

            // Reset height tracker when on a surface
            if (!canUpdateHeightTracker)
            {
                canUpdateHeightTracker = true;
            }
        }
        else
        {
            Unstick();
        }

        if (hit.collider == null)
        {
            isGrounded = false;

            if (!isStickey)
            {
                rb.AddForce(Vector3.up * gravity * 6);
            }

            Debug.DrawLine(groundCheckPoint.position, new Vector3(groundCheckPoint.position.x, groundCheckPoint.position.y - groundCheckDistance, groundCheckPoint.position.z), Color.green);

            if(autoCorrectRotation && (transform.rotation.x != 0 || transform.rotation.z != 0))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w), 1 * Time.deltaTime);
            }

            SetHeightTracker();
        }
        else
        {
            TrackGround(hit);
        }

        if (hasCollided && hit.collider == null)
        {
            if (transform.rotation.x > 0.65f || transform.rotation.x < -0.65f || transform.rotation.z > 0.65f || transform.rotation.z < -0.65f)
            {
                IncorrectLanding();

                //levelGenerationManager.GetComponent<LevelGeneration>().RestartGame();
            }
        }
    }

    // Set the height tracker's location
    void SetHeightTracker()
    {
        if (canUpdateHeightTracker)
        {
            // If there isn't already a height tracker, create one
            if (heightTracker == null)
            {
                heightTracker = new GameObject();
            }

            // Set the position to the player's position
            heightTracker.transform.position = gameObject.transform.position;

            canUpdateHeightTracker = false;
        }
    }

    void CalculateHeight()
    {
        if (heightTracker)
        {
            if (!canUpdateHeightTracker)
            {
                // Calculate the y component distance from the player and the height tracker 
                float distance = transform.position.y - heightTracker.transform.position.y;

                // If the distance is less than the fallResetDistance, then reset the game
                if (distance < fallResetDistance)
                {
                    levelGenerationManager.GetComponent<LevelGeneration>().RestartGame();
                }
        }
        }
    }

    void Stick()
    {
        isStickey = true;

        rb.useGravity = false;
    }

    void Unstick()
    {
        if (isStickey)
        {
            isStickey = false;

            rb.useGravity = true;

            groundRotateMultiplier = 0.1f;

            currentMovementSpeed = movementSpeed;

            canStickey = false;
            Invoke("CanStick", 0.5f);
        }
    }

    void IncorrectLanding()
    {
        if (rb)
        {
            rb.velocity = new Vector3(0, 0, 0);
        }

        if (startingLocation)
        {
            transform.rotation = startingLocation.rotation;
        }
    }

    void CanStick()
    {
        canStickey = true;
    }

    // Tell the level generater the current track piece the player is on
    void TrackGround(RaycastHit hit)
    {
        if(hit.collider != null)
        {
            currentTrack = hit.collider.gameObject.transform.parent.gameObject;
            levelGenerationManager.GetComponent<LevelGeneration>().ChangeTrack(hit.collider.gameObject.transform.parent.gameObject);
        }
    }
}
