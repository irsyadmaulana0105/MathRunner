using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float forwardSpeed = 10f;
    private Vector3 direction;
    private int desireLane = 1; // 0 = kiri, 1 = tengah, 2 = kanan
    public float laneDistance = 4; // jarak antar jalur

    public float jumpForce = 10f;
    public float gravity = -20f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Time.timeScale = 1.2f;
    }

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    void Update()
    {
        // Gerakan maju
        direction.z = forwardSpeed;
        
        // Lompat & Gravitasi
        if (controller.isGrounded)
        {
            direction.y = -1f;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            direction.y += gravity * Time.deltaTime;
        }

        // Input lane
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desireLane++;
            if (desireLane > 2) desireLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desireLane--;
            if (desireLane < 0) desireLane = 0;
        }

        // Pindah lane secara halus
        Vector3 targetPosition = transform.position.z * Vector3.forward + transform.position.y * Vector3.up;

        if (desireLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desireLane == 2)
            targetPosition += Vector3.right * laneDistance;

        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).x;

        // Gabungkan gerakan x (pindah jalur), y (lompat/gravitasi), dan z (maju)
        controller.Move(new Vector3(moveVector.x, direction.y, direction.z) * Time.deltaTime);

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
        }
    }
}
