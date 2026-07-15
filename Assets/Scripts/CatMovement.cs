using UnityEngine;

public class CatMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float rotationSpeed = 10f;
    
    [Header("Camera")]
    [SerializeField] Transform cam;
    [SerializeField] float mouseSensitivity = 200f;
    [SerializeField] float cameraDistance = 5f;
    [SerializeField] float cameraHeight = 2f;
    [SerializeField] LayerMask obstaclesMask = -1;
    
    [Header("Scale")]
    [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 2f;

    private Rigidbody rb;
    private float yaw, pitch = 20f;
    private bool grounded;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // 🔥 CONFIGURACIÓN PARA QUE EL GATO SIEMPRE ESTÉ DE PIE
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezeRotationY; // Opcional: si quieres que NO rote en Y

        // Buscar GameManager para la pausa
        gameManager = FindObjectOfType<GameManager>();
        
        // Ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Si el juego está pausado, no hacer nada
        if (gameManager != null && gameManager.EstaPausado())
            return;

        Move();
        Jump();
        HandleScale();
    }

    void LateUpdate()
    {
        // Si el juego está pausado, no actualizar cámara
        if (gameManager != null && gameManager.EstaPausado())
            return;
            
        UpdateCamera();
    }

    void Move()
    {
        // Obtener dirección de movimiento
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
            // Movimiento suave
            Vector3 targetPosition = transform.position + dir * moveSpeed * Time.deltaTime;
            rb.MovePosition(targetPosition);

            // Rotación suave SOLO en Y (el gato siempre mira hacia donde se mueve)
            Quaternion targetRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateCamera()
    {
        // Rotación de cámara con mouse
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime, -20, 80);

        // Posición de la cámara
        Vector3 target = transform.position + Vector3.up * cameraHeight;
        Vector3 desiredPos = target + Quaternion.Euler(pitch, yaw, 0) * Vector3.back * cameraDistance;

        // Evitar que la cámara atraviese paredes
        if (Physics.Linecast(target, desiredPos, out RaycastHit hit, obstaclesMask))
        {
            cam.position = hit.point - (desiredPos - target).normalized * 0.1f;
        }
        else
        {
            cam.position = desiredPos;
        }

        cam.LookAt(target);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
    }

    void HandleScale()
    {
        float scaleChange = 0;
        // Descomentar para usar escala
        // if (Input.GetKey(KeyCode.E)) scaleChange = scaleSpeed * Time.deltaTime;
        // if (Input.GetKey(KeyCode.Q)) scaleChange = -scaleSpeed * Time.deltaTime;
        
        if (scaleChange != 0)
        {
            float newScale = Mathf.Clamp(transform.localScale.x + scaleChange, minScale, maxScale);
            transform.localScale = Vector3.one * newScale;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    // Método para reiniciar posición (opcional)
    public void ResetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}