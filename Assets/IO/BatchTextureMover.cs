//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;

//public class BatchTextureMover : MonoBehaviour
//{
//    public List<Material> materials;
//    public List<Texture2D> textures;
//    public bool use = false;

//    private const string TEXTURE_FOLDER_PATH = "Assets/Content/Textures/";

//    void OnValidate()
//    {
//        if (!use)
//        {
//            Move();
//            Debug.Log("Attemtped to move textures");
//            //use = true;
//        }
//    }

//    public void Move()
//    {
//        foreach (Texture texture in textures)
//        {
//            bool isAssigned = false;
//            foreach (Material material in materials)
//            {
//                if (material.mainTexture == texture)
//                {
//                    isAssigned = true;
//                    break;
//                }
//            }
//            if (!isAssigned)
//            {
//                string path = AssetDatabase.GetAssetPath(texture);
//                string fileName = Path.GetFileName(path);
//                string destinationPath = "Assets/Content/TextureD/" + fileName;
//                FileUtil.MoveFileOrDirectory(path, destinationPath);
//            }
//        }
//    }
//}
//    //foreach (Material mat in materials)
//    //{
//    //    foreach (Texture2D tex in uniqueTextures)
//    //    {
//    //        if (tex.name == mat.name)
//    //        {
//    //            // Load the texture from its new project folder location
//    //            Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TEXTURE_FOLDER_PATH + tex.name + ".png");
//    //            mat.mainTexture = loadedTexture;
//    //            break;
//    //        }
//    //    }
//    //}
