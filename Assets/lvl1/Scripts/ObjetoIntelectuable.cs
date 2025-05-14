using UnityEngine;
// No necesitas UnityEngine.UI aquí si el panel se encarga del texto

public class ObjetoInteractuable : MonoBehaviour
{
    [TextArea] // Esto permite escribir texto más largo en el Inspector
    public string informacion;
    public PanelInformacion panelInfo; // Referencia al script PanelInformacion

    private void OnMouseDown()
    {
        // Este método se llama cuando se hace clic en un GameObject con un Collider
        Debug.Log("Se hizo clic en: " + gameObject.name);
        Debug.Log("Información: " + informacion);

        // Mostrar el panel y la información si la referencia al PanelInformacion es válida
        if (panelInfo != null)
        {
            panelInfo.MostrarPanel(informacion);
        }
        else
        {
            Debug.LogError("No se ha asignado el script PanelInformacion en el Inspector del objeto: " + gameObject.name);
        }
    }
}