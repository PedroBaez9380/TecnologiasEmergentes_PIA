using UnityEngine;
using TMPro;
using System;
using System.IO;


public class Interactor : MonoBehaviour
{
    [Header("Interfaz")]
    public GameObject interactionPanel;
    public TextMeshProUGUI interactionText;

    [Header("Configuración de Interacción")]
    public float interactionDistance = 3.0f;
    public LayerMask interactionLayer;
    public Material cableMaterial;


    [Header("Elementos UI")]
    public TextMeshProUGUI interactionPromptText;
    public TextMeshProUGUI interactionPromptTextExtra;
    public TextMeshProUGUI interactionPromptTextExtraLVL5;
    public TextMeshProUGUI interactionPromtptLocker;
    public TMP_Text selectedDeviceText;

    public GameObject CableLVL5;


    private GameObject currentHitObject;
    private LockerInteraction_lvl4 lockerInteraction;
    private ButtonProperties buttonProperties;
    private Transform selectedTable;
    private bool isPlacingDevice = false;

    private GameObject selectedDevice = null; // Para el primer dispositivo seleccionado
    private GameObject targetDevice = null;   // Para el segundo dispositivo seleccionado
    private GameObject currentCable = null;  // Para el LineRenderer
    private IPAsignation ipAsignation;
    private IPAsignationLVL5 ipAsignationLVL5;

    //Para el archivo de logs
    private string logFilePath;
    private string logFileName = "my_custom_log.txt";

