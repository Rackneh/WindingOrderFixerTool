using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ContinueBatchAssigner : EditorWindow
{
    private const string TEXTURE_FOLDER_PATH = "Assets/Content/Textures/";
    private const string MATERIAL_FOLDER_PATH = "Assets/Content/Materials/";

    [MenuItem("Custom/Continue Assign Textures to Materials")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ContinueBatchAssigner));
    }
    private void OnGUI()
    {
        GUILayout.Label("Continue Texture Assigner", EditorStyles.boldLabel);


        if (GUILayout.Button("Continue Assigning Textures"))
        {
            try
            {
                ContinueAssigningTextures();
                Debug.Log("Texture assignment completed successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("An error occurred during texture assignment: " + ex.Message);
            }
        }
    }
    private void ContinueAssigningTextures()
    {
        // Phase 3: Create a list of unique textures from "Assets/Content/Textures/".
        string[] existingTextureFiles = Directory.GetFiles(TEXTURE_FOLDER_PATH, "*.png", SearchOption.TopDirectoryOnly);
        List<Texture2D> existingTextures = new List<Texture2D>();

        foreach (string textureFile in existingTextureFiles)
        {
            try
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFile);
                if (texture != null)
                {
                    existingTextures.Add(texture);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error loading existing texture file '" + textureFile + "': " + ex.Message);
            }
        }

        // Phase 4: Go over all textures in "Assets/Import" and assign them to materials.
        string[] importedTextureFiles = Directory.GetFiles("Assets/Import", "*.tga", SearchOption.AllDirectories);

        foreach (string textureFile in importedTextureFiles)
        {
            try
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureFile);
                if (texture != null)
                {
                    if (!existingTextures.Exists(t => t.name == texture.name))
                    {
                        // If the texture is not already in "Assets/Content/Textures", move it there.
                        string newTexturePath = TEXTURE_FOLDER_PATH + texture.name + ".png";
                        AssetDatabase.MoveAsset(textureFile, newTexturePath);
                        Debug.Log("Texture assigned " + texture.name + " at " + newTexturePath);
                    }
                    else
                    {
                        Debug.Log("Texture " + texture.name + " already exists in " + TEXTURE_FOLDER_PATH);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error processing imported texture file '" + textureFile + "': " + ex.Message);
            }
        }
        string[] materialFiles = Directory.GetFiles("Assets/Import", "*.mat", SearchOption.AllDirectories);

        foreach (string materialFile in materialFiles)
        {
            try
            {
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialFile);
                if (material != null)
                {
                    // Find a texture in uniqueTextures with the same name as the material.
                    Texture2D matchingTexture = existingTextures.Find(tex => tex.name == material.name);

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
