using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el UI Text

public class Interactor : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [Tooltip("Distancia máxima para interactuar con objetos.")]
    public float interactionDistance = 3.0f; // Ajusta esta distancia según tu escena
    [Tooltip("Máscara de capas para que el raycast solo detecte lo que está en estas capas.")]
    public LayerMask interactionLayer;

    [Header("Elementos UI")]
    [Tooltip("Arrastra aquí el objeto de Texto UI para el prompt de interacción.")]
    public TextMeshProUGUI interactionPromptText; // Usa TextMeshProUGUI si usas TextMeshPro

    private GameObject currentHitObject; // El objeto que el raycast golpeó actualmente
    private ButtonProperties currentButton; // El script ButtonProperties del objeto golpeado

    void Start()
    {
        // Asegúrate de que el texto del prompt esté inicialmente oculto
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
        else
        {
            // Este error aparecerá si olvidas asignar el texto UI en el Inspector
            Debug.LogError("Interaction Prompt Text no asignado en Interactor script.");
        }
    }

    void Update()
    {
        // Resetea las referencias en cada frame
        currentHitObject = null;
        currentButton = null;

        // Dibuja el rayo en la vista de escena para debug (solo visible si Gizmos están activados en la Scene view)
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.red);

        // Realiza el Raycast desde la posición y dirección de la cámara
        RaycastHit hit; // Variable para almacenar la información del golpe

        // Physics.Raycast(origen, dirección, out hit, distancia máxima, máscara de capa)
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactionLayer))
        {
            // ¡Si entramos aquí, significa que el raycast SÍ golpeó algo en una capa permitida dentro de la distancia!

            // Puedes descomentar esta línea para ver qué objeto exacto está golpeando el raycast
            // Debug.Log("Raycast golpeó: " + hit.collider.gameObject.name + " en capa: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            // --- ¡DECLARAMOS LA VARIABLE 'button' PRIMERO! ---
            // Intenta obtener el script ButtonProperties del objeto que fue golpeado
            ButtonProperties button = hit.collider.GetComponent<ButtonProperties>(); // <-- Declaración de 'button'
            // --------------------------------------------

            // --- Ahora sí, usamos la variable 'button' --
            // Este log te dice si GetComponent encontró el script ButtonProperties en el objeto golpeado
            //Debug.Log("Resultado de GetComponent<ButtonProperties>(): " + (button != null ? "Encontrado" : "NULL"));
            // -----------------------------------------


            // Si 'button' NO es null (significa que el objeto golpeado SÍ tenía el script ButtonProperties)
            if (button != null)
            {
                // Estamos mirando un objeto interactuable con el script ButtonProperties
                currentHitObject = hit.collider.gameObject; // Guardamos el objeto golpeado
                currentButton = button; // Guardamos la referencia al script ButtonProperties

                // Muestra el prompt de interacción si no está visible actualmente
                if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                {
                    // Este código muestra el mensaje "Presiona E para Interactuar"
                    interactionPromptText.text = "Presiona E para Interactuar";
                    interactionPromptText.gameObject.SetActive(true); // Hacemos visible el texto UI
                }

                // --- Lógica de Interacción al presionar 'E' ---
                // Si el jugador presiona la tecla 'E' (Input.GetKeyDown detecta la primera pulsación)
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Llama al método Interact del script ButtonProperties en el objeto que golpeamos
                    currentButton.Interact(); // Usamos currentButton (que ya validamos que NO es null)
                    // button.Interact(); // También podrías usar directamente la variable local 'button' aquí

                    // Opcional: Ocultar el prompt inmediatamente después de interactuar
                    // interactionPromptText.gameObject.SetActive(false);
                }
            }
            else // Si 'button' es null (el objeto golpeado NO tenía el script ButtonProperties)...
            {
                // Si antes se estaba mostrando un prompt (porque quizás antes mirábamos un botón), lo ocultamos
                if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.gameObject.SetActive(false);
                }
            }
        }
        else // Si el raycast NO golpeó nada en las capas permitidas dentro de la distancia...
        {
            // Si antes se estaba mostrando un prompt, lo ocultamos
            if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }
}