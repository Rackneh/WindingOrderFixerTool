using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureToMaterialAssigner : EditorWindow
{
    private const string TEXTURE_FOLDER_PATH = "Assets/Content/Textures/";
    private const string MATERIAL_FOLDER_PATH = "Assets/Content/Materials/";

    [MenuItem("Tools/Assign Textures to Materials")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TextureToMaterialAssigner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Texture Material Assigner", EditorStyles.boldLabel);

        if (GUILayout.Button("Assign Textures to Materials"))
        {
            try
            {
                AssignTexturesToMaterials();
                Debug.Log("Texture assignment completed successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("An error occurred during texture assignment: " + ex.Message);
            }
        }
    }

    private static void AssignTexturesToMaterials()
    {


        string[] materialFiles = Directory.GetFiles(MATERIAL_FOLDER_PATH, "*.mat");

        foreach (string materialFile in materialFiles)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialFile);

            if (material != null && material.mainTexture == null)
            {
                string textureName = Path.GetFileNameWithoutExtension(materialFile);
                string pngTexturePath = Path.Combine(TEXTURE_FOLDER_PATH, textureName + ".png");
                string tgaTexturePath = Path.Combine(TEXTURE_FOLDER_PATH, textureName + ".tga");

                Texture2D texture = null;

                if (File.Exists(pngTexturePath))
                {
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(pngTexturePath);
                }
                else if (File.Exists(tgaTexturePath))
                {
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(tgaTexturePath);
                }

                if (texture != null)
                {
                    material.mainTexture = texture;
                    Debug.Log("Assigned texture to material: " + material.name);
                    EditorUtility.SetDirty(material);
                }
                else
                {
                    Debug.LogWarning("Texture not found for material: " + material.name);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;

//public class TextureToMaterialAssigner : EditorWindow
//{
//    private const string TEXTURE_FOLDER_PATH = "Assets/Content/Textures/";
//    private const string MATERIAL_FOLDER_PATH = "Assets/Content/Materials/";

//    [MenuItem("Custom/Assign Textures to Materials")]
//    public static void ShowWindow()
//    {
//        EditorWindow.GetWindow(typeof(TextureToMaterialAssigner));
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Texture Material Assigner", EditorStyles.boldLabel);

//        if (GUILayout.Button("Assign Textures to Materials"))
//        {
//            try
//            {
//                AssignTexturesToMaterials();
//                Debug.Log("Texture assignment completed successfully.");
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogError("An error occurred during texture assignment: " + ex.Message);
//            }
//        }
//    }

//    private void AssignTexturesToMaterials()
//    {
//        // Phase 1: Create a list of unique textures from "Assets/Content/Textures/".
//        string[] textureFiles = Directory.GetFiles(TEXTURE_FOLDER_PATH, "*.png", SearchOption.TopDirectoryOnly);
//        List<Texture2D> textures = new List<Texture2D>();

//        foreach (string textureFile in textureFiles)
//        {
//            try
//            {
//                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFile);
//                if (texture != null)
//                {
//                    textures.Add(texture);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogWarning("Error loading texture file '" + textureFile + "': " + ex.Message);
//            }
//        }

//        // Phase 2: Create a list of materials from "Assets/Content/Materials/".
//        string[] materialFiles = Directory.GetFiles(MATERIAL_FOLDER_PATH, "*.mat", SearchOption.TopDirectoryOnly);
//        List<Material> materials = new List<Material>();

//        foreach (string materialFile in materialFiles)
//        {
//            try
//            {
//                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialFile);
//                if (material != null)
//                {
//                    materials.Add(material);
//                }
//            }
//            catch (System.Exception ex)
//            {
//                Debug.LogWarning("Error loading material file '" + materialFile + "': " + ex.Message);
//            }
//        }

//        // Phase 3: Assign textures to materials with the same name.
//        foreach (Material material in materials)
//        {
//            string textureName = material.name;
//            Texture2D matchingTexture = textures.Find(tex => tex.name == textureName);

//            if (matchingTexture != null)
//            {
//                material.mainTexture = matchingTexture;
//                Debug.Log("Assigned texture to material: " + material.name);
//            }
//            else
//            {
//                Debug.LogWarning("No matching texture found for material: " + material.name);
//            }
//        }
//    }
//}
