using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public UIManager uiManager;
    private DeviceInfo deviceInfo;

    void Start()
    {
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
