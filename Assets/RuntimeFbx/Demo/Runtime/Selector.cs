using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public Material highlightMaterial;
    public Transform selectedTransform;

    private List<Transform> objTransforms;
    private Camera mainCamera;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private bool lastFrameHit = false;

    private void Awake()
    {
        highlightMaterial.SetShaderPassEnabled("Always", false);
    }

    void Start()
    {
        mainCamera = Camera.main;
        objTransforms = new List<Transform>(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            objTransforms.Add(child);
            var meshRenderer = child.GetComponent<MeshRenderer>();
            meshRenderer.material = Instantiate(highlightMaterial);
        }
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
        {
            if (lastFrameHit)
            {
                UpdateMaterials(null);
            }
            lastFrameHit = false;
            return;
        }


        var hitTransform = hit.transform;
        if (!objTransforms.Contains(hit.transform))
        {
            if (lastFrameHit)
            {
                UpdateMaterials(null);
            }
            lastFrameHit = false;
            return;
        }

        lastFrameHit = true;

        UpdateMaterials(hitTransform);

        if (Input.GetMouseButtonUp(0))
        {
            selectedTransform = hitTransform;
        }
    }

    private void UpdateMaterials(Transform hoverTransform)
    {
        if (hoverTransform == null)
        {
            foreach (var objTransform in objTransforms)
            {
                if (selectedTransform == objTransform)
                {
                    continue;
                }

                var hitMeshRenderer = objTransform.GetComponent<MeshRenderer>();
                var material = hitMeshRenderer.sharedMaterial;
                EnableOutlinePass(material, false);
            }
            return;
        }

        foreach (var objTransform in objTransforms)
        {
            var hitMeshRenderer = objTransform.GetComponent<MeshRenderer>();
            var material = hitMeshRenderer.sharedMaterial;
            if (objTransform == selectedTransform)
            {
                EnableOutlinePass(material, true);
                SetOutlineColor(material, new Color32(219,233,58, 255));
            }
            else
            {
                bool enableOutline = objTransform == hoverTransform;
                EnableOutlinePass(material, enableOutline);
                if (enableOutline)
                {
                    SetOutlineColor(material, Color.green);
                }
            }
        }
    }

    private void EnableOutlinePass(Material material, bool enableOutline)
    {
        material.SetShaderPassEnabled("Always", enableOutline);
    }

    private void SetOutlineColor(Material material, Color color)
    {
        material.SetColor(ColorProperty, color);
    }

}
