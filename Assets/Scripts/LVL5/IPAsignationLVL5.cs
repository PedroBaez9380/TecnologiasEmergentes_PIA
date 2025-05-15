using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class IPAsignationLVL5 : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject ipMenuPrefab; // Cambiado a prefab del men� de IP
    public GameObject player;
    private TMP_InputField ipInput;
    private TMP_InputField subnetMaskInput;
    private TMP_Text warningText;

    private CharacterController playerController;
    private PlayerController playerScript;
    private GameObject currentIpMenuInstance; // Instancia actual del men�

    private string assignedIPAddress = "";
    private string assignedSubnetMask = "";

    private string ipKey;
    private string maskKey;
    private bool isMenuOpen = false;

    void Start()
    {
        

        if (player == null)
        {
            Debug.LogError("El objeto del jugador no est� asignado en IPAsignation.");
            return;
        }

        playerController = player.GetComponent<CharacterController>();
        playerScript = player.GetComponent<PlayerController>();

        if (playerController == null || playerScript == null)
        {
            Debug.LogError("Faltan referencias al CharacterController o al script PlayerController del jugador.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenIPMenu()
    {
        if (isMenuOpen) return;

        if (ipMenuPrefab == null)
        {
            Debug.LogError("�ipMenuPrefab no est� asignado en el inspector!");
            return;
        }

        try
        {
            // Instanciar el men�
            currentIpMenuInstance = Instantiate(ipMenuPrefab, ipMenuPrefab.transform.parent);
            currentIpMenuInstance.SetActive(true);

            // Obtener referencias CON LA RUTA COMPLETA
            ipInput = currentIpMenuInstance.transform.Find("DirIP/IP-Field")?.GetComponent<TMP_InputField>();
            subnetMaskInput = currentIpMenuInstance.transform.Find("Mascara/Mascara-Field")?.GetComponent<TMP_InputField>();
            warningText = currentIpMenuInstance.transform.Find("IP-Validation")?.GetComponent<TMP_Text>();

            // Verificaci�n EXTRA con rutas completas
            if (ipInput == null) Debug.LogError("No se encontr� DirIP/IP-Field");
            if (subnetMaskInput == null) Debug.LogError("No se encontr� Mascara/Mascara-Field");
            if (warningText == null) Debug.LogError("No se encontr� IP-Validation");

            // Configurar botones (estos s� son hijos directos seg�n tu estructura)
            Button saveButton = currentIpMenuInstance.transform.Find("ButtonSave")?.GetComponent<Button>();
            Button exitButton = currentIpMenuInstance.transform.Find("BotonSalir")?.GetComponent<Button>();
            

            Debug.Log("El valor de saveButton es:" + saveButton);
            if (saveButton != null)
            {
                // 1. Limpieza completa previa (como ya tienes)
                ForceClearButtonListeners(saveButton);

                // 2. Asignaci�n del nuevo listener
                saveButton.onClick.AddListener(() =>
                {
                    Debug.Log("Nuevo listener ejecutado en: " + gameObject.name);

                    if (ipInput == null || subnetMaskInput == null)
                    {
                        Debug.LogError("Componentes de entrada no encontrados");
                        return;
                    }

                    AssignIPAddress(gameObject);
                });

                // 3. Opcional: Asignaci�n persistente para el editor (solo durante el desarrollo)
#if UNITY_EDITOR
                UnityEditor.Events.UnityEventTools.AddPersistentListener(
                    saveButton.onClick,
                    new UnityEngine.Events.UnityAction(() => AssignIPAddress(gameObject))
                );
#endif

                // 4. Verificaci�n final
                Debug.Log("=== LISTENER FINAL CONFIGURADO ===");
                DisplayButtonListeners(saveButton);
            }
            else
            {
                Debug.LogError("No se encontr� ButtonSave en el men� instanciado");
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(CloseIPMenu);
            }
            else
            {
                Debug.LogError("No se encontr� BotonSalir");
            }

            // Configurar PlayerPrefs keys
            ipKey = $"IP_{gameObject.name}";
            maskKey = $"Mask_{gameObject.name}";

            LoadIPConfiguration();
            LockPlayerControls(true);
            isMenuOpen = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al abrir el men�: {e.Message}");
            if (currentIpMenuInstance != null)
                Destroy(currentIpMenuInstance);
        }
    }

    // M�todo para limpieza completa de listeners
    private void ForceClearButtonListeners(Button button)
    {
        if (button == null) return;

        // Limpieza en tiempo de ejecuci�n
        button.onClick = new Button.ButtonClickedEvent();

        // Limpieza de listeners persistentes (editor)
#if UNITY_EDITOR
        int persistentCount = button.onClick.GetPersistentEventCount();
        for (int i = persistentCount - 1; i >= 0; i--)
        {
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick, i);
        }
#endif
    }

    // M�todo para mostrar los listeners (como ya lo tienes)
    private void DisplayButtonListeners(Button btn)
    {
        if (btn == null) return;

        int count = btn.onClick.GetPersistentEventCount();
        Debug.Log($"El bot�n '{btn.name}' tiene {count} listeners persistentes");

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"- Listener {i}: " +
                     $"Objeto: {btn.onClick.GetPersistentTarget(i)} | " +
                     $"M�todo: {btn.onClick.GetPersistentMethodName(i)}");
        }

        // Mostrar tambi�n listeners din�micos (no persistentes)
        //Debug.Log($"Listeners din�micos: {btn.onClick.GetInvocationList().Length}");
    }
    //auxiliar, temporal para veriricar listeners de boton guardar interfaz
    //private void DisplayButtonListeners(Button btn)
    //{
    //    if (btn == null) return;

    //    int count = btn.onClick.GetPersistentEventCount();
    //    Debug.Log($"El bot�n '{btn.name}' tiene {count} listeners");

    //    for (int i = 0; i < count; i++)
    //    {
    //        Debug.Log($"- Listener {i}: " +
    //                 $"Objeto: {btn.onClick.GetPersistentTarget(i)} | " +
    //                 $"M�todo: {btn.onClick.GetPersistentMethodName(i)}");
    //    }
    //}

    public void CloseIPMenu()
    {
        if (currentIpMenuInstance != null)
        {
            Destroy(currentIpMenuInstance);
            currentIpMenuInstance = null;
        }

        LockPlayerControls(false);
        isMenuOpen = false;
    }

    public void AssignIPAddress(GameObject targetDevice)
    {
        string ip = ipInput.text.Trim();
        string mask = subnetMaskInput.text.Trim();

        if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(mask))
        {
            warningText.text = "�Por favor, rellena ambos campos!";
            warningText.gameObject.SetActive(true);
            return;
        }

        warningText.gameObject.SetActive(false);

        if (!IsValidIPAddress(ip))
        {
            warningText.text = "Direcci�n IP inv�lida. Formato correcto: ###.###.###.### (0-255)";
            warningText.gameObject.SetActive(true);
            ipInput.text = "";
            return;
        }

        if (!IsValidSubnetMask(mask))
        {
            warningText.text = "M�scara de subred inv�lida. Formato correcto: ###.###.###.### (0-255)";
            warningText.gameObject.SetActive(true);
            subnetMaskInput.text = "";
            return;
        }

        // Limpiar PlayerPrefs antiguos para este dispositivo
        PlayerPrefs.DeleteKey(ipKey);
        PlayerPrefs.DeleteKey(maskKey);

        // Guardar nuevos valores
        assignedIPAddress = ip;
        assignedSubnetMask = mask;
        PlayerPrefs.SetString(ipKey, assignedIPAddress);
        PlayerPrefs.SetString(maskKey, assignedSubnetMask);
        PlayerPrefs.Save();

        Debug.Log($"Configuraci�n guardada para {targetDevice.name}: IP={assignedIPAddress}, M�scara={assignedSubnetMask}");

        CloseIPMenu();
    }

    private void LoadIPConfiguration()
    {
        if (PlayerPrefs.HasKey(ipKey))
        {
            assignedIPAddress = PlayerPrefs.GetString(ipKey);
            if (ipInput != null) ipInput.text = assignedIPAddress;
        }

        if (PlayerPrefs.HasKey(maskKey))
        {
            assignedSubnetMask = PlayerPrefs.GetString(maskKey);
            if (subnetMaskInput != null) subnetMaskInput.text = assignedSubnetMask;
        }
    }

    public bool IsValidIPAddress(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return false;
        string pattern = @"^((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.){3}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$";
        return System.Text.RegularExpressions.Regex.IsMatch(ip, pattern);
    }

    public bool IsValidSubnetMask(string mask)
    {
        if (string.IsNullOrEmpty(mask)) return false;
        string[] validMasks = {
        "255.255.255.255", "255.255.255.254", "255.255.255.252",
        "255.255.255.248", "255.255.255.240", "255.255.255.224",
        "255.255.255.192", "255.255.255.128", "255.255.255.0",
        "255.255.254.0", "255.255.252.0", "255.255.248.0",
        "255.255.240.0", "255.255.224.0", "255.255.192.0",
        "255.255.128.0", "255.255.0.0", "255.254.0.0",
        "255.252.0.0", "255.248.0.0", "255.240.0.0",
        "255.224.0.0", "255.192.0.0", "255.128.0.0",
        "255.0.0.0", "254.0.0.0", "252.0.0.0",
        "248.0.0.0", "240.0.0.0", "224.0.0.0",
        "192.0.0.0", "128.0.0.0", "0.0.0.0"
    };
        return Array.IndexOf(validMasks, mask) != -1;
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

    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }

    public void InteractWithIPAssignable()
    {
        OpenIPMenu();
    }

    public string GetAssignedIPAddress()
    {
        return assignedIPAddress;
    }

    public string GetAssignedSubnetMask()
    {
        return assignedSubnetMask;
    }

    void OnApplicationQuit()
    {
        // Limpiar solo las claves relacionadas con este dispositivo
        PlayerPrefs.DeleteKey($"IP_{gameObject.name}");
        PlayerPrefs.DeleteKey($"Mask_{gameObject.name}");
        PlayerPrefs.Save();

        Debug.Log($"Configuraci�n de red borrada para {gameObject.name}");
    }


}
