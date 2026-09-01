using UnityEngine;
using FMODUnity;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    [SerializeField] EventReference FootstepEvent;
    public float FSrate = 1.5f;
    private float tiiime = 0f;

    [Header("Movement")]
    public float MoveSpeed;
    public Transform Orientation;
    private bool moving = false;

    float HorizontalInput;
    float VerticalInput;

    Vector3 MoveDirection;

    Rigidbody rb;

    [Header("Grounding")]
    public float GroundDrag;
    public float GroundStickForce = 30f;
    public float GroundCheckDistance = 0.2f;
    public LayerMask GroundLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        SpeedControl();
        rb.linearDamping = GroundDrag;

        tiiime += Time.deltaTime;
        if(moving)
        {
            if(tiiime >= FSrate)
            {
                Footsteps();
                tiiime = 0f;
            }
        }
    }

    public void Footsteps()
    {
        RuntimeManager.PlayOneShotAttached(FootstepEvent, player);
    }

    void FixedUpdate()
    {
        MovePlayer();
        GroundStick();
    }

    private void MyInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        if(HorizontalInput == 0 && VerticalInput == 0)
        {
            moving = false;
        }
        else
        {
            moving = true;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        MoveDirection = Orientation.forward * VerticalInput + Orientation.right * HorizontalInput;

        rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        if (flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;

            rb.linearVelocity = new Vector3(
                limitedVel.x,
                rb.linearVelocity.y,
                limitedVel.z
            );
        }
    }

    private void GroundStick()
    {
        // Check if there is ground directly underneath the player
        bool grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            GroundCheckDistance,
            GroundLayer
        );

        if (grounded)
        {
            // Push the player downward so they stay connected to the ground
            rb.AddForce(Vector3.down * GroundStickForce, ForceMode.Force);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * GroundCheckDistance
        );
    }
}