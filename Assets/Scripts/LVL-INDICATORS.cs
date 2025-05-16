using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el UI Text
using System.Collections; // Necesario para usar Coroutines

// Este script se adjunta al objeto que tiene el Box Collider que actua como Trigger
public class TriggerMessage : MonoBehaviour
{
    [Header("Configuraci�n del Mensaje")]
    [Tooltip("Arrastra aqu� el objeto de Texto UI para mostrar el mensaje.")]
    public TextMeshProUGUI messageTextUI;

    [Tooltip("El mensaje que se mostrar� al entrar en el trigger.")]
    [TextArea(3, 5)]
    public string messageToShow = "�Entraste a la zona!";

    [Tooltip("Duraci�n en segundos que el mensaje estar� visible.")]
    public float displayDuration = 5.0f;

    private Coroutine hideMessageCoroutine;

    [Header("Configuraci�n del Otro Trigger")]
    [Tooltip("Arrastra aqu� el otro Collider que quieres modificar.")]
    public Collider targetCollider;

    [Tooltip("Indica si el otro collider debe ser un trigger (true) o no (false).")]
    public bool setTargetColliderToTrigger = true;

    [Header("Objeto a Desactivar")]
    public GameObject objectToDeactivate;


    void Start()
    {
        if (messageTextUI != null)
        {
            messageTextUI.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMessage();

            // Modifica la propiedad "Is Trigger" del otro collider
            if (targetCollider != null)
            {
                targetCollider.isTrigger = setTargetColliderToTrigger;
            }

            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
            }
        }
    }

    private void ShowMessage()
    {
        if (messageTextUI != null)
        {
            if (hideMessageCoroutine != null)
            {
                StopCoroutine(hideMessageCoroutine);
            }

            messageTextUI.text = messageToShow;
            messageTextUI.gameObject.SetActive(true);

            hideMessageCoroutine = StartCoroutine(HideMessageAfterDuration(displayDuration));
        }
        else
        {
            Debug.LogWarning("Message Text UI no asignado en TriggerMessage script en " + gameObject.name);
        }
    }

    private IEnumerator HideMessageAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        HideMessage();
    }

    private void HideMessage()
    {
        if (messageTextUI != null)
        {
            messageTextUI.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }
    }
}
