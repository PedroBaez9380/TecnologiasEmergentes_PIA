using UnityEngine;

// Este script se adjunta al objeto que quieres que sea interactuable (tu botón)
// Necesita tener un Collider para ser detectado por el Raycast.
public class ButtonProperties : MonoBehaviour
{
    [Header("Objeto a Modificar")]
    [Tooltip("Arrastra aquí el objeto cuyo Box Collider quieres cambiar a Trigger.")]
    public BoxCollider objectToModifyCollider; // Referencia al collider del OTRO objeto

    [Header("Objeto a Desactivar")]
    [Tooltip("Arrastra aquí el objeto que quieres desactivar al presionar el botón.")]
    public GameObject objectToDeactivate;  // Nuevo: Referencia al GameObject a desactivar

    // Puedes añadir más propiedades aquí si el botón necesita hacer más cosas
    // Por ejemplo: sonido de interacción, animación, etc.

    // Este método puede ser llamado por el script Interactor
    // para ejecutar la acción principal del botón
    public void Interact()
    {
        if (objectToModifyCollider != null)
        {
            // Cambia el isTrigger del collider referenciado a true
            objectToModifyCollider.isTrigger = true;
            Debug.Log("¡Botón presionado! El collider del objeto referenciado ahora es Trigger.");

            // Desactiva el GameObject si está asignado
            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
            }

            // Aquí podrías añadir más lógica, como reproducir un sonido o animación
            // GetComponent<AudioSource>()?.Play();
        }
        else
        {
            Debug.LogWarning("ButtonProperties en " + gameObject.name + " no tiene un BoxCollider referenciado para modificar.");
        }
    }
}
