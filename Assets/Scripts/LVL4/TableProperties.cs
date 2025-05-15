using Unity.VisualScripting;
using UnityEngine;

public class TableProperties : MonoBehaviour
{
    public bool isOccupied = false;  // Campo que indica si la mesa est� ocupada
    private GameObject placedDevice;  // Referencia al dispositivo colocado
    private Material originalMaterial;  // Material original de la mesa
    public Material highlightMaterial;  // Material para el resaltado
    public Material deleteMaterial;

    void Awake()
    {
        // Guarda una instancia del material original para restaurarlo m�s tarde
        originalMaterial = GetComponent<Renderer>().material;
    }

    // M�todo para colocar un dispositivo
    public void PlaceDevice(GameObject device)
    {
        placedDevice = device;
        isOccupied = true;

        //device.transform.SetParent(this.transform);
    }

    // M�todo para quitar el dispositivo
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

    // M�todo para verificar si la mesa est� ocupada
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // M�todo para cambiar al material de resaltado
    // M�todo para cambiar al material de resaltado
    public void Highlight(bool highlightForDelete)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (highlightForDelete)
        {
            // Resaltado para eliminaci�n
            if (deleteMaterial != null)
            {
                renderer.material = deleteMaterial;
                Debug.Log("Mesa resaltada para eliminaci�n");
            }
        }
        else
        {
            // Resaltado para colocaci�n (mesas disponibles)
            if (!isOccupied && highlightMaterial != null)
            {
                Debug.Log("Antes de ejecutar el renderer.material");
                renderer.material = highlightMaterial;
                Debug.Log("Mesa resaltada para colocaci�n");
            }
        }
    }




    // M�todo para restaurar el material original
    public void RestoreMaterial()
    {
        if (originalMaterial != null)
        {
            GetComponent<Renderer>().material = originalMaterial;
        }
    }
}


