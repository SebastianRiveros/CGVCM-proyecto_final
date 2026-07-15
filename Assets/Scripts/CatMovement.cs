using UnityEngine;

public class CatMovement : MonoBehaviour {
    [Header("Movement")] [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [Header("Camera")] [SerializeField] Transform cam;
    [SerializeField] float mouseSensitivity = 200f;
    [SerializeField] float cameraDistance = 5f;
    [SerializeField] float cameraHeight = 2f;
    [SerializeField] LayerMask obstaclesMask = -1;
    [Header("Scale")] [SerializeField] float scaleSpeed = 1f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 2f;

    Rigidbody rb;
    float yaw, pitch = 20f;
    bool grounded;

    void Start() {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() { Move(); Jump(); HandleScale(); }
    void LateUpdate() => UpdateCamera();

    void Move() {
        Vector3 dir = (cam.forward * Input.GetAxisRaw("Vertical") + cam.right * Input.GetAxisRaw("Horizontal"));
        dir.y = 0;
        dir.Normalize();
        if (dir != Vector3.zero) {
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90, 0), 10 * Time.deltaTime);
        }
    }

    void UpdateCamera() {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime, -20, 80);
        Vector3 target = transform.position + Vector3.up * cameraHeight;
        Vector3 desiredPos = target + Quaternion.Euler(pitch, yaw, 0) * Vector3.back * cameraDistance;
        if (Physics.Linecast(target, desiredPos, out RaycastHit hit, obstaclesMask))
            cam.position = hit.point - (desiredPos - target).normalized * 0.1f;
        else cam.position = desiredPos;
        cam.LookAt(target);
    }

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
    }

    void HandleScale() {
        float scaleChange = 0;
        //if (Input.GetKey(KeyCode.E)) scaleChange = scaleSpeed * Time.deltaTime;
        //if (Input.GetKey(KeyCode.Q)) scaleChange = -scaleSpeed * Time.deltaTime;
        if (scaleChange != 0) {
            float newScale = Mathf.Clamp(transform.localScale.x + scaleChange, minScale, maxScale);
            transform.localScale = Vector3.one * newScale;
        }
    }

    void OnCollisionEnter(Collision c) => grounded = true;
}