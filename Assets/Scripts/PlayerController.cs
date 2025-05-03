
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float playerSpeed = 20f;
    public float mouseSensitivity = 100f;

    [Header("Salto y gravedad")]
    public float gravity = 20f;  // Más fuerte que la real
    public float jumpHeight = 2f;   // Menor altura, pero más rápida
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public Transform cameraHolder;
    private CharacterController player;
    private float xRotation;
    private Vector3 currentMoveVelocity;
    private float fallVelocity;

    [Header("Ground Check")]
    public Transform groundCheckPoint;       // Arrástralo luego en el inspector
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayers;           // Crea una capa “Ground” e incluye mesas, suelos, muebles…
    private bool isGrounded;

    void Start()
    {
        player = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Configuración de la cámara (igual que antes)...
        if (cameraHolder == null)
        {
            GameObject go = new GameObject("CameraHolder");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            cameraHolder = go.transform;

            if (Camera.main != null)
            {
                Camera.main.transform.parent = cameraHolder;
                Camera.main.transform.localPosition = Vector3.zero;
                Camera.main.transform.localRotation = Quaternion.identity;
            }
            else Debug.LogError("No se encontró la Main Camera.");
        }
    }

    void Update()
    {
        // (1) Rotación y movimiento horizontal como antes...
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation - mouseY, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical");
        Vector3 dir = (transform.forward * v + transform.right * h).normalized * playerSpeed;
        currentMoveVelocity.x = dir.x;
        currentMoveVelocity.z = dir.z;

        // (2) Salto
        if (Input.GetButtonDown("Jump") && player.isGrounded)
            fallVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);

        // (3) Gravedad avanzada con “pegamento” al suelo
        if (player.isGrounded)
        {
            if (fallVelocity < 0f)
                fallVelocity = -2f;   // <-- aquí en lugar de 0
        }
        else
        {
            if (fallVelocity < 0f)
                fallVelocity -= gravity * fallMultiplier * Time.deltaTime;
            else if (fallVelocity > 0f && !Input.GetButton("Jump"))
                fallVelocity -= gravity * lowJumpMultiplier * Time.deltaTime;
            else
                fallVelocity -= gravity * Time.deltaTime;
        }

        // (4) Aplicar movimiento y debug
        //Debug.Log($"Velocidad: {currentMoveVelocity}");

        currentMoveVelocity.y = fallVelocity;
        player.Move(currentMoveVelocity * Time.deltaTime * 3f);

        //Debug.Log($"[After Move] isGrounded = {player.isGrounded}");
    }

}
