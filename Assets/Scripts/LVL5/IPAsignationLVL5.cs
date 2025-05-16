using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class IPAsignationLVL5 : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject ipMenuPrefab;
    public GameObject player;
    private TMP_InputField ipInput;
    private TMP_InputField subnetMaskInput;
    private TMP_Text warningText;


    [Header("Configuración de IP por Defecto")]
    public string defaultIPAddress = "192.168.1.100"; // IP por defecto (puedes cambiarla en el Inspector)
    public string defaultSubnetMask = "255.255.255.0"; // Máscara por defecto (puedes cambiarla en el Inspector)

    private CharacterController playerController;
    private PlayerController playerScript;
    private GameObject currentIpMenuInstance;

    private string assignedIPAddress = "";
    private string assignedSubnetMask = "";

    private string ipKey;
    private string maskKey;
    private bool isMenuOpen = false;

    void Awake() // Usamos Awake para asegurarnos de que se ejecute antes de Start
    {
        // Limpiar y establecer las IPs por defecto al inicio
        ClearAndSetDefaultIPs();
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("El objeto del jugador no está asignado en IPAsignation.");
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

        // Configurar PlayerPrefs keys
        ipKey = $"IP_{gameObject.name}";
        maskKey = $"Mask_{gameObject.name}";

        LoadIPConfiguration(); // Cargar la configuración al inicio
    }

    private void ClearAndSetDefaultIPs()
    {
        // Borrar cualquier IP asignada previamente
        PlayerPrefs.DeleteKey(ipKey);
        PlayerPrefs.DeleteKey(maskKey);

        // Establecer las IPs por defecto
        PlayerPrefs.SetString(ipKey, defaultIPAddress);
        PlayerPrefs.SetString(maskKey, defaultSubnetMask);
        PlayerPrefs.Save(); // Guardar inmediatamente
    }

    public void OpenIPMenu()
    {
        if (isMenuOpen) return;

        if (ipMenuPrefab == null)
        {
            Debug.LogError("¡ipMenuPrefab no está asignado en el inspector!");
            return;
        }

        try
        {
            currentIpMenuInstance = Instantiate(ipMenuPrefab, ipMenuPrefab.transform.parent);
            currentIpMenuInstance.SetActive(true);

            ipInput = currentIpMenuInstance.transform.Find("DirIP/IP-Field")?.GetComponent<TMP_InputField>();
            subnetMaskInput = currentIpMenuInstance.transform.Find("Mascara/Mascara-Field")?.GetComponent<TMP_InputField>();
            warningText = currentIpMenuInstance.transform.Find("IP-Validation")?.GetComponent<TMP_Text>();

            if (ipInput == null) Debug.LogError("No se encontró DirIP/IP-Field");
            if (subnetMaskInput == null) Debug.LogError("No se encontró Mascara/Mascara-Field");
            if (warningText == null) Debug.LogError("No se encontró IP-Validation");

            Button saveButton = currentIpMenuInstance.transform.Find("ButtonSave")?.GetComponent<Button>();
            Button exitButton = currentIpMenuInstance.transform.Find("BotonSalir")?.GetComponent<Button>();

            if (saveButton != null)
            {
                ForceClearButtonListeners(saveButton);
                saveButton.onClick.AddListener(() =>
                {
                    AssignIPAddress(gameObject);
                });

#if UNITY_EDITOR
                UnityEditor.Events.UnityEventTools.AddPersistentListener(
                    saveButton.onClick,
                    new UnityEngine.Events.UnityAction(() => AssignIPAddress(gameObject))
                );
#endif
            }
            else
            {
                Debug.LogError("No se encontró ButtonSave en el menú instanciado");
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(CloseIPMenu);
            }
            else
            {
                Debug.LogError("No se encontró BotonSalir");
            }

            // Cargar la configuración en el menú
            LoadIPConfigurationIntoMenu(); // Cargar la configuración en el menú
            LockPlayerControls(true);
            isMenuOpen = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al abrir el menú: {e.Message}");
            if (currentIpMenuInstance != null)
                Destroy(currentIpMenuInstance);
        }
    }

    private void ForceClearButtonListeners(Button button)
    {
        if (button == null) return;

        button.onClick = new Button.ButtonClickedEvent();

#if UNITY_EDITOR
        int persistentCount = button.onClick.GetPersistentEventCount();
        for (int i = persistentCount - 1; i >= 0; i--)
        {
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick, i);
        }
#endif
    }

    private void LoadIPConfiguration()
    {
        // Cargar las IPs asignadas por el usuario (si existen)
        if (PlayerPrefs.HasKey(ipKey))
        {
            assignedIPAddress = PlayerPrefs.GetString(ipKey);
        }
        else
        {
            assignedIPAddress = defaultIPAddress; // Cargar la IP por defecto
        }

        if (PlayerPrefs.HasKey(maskKey))
        {
            assignedSubnetMask = PlayerPrefs.GetString(maskKey);
        }
        else
        {
            assignedSubnetMask = defaultSubnetMask; // Cargar la máscara por defecto
        }
    }

    private void LoadIPConfigurationIntoMenu()
    {
        if (ipInput != null) ipInput.text = assignedIPAddress;
        if (subnetMaskInput != null) subnetMaskInput.text = assignedSubnetMask;
    }

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
            warningText.text = "¡Por favor, rellena ambos campos!";
            warningText.gameObject.SetActive(true);
            return;
        }

        warningText.gameObject.SetActive(false);

        if (!IsValidIPAddress(ip))
        {
            warningText.text = "Dirección IP inválida. Formato correcto: ###.###.###.### (0-255)";
            warningText.gameObject.SetActive(true);
            ipInput.text = "";
            return;
        }

        if (!IsValidSubnetMask(mask))
        {
            warningText.text = "Máscara de subred inválida. Formato correcto: ###.###.###.### (0-255)";
            warningText.gameObject.SetActive(true);
            subnetMaskInput.text = "";
            return;
        }

        // Guardar los nuevos valores (sobrescribiendo los anteriores)
        assignedIPAddress = ip;
        assignedSubnetMask = mask;
        PlayerPrefs.SetString(ipKey, assignedIPAddress);
        PlayerPrefs.SetString(maskKey, assignedSubnetMask);
        PlayerPrefs.Save();

        Debug.Log($"Configuración guardada para {targetDevice.name}: IP={assignedIPAddress}, Máscara={assignedSubnetMask}");

        CloseIPMenu();
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
        "255.255.254.0", "255.252.0", "255.248.0",
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

        Debug.Log($"Configuración de red borrada para {gameObject.name}");
    }

    
}