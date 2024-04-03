using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class PreProcessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        if (importer == null) return;
        //ordered list of presets that set default values when applied to target
        //the presets here are already filtered by preset manager
        Preset[] presets = Preset.GetDefaultPresetsForObject(importer);
        int presetsLength = presets.Length;
        for (int i = 0; i < presetsLength; i++)
        {
            Preset preset = presets[i];
            preset.ApplyTo(importer);
        }
    }

    void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        String name = importer.assetPath.ToLower();
        if (name.Substring(name.Length - 4, 4) == ".fbx")
        {
            importer.globalScale = 1.0F;
            importer.generateAnimations = ModelImporterGenerateAnimations.None;

            // Update Material settings
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
            importer.materialLocation = ModelImporterMaterialLocation.External;
            importer.materialName = ModelImporterMaterialName.BasedOnTextureName;
            importer.materialSearch = ModelImporterMaterialSearch.Everywhere;
        }
    }
    //private void OnPostprocessModel(GameObject model)
    //{
    //    // Check if the imported asset is a model
    //    if (assetPath.ToLower().EndsWith(".fbx"))
    //    {
    //        // Create a prefab from the imported model
    //        CreatePrefab(model);
    //    }
    //}
    //private void CreatePrefab(GameObject model)
    //{
    //    // Set the prefab path and name (you can customize this)
    //    string prefabFolder = "Assets/Resources/Prefabs/";
    //    string prefabName = Path.GetFileNameWithoutExtension(assetPath);

    //    // Ensure the folder exists, create if not
    //    if (!Directory.Exists(prefabFolder))
    //    {
    //        Directory.CreateDirectory(prefabFolder);
    //    }

    //    // Combine folder path and prefab name
    //    string prefabPath = Path.Combine(prefabFolder, prefabName + ".prefab");

    //    // Create the prefab
    //    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(model, prefabPath);

    //    Debug.Log("Prefab created at: " + prefabPath);
    //}


}