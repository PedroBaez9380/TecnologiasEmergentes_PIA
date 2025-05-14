using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el UI Text
using System.Collections; // Necesario para usar Coroutines

// Este script se adjunta al objeto que tiene el Box Collider que actua como Trigger
public class TriggerMessage : MonoBehaviour
{
    [Header("Configuraci�n del Mensaje")]
    [Tooltip("Arrastra aqu� el objeto de Texto UI para mostrar el mensaje.")]
    public TextMeshProUGUI messageTextUI; // El mismo texto UI que usas para el prompt del bot�n, o uno diferente

    [Tooltip("El mensaje que se mostrar� al entrar en el trigger.")]
    [TextArea(3, 5)] // Permite editar el texto en varias l�neas en el Inspector
    public string messageToShow = "�Entraste a la zona!"; // El mensaje por defecto

    [Tooltip("Duraci�n en segundos que el mensaje estar� visible.")]
    public float displayDuration = 5.0f; // Duraci�n en segundos

    private Coroutine hideMessageCoroutine; // Referencia para controlar la coroutine


    void Start()
    {
        // Opcional: Aseg�rate de que el texto est� oculto al inicio si no lo haces en el Editor
        if (messageTextUI != null)
        {
            messageTextUI.gameObject.SetActive(false);
        }
    }

    // Este m�todo se llama cuando OTRO collider entra en ESTE collider (si este est� marcado como Is Trigger)
    void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entr� es el jugador
        // Puedes hacerlo por Tag ("Player") o por Componente (CharacterController, etc.)
        // Aseg�rate de que tu GameObject de personaje tenga la Tag "Player"
        if (other.CompareTag("Player")) // Verifica si el objeto tiene la Tag "Player"
        {
            // Si es el jugador, muestra el mensaje
            ShowMessage();
        }
        // Si usas CharacterController, podr�as verificar as�:
        // if (other.GetComponent<CharacterController>() != null)
        // {
        //     ShowMessage();
        // }
    }

    // Opcional: Puedes a�adir OnTriggerExit si quieres hacer algo cuando el jugador sale del trigger
    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player")) // O la verificaci�n del CharacterController
    //     {
    //         // Ocultar mensaje inmediatamente al salir si lo prefieres, aunque ya se ocultar� por tiempo
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

            // Inicia la coroutine para ocultar el mensaje despu�s de la duraci�n
            hideMessageCoroutine = StartCoroutine(HideMessageAfterDuration(displayDuration));
        }
        else
        {
            Debug.LogWarning("Message Text UI no asignado en TriggerMessage script en " + gameObject.name);
        }
    }

    private IEnumerator HideMessageAfterDuration(float duration)
    {
        // Espera la duraci�n especificada en segundos
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

    // M�todo para detener la coroutine si es necesario (ej. al desactivar el objeto)
    void OnDisable()
    {
        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }
    }
}
