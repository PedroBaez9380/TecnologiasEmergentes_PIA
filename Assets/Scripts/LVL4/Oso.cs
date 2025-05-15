using UnityEngine;
using TMPro;

public class Oso : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public float interactionDistance = 7.0f;
    public LayerMask interactionLayer;
    public string interactionButton = "E";
    public TextMeshProUGUI interactionPromptText;
    public GameObject MenuExplicacion;

    private bool canInteract = false;
    private GameObject player;

    private CharacterController playerController;
    private PlayerController playerScript;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No se encontró el jugador con el tag 'Player'.");
        }

        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }

        playerScript = player.GetComponent<PlayerController>();
        playerController = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        canInteract = false; //  Resetear en cada frame

        //  Raycast desde la posición del jugador hacia adelante
        if (player != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, interactionDistance, interactionLayer))
            {
                //  Si el raycast golpea al Oso
                if (hit.collider.gameObject == this.gameObject)
                {
                    canInteract = true;

                    //  *** Lógica de visualización de texto de la versión anterior ***
                    if (interactionPromptText != null)
                    {
                        interactionPromptText.text = "Presiona " + interactionButton + " para Hablar";
                        interactionPromptText.gameObject.SetActive(true);
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Hablar(); //  Solo abre el menú si se presiona "E"
                    }
                }
                else
                {
                    //  *** Desactiva el texto si no se interactúa ***
                    if (interactionPromptText != null)
                    {
                        interactionPromptText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                //  *** Desactiva el texto si no se interactúa ***
                if (interactionPromptText != null)
                {
                    interactionPromptText.gameObject.SetActive(false);
                }
            }
        }
    }

    void Hablar()
    {
        MenuExplicacion.gameObject.SetActive(true);
        LockPlayerControls(true);
    }

    public void CerrarMenu()
    {
        MenuExplicacion.gameObject.SetActive(false);
        LockPlayerControls(false);
    }

    private void LockPlayerControls(bool lockControls)
    {
        if (playerController != null)
        {
            playerController.enabled = !lockControls;
        }

        if (playerScript != null)
        {
            playerScript.enabled = !lockControls;
        }

        Cursor.lockState = lockControls ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockControls;
    }
}
