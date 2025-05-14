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
    private int pcCounter = 1;
    private int routerCounter = 1;
    private int serverCounter = 1;
    private int switchCounter = 1;

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

        // Asegúrate de que el objeto "player" tenga asignada la PlayerCamera
        if (player == null)
        {
            Debug.LogError("El objeto del jugador no está asignado en LockerInteraction_lvl4.");
            return;
        }

        playerController = player.GetComponent<CharacterController>();
        playerScript = player.GetComponent<PlayerController>();

        if (playerController == null || playerScript == null)
        {
            Debug.LogError("Faltan referencias al CharacterController o al script PlayerController del jugador.");
        }

        // Aquí buscamos el componente Interactor dentro de la PlayerCamera
        interactor = player.GetComponentInChildren<Interactor>();  // Buscar en los hijos del jugador (PlayerCamera)

        if (interactor == null)
        {
            Debug.LogError("No se encontró el componente Interactor en la PlayerCamera.");
        }
    }

    void Update()
    {
        // Verificar si se ha seleccionado una mesa y si está ocupada
        if (selectedTableProperties != null)
        {
            // Si la mesa está ocupada y el jugador presiona 'E', eliminar el dispositivo
            if (selectedTableProperties.IsOccupied() && Input.GetKeyDown(KeyCode.E))
            {
                selectedTableProperties.RemoveDevice();  // Elimina el dispositivo de la mesa
                selectedDeviceText.text = "Dispositivo eliminado de la mesa.";
                selectedDeviceText.gameObject.SetActive(true);  // Muestra el mensaje
            }
            else if (!selectedTableProperties.IsOccupied())
            {
                // Si la mesa no está ocupada, mostrar mensaje para colocar un dispositivo
                selectedDeviceText.text = "Mesa libre. Coloca un dispositivo.";
                selectedDeviceText.gameObject.SetActive(true);
            }
        }

        if (currentDevice != null && Input.GetKeyDown(KeyCode.Z))
        {
            CancelPlacement();
        }
    }

    [System.Serializable]
    public struct DevicePlacementSettings
    {
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
    }
    // Añade esto en LockerInteraction_lvl4.cs
    [Header("Ajustes de Posición")]
    public DevicePlacementSettings pcPlacement;
    public DevicePlacementSettings routerPlacement;
    public DevicePlacementSettings serverPlacement;
    public DevicePlacementSettings switchPlacement;

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
            // Muestra el mensaje de colocación
            // Muestra el mensaje de colocación
            selectedDeviceText.text = "Colocando " + currentDevice.name + "..." + "Presiona Z para cancelar";
            selectedDeviceText.gameObject.SetActive(true);

            // Notifica al Interactor para activar el modo de colocación
            interactor = player.GetComponentInChildren<Interactor>();  // Asegúrate de buscar el Interactor en los hijos si se mueve
            if (interactor != null)
            {
                interactor.EnableDevicePlacement(this);
                //Debug.Log("Modo de colocación activado para " + currentDevice.name);
            }
            else
            {
                Debug.Log("No se encontró el componente Interactor en el jugador.");
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
        interactor.DisableDevicePlacement();  // Asegúrate de tener este método en tu Interactor
        Debug.Log("Colocación del dispositivo cancelada.");
    }


    public GameObject GetCurrentDevice()
    {
        return currentDevice;
    }

    public void PlaceDeviceOnTable(Transform selectedTable)
    {
        if (currentDevice != null && selectedTable != null)
        {
            TableProperties tableProperties = selectedTable.GetComponent<TableProperties>();

            if (tableProperties != null && !tableProperties.IsOccupied())
            {
                // Instanciar el dispositivo en el mundo (no como hijo de la mesa)
                currentDeviceInstance = Instantiate(currentDevice);

                // Variables para el collider y el tag
                Collider deviceCollider = null;
                string deviceTag = "";

                // Configurar posición/rotación, collider y tag basada en el tipo de dispositivo
                switch (deviceDropdown.options[deviceDropdown.value].text)
                {
                    case "PC":
                        currentDeviceInstance.name = "PC_Clone_" + pcCounter;
                        pcCounter++;
                        currentDeviceInstance.transform.position = selectedTable.position + new Vector3(-75f, 5.05f, -138.5f) - new Vector3(-74.28487f, 4.884095f, -138.4746f);
                        //currentDeviceInstance.transform.rotation = selectedTable.rotation * Quaternion.Euler(0f, -76.079f, 0f);
                        deviceTag = "PC4";
                        
                        break;
                    case "Router":
                        currentDeviceInstance.name = "Router_Clone_" + routerCounter;
                        routerCounter++;
                        currentDeviceInstance.transform.position = selectedTable.position + selectedTable.TransformDirection(new Vector3(0f, 0.3f, 0f));
                        //currentDeviceInstance.transform.rotation = selectedTable.rotation;
                        deviceTag = "Router4";
                        
                        break;
                    case "Servidor":
                        currentDeviceInstance.name = "Servidor_Clone_" + serverCounter;
                        serverCounter++;
                        currentDeviceInstance.transform.position = selectedTable.position + new Vector3(-86.62688f, 7.68f, -156.62f) - new Vector3(-86.62688f, 5.184103f, -156.62f);
                        //currentDeviceInstance.transform.rotation = selectedTable.rotation * Quaternion.Euler(0f, 123.595f, 0f) * Quaternion.Euler(0f, -33.381f, 0f);
                        deviceTag = "Server4";
                        
                        break;
                    case "Switch":
                        currentDeviceInstance.name = "Switch_Clone_" + switchCounter;
                        switchCounter++;
                        currentDeviceInstance.transform.position = selectedTable.position + selectedTable.TransformDirection(new Vector3(0f, 0.15f, 0f));
                        //currentDeviceInstance.transform.rotation = selectedTable.rotation;
                        deviceTag = "Switch4";
                        
                        break;
                }
                // Ajustar el collider (opcional, pero recomendado)
                // Ajustar el collider (opcional, pero recomendado)
                

                // Forzar la escala y rotación original del prefab
                currentDeviceInstance.transform.localScale = currentDevice.transform.localScale;
                currentDeviceInstance.transform.localRotation = currentDevice.transform.localRotation; // Aplicar rotación original

                // Asegurar que el objeto tenga el tag correcto
                currentDeviceInstance.tag = deviceTag;

                // Ajustar el collider (opcional, pero recomendado)
                if (deviceCollider != null)
                {
                    // Aquí puedes ajustar el tamaño y el centro del collider si es necesario
                    // Por ejemplo:
                    // deviceCollider.bounds.size = new Vector3(1f, 1f, 1f); // Ajustar tamaño
                    // deviceCollider.center = new Vector3(0f, 0f, 0f); // Ajustar centro
                }

                tableProperties.PlaceDevice(currentDeviceInstance);
                selectedDeviceText.gameObject.SetActive(false);
                currentDevice = null;

                if (interactor != null)
                {
                    interactor.RefreshPrompt();
                    interactor.DisableDevicePlacement();
                }

                Debug.Log($"Dispositivo colocado. Escala: {currentDeviceInstance.transform.localScale}, Tag: {currentDeviceInstance.tag}");
            }
        }
    }

    private DevicePlacementSettings GetPlacementSettings(string deviceName)
    {
        Debug.Log(deviceName);
        switch (deviceName)
        {
            case "PC": return pcPlacement;
            case "Router": return routerPlacement;
            case "Servidor": return serverPlacement;
            case "Switch": return switchPlacement;
            default: return new DevicePlacementSettings();
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











