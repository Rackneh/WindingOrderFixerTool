//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class ImportStructure : EditorWindow
//{
//    [MenuItem("Tools/Generate Import Structure File")]
//    private static void GenerateImportStructureFile()
//    {
//        string importFolderPath = "Assets/Import";
//        string outputFile = "Assets/Import/ImportStructure.txt";

//        List<string> importStructure = GenerateImportStructure(importFolderPath);

//        // Write the import structure to a text file.
//        File.WriteAllLines(outputFile, importStructure);

//        AssetDatabase.Refresh();
//        Debug.Log("Import structure file generated: " + outputFile);
//    }

//    private static List<string> GenerateImportStructure(string importFolderPath)
//    {
//        List<string> importStructure = new List<string>();

//        // Recursively collect FBX file paths from Import folder.
//        string[] fbxFiles = Directory.GetFiles(importFolderPath, "*.fbx", SearchOption.AllDirectories);

//        foreach (string fbxFile in fbxFiles)
//        {
//            // Load the FBX model.
//            GameObject loadedModel = AssetDatabase.LoadAssetAtPath<GameObject>(fbxFile);

//            // Get the model name.
//            string modelName = Path.GetFileNameWithoutExtension(fbxFile);

//            // Get the materials and textures for the model.
//            List<string> modelInfo = GetModelInfo(loadedModel, modelName);
//            importStructure.AddRange(modelInfo);
//        }

//        return importStructure;
//    }

//    private static List<string> GetModelInfo(GameObject model, string parentName)
//    {
//        List<string> modelInfo = new List<string>();

//        // Iterate through the materials of the model.
//        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);

//        foreach (Renderer renderer in renderers)
//        {
//            Material material = renderer.sharedMaterial;
//            string materialName = (material != null) ? material.name : "None";

//            // Iterate through the textures of the material.
//            string textureNames = GetTextureNames(material);

//            // Add the model, material, and texture info to the list.
//            modelInfo.Add($"{parentName}, {materialName}, {textureNames}");

//            // Iterate through child objects.
//            Transform[] children = renderer.GetComponentsInChildren<Transform>(true);

//            foreach (Transform child in children)
//            {
//                if (child != renderer.transform) // Skip the renderer itself.
//                {
//                    string childName = child.name;
//                    modelInfo.Add($"     {childName}, {materialName}, {textureNames}");
//                }
//            }
//        }

//        return modelInfo;
//    }

//    private static string GetTextureNames(Material material)
//    {
//        if (material == null)
//            return "None";

//        string textureNames = "";
//        Texture textures = material.GetTexture("_MainTex");

//        foreach (Texture texture in textures)
//        {
//            if (texture != null)
//            {
//                string textureName = AssetDatabase.GetAssetPath(texture);
//                textureNames += Path.GetFileNameWithoutExtension(textureName) + ", ";
//            }
//        }

//        return textureNames.TrimEnd(',', ' ');
//    }
//}
