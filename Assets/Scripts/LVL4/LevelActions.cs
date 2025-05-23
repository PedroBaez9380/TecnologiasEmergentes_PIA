using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;
using System.Net;
using System;

public enum ButtonActionType { Clean, Validate }
public class LevelActions : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public float interactionDistance = 3.0f; // Distancia de interacción
    public LayerMask interactionLayer;       // Capa para la interacción
    public string cleanButtonText = "Presiona E para Limpiar";
    public string validateButtonText = "Presiona E para Validar";
    public TextMeshProUGUI interactionPromptText; // Asigna el mismo TextMeshProUGUI del Interactor
    public float fadeOutTime = 1.0f;             // Tiempo para desvanecer el texto
    public string ipAssignableTag = "PC4";
    public ButtonActionType buttonType;

    [Header("Objeto a Desactivar")]
    public GameObject objectToDeactivate;


    [Header("Referencias a Objetos")]
    public List<TableProperties> tables;
    public List<IPAsignation> ipAssignableDevices;
    public GameObject unlockableObject; // El objeto con el collider a activar
    public TextMeshProUGUI alertText;  // Asigna tu TextMeshProUGUI para alertas
    public float alertDuration = 2f;    // Duración de las alertas
    private Collider unlockableCollider;
    private bool canInteract = false;
    private GameObject player; // Referencia al jugador
    private bool isValid;



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
        else
        {
            Debug.LogError("No se asignó el TextMeshProUGUI para el prompt de interacción.");
        }

        alertText.gameObject.SetActive(false);

        if (unlockableObject != null)
        {
            unlockableCollider = unlockableObject.GetComponent<Collider>();


            if (unlockableCollider != null)
            {
                unlockableCollider.isTrigger = false; // Asegurar que no sea trigger al inicio
            }
            else
            {
                Debug.LogError("Unlockable Object no tiene un Collider!");
            }
        }
        else
        {
            Debug.LogError("No se asignó el objeto desbloqueable.");
        }
    }

    void Update()
    {
        // Realiza el raycast desde el jugador
        if (player != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, interactionDistance, interactionLayer))
            {
                if (hit.collider.gameObject == this.gameObject) // Verifica si el raycast golpea este objeto (botón)
                {
                    canInteract = true;
                    ShowInteractionPrompt();

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PerformAction(this.gameObject.name); // Usa el nombre del botón para la acción
                    }
                }
                else
                {
                    canInteract = false;
                    HideInteractionPrompt();
                }
            }
            else
            {
                canInteract = false;
                HideInteractionPrompt();
            }
        }
    }

    void ShowInteractionPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = (buttonType == ButtonActionType.Clean) ? cleanButtonText : validateButtonText;
            interactionPromptText.gameObject.SetActive(true);
        }
    }

    void HideInteractionPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    // Este método lo llamarán los botones
    public void PerformAction(string actionType)
    {
        switch (actionType.ToLower())
        {
            case "cleanbutton":
                CleanLevel();
                break;
            case "validatebutton":
                ValidateLevel();
                break;
            default:
                Debug.LogError("Tipo de acción desconocida: " + actionType);
                break;
        }
    }

    void CleanLevel()
    {
        Debug.Log("Limpiando el nivel...");

        // 1. Eliminar dispositivos de las mesas
        foreach (var table in tables)
        {
            table.RemoveDevice();
        }

        // 2. Eliminar dispositivos por tag
        string[] deviceTags = { "PC4", "Router4", "Server4", "Switch4" };
        foreach (string tag in deviceTags)
        {
            GameObject[] devices = GameObject.FindGameObjectsWithTag(tag);
            foreach (var device in devices)
            {
                Destroy(device);
            }
        }

        // 3. Eliminar cables
        GameObject[] cables = GameObject.FindGameObjectsWithTag("Cable4");
        foreach (var cable in cables)
        {
            Destroy(cable);
        }

        // 4. Limpiar PlayerPrefs
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs limpiado.");
        
        ResetTableOccupiedStatus();

        ShowAlert("Nivel Limpiado", alertDuration);
        StartCoroutine(FadeOutText());
    }

    void ValidateLevel()
    {
        Debug.Log("Validando el nivel...");
        isValid = true;

        // 1. Validar mesas ocupadas
        int occupiedTableCount = tables.Count(table => table.IsOccupied());
        Debug.Log(occupiedTableCount);
        if (occupiedTableCount < 5)
        {
            ShowAlert("Faltan dispositivos en las mesas.", alertDuration);
            isValid = false;
            return;
        }

        // 2. Validar conexiones de cables (esto es más complejo)
        if (!AreDevicesConnected())
        {
            ShowAlert("No todos los dispositivos están conectados.", alertDuration);
            isValid = false;
            return;
        }

        // 3. Validar IPs y Máscaras
        if (!AreIPsAndMasksValid())
        {
            ShowAlert("Configuración de red inválida. Verifica las IP y las mascaras de subred", alertDuration);
            isValid = false;
            return;
        }

        // 4. Si todo es válido, desbloquear
        if (isValid)
        {
            ShowAlert("Nivel 3 Completado! Nivel 4 Desbloqueado.", alertDuration);
            EnableUnlockable();
        }
        StartCoroutine(FadeOutText());
    }

    bool AreDevicesConnected()
    {
        // Verificar la cantidad de mesas ocupadas
        int occupiedTableCount = tables.Count(table => table.IsOccupied());

        // Si hay menos de 5 mesas ocupadas, no se cumplen los requisitos
        if (occupiedTableCount < 5)
        {
            ShowAlert("Faltan dispositivos en las mesas.", alertDuration);
            return false;
        }

        // Obtener todos los cables en la escena usando el tag "Cable4"
        GameObject[] allCables = GameObject.FindGameObjectsWithTag("Cable4");

        // Verificar si la cantidad de cables es al menos uno menos que la cantidad de mesas ocupadas
        if (allCables.Length < occupiedTableCount - 1)
        {
            ShowAlert($"Se necesitan al menos {occupiedTableCount - 1} cables para {occupiedTableCount} dispositivos", alertDuration);
            return false;
        }

        // Si la cantidad de cables es suficiente, devolvemos true
        return true;
    }

    // Construye un grafo de conexiones entre dispositivos
    Dictionary<GameObject, List<GameObject>> BuildConnectionGraph(GameObject[] devices, GameObject[] cables)
    {
        var graph = new Dictionary<GameObject, List<GameObject>>();

        // Inicializar el grafo
        foreach (var device in devices)
        {
            graph[device] = new List<GameObject>();
        }

        // Analizar cada cable para construir las conexiones
        foreach (var cable in cables)
        {
            LineRenderer line = cable.GetComponent<LineRenderer>();
            if (line != null && line.positionCount >= 2)
            {
                Vector3 start = line.GetPosition(0);
                Vector3 end = line.GetPosition(1);

                GameObject device1 = FindClosestDevice(start, devices);
                GameObject device2 = FindClosestDevice(end, devices);

                if (device1 != null && device2 != null && device1 != device2)
                {
                    if (!graph[device1].Contains(device2))
                        graph[device1].Add(device2);

                    if (!graph[device2].Contains(device1))
                        graph[device2].Add(device1);
                }
            }
        }

        return graph;
    }

    // Encuentra el dispositivo más cercano a una posición
    GameObject FindClosestDevice(Vector3 position, GameObject[] devices)
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (var device in devices)
        {
            float dist = Vector3.Distance(position, device.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = device;
            }
        }

        return minDistance < 2.0f ? closest : null; // 2.0f es un umbral razonable
    }

    bool AreIPsAndMasksValid()
    {
        // Buscar todos los dispositivos que probablemente tengan IP asignable
        GameObject[] pcDevices = GameObject.FindGameObjectsWithTag("PC4");
        GameObject[] routerDevices = GameObject.FindGameObjectsWithTag("Router4");
        GameObject[] serverDevices = GameObject.FindGameObjectsWithTag("Server4");

        // Combinar todos los dispositivos en un solo array
        GameObject[] ipAssignableDevices = pcDevices.Concat(routerDevices).Concat(serverDevices).ToArray();

        Debug.Log("Dispositivos con posible IP asignable encontrados: " + ipAssignableDevices.Length);

        if (ipAssignableDevices.Length == 0) return true; // Si no hay dispositivos, es válido

        string firstNetworkAddress = "";
        string firstSubnetMask = "";
        bool anyValidAddressFound = false;
        bool networkAddressCalculationFailed = false;

        HashSet<string> seenIPAddresses = new HashSet<string>(); // Para detectar IPs duplicadas

        foreach (GameObject device in ipAssignableDevices)
        {
            if (device != null)
            {
                IPAsignation ipAsignation = device.GetComponent<IPAsignation>();
                if (ipAsignation != null)
                {
                    string deviceKey = $"IP_{device.name}";
                    string maskKey = $"Mask_{device.name}";
                    string ipAddressString = PlayerPrefs.GetString(deviceKey, "");
                    string subnetMaskString = PlayerPrefs.GetString(maskKey, "");

                    Debug.Log($"Device: {device.name}, IP: {ipAddressString}, Mask: {subnetMaskString}");

                    if (string.IsNullOrEmpty(ipAddressString))
                    {
                        ShowAlert($"El dispositivo {device.name} no tiene IP asignada.", alertDuration);
                        return false;
                    }

                    if (string.IsNullOrEmpty(subnetMaskString))
                    {
                        ShowAlert($"El dispositivo {device.name} no tiene Máscara asignada.", alertDuration);
                        return false;
                    }

                    try
                    {
                        IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                        IPAddress subnetMask = IPAddress.Parse(subnetMaskString);

                        if (!ipAsignation.IsValidIPAddress(ipAddressString) || !ipAsignation.IsValidSubnetMask(subnetMaskString))
                        {
                            ShowAlert($"Configuración inválida en {device.name}", alertDuration);
                            return false;
                        }

                        // Verificar IPs duplicadas
                        if (seenIPAddresses.Contains(ipAddressString))
                        {
                            ShowAlert($"La IP {ipAddressString} está duplicada.", alertDuration);
                            return false;
                        }
                        seenIPAddresses.Add(ipAddressString);

                        string networkAddress = GetNetworkAddress(ipAddress, subnetMask);

                        Debug.Log($"Calculated networkAddress: {networkAddress}, firstNetworkAddress: {firstNetworkAddress}, anyValidAddressFound: {anyValidAddressFound}");

                        if (!string.IsNullOrEmpty(networkAddress))
                        {
                            if (!anyValidAddressFound)
                            {
                                firstNetworkAddress = networkAddress;
                                firstSubnetMask = subnetMaskString;
                                anyValidAddressFound = true;
                            }
                            else if (networkAddress != firstNetworkAddress || subnetMaskString != firstSubnetMask)
                            {
                                ShowAlert($"El dispositivo {device.name} está en una red diferente o tiene una máscara diferente.", alertDuration);
                                return false;
                            }
                        }
                        else
                        {
                            networkAddressCalculationFailed = true;
                        }
                    }
                    catch (Exception)
                    {
                        ShowAlert($"Error en la configuración de {device.name}", alertDuration);
                        return false;
                    }
                }
            }
        }

        if (networkAddressCalculationFailed)
        {
            ShowAlert("Error al calcular las direcciones de red.", alertDuration);
            return false;
        }

        if (!anyValidAddressFound && !networkAddressCalculationFailed)
        {
            ShowAlert("Ningún dispositivo tiene configuración de red válida.", alertDuration);
        }

        Debug.Log($"AreIPsAndMasksValid returning: {anyValidAddressFound}");
        return anyValidAddressFound;
    }

    string GetNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
    {
        try
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();

            Debug.Log($"IP Address: {ipAddress}, Subnet Mask: {subnetMask}"); // Log inicial

            // Asegúrate de que las longitudes sean correctas (IPv4)
            if (ipBytes.Length != 4 || maskBytes.Length != 4)
            {
                Debug.LogError("Dirección IP o Máscara de Subred no válida (IPv4).");
                return "";
            }

            byte[] networkBytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            IPAddress networkAddress = new IPAddress(networkBytes);
            Debug.Log($"Calculated Network Address: {networkAddress}"); // Log del resultado
            return networkAddress.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError("Error al calcular la dirección de red: " + e.Message);
            return "";
        }
    }

    void ShowAlert(string message, float duration)
    {
        alertText.text = message;
        alertText.gameObject.SetActive(true);
        StartCoroutine(HideAlertAfterDelay(duration));
    }

    IEnumerator HideAlertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        alertText.gameObject.SetActive(false);
    }

    void EnableUnlockable()
    {
        if (unlockableCollider != null)
        {
            unlockableCollider.isTrigger = true;
            objectToDeactivate.SetActive(false);
        }
    }

    IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(2f);
        HideInteractionPrompt();
    }

    void ResetTableOccupiedStatus()
    {
        GameObject[] tableObjects = GameObject.FindGameObjectsWithTag("Table");
        foreach (GameObject tableObject in tableObjects)
        {
            TableProperties tableProperties = tableObject.GetComponent<TableProperties>();
            if (tableProperties != null)
            {
                tableProperties.isOccupied = false;
            }
        }
    }
}
