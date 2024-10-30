using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    Movement movement;

    [SerializeField] Vector2 turnVector = new Vector2();
    [SerializeField] Vector2 moveVector = new Vector2();

    [SerializeField] Transform player;
    [SerializeField] Vector3 targetPosition = Vector3.zero;

    bool sneak;
    bool sprint;

    [SerializeField] LayerMask targetMask;

    float tickTime = 1f;

    void Start()
    {
        movement = GetComponent<Movement>();
        //StartCoroutine(nameof(Evaluate));
    }

    void StateHandler()
    {
        if (sneak)
        {
            movement.ChangeState(Movement.MovementState.crouching);
        }
        else if (movement.grounded && sprint)
        {
            movement.ChangeState(Movement.MovementState.sprinting);
        }
        else if (movement.grounded)
        {
            movement.ChangeState(Movement.MovementState.walking);
        }
        else
        {
            movement.ChangeState(Movement.MovementState.air);
        }
    }

    void Update()
    {
        StateHandler();

        movement.Turn(turnVector);

        //LookForTarget();
        /*if (targetPosition == Vector3.zero || Vector3.Distance(targetPosition, transform.position) < 2)
        {
            RandomTarget();
        }*/
        targetPosition = player.position;

        FindRotation();

        FindMovement();
    }

    void FixedUpdate()
    {
        movement.Move(moveVector);
    }

    IEnumerator Evaluate()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);

            //LookForTarget();
            if (targetPosition == Vector3.zero || Vector3.Distance(targetPosition, transform.position) < 1)
            {
                RandomTarget();
            }

            FindRotation();

            FindMovement();
        }
    }

    void LookForTarget()
    {
        //sphere overlap to find target
        //Ray towards target to see if it's visible?

        
    }

    void RandomTarget()
    {
        //shoot random ray if no target in overlap
        Vector3 direction = Random.rotation.eulerAngles;
        Ray ray = new Ray(transform.position + transform.up * 1, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 25f))
        {
            if (hit.distance > 3)
            {
                Debug.Log(direction);
                targetPosition = hit.point;
                Debug.DrawRay(transform.position + transform.up * 1, direction * 25, Color.white, 60);
            }
        }
        else
        {
            targetPosition = Vector3.zero;
        }
    }

    void FindRotation()
    {
        //find rotation towards target

        if (targetPosition != Vector3.zero)
        {
            Vector3 localUp = -(transform.position - Vector3.zero).normalized;

            // Get direction to the target
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;

            // Project the direction to target onto the plane perpendicular to the NPC's local "up"
            Vector3 flatDirectionToTarget = Vector3.ProjectOnPlane(directionToTarget, localUp).normalized;

            // Get current forward direction (projected onto the local plane as well)
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, localUp).normalized;

            // Calculate the angle between the forward and target direction in the local plane
            float angleToTarget = Vector3.SignedAngle(flatForward, flatDirectionToTarget, localUp);

            turnVector.x = angleToTarget / 2;
        }
        else
        {
            turnVector = Vector2.zero;
        }
    }

    void FindMovement()
    {
        //find movement towards target
        /*
        if (targetPosition != Vector3.zero)
        {
            moveVector = new Vector2(1, 0);
        }
        else
        {
            // If no target, stop movement
            moveVector = Vector2.zero;
        }*/
    }
}
