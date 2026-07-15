using UnityEngine;

public class CatMovement : MonoBehaviour
{
    [Header("movimiento")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float rotationSpeed = 10f;
    
    [Header("camara")]
    [SerializeField] Transform cam;
    [SerializeField] float mouseSensitivity = 200f;
    [SerializeField] float cameraDistance = 5f;
    [SerializeField] float cameraHeight = 2f;
    [SerializeField] LayerMask obstaclesMask = -1;
    
    private Rigidbody rb;
    private float yaw, pitch = 20f;
    private bool grounded;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        // mantiene al gato siempre de pie, solo rota en Y
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationZ;

        gameManager = FindObjectOfType<GameManager>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (gameManager != null && gameManager.EstaPausado()) return;
        
        Movimiento();
        Saltar();
    }

    void LateUpdate()
    {
        if (gameManager != null && gameManager.EstaPausado()) return;
        ActualizarCamara();
    }

    void Movimiento()
    {
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 dir = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal"));
        dir.y = 0;
        dir.Normalize();

        if (dir != Vector3.zero)
        {
            Vector3 targetPos = transform.position + dir * moveSpeed * Time.deltaTime;
            rb.MovePosition(targetPos);

            // solo rota en Y, el gato siempre mira hacia donde se mueve
            Quaternion targetRot = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void ActualizarCamara()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime, -20, 80);

        Vector3 target = transform.position + Vector3.up * cameraHeight;
        Vector3 desiredPos = target + Quaternion.Euler(pitch, yaw, 0) * Vector3.back * cameraDistance;

        if (Physics.Linecast(target, desiredPos, out RaycastHit hit, obstaclesMask))
            cam.position = hit.point - (desiredPos - target).normalized * 0.1f;
        else
            cam.position = desiredPos;

        cam.LookAt(target);
    }

    void Saltar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    public void ResetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}