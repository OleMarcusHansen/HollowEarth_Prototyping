using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;

    [Header("Turning")]
    [SerializeField] float lookSpeed = 7f;
    float verticalLookRotation;
    [SerializeField] Transform head;

    [Header("Movement")]
    float moveSpeed;
    Vector3 moveDirection;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float airSpeed;

    [SerializeField] float maxSlopeAngle;
    RaycastHit hit;
    [SerializeField] LayerMask groundedMask;
    public bool grounded = true;

    [SerializeField] float jumpForce = 25;


    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ChangeState(MovementState newState)
    {
        if (state == newState)
        {
            return;
        }
        state = newState;

        if (state == MovementState.walking)
        {
            moveSpeed = walkSpeed;
        }
        else if (state == MovementState.sprinting)
        {
            moveSpeed = sprintSpeed;
        }
        else if (state == MovementState.crouching)
        {
            moveSpeed = crouchSpeed;
        }
        else if (state == MovementState.air)
        {
            moveSpeed = airSpeed;
        }
    }

    void Update()
    {
        // Grounded Check
        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out hit, .2f, groundedMask))
        {
            float angle = Vector3.Angle(transform.up, hit.normal);
            if (angle < maxSlopeAngle)
            {
                grounded = true;

                /*if (gravity.enabled)
                {
                    gravity.enabled = false;
                }*/
            }
        }
        if (grounded == true)
        {
            rb.drag = 5;
        }
        else
        {
            rb.drag = 0;
            /*
            if (!gravity.enabled)
            {
                gravity.enabled = true;
            }*/
        }
    }

    public void Turn(Vector2 turnVector)
    {
        //Vector2 turnVector = inputActions.Player.Look.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * turnVector.x * .005f * lookSpeed);

        verticalLookRotation += turnVector.y * .005f * lookSpeed;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
        head.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void Move(Vector2 moveVector)
    {
        //Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();
        moveDirection = transform.forward * moveVector.y + transform.right * moveVector.x;
        Vector3 projectedDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;
        rb.AddForce(projectedDirection * moveSpeed * 100f, ForceMode.Force);

        if (grounded)
        {
            SpeedControl();
        }
    }
    public void SpeedControl()
    {
        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }

    public void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}
