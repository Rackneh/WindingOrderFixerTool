using System;
using UnityEngine;
using UnityEngine.UI;

public class Importer : MonoBehaviour
{
    public Text fileNameLabel;
    public DragDropHandler dragDropHandler;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    void Start ()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        dragDropHandler.fileDropEvent += delegate (string[] paths)
        {
            fileNameLabel.text = "";
            foreach (string path in paths)
            {
                fileNameLabel.text += path + "\n";
            }

            if (paths.Length == 0)
            {
                return;
            }

            try
            {
                Mesh mesh = RuntimeFbx.Util.LoadMesh(paths[0]);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                if (mesh)
                {
                    meshFilter.sharedMesh = mesh;
                }

                var boundSize = mesh.bounds.size.magnitude;
                var scale = Vector3.one * 4 / boundSize;
                transform.localScale = scale;
                meshRenderer.enabled = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        };
    }

    private float lastX = 0;
    void Update()
    {
        var x = Input.mousePosition.x;
        if(Input.GetMouseButton(0))
        {
            var deltaX = lastX - x;
            var angles = transform.eulerAngles;
            angles.y += deltaX;
            transform.eulerAngles = angles;
        }
        lastX = x;
    }
}
