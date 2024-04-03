using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BatchTextureAssigner : EditorWindow
{
    private const string TEXTURE_FOLDER_PATH = "Assets/Content/Textures/";
    private const string MATERIAL_FOLDER_PATH = "Assets/Content/Materials/";

    [MenuItem("Custom/Assign Textures to Materials in Batch")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BatchTextureAssigner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Texture Assigner", EditorStyles.boldLabel);

        if (GUILayout.Button("Assign Textures to Materials in Batch"))
        {
            try
            {
                AssignTexturesToMaterials();
                Debug.Log("Batch texture assignment completed successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("An error occurred during batch texture assignment: " + ex.Message);
            }
        }
    }

    private void AssignTexturesToMaterials()
    {
        // Phase 1: Create a list of unique textures and move them to the "Assets/Content/Textures" folder.
        string[] textureFiles = Directory.GetFiles("Assets/Import", "*.tga", SearchOption.AllDirectories);
        List<Texture2D> uniqueTextures = new List<Texture2D>();

        foreach (string textureFile in textureFiles)
        {
            try
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFile);
                if (texture != null)
                {
                    string texturePath = AssetDatabase.GetAssetPath(texture);
                    string newTexturePath = TEXTURE_FOLDER_PATH + texture.name + ".png";
                    
                    if (!uniqueTextures.Exists(t => t.name == texture.name))
                    {
                        // Check if a texture with the same name exists in the "Assets/Content/Textures" folder.
                        if (!AssetDatabase.LoadAssetAtPath<Texture2D>(newTexturePath))
                        {
                            // If not, add it to the uniqueTextures list and move it.
                            uniqueTextures.Add(texture);
                            AssetDatabase.MoveAsset(texturePath, newTexturePath);
                            Debug.Log("Texture assigned " + texture.name + " at " + newTexturePath);
                        }
                        else
                        {
                            // Delete the duplicate texture in "Assets/Import/".
                            AssetDatabase.DeleteAsset(texturePath);
                            Debug.Log("Duplicate texture deleted: " + texture.name);
                        }
                    }
                    else
                    {
                        // The texture is not unique; delete it.
                        AssetDatabase.DeleteAsset(texturePath);
                        Debug.Log("Texture deleted: " + texture.name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error processing texture file '" + textureFile + "': " + ex.Message);
            }
        }
        // Phase 2: Go over all materials and assign textures with matching names from uniqueTextures.
        string[] materialFiles = Directory.GetFiles("Assets/Import", "*.mat", SearchOption.AllDirectories);

        foreach (string materialFile in materialFiles)
        {
            try
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialFile);
                if (material != null)
                {
                    // Find a texture in uniqueTextures with the same name as the material.
                    Texture2D matchingTexture = uniqueTextures.Find(tex => tex.name == material.name);

                    if (matchingTexture != null)
                    {
                        string materialPath = AssetDatabase.GetAssetPath(material);
                        string newMaterialPath = MATERIAL_FOLDER_PATH + material.name + ".mat";
                        AssetDatabase.MoveAsset(materialPath, newMaterialPath);

                        // Assign the matching texture to the material.
                        material.mainTexture = matchingTexture;
                        Debug.Log("Material assigned " + material.name + " at " + newMaterialPath);
                    }
                    else
                    {
                        Debug.LogWarning("No matching texture found for material: " + material.name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error processing material file '" + materialFile + "': " + ex.Message);
            }
        }
    }
}

