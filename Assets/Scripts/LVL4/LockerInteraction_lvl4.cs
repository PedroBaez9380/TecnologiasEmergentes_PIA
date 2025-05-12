using UnityEngine;
using TMPro;

public class LockerInteraction_lvl4 : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject lockerMenu;
    public TMP_Dropdown deviceDropdown;
    public GameObject routerPrefab;
    public GameObject pcPrefab;
    public GameObject serverPrefab;
    public GameObject switchPrefab;
    public Transform[] tableSpawnPoints;
    public GameObject player;
    public TMP_Text selectedDeviceText;
    public TableProperties selectedTableProperties;

    private GameObject currentDevice;
    private CharacterController playerController;
    private PlayerController playerScript;
    private Interactor interactor;  // Referencia al interactor
    private GameObject currentDeviceInstance;

    void Start()
    {
        selectedDeviceText.gameObject.SetActive(false);

        if (lockerMenu != null)
        {
            lockerMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("No se ha asignado el objeto lockerMenu en LockerInteraction_lvl4.");
        }

        // Aseg�rate de que el objeto "player" tenga asignada la PlayerCamera
        if (player == null)
        {
            Debug.LogError("El objeto del jugador no est� asignado en LockerInteraction_lvl4.");
            return;
        }

        playerController = player.GetComponent<CharacterController>();
        playerScript = player.GetComponent<PlayerController>();

        if (playerController == null || playerScript == null)
        {
            Debug.LogError("Faltan referencias al CharacterController o al script PlayerController del jugador.");
        }

        // Aqu� buscamos el componente Interactor dentro de la PlayerCamera
        interactor = player.GetComponentInChildren<Interactor>();  // Buscar en los hijos del jugador (PlayerCamera)

        if (interactor == null)
        {
            Debug.LogError("No se encontr� el componente Interactor en la PlayerCamera.");
        }
    }

    void Update()
    {
        // Verificar si se ha seleccionado una mesa y si est� ocupada
        if (selectedTableProperties != null)
        {
            // Si la mesa est� ocupada y el jugador presiona 'E', eliminar el dispositivo
            if (selectedTableProperties.IsOccupied() && Input.GetKeyDown(KeyCode.E))
            {
                selectedTableProperties.RemoveDevice();  // Elimina el dispositivo de la mesa
                selectedDeviceText.text = "Dispositivo eliminado de la mesa.";
                selectedDeviceText.gameObject.SetActive(true);  // Muestra el mensaje
            }
            else if (!selectedTableProperties.IsOccupied())
            {
                // Si la mesa no est� ocupada, mostrar mensaje para colocar un dispositivo
                selectedDeviceText.text = "Mesa libre. Coloca un dispositivo.";
                selectedDeviceText.gameObject.SetActive(true);
            }
        }

        if (currentDevice != null && Input.GetKeyDown(KeyCode.Z))
        {
            CancelPlacement();
        }
    }

    public void InteractWithLocker()
    {
        if (!lockerMenu.activeSelf)
        {
            OpenLockerMenu();
        }
        else
        {
            CloseLockerMenu();
        }
    }

    public void OpenLockerMenu()
    {
        lockerMenu.SetActive(true);
        LockPlayerControls(true);

        if (deviceDropdown != null)
        {
            deviceDropdown.value = 0;
        }
    }

    public void CloseLockerMenu()
    {
        lockerMenu.SetActive(false);
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

    public void SelectDevice()
    {
        currentDevice = null;

        switch (deviceDropdown.options[deviceDropdown.value].text)
        {
            case "Router":
                currentDevice = routerPrefab;
                break;
            case "PC":
                currentDevice = pcPrefab;
                break;
            case "Servidor":
                currentDevice = serverPrefab;
                break;
            case "Switch":
                currentDevice = switchPrefab;
                break;
        }
    }

    public void StartPlacementDevice()
    {
        if (currentDevice != null)
        {
            // Muestra el mensaje de colocaci�n
            selectedDeviceText.text = "Colocando " + currentDevice.name + "..." + "Presiona Z para cancelar";
            selectedDeviceText.gameObject.SetActive(true);

            // Notifica al Interactor para activar el modo de colocaci�n
            interactor = player.GetComponentInChildren<Interactor>();  // Aseg�rate de buscar el Interactor en los hijos si se mueve
            if (interactor != null)
            {
                interactor.EnableDevicePlacement(this);
                Debug.Log("Modo de colocaci�n activado para " + currentDevice.name);
            }
            else
            {
                Debug.Log("No se encontr� el componente Interactor en el jugador.");
            }
        }
        else
        {
            Debug.Log("No se ha seleccionado un dispositivo.");
        }
    }

    public void CancelPlacement()
    {
        currentDevice = null;
        selectedDeviceText.gameObject.SetActive(false);
        interactor.DisableDevicePlacement();  // Aseg�rate de tener este m�todo en tu Interactor
        Debug.Log("Colocaci�n del dispositivo cancelada.");
    }


    public GameObject GetCurrentDevice()
    {
        return currentDevice;
    }

    public void PlaceDeviceOnTable(Transform selectedTable)
    {
        // Si se ha seleccionado un dispositivo y la mesa est� vac�a
        if (currentDevice != null && selectedTable != null)
        {
            // Accede al script TableProperties y verifica si la mesa est� ocupada
            TableProperties tableProperties = selectedTable.GetComponent<TableProperties>();

            if (tableProperties != null)
            {
                if (tableProperties.IsOccupied()) // Si la mesa ya est� ocupada
                {
                    // Si ya hay un dispositivo en la mesa, mostramos un mensaje
                    Debug.Log("La mesa ya est� ocupada. No puedes colocar otro dispositivo.");
                    selectedDeviceText.text = "La mesa ya est� ocupada. No puedes colocar otro dispositivo.";
                    selectedDeviceText.gameObject.SetActive(true);
                    return; // Salimos de la funci�n, no colocamos el nuevo dispositivo
                }

                // Coloca el nuevo dispositivo en la mesa
                currentDeviceInstance = Instantiate(currentDevice, selectedTable.position, selectedTable.rotation);
                tableProperties.PlaceDevice(currentDeviceInstance); // Marca la mesa como ocupada

                //selectedTable.tag = "TableOcuppied";  // Cambia el tag de la mesa (si es necesario)

                // Desactiva el texto de selecci�n
                selectedDeviceText.gameObject.SetActive(false);

                Debug.Log("Dispositivo " + currentDevice.name + " colocado en la mesa.");

                // Limpia la referencia del dispositivo despu�s de usarlo
                currentDevice = null;
                interactor.RefreshPrompt();
            }
            else
            {
                Debug.LogWarning("El script TableProperties no est� asignado a la mesa.");
            }
        }
        
    }




    public void CloseLockerMenuButton()
    {
        CloseLockerMenu();
    }

    public void OpenLockerMenuButton()
    {
        OpenLockerMenu();
    }
}