    void Start()
    {
        logFilePath = Application.persistentDataPath + "/" + logFileName;
        Debug.Log("Log file path: " + logFilePath); // Para debug en el editor

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

        bool isLookingAtTable = false;  // Nueva variable para saber si estás mirando una mesa

        // Realiza el raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactionLayer))
        {
            // Dibujar el raycast para debug (línea roja)
            Debug.DrawLine(transform.position, hit.point, Color.red);

            GameObject hitObject = hit.collider.gameObject;

            // Verifica si golpeó una mesa
            if (hit.collider.CompareTag("Table"))
            {
                isLookingAtTable = true;  // Establece que estamos mirando una mesa
                //Debug.Log("Detecto la mesa");
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

                            // Cambia el material solo cuando el texto de interacción está activo
                            tableProperties.Highlight(true);  // Activar el material de eliminación
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
                            interactionPromptText.text = "<color=black>Presiona E para colocar un dispositivo</color>";
                            interactionPromptText.gameObject.SetActive(true);
                        }

                        // Verifica si estás en el modo de colocar un dispositivo
                        if (isPlacingDevice && Input.GetKeyDown(KeyCode.E))
                        {
                            lockerInteraction.PlaceDeviceOnTable(selectedTable);
                            isPlacingDevice = false;
                        }
                    }
                }
            }
            // Verifica si golpeó un LockerInteraction_lvl4 (interacción con locker)
            else if (hit.collider.GetComponent<LockerInteraction_lvl4>() != null)
            {
                interactionPromptText.text = "Presiona E para abrir el locker";
                interactionPromptText.gameObject.SetActive(true);
                lockerInteraction = hit.collider.GetComponent<LockerInteraction_lvl4>();

                //if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                //{
                //    interactionPromptText.text = "Presiona E para abrir el locker";
                //    interactionPromptText.gameObject.SetActive(false);
                //}

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

            //LVL4
            else if (hitObject.CompareTag("PC4") || hitObject.CompareTag("Router4") || hitObject.CompareTag("Switch4") || hitObject.CompareTag("Server4"))
            {
                
                interactionPromptText.text = "Presiona E para conectar cable";
                interactionPromptText.gameObject.SetActive(true);

                // Lógica para Conectar Cables (Código Existente)
                if (selectedDevice == null)
                {
                    // Seleccionar el primer dispositivo
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        selectedDevice = hitObject;
                        selectedDeviceText.text = "Selecciona otro dispositivo para conectar. Para cancelar, presiona Z mientras miras al dispositivo";
                        selectedDeviceText.gameObject.SetActive(true);
                        interactionPromptText.gameObject.SetActive(false);
                    }
                }
                else if (selectedDevice != hitObject)
                {
                    // Seleccionar el segundo dispositivo
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        LogToFile("Comienza conexion de cable entre dispositivos.");
                        targetDevice = hitObject;
                        LogToFile("Selected Device:" + selectedDevice);
                        LogToFile("Target Device:" + targetDevice);

                        bool canConnectResult = CanConnect(selectedDevice, targetDevice); // Guarda el resultado en una variable
                        LogToFile("Resultado de CanConnect: " + canConnectResult.ToString());

                        if (CanConnect(selectedDevice, targetDevice))
                        {
                            // Conectar los dispositivos
                            CreateCable(GetDeviceCenter(selectedDevice), GetDeviceCenter(targetDevice));

                            // Limpiar la selección
                            LogToFile("Comienza limpiar seleccion.");
                            selectedDevice = null;
                            targetDevice = null;
                            interactionPromptText.gameObject.SetActive(false);
                            selectedDeviceText.gameObject.SetActive(false);
                            LogToFile("Finaliza limpiar seleccion.");
                        }
                        else
                        {
                            interactionPromptText.text = "No se pueden conectar.";
                            interactionPromptText.gameObject.SetActive(true);
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    selectedDevice = null;
                    selectedDeviceText.gameObject.SetActive(false);
                    interactionPromptText.gameObject.SetActive(false); // Opcional: Ocultar el texto principal también
                }

                //Lógica para Asignar IP (Bloque Separado)
                if (hitObject.GetComponent<IPAsignation>() != null)
                {
                    ipAsignation = hitObject.GetComponent<IPAsignation>();
                    interactionPromptTextExtra.text = "Presiona F para configurar IP";
                    interactionPromptTextExtra.gameObject.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.F)) //&& !ipAsignation.IsMenuOpen()
                    {
                        interactionPromptTextExtra.gameObject.SetActive(false);
                        ipAsignation.InteractWithIPAssignable();
                    }
                }
            }
            else if (hitObject.GetComponent<IPAsignationLVL5>() != null && hitObject.CompareTag("PCLVL5"))
            {
                
                ipAsignationLVL5 = hitObject.GetComponent<IPAsignationLVL5>();
                interactionPromptTextExtraLVL5.text = "Presiona F para configurar IP";
                interactionPromptTextExtraLVL5.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F)) //&& !ipAsignation.IsMenuOpen()
                {
                    Debug.Log("Si detecta la F");
                    interactionPromptTextExtraLVL5.gameObject.SetActive(false);
                    ipAsignationLVL5.InteractWithIPAssignable();
                }
            }
            else if (hitObject.CompareTag("SWITCHLVL5"))
            {

                
                interactionPromptText.text = "Presiona E para conectar cable";
                interactionPromptText.gameObject.SetActive(true);

                // Lógica para Conectar Cables (Código Existente)
                if (selectedDevice == null)
                {
                    // Seleccionar el primer dispositivo
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        selectedDevice = hitObject;
                        selectedDeviceText.text = "Selecciona otro dispositivo para conectar. Para cancelar, presiona Z mientras miras al dispositivo";
                        selectedDeviceText.gameObject.SetActive(true);
                        interactionPromptText.gameObject.SetActive(false);
                    }
                }
                else if (selectedDevice != hitObject)
                {
                    // Seleccionar el segundo dispositivo
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        targetDevice = hitObject;
                        if (CanConnect(selectedDevice, targetDevice))
                        {
                            EnableCable();

                            // Limpiar la selección
                            selectedDevice = null;
                            targetDevice = null;
                            interactionPromptText.gameObject.SetActive(false);
                            selectedDeviceText.gameObject.SetActive(false);
                        }
                        else
                        {
                            interactionPromptText.text = "No se pueden conectar.";
                            interactionPromptText.gameObject.SetActive(true);
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    selectedDevice = null;
                    selectedDeviceText.gameObject.SetActive(false);
                    interactionPromptText.gameObject.SetActive(false); // Opcional: Ocultar el texto principal también
                }
            }

            else
            {

                interactionPromptText.gameObject.SetActive(false);

                // Si no hay nada sobre lo que interactuar, desactiva el texto de interacción
                if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
                {
                    Debug.Log("Ocultando Texto Principal");
                    interactionPromptText.gameObject.SetActive(false);
                }

                if (interactionPromptTextExtra != null && interactionPromptTextExtra.gameObject.activeSelf)
                {
                    Debug.Log("Ocultando Texto Extra");
                    interactionPromptTextExtra.gameObject.SetActive(false);
                }

                if (interactionPromptTextExtraLVL5 != null && interactionPromptTextExtraLVL5.gameObject.activeSelf)
                {
                    Debug.Log("Ocultando Texto Extra LVL5");
                    interactionPromptTextExtraLVL5.gameObject.SetActive(false);
                }
            }
            
        }
        
        else
        {
            interactionPromptText.gameObject.SetActive(false);
            interactionPromptTextExtra.gameObject.SetActive(false);
            
            // Si no detecta nada con el raycast, desactiva el texto de interacción
        }

        // Asegúrate de restaurar el material de las mesas si no las estás mirando
        // Asegúrate de restaurar el material de las mesas si no las estás mirando
        if (!isLookingAtTable && !isPlacingDevice)
        {
            foreach (GameObject mesa in GameObject.FindGameObjectsWithTag("Table"))
            {
                TableProperties properties = mesa.GetComponent<TableProperties>();
                if (properties != null)
                {
                    properties.RestoreMaterial();  // Restauramos el material cuando no se está mirando
                }
            }
        }

    }

    bool CanConnect(GameObject device1, GameObject device2)
    {
        // Lógica para checar si se pueden conectar los dispositivos
        // Por ejemplo:
        if (device1.CompareTag("PC4") && device2.CompareTag("PC4"))
        {
            return false; // No permitir PC a PC
        }
        return true;
    }

    // Interactor.cs
    Vector3 GetDeviceCenter(GameObject device)
    {
        Renderer renderer = null;

        // Intenta obtener el Renderer del objeto padre
        renderer = device.GetComponent<Renderer>();

        // Si no hay Renderer en el padre, busca en los hijos
        if (renderer == null)
        {
            renderer = device.GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            return renderer.bounds.center;
        }
        else
        {
            Debug.LogError($"GameObject '{device.name}' and its children do not have a Renderer. Returning its position as center.");
            return device.transform.position;
        }
    }

    // Interactor.cs

    void CreateCable(Vector3 startPoint, Vector3 endPoint)
    {
        LogToFile("Entrando en CreateCable(). Start: " + startPoint + ", End: " + endPoint);
        try
        {
            LogToFile("Intentando crear cable...");
            currentCable = new GameObject("Cable");
            currentCable.tag = "Cable4";
            LogToFile("GameObject 'Cable' creado.");
            LineRenderer lineRenderer = currentCable.AddComponent<LineRenderer>();
            LogToFile("LineRenderer agregado.");
            lineRenderer.positionCount = 2;
            LogToFile("positionCount establecido en 2.");
            lineRenderer.SetPosition(0, startPoint);
            LogToFile("startPoint establecido.");
            lineRenderer.SetPosition(1, endPoint);
            LogToFile("endPoint establecido.");
            lineRenderer.startWidth = 0.05f;
            LogToFile("startWidth establecido.");
            lineRenderer.endWidth = 0.05f;
            LogToFile("endWidth establecido.");

            // Asigna el material desde la variable pública
            if (cableMaterial != null)
            {
                lineRenderer.material = cableMaterial;
                LogToFile("Material asignado desde la variable 'cableMaterial'.");
            }
            else
            {
                LogToFile("¡Advertencia! 'cableMaterial' no está asignado en el Inspector.");
                // Puedes agregar un material por defecto aquí si lo deseas como respaldo
                // lineRenderer.material = new Material(Shader.Find("Standard"));
                // lineRenderer.material.color = Color.red;
            }

            LogToFile("Color (si se aplicó por defecto) asignado.");
            LogToFile("Cable creado exitosamente.");
            selectedDeviceText.gameObject.SetActive(false);
            interactionPromptText.gameObject.SetActive(false);
            LogToFile("Se ocultaron los elementos");
        }
        catch (Exception e)
        {
            LogToFile("Error en CreateCable(): " + e.Message);
            Debug.LogError("Error en CreateCable(): " + e.Message); // Mantén esto para la consola del editor
        }
        LogToFile("Saliendo de CreateCable().");
    }


    // Método para activar el modo de colocación de dispositivos
    // Activa el modo de colocación
    // Método para activar el modo de colocación de dispositivos
    public void EnableDevicePlacement(LockerInteraction_lvl4 locker)
    {
        lockerInteraction = locker;
        isPlacingDevice = true;
        //Debug.Log("Modo de colocación activado.");

        // Resalta solo las mesas disponibles
        foreach (GameObject mesa in GameObject.FindGameObjectsWithTag("Table"))
        {
            TableProperties properties = mesa.GetComponent<TableProperties>();
            if (properties != null)
            {
                // Resalta las mesas disponibles para colocación
                properties.Highlight(false);
            }
        }
    }


    // Desactiva el modo de colocación
    public void DisableDevicePlacement()
    {
        isPlacingDevice = false;
        lockerInteraction = null;
        //Debug.Log("Modo de colocación desactivado.");

        // Restaura el material original de todas las mesas
        foreach (GameObject mesa in GameObject.FindGameObjectsWithTag("Table"))
        {
            TableProperties properties = mesa.GetComponent<TableProperties>();
            if (properties != null)
            {
                properties.RestoreMaterial();
            }
        }
    }


    // En Interactor.cs
    public void RefreshPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    
    public void EnableCable()
    {
        CableLVL5.SetActive(true);
    }

    //Logs en txt
    void LogToFile(string message)
    {
        try
        {
            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                string logEntry = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message;
                writer.WriteLine(logEntry);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing to log file: " + e.Message);
        }
    }


}