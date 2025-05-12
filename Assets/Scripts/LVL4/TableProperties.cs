using UnityEngine;

public class TableProperties : MonoBehaviour
{
    public bool isOccupied = false;  // Campo que indica si la mesa est� ocupada
    private GameObject placedDevice;  // Referencia al dispositivo colocado

    // M�todo para colocar un dispositivo
    public void PlaceDevice(GameObject device)
    {
        placedDevice = device;
        isOccupied = true;  // La mesa est� ocupada
    }

    // M�todo para quitar el dispositivo
    public void RemoveDevice()
    {
        if (placedDevice != null)
        {
            Destroy(placedDevice);
            placedDevice = null;
            isOccupied = false;  // La mesa ya no est� ocupada
            Debug.Log("Dispositivo eliminado de la mesa.");

        }
    }

    // M�todo para verificar si la mesa est� ocupada
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // M�todo para establecer el estado de ocupaci�n desde otro script
    public void SetOccupied(bool status)
    {
        isOccupied = status;
    }
}


