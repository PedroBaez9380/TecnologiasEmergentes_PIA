using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public UIManager uiManager;          // Asignar en el Inspector (arrastra el UIManager aquí)
    private DeviceInfo deviceInfo;       // El script con los datos del dispositivo

    void Start()
    {
        // Obtener el componente DeviceInfo del objeto
        deviceInfo = GetComponent<DeviceInfo>();
    }

    void OnMouseDown()
    {
        if (uiManager != null && deviceInfo != null)
        {
            uiManager.ShowInfo(deviceInfo);
        }
        else
        {
            Debug.LogWarning("Falta asignar UIManager o DeviceInfo en: " + gameObject.name);
        }
    }
}
