using UnityEngine;
using TMPro;

public class Interactor : MonoBehaviour
{
    [Header("Configuraci�n de Interacci�n")]
    public float interactionDistance = 3.0f;
    public LayerMask interactionLayer;

    [Header("Elementos UI")]
    public TextMeshProUGUI interactionPromptText;

    private GameObject currentHitObject;
    private LockerInteraction_lvl4 lockerInteraction;
    private ButtonProperties buttonProperties;
    private Transform selectedTable;
    private bool isPlacingDevice = false;

    void Start()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Interaction Prompt Text no asignado en Interactor script.");
        }
    }

    void Update()
    {
        currentHitObject = null;
        selectedTable = null;
        buttonProperties = null;

        // Realiza el raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactionLayer))
        {
            // Dibujar el raycast para debug (l�nea roja)
            Debug.DrawLine(transform.position, hit.point, Color.red);

            // Verifica si golpe� una mesa
            if (hit.collider.CompareTag("Table"))
            {
                Debug.Log("Detecto la mesa");
                selectedTable = hit.collider.transform;
                TableProperties tableProperties = selectedTable.GetComponent<TableProperties>();

                if (tableProperties != null)
                {
                    // Si la mesa est� ocupada, muestra el mensaje para eliminar el dispositivo
                    if (tableProperties.IsOccupied())
                    {
                        if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                        {
                            interactionPromptText.text = "<color=red>Presiona E para eliminar el dispositivo</color>";

                            interactionPromptText.gameObject.SetActive(true);
                        }

                        // Si se presiona E, elimina el dispositivo de la mesa
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            tableProperties.RemoveDevice();
                            RefreshPrompt();
                        }
                    }
                    else
                    {
                        // Si la mesa no est� ocupada, muestra que puedes colocar un dispositivo
                        if (lockerInteraction != null && interactionPromptText != null && !interactionPromptText.gameObject.activeSelf && lockerInteraction.GetCurrentDevice() != null)

                        {
                            interactionPromptText.text = "Presiona E para colocar un dispositivo";
                            interactionPromptText.gameObject.SetActive(true);
                        }

                        // Verifica si est�s en el modo de colocar un dispositivo
                        if (isPlacingDevice && Input.GetKeyDown(KeyCode.E))
                        {
                            // Aqu�, agregar�s la l�gica de colocar el dispositivo en la mesa
                            lockerInteraction.PlaceDeviceOnTable(selectedTable);
                            isPlacingDevice = false;
                        }
                    }
                }
            }
            // Verifica si golpe� un LockerInteraction_lvl4 (interacci�n con locker)
            else if (hit.collider.GetComponent<LockerInteraction_lvl4>() != null)
            {
                lockerInteraction = hit.collider.GetComponent<LockerInteraction_lvl4>();

                if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.text = "Presiona E para abrir el locker";
                    interactionPromptText.gameObject.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    lockerInteraction.OpenLockerMenu();
                    isPlacingDevice = true; // Esto puede habilitar el modo para colocar un dispositivo en el futuro
                }
            }
            // Verifica si golpe� un Button (interacci�n con bot�n)
            else if (hit.collider.GetComponent<ButtonProperties>() != null)
            {
                buttonProperties = hit.collider.GetComponent<ButtonProperties>();

                if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.text = "Presiona E para presionar el bot�n";
                    interactionPromptText.gameObject.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    buttonProperties.Interact();
                }
            }
            else
            {
                // Si no hay nada sobre lo que interactuar, desactiva el texto de interacci�n
                if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // Si no detecta nada con el raycast, desactiva el texto de interacci�n
            if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }

    // M�todo para activar el modo de colocaci�n de dispositivos
    public void EnableDevicePlacement(LockerInteraction_lvl4 locker)
    {
        lockerInteraction = locker;
        isPlacingDevice = true;
        Debug.Log("Modo de colocaci�n activado.");
    }

    // Desactiva el modo de colocaci�n de dispositivos
    public void DisableDevicePlacement()
    {
        isPlacingDevice = false;
        lockerInteraction = null;
        Debug.Log("Modo de colocaci�n desactivado.");
    }

    // En Interactor.cs
    public void RefreshPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }


}












