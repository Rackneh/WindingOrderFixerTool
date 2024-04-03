using System.IO;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Collections.Generic;
using RuntimeFbx;
using AsciiFBXExporter;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Main : MonoBehaviour
{
    public GameObject objectPosition; // Assign your target position in the Inspector.
    public bool use = true; // Toggle this in the Inspector to start/stop the script.
    public int delay;

    private List<string> fbxFiles;
    private int currentFbxIndex = 0;

    void Start()
    {
        if (use)
        {
            string importFolderPath = "Assets/Import";
            string exportFolderPath = "Assets/Export";

            // Recursively collect FBX file paths from Import folder.
            fbxFiles = new List<string>(Directory.GetFiles(importFolderPath, "*.fbx", SearchOption.AllDirectories));

            // Start processing the FBX files asynchronously.
            ProcessNextFbx(exportFolderPath);
        }
    }
    //public void Update()
    //{
    //        if (use)
    //        {
    //            string importFolderPath = "Assets/Import";
    //            string exportFolderPath = "Assets/Export";

    //            // Recursively collect FBX file paths from Import folder.
    //            fbxFiles = new List<string>(Directory.GetFiles(importFolderPath, "*.fbx", SearchOption.AllDirectories));

    //            // Start processing the FBX files asynchronously.
    //            ProcessNextFbx(exportFolderPath);
    //        }
    //}

    async void ProcessNextFbx(string exportFolderPath)
    {
        // Check if there are more FBX files to process.
        if (use && currentFbxIndex < fbxFiles.Count)
        {
            // Load the FBX model.
            string fbxFile = fbxFiles[currentFbxIndex];
            GameObject loadedModel = AssetDatabase.LoadAssetAtPath<GameObject>(fbxFile);

            // Instantiate it at the target position.
            GameObject instantiatedModel = Instantiate(loadedModel, objectPosition.transform);

            // Fix normals.
            FlipNormals(objectPosition);

            List<string> meshNamesWithMaterials = new List<string>();
            List<Material> materialsToAssign = new List<Material>();

            // Iterate through all renderers in the instantiated model.
            Renderer[] renderers = instantiatedModel.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer)
                {
                    // Handle MeshRenderer.
                    MeshRenderer meshRenderer = (MeshRenderer)renderer;

                    // Add your logic here for MeshRenderer if needed.
                }
                else if (renderer is SkinnedMeshRenderer)
                {
                    // Skip SkinnedMeshRenderer and move on to the next FBX file.
                    currentFbxIndex++;
                    DestroyImmediate(instantiatedModel);
                    await Task.Delay(delay); // Delay to allow Unity to update the UI.
                    ProcessNextFbx(exportFolderPath);
                    return; // Exit the loop to avoid further processing for SkinnedMeshRenderer.
                }
            }

            // Calculate the export path.
            string relativePath = fbxFile.Replace("Assets/Import", "");
            string exportPath = exportFolderPath + relativePath;

            // Create the export directory if it doesn't exist.
            Directory.CreateDirectory(Path.GetDirectoryName(exportPath));

            // Export the model to the same folder hierarchy inside Export.
            FBXExporter.ExportGameObjToFBX(instantiatedModel, exportPath, false, false);

            // Refresh the AssetDatabase to ensure changes are detected.
            AssetDatabase.Refresh();

            // Increment the index to process the next FBX file.
            currentFbxIndex++;
            DestroyImmediate(instantiatedModel);

            // Continue processing the next FBX file asynchronously.
            await Task.Delay(delay); // Delay to allow Unity to update the UI.
            ProcessNextFbx(exportFolderPath);
        }
    }

    public void FlipNormals(GameObject obj)
    {
        MeshFixer meshFixer = obj.GetComponent<MeshFixer>();
        if (meshFixer != null)
        {
            meshFixer.FixNormals(obj.transform, 0.01f);
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);
            FlipNormals(child.gameObject);
        }
    }
}
