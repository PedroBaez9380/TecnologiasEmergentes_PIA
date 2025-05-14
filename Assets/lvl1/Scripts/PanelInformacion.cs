using UnityEngine;
using UnityEngine.UI; // Si usas el sistema de UI estándar
// using TMPro;       // Si usas TextMeshPro

public class PanelInformacion : MonoBehaviour
{
    public GameObject panel; // Referencia al Panel de información
    public Text textoInformacion; // Referencia al componente Text dentro del panel
    // public TMP_Text textoInformacion; // Si usas TextMeshPro
    public KeyCode teclaCierre = KeyCode.Escape; // Tecla predeterminada para cerrar el panel

    void Start()
    {
        // Ocultar el panel al inicio
        if (panel != null)
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogError("No se ha asignado el Panel en el Inspector del script PanelInformacion.");
        }

        // Asegurarse de que el texto de información esté asignado
        if (textoInformacion == null)
        {
            Debug.LogError("No se ha asignado el Text (o TextMeshPro) en el Inspector del script PanelInformacion.");
        }
    }

    void Update()
    {
        // Verificar si se ha presionado la tecla de cierre y si el panel está activo
        if (panel != null && panel.activeSelf && Input.GetKeyDown(teclaCierre))
        {
            OcultarPanel();
        }
    }

    // Método para mostrar el panel y actualizar la información
    public void MostrarPanel(string info)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        if (textoInformacion != null)
        {
            textoInformacion.text = info;
        }
    }

    // Método para ocultar el panel
    public void OcultarPanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}