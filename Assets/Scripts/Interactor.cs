using UnityEngine;
using TMPro;

public class Interactor : MonoBehaviour
{
    [Header("Configuración de Interacción")]
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
            // Dibujar el raycast para debug (línea roja)
            Debug.DrawLine(transform.position, hit.point, Color.red);

            // Verifica si golpeó una mesa
            if (hit.collider.CompareTag("Table"))
            {
                Debug.Log("Detecto la mesa");
                selectedTable = hit.collider.transform;
                TableProperties tableProperties = selectedTable.GetComponent<TableProperties>();

                if (tableProperties != null)
                {
                    // Si la mesa está ocupada, muestra el mensaje para eliminar el dispositivo
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
                        // Si la mesa no está ocupada, muestra que puedes colocar un dispositivo
                        if (lockerInteraction != null && interactionPromptText != null && !interactionPromptText.gameObject.activeSelf && lockerInteraction.GetCurrentDevice() != null)

                        {
                            interactionPromptText.text = "Presiona E para colocar un dispositivo";
                            interactionPromptText.gameObject.SetActive(true);
                        }

                        // Verifica si estás en el modo de colocar un dispositivo
                        if (isPlacingDevice && Input.GetKeyDown(KeyCode.E))
                        {
                            // Aquí, agregarás la lógica de colocar el dispositivo en la mesa
                            lockerInteraction.PlaceDeviceOnTable(selectedTable);
                            isPlacingDevice = false;
                        }
                    }
                }
            }
            // Verifica si golpeó un LockerInteraction_lvl4 (interacción con locker)
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
            // Verifica si golpeó un Button (interacción con botón)
            else if (hit.collider.GetComponent<ButtonProperties>() != null)
            {
                buttonProperties = hit.collider.GetComponent<ButtonProperties>();

                if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.text = "Presiona E para presionar el botón";
                    interactionPromptText.gameObject.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    buttonProperties.Interact();
                }
            }
            else
            {
                // Si no hay nada sobre lo que interactuar, desactiva el texto de interacción
                if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // Si no detecta nada con el raycast, desactiva el texto de interacción
            if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }

    // Método para activar el modo de colocación de dispositivos
    public void EnableDevicePlacement(LockerInteraction_lvl4 locker)
    {
        lockerInteraction = locker;
        isPlacingDevice = true;
        Debug.Log("Modo de colocación activado.");
    }

    // Desactiva el modo de colocación de dispositivos
    public void DisableDevicePlacement()
    {
        isPlacingDevice = false;
        lockerInteraction = null;
        Debug.Log("Modo de colocación desactivado.");
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












