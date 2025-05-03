using UnityEngine;

public class ControlPuertaCorrediza : MonoBehaviour
{
    private Animator animator;
    private bool puertaAbierta = false;
    private string tagPersonaje = "Player"; // Etiqueta (Tag) que debe tener tu personaje

    void Start()
    {
        // Obtén la referencia al componente Animator del objeto "SlidingDoor"
        animator = GetComponentInParent<Animator>();

        // Asegúrate de que el Animator existe
        if (animator == null)
        {
            Debug.LogError("El objeto " + gameObject.name + " no tiene un componente Animator.");
            enabled = false; // Desactiva el script si no hay Animator
        }
    }

    // Este método se llama cuando otro Collider entra en el Trigger de este objeto
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entró en el Trigger es el personaje
        if (other.CompareTag(tagPersonaje) && !puertaAbierta)
        {
            // Si la puerta está cerrada y el personaje entra, ábrela
            puertaAbierta = true;
            animator.SetBool("Abrir", puertaAbierta);
        }
    }

    // Este método se llama cuando otro Collider sale del Trigger de este objeto
    private void OnTriggerExit(Collider other)
{
    Debug.Log("OnTriggerExit llamado por: " + other.gameObject.name + " con etiqueta: " + other.tag);
    // Verifica si el objeto que salió del Trigger es el personaje y si la puerta está abierta
    if (other.CompareTag(tagPersonaje) && puertaAbierta)
    {
        puertaAbierta = false;
        animator.SetBool("Abrir", puertaAbierta);
        Debug.Log("Puerta Abierta (Cerrando): " + puertaAbierta);
    }
}
}
