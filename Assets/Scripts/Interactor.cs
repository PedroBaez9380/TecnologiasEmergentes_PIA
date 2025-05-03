using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el UI Text

public class Interactor : MonoBehaviour
{
    [Header("Configuraci�n de Interacci�n")]
    [Tooltip("Distancia m�xima para interactuar con objetos.")]
    public float interactionDistance = 3.0f; // Ajusta esta distancia seg�n tu escena
    [Tooltip("M�scara de capas para que el raycast solo detecte lo que est� en estas capas.")]
    public LayerMask interactionLayer;

    [Header("Elementos UI")]
    [Tooltip("Arrastra aqu� el objeto de Texto UI para el prompt de interacci�n.")]
    public TextMeshProUGUI interactionPromptText; // Usa TextMeshProUGUI si usas TextMeshPro

    private GameObject currentHitObject; // El objeto que el raycast golpe� actualmente
    private ButtonProperties currentButton; // El script ButtonProperties del objeto golpeado

    void Start()
    {
        // Aseg�rate de que el texto del prompt est� inicialmente oculto
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
        else
        {
            // Este error aparecer� si olvidas asignar el texto UI en el Inspector
            Debug.LogError("Interaction Prompt Text no asignado en Interactor script.");
        }
    }

    void Update()
    {
        // Resetea las referencias en cada frame
        currentHitObject = null;
        currentButton = null;

        // Dibuja el rayo en la vista de escena para debug (solo visible si Gizmos est�n activados en la Scene view)
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.red);

        // Realiza el Raycast desde la posici�n y direcci�n de la c�mara
        RaycastHit hit; // Variable para almacenar la informaci�n del golpe

        // Physics.Raycast(origen, direcci�n, out hit, distancia m�xima, m�scara de capa)
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactionLayer))
        {
            // �Si entramos aqu�, significa que el raycast S� golpe� algo en una capa permitida dentro de la distancia!

            // Puedes descomentar esta l�nea para ver qu� objeto exacto est� golpeando el raycast
            // Debug.Log("Raycast golpe�: " + hit.collider.gameObject.name + " en capa: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            // --- �DECLARAMOS LA VARIABLE 'button' PRIMERO! ---
            // Intenta obtener el script ButtonProperties del objeto que fue golpeado
            ButtonProperties button = hit.collider.GetComponent<ButtonProperties>(); // <-- Declaraci�n de 'button'
            // --------------------------------------------

            // --- Ahora s�, usamos la variable 'button' --
            // Este log te dice si GetComponent encontr� el script ButtonProperties en el objeto golpeado
            //Debug.Log("Resultado de GetComponent<ButtonProperties>(): " + (button != null ? "Encontrado" : "NULL"));
            // -----------------------------------------


            // Si 'button' NO es null (significa que el objeto golpeado S� ten�a el script ButtonProperties)
            if (button != null)
            {
                // Estamos mirando un objeto interactuable con el script ButtonProperties
                currentHitObject = hit.collider.gameObject; // Guardamos el objeto golpeado
                currentButton = button; // Guardamos la referencia al script ButtonProperties

                // Muestra el prompt de interacci�n si no est� visible actualmente
                if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                {
                    // Este c�digo muestra el mensaje "Presiona E para Interactuar"
                    interactionPromptText.text = "Presiona E para Interactuar";
                    interactionPromptText.gameObject.SetActive(true); // Hacemos visible el texto UI
                }

                // --- L�gica de Interacci�n al presionar 'E' ---
                // Si el jugador presiona la tecla 'E' (Input.GetKeyDown detecta la primera pulsaci�n)
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Llama al m�todo Interact del script ButtonProperties en el objeto que golpeamos
                    currentButton.Interact(); // Usamos currentButton (que ya validamos que NO es null)
                    // button.Interact(); // Tambi�n podr�as usar directamente la variable local 'button' aqu�

                    // Opcional: Ocultar el prompt inmediatamente despu�s de interactuar
                    // interactionPromptText.gameObject.SetActive(false);
                }
            }
            else // Si 'button' es null (el objeto golpeado NO ten�a el script ButtonProperties)...
            {
                // Si antes se estaba mostrando un prompt (porque quiz�s antes mir�bamos un bot�n), lo ocultamos
                if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.gameObject.SetActive(false);
                }
            }
        }
        else // Si el raycast NO golpe� nada en las capas permitidas dentro de la distancia...
        {
            // Si antes se estaba mostrando un prompt, lo ocultamos
            if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
            {
                interactionPromptText.gameObject.SetActive(false);
            }
        }
    }
}