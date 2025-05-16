using UnityEngine;
using System.Collections;
using System.Net;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class Level5Validation : MonoBehaviour
{
    [Header("Referencias a Objetos")]
    public GameObject monitor43;
    public GameObject monitor32;
    public GameObject cableFijo10Cross;
    public Camera camaraSalon5;
    public Animator pingAnimator;
    public float cameraDelay = 2f;
    public float animationDelay = 12f;
    public TextMeshProUGUI alertText;
    public float alertDuration = 2f;
    public GameObject PantallaFinal;

    [Header("Configuración de Interacción")]
    public float interactionDistance = 3.0f;
    public LayerMask interactionLayer;
    public string validateButtonText = "Presiona E para Validar";
    public TextMeshProUGUI interactionPromptText;

    private Camera originalMainCamera;
    private bool isValid;
    private bool pingAnimationActive = false;
    private GameObject player;
    private PlayerController playerController;
    private Interactor interactor;
    private bool validationAttempted = false;
    private bool playerControlsLocked = false;
    private BoxCollider interactionCollider;

    void Start()
    {
        // ... (Your existing Start() code - this is fine)
        if (monitor43 == null || monitor32 == null || cableFijo10Cross == null || camaraSalon5 == null || pingAnimator == null || alertText == null || interactionPromptText == null)
        {
            Debug.LogError("Faltan referencias a objetos del Nivel 5.");
            Debug.Break();
            return;
        }

        originalMainCamera = GameObject.Find("PlayerCamera")?.GetComponent<Camera>();
        alertText.gameObject.SetActive(false);
        interactionPromptText.gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("No se encontró el jugador con el tag 'Player'.");
        }

        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("No se encontró el componente PlayerController en el jugador.");
        }

        GameObject playerCamera = GameObject.Find("PlayerCamera");
        if (playerCamera != null)
        {
            interactor = playerCamera.GetComponent<Interactor>();
            if (interactor == null)
            {
                Debug.LogError("No se encontró el componente Interactor en PlayerCamera.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto PlayerCamera.");
        }

        PantallaFinal.gameObject.SetActive(false);
        SetPlayerControls(false);

        interactionCollider = GetComponent<BoxCollider>();
        if (interactionCollider == null)
        {
            Debug.LogError("No se encontró BoxCollider en este objeto.");
        }
    }

    void Update()
    {
        // ... (Your existing Update() code - this is fine)
        if (player != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, interactionDistance, interactionLayer))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    ShowInteractionPrompt();

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ValidateAndRunSequence();
                    }
                }
                else
                {
                    HideInteractionPrompt();
                }
            }
            else
            {
                HideInteractionPrompt();
            }
        }
    }

    public void ValidateAndRunSequence()
    {
        // ... (Your existing ValidateAndRunSequence() code - this is fine)
        Debug.Log("ValidateAndRunSequence() called.");
        isValid = true;
        validationAttempted = true;

        PrintMonitorIPs();

        if (!AreIPsAndMasksValid())
        {
            Debug.Log("AreIPsAndMasksValid() returned false.");
            isValid = false;
            ShowAlert("Configuración de red incorrecta. Verifica las IPs y la máscara.", alertDuration, false);
            return;
        }
        Debug.Log("AreIPsAndMasksValid() returned true.");

        if (!IsCableConnected())
        {
            Debug.Log("IsCableConnected() returned false.");
            isValid = false;
            ShowAlert("El cable no está conectado.", alertDuration, false);
            return;
        }
        Debug.Log("IsCableConnected() returned true.");

        if (isValid)
        {
            Debug.Log("Validation successful. Starting Nivel5Sequence().");

            // Desactivar el collider PERMANENTEMENTE después de la validación exitosa
            if (interactionCollider != null)
            {
                interactionCollider.enabled = false;
            }

            originalMainCamera.gameObject.SetActive(false);
            camaraSalon5.gameObject.SetActive(true);

            HideInteractionPrompt();
            if (interactor != null)
            {
                HideInteractionPrompt();
            }

            pingAnimator.SetBool("PingActivo", true);
            pingAnimationActive = true;

            StartCoroutine(Nivel5Sequence());
        }
        else
        {
            Debug.Log("Validation failed.");
        }
    }

    bool AreIPsAndMasksValid()
    {
        bool allValid = true;
        bool anyValidAddressFound = false;
        string firstNetworkAddress = "";
        string firstSubnetMask = "";
        HashSet<string> seenIPAddresses = new HashSet<string>();

        // Validar Monitor43
        if (monitor43 != null)
        {
            IPAsignationLVL5 ipAsignation43 = monitor43.GetComponent<IPAsignationLVL5>();
            if (ipAsignation43 != null)
            {
                string ip43 = PlayerPrefs.GetString($"IP_{monitor43.name}", "");
                string mask43 = PlayerPrefs.GetString($"Mask_{monitor43.name}", "");

                if (string.IsNullOrEmpty(ip43) || string.IsNullOrEmpty(mask43))
                {
                    ShowAlert($"IP o Máscara no asignada en {monitor43.name}", alertDuration, false);
                    return false; // Salir inmediatamente si falta algo
                }

                if (!ipAsignation43.IsValidIPAddress(ip43) || !ipAsignation43.IsValidSubnetMask(mask43))
                {
                    ShowAlert($"Configuración inválida en {monitor43.name}", alertDuration, false);
                    return false; // Salir si la IP o Máscara no es válida
                }

                if (seenIPAddresses.Contains(ip43))
                {
                    ShowAlert($"La IP {ip43} está duplicada.", alertDuration, false);
                    return false; // Salir si la IP está duplicada
                }
                seenIPAddresses.Add(ip43);

                firstNetworkAddress = GetNetworkAddress(IPAddress.Parse(ip43), IPAddress.Parse(mask43));
                firstSubnetMask = mask43;
                anyValidAddressFound = true;
            }
            else
            {
                Debug.LogError("No se encontró IPAsignationLVL5 en " + monitor43.name);
                return false; // Salir si falta el componente
            }
        }
        else
        {
            Debug.LogError("Referencia a monitor43 es null en AreIPsAndMasksValid()");
            return false; // Salir si la referencia es nula
        }

        // Validar Monitor32 (similar a Monitor43)
        if (monitor32 != null)
        {
            IPAsignationLVL5 ipAsignation32 = monitor32.GetComponent<IPAsignationLVL5>();
            if (ipAsignation32 != null)
            {
                string ip32 = PlayerPrefs.GetString($"IP_{monitor32.name}", "");
                string mask32 = PlayerPrefs.GetString($"Mask_{monitor32.name}", "");

                if (string.IsNullOrEmpty(ip32) || string.IsNullOrEmpty(mask32))
                {
                    ShowAlert($"IP o Máscara no asignada en {monitor32.name}", alertDuration, false);
                    return false;
                }

                if (!ipAsignation32.IsValidIPAddress(ip32) || !ipAsignation32.IsValidSubnetMask(mask32))
                {
                    ShowAlert($"Configuración inválida en {monitor32.name}", alertDuration, false);
                    return false;
                }

                if (seenIPAddresses.Contains(ip32))
                {
                    ShowAlert($"La IP {ip32} está duplicada.", alertDuration, false);
                    return false;
                }
                seenIPAddresses.Add(ip32);

                string networkAddress32 = GetNetworkAddress(IPAddress.Parse(ip32), IPAddress.Parse(mask32));
                if (networkAddress32 != firstNetworkAddress || mask32 != firstSubnetMask)
                {
                    ShowAlert($"El dispositivo {monitor32.name} está en una red diferente o tiene una máscara diferente.", alertDuration, false);
                    return false;
                }
            }
            else
            {
                Debug.LogError("No se encontró IPAsignationLVL5 en " + monitor32.name);
                return false;
            }
        }
        else
        {
            Debug.LogError("Referencia a monitor32 es null en AreIPsAndMasksValid()");
            return false;
        }

        return allValid;
    }

    bool IsCableConnected()
    {
        // ... (Your existing IsCableConnected() code - this is fine)
        Debug.Log("IsCableConnected() called.");
        Debug.Log($"cableFijo10Cross.activeSelf: {cableFijo10Cross.activeSelf}");

        if (!cableFijo10Cross.activeSelf)
        {
            Debug.Log("Cable is not active.");
            ShowAlert("El cable no está conectado.", alertDuration, false);
            return false;
        }
        Debug.Log("Cable is active. IsCableConnected() returning true.");
        return true;
    }

    IEnumerator Nivel5Sequence()
    {
        // ... (Your existing Nivel5Sequence() code - this is fine)
        yield return new WaitForSeconds(cameraDelay);

        yield return new WaitForSeconds(animationDelay);

        camaraSalon5.gameObject.SetActive(false);
        originalMainCamera.gameObject.SetActive(true);
        if (pingAnimationActive)
        {
            pingAnimator.SetBool("PingActivo", false);
            pingAnimationActive = false;
        }

        ShowAlert("Nivel completado!", alertDuration, true);
        PantallaFinal.gameObject.SetActive(true);
    }

    string GetNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
    {
        // ... (Your existing GetNetworkAddress() code - this is fine)
        try
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();

            Debug.Log($"IP Address: {ipAddress}, Subnet Mask: {subnetMask}");

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
            Debug.Log($"Calculated Network Address: {networkAddress}");
            return networkAddress.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError("Error al calcular la dirección de red: " + e.Message);
            return "";
        }
    }

    void ShowAlert(string message, float duration, bool blockControls)
    {
        // ... (Your existing ShowAlert() code - this is fine)
        alertText.text = message;
        alertText.gameObject.SetActive(true);
        if (blockControls)
        {
            SetPlayerControls(true);
        }
        StartCoroutine(HideAlertAfterDelay(duration));
    }

    IEnumerator HideAlertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        alertText.gameObject.SetActive(false);
        //  **** REMOVED UNLOCKING LOGIC ****
        //if (validationAttempted && playerControlsLocked)
        //{
        //    SetPlayerControls(false);
        //    validationAttempted = false;
        //}
    }

    void ShowInteractionPrompt()
    {
        // ... (Your existing ShowInteractionPrompt() code - this is fine)
        if (interactionPromptText != null)
        {
            interactionPromptText.text = validateButtonText;
            interactionPromptText.gameObject.SetActive(true);
        }
    }

    void HideInteractionPrompt()
    {
        // ... (Your existing HideInteractionPrompt() code - this is fine)
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    public void HideFinalScreen()
    {
        // ... (Your existing HideFinalScreen() code - this is the CORRECT place to unlock)
        PantallaFinal.gameObject.SetActive(false);
        SetPlayerControls(false);
    }

    private void SetPlayerControls(bool lockControls)
    {
        // ... (Your existing SetPlayerControls() code - this is fine)
        playerControlsLocked = lockControls;
        if (playerController != null)
        {
            playerController.enabled = !lockControls;
        }
        Cursor.lockState = lockControls ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockControls;
    }

    void PrintMonitorIPs()
    {
        if (monitor43 != null)
        {
            IPAsignationLVL5 ipAsignation = monitor43.GetComponent<IPAsignationLVL5>(); // Corregido aquí
            if (ipAsignation != null)
            {
                string ip = PlayerPrefs.GetString($"IP_{monitor43.name}", "No asignada");
                string mask = PlayerPrefs.GetString($"Mask_{monitor43.name}", "No asignada");
                Debug.Log($"Monitor43 - IP: {ip}, Máscara: {mask}");
            }
            else
            {
                Debug.Log("Monitor43 no tiene componente IPAsignationLVL5.");
            }
        }
        // ... (Código similar para monitor32, también corregido)
    }
}