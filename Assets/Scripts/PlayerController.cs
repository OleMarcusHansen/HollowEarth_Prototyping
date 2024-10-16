using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    DefaultPlayerActions inputActions;

    Rigidbody rb;

    [SerializeField] Transform moon;
    [SerializeField] TileManager firmamentTileManager;
    [SerializeField] TileManager moonTileManager;

    Movement movement;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Awake()
    {
        inputActions = new DefaultPlayerActions();
        inputActions.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        movement = GetComponent<Movement>();
    }

    void StateHandler()
    {
        if (inputActions.Player.Crouch.IsPressed())
        {
            movement.ChangeState(Movement.MovementState.crouching);
        }
        else if (movement.grounded && inputActions.Player.Sprint.IsPressed())
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

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && movement.grounded)
        {
            movement.Jump();
        }
    }

    void Update()
    {
        StateHandler();

        // Turning
        movement.Turn(inputActions.Player.Look.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        movement.Move(inputActions.Player.Move.ReadValue<Vector2>());
    }


    public void Moon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!moonTileManager.loading)
            {
                transform.position = moon.position;
                StartCoroutine(moonTileManager.LoadTiles(false));
            }
        }
    }

    public void Firmament(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!firmamentTileManager.loading)
            {
                List<Vector3> spawnpoints = new List<Vector3>();

                foreach (int i in firmamentTileManager.specialTiles)
                {
                    if (firmamentTileManager.tileDatas[i].itemSlot.item == ItemId.Spawnbush)
                    {
                        spawnpoints.Add(firmamentTileManager.tileDatas[i].position);
                    }
                }
                if (spawnpoints.Count == 0)
                {
                    Debug.LogWarning("No spawnpoint found");
                    return;
                }

                transform.position = spawnpoints[Random.Range(0, spawnpoints.Count - 1)];
                rb.velocity = Vector3.zero;
                GetComponent<TileController>().loadedPosition = transform.position;
                StartCoroutine(firmamentTileManager.LoadTiles(false));
            }
        }
    }
}
