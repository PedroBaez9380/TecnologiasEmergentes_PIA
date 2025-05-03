using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el UI Text
using System.Collections; // Necesario para usar Coroutines

// Este script se adjunta al objeto que tiene el Box Collider que actua como Trigger
public class TriggerMessage : MonoBehaviour
{
    [Header("Configuración del Mensaje")]
    [Tooltip("Arrastra aquí el objeto de Texto UI para mostrar el mensaje.")]
    public TextMeshProUGUI messageTextUI; // El mismo texto UI que usas para el prompt del botón, o uno diferente

    [Tooltip("El mensaje que se mostrará al entrar en el trigger.")]
    [TextArea(3, 5)] // Permite editar el texto en varias líneas en el Inspector
    public string messageToShow = "¡Entraste a la zona!"; // El mensaje por defecto

    [Tooltip("Duración en segundos que el mensaje estará visible.")]
    public float displayDuration = 5.0f; // Duración en segundos

    private Coroutine hideMessageCoroutine; // Referencia para controlar la coroutine


    void Start()
    {
        // Opcional: Asegúrate de que el texto esté oculto al inicio si no lo haces en el Editor
        if (messageTextUI != null)
        {
            messageTextUI.gameObject.SetActive(false);
        }
    }

    // Este método se llama cuando OTRO collider entra en ESTE collider (si este está marcado como Is Trigger)
    void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entró es el jugador
        // Puedes hacerlo por Tag ("Player") o por Componente (CharacterController, etc.)
        // Asegúrate de que tu GameObject de personaje tenga la Tag "Player"
        if (other.CompareTag("Player")) // Verifica si el objeto tiene la Tag "Player"
        {
            // Si es el jugador, muestra el mensaje
            ShowMessage();
        }
        // Si usas CharacterController, podrías verificar así:
        // if (other.GetComponent<CharacterController>() != null)
        // {
        //     ShowMessage();
        // }
    }

    // Opcional: Puedes añadir OnTriggerExit si quieres hacer algo cuando el jugador sale del trigger
    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player")) // O la verificación del CharacterController
    //     {
    //         // Ocultar mensaje inmediatamente al salir si lo prefieres, aunque ya se ocultará por tiempo
    //         // HideMessage();
    //     }
    // }


    private void ShowMessage()
    {
        if (messageTextUI != null)
        {
            // Detiene cualquier coroutine anterior para evitar que se oculte prematuramente
            if (hideMessageCoroutine != null)
            {
                StopCoroutine(hideMessageCoroutine);
            }

            // Establece el texto y lo hace visible
            messageTextUI.text = messageToShow;
            messageTextUI.gameObject.SetActive(true);

            // Inicia la coroutine para ocultar el mensaje después de la duración
            hideMessageCoroutine = StartCoroutine(HideMessageAfterDuration(displayDuration));
        }
        else
        {
            Debug.LogWarning("Message Text UI no asignado en TriggerMessage script en " + gameObject.name);
        }
    }

    private IEnumerator HideMessageAfterDuration(float duration)
    {
        // Espera la duración especificada en segundos
        yield return new WaitForSeconds(duration);

        // Oculta el texto del mensaje
        HideMessage();
    }

    private void HideMessage()
    {
        if (messageTextUI != null)
        {
            messageTextUI.gameObject.SetActive(false);
        }
    }

    // Método para detener la coroutine si es necesario (ej. al desactivar el objeto)
    void OnDisable()
    {
        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }
    }
}
