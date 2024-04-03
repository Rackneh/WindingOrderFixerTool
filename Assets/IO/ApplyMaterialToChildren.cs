using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ApplyMaterialToChildren : MonoBehaviour
{
    public Material materialToApply;
    public List<string> childObjectNames;
    public bool applyMaterial = false;
    public bool skipObjectsToSkip = true;
    public List<GameObject> objectsToSkip;
    public List<GameObject> objectsToGoThrough; // new list to go through specific objects

    void Update()
    {
        if (applyMaterial)
        {
            foreach (GameObject obj in objectsToGoThrough) // process only the objects in objectsToGoThrough
            {
                RecursiveApplyMaterial(obj.transform);
            }
            // reset the applyMaterial variable to prevent continuous application
            applyMaterial = false;
        }
    }

    void RecursiveApplyMaterial(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (skipObjectsToSkip && objectsToSkip.Contains(child.gameObject))
            {
                // Skip this child if it is in the objectsToSkip list and skipObjectsToSkip is true
                continue;
            }

            if (childObjectNames.Contains(child.name))
            {
                MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                SkinnedMeshRenderer skinnedMeshRenderer = child.GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer != null)
                {
                    skinnedMeshRenderer.material = materialToApply;
                }
            }
            // Recursively call this method for each child object
            RecursiveApplyMaterial(child);
        }
    }
}
