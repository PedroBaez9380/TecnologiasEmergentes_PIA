using UnityEngine;
using TMPro;
using System.Collections;

public class LVL5_INDICATORS : MonoBehaviour
{
    [Header("Configuración del Mensaje")]
    public TextMeshProUGUI messageTextUI;
    [TextArea(3, 5)]
    public string messageToShow = "¡Entraste a la zona!";
    public float displayDuration = 5.0f;
    private Coroutine hideMessageCoroutine;

    [Header("Configuración del Otro Trigger")]
    public Collider targetCollider;
    public bool setTargetColliderToTrigger = true;

    [Header("Configuración del Menú")]
    public GameObject menuToOpen; // Assign the menu GameObject in the Inspector
    private PlayerController playerController; // Reference to PlayerController

    [Header("Objeto a Desactivar")]
    public GameObject objectToDeactivate;



    void Start()
    {
        if (messageTextUI != null)
        {
            messageTextUI.gameObject.SetActive(false);
        }

        if (menuToOpen != null)
        {
            menuToOpen.SetActive(false); // Ensure menu is initially hidden
        }

        // Get PlayerController reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("No Player found with tag 'Player' - Required!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMessage();
            OpenMenuAndBlockControls();

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
            Debug.LogWarning("Message Text UI not assigned in TriggerMessage script on " + gameObject.name);
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

    private void OpenMenuAndBlockControls()
    {
        if (menuToOpen != null)
        {
            menuToOpen.SetActive(true);
        }

        if (playerController != null)
        {
            playerController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // This function will be assigned to a button to close the menu
    public void CloseMenuAndRestoreControls()
    {
        if (menuToOpen != null)
        {
            menuToOpen.SetActive(false);
        }

        if (playerController != null)
        {
            playerController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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