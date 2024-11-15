using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    public Rigidbody rb;
    public Transform head;



    [Header("Configurations")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float impactThreshold;



    [Header("Runtime")]
    Vector3 newVelocity;
    bool isGrounded = false;
    bool isJumping = false;
    float vyCache;


    [Header("Audio")]
    public AudioSource audioWalk;

    // Start is called before the first frame update
    void Start() {
        //  Hide and lock the mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }




    // Update is called once per frame
    void Update() {
        // Horizontal rotation
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);   // Adjust the multiplier for different rotation speed

        newVelocity = Vector3.up * rb.velocity.y;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        newVelocity.x = Input.GetAxis("Horizontal") * speed;
        newVelocity.z = Input.GetAxis("Vertical") * speed;

        if (isGrounded) {
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
                newVelocity.y = jumpSpeed;
                isJumping = true;
            }
        }
        rb.velocity = transform.TransformDirection(newVelocity);

        bool isMovingOnGround = (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f) && isGrounded;

        audioWalk.enabled = isMovingOnGround;
        audioWalk.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.75f : 1f;

        // Check if the player is touching a wall in front or behind
        bool isTouchingWall = Physics.Raycast(transform.position, transform.forward, 1f) || Physics.Raycast(transform.position, -transform.forward, 1f);

        if (isTouchingWall && isMovingOnGround) {
            audioWalk.enabled = false;
        }        
    }

    void FixedUpdate() {
        //  Shoot a ray of 1 unit towards the ground
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f)) {
            isGrounded = true;
        }
        else isGrounded = false;

        //  Cache the velocity in the y direction
        vyCache = rb.velocity.y;
    }

    void LateUpdate() {
        // Vertical rotation
        Vector3 e = head.eulerAngles;
        //e.x -= Input.GetAxis("Mouse Y") * 2f;   //  Edit the multiplier to adjust the rotate speed
        e.x = RestrictAngle(e.x, -85f, 85f);    //  This is clamped to 85 degrees. You may edit this.
        head.eulerAngles = e;
    }




    //  This will be called constantly
    void OnCollisionStay(Collision col) {
        if (Vector3.Dot(col.GetContact(0).normal, Vector3.up) <= .6f)
            return;

        isGrounded = true;
        isJumping = false;
    }

    //  This will be called once only during collision
    void OnCollisionEnter(Collision col) {
        //  Prevent fall damage when hitting a vertical wall
        if (Vector3.Dot(col.GetContact(0).normal, Vector3.up) < .5f) {
            if (rb.velocity.y < -5f) {
                rb.velocity = Vector3.up * rb.velocity.y;
                return;
            }
        }

        //  Calculate impact force
        float acceleration = (rb.velocity.y - vyCache) / Time.fixedDeltaTime;
        float impactForce = rb.mass * Mathf.Abs(acceleration);

        //  This triggers the fall damage
        //  Add your code to the OnFallDamage() function below
        if (impactForce >= impactThreshold)
            OnFallDamage();
    }

    void OnCollisionExit(Collision col) {
        isGrounded = false;
    }




    //  Add your fall damage code here!
    void OnFallDamage() {
    }



    //  A helper function
    //  Clamp the vertical head rotation (prevent bending backwards)
    public static float RestrictAngle(float angle, float angleMin, float angleMax) {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > angleMax)
            angle = angleMax;
        if (angle < angleMin)
            angle = angleMin;

        return angle;
    }
}