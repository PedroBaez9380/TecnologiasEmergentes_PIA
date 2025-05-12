using UnityEngine;

public class TableProperties : MonoBehaviour
{
    public bool isOccupied = false;  // Campo que indica si la mesa está ocupada
    private GameObject placedDevice;  // Referencia al dispositivo colocado

    // Método para colocar un dispositivo
    public void PlaceDevice(GameObject device)
    {
        placedDevice = device;
        isOccupied = true;  // La mesa está ocupada
    }

    // Método para quitar el dispositivo
    public void RemoveDevice()
    {
        if (placedDevice != null)
        {
            Destroy(placedDevice);
            placedDevice = null;
            isOccupied = false;  // La mesa ya no está ocupada
            Debug.Log("Dispositivo eliminado de la mesa.");

        }
    }

    // Método para verificar si la mesa está ocupada
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // Método para establecer el estado de ocupación desde otro script
    public void SetOccupied(bool status)
    {
        isOccupied = status;
    }
}


