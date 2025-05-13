using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;
    public Text nameText;
    public Text descriptionText;

    public GameObject player;
    public MonoBehaviour playerMovementScript; // Ej: FirstPersonController, etc.

    void Start()
    {
        infoPanel.SetActive(false);
        LockCursor(true); // Bloqueamos el cursor al inicio
    }

    public void ShowInfo(DeviceInfo deviceInfo)
    {
        infoPanel.SetActive(true);
        nameText.text = deviceInfo.deviceName;
        descriptionText.text = deviceInfo.deviceDescription;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        LockCursor(false); // Mostrar cursor al activar UI
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        LockCursor(true); // Ocultar cursor al volver al juego
    }

    void LockCursor(bool lockState)
    {
        if (lockState)
        {
            Cursor.lockState = CursorLockMode.Locked; // Oculta y bloquea el cursor
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Libera y muestra el cursor
            Cursor.visible = true;
        }
    }
}
