using UnityEngine;
using UnityEditor;

public class AddCollidersUtility
{
    private const string meshMenuPath = "Tools/Physics/Add Mesh Colliders To Selection";
    private const string boxMenuPath = "Tools/Physics/Add Box Colliders To Selection";
    private const string removeMenuPath = "Tools/Physics/Remove Colliders From Selection";

    [MenuItem(meshMenuPath)]
    static void AddMeshColliders()
    {
        int count = 0;
        foreach (GameObject go in Selection.gameObjects)
        {
            foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
            {
                var obj = t.gameObject;
                if (obj.GetComponent<MeshFilter>() != null && obj.GetComponent<Collider>() == null)
                {
                    var mf = obj.GetComponent<MeshFilter>();
                    var mc = obj.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = true;
                    count++;
                }
            }
        }
        Debug.Log($"[AddMeshColliders] Se añadieron {count} MeshCollider(s).");
    }

    [MenuItem(boxMenuPath)]
    static void AddBoxColliders()
    {
        int count = 0;
        foreach (GameObject go in Selection.gameObjects)
        {
            foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
            {
                var obj = t.gameObject;
                if (obj.GetComponent<Collider>() == null)
                {
                    obj.AddComponent<BoxCollider>();
                    count++;
                }
            }
        }
        Debug.Log($"[AddBoxColliders] Se añadieron {count} BoxCollider(s).");
    }

    [MenuItem(removeMenuPath)]
    static void RemoveColliders()
    {
        int count = 0;
        foreach (GameObject go in Selection.gameObjects)
        {
            foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
            {
                var obj = t.gameObject;
                var colliders = obj.GetComponents<Collider>();
                foreach (var col in colliders)
                {
                    Object.DestroyImmediate(col);
                    count++;
                }
            }
        }
        Debug.Log($"[RemoveColliders] Se eliminaron {count} Collider(s).");
    }

    // Habilita los menús solo si hay objetos seleccionados
    [MenuItem(meshMenuPath, true)]
    [MenuItem(boxMenuPath, true)]
    [MenuItem(removeMenuPath, true)]
    static bool ValidateMenuItems()
    {
        return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
    }
}


