using Unity.VisualScripting;
using UnityEngine;

public class TableProperties : MonoBehaviour
{
    public bool isOccupied = false;  // Campo que indica si la mesa está ocupada
    private GameObject placedDevice;  // Referencia al dispositivo colocado
    private Material originalMaterial;  // Material original de la mesa
    public Material highlightMaterial;  // Material para el resaltado
    public Material deleteMaterial;

    void Awake()
    {
        // Guarda una instancia del material original para restaurarlo más tarde
        originalMaterial = GetComponent<Renderer>().material;
    }

    // Método para colocar un dispositivo
    public void PlaceDevice(GameObject device)
    {
        placedDevice = device;
        isOccupied = true;

        //device.transform.SetParent(this.transform);
    }

    // Método para quitar el dispositivo
    public void RemoveDevice()
    {
        if (placedDevice != null)
        {
            Destroy(placedDevice);
            placedDevice = null;
            isOccupied = false;
            Debug.Log("Dispositivo eliminado de la mesa.");
        }
    }

    // Método para verificar si la mesa está ocupada
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // Método para cambiar al material de resaltado
    // Método para cambiar al material de resaltado
    public void Highlight(bool highlightForDelete)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (highlightForDelete)
        {
            // Resaltado para eliminación
            if (deleteMaterial != null)
            {
                renderer.material = deleteMaterial;
                Debug.Log("Mesa resaltada para eliminación");
            }
        }
        else
        {
            // Resaltado para colocación (mesas disponibles)
            if (!isOccupied && highlightMaterial != null)
            {
                Debug.Log("Antes de ejecutar el renderer.material");
                renderer.material = highlightMaterial;
                Debug.Log("Mesa resaltada para colocación");
            }
        }
    }




    // Método para restaurar el material original
    public void RestoreMaterial()
    {
        if (originalMaterial != null)
        {
            GetComponent<Renderer>().material = originalMaterial;
        }
    }
}


