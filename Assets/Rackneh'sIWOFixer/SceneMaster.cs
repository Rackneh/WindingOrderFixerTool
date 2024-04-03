///////////////////////// \\\\\\\\\\\\\\\\\\\_\?\X\\
/////////      MeshFixer SceneMaster      \\\\\\\\\\
//      General Purpose Winding Order Fixer       \\
//                                                \\
// Created by ChatGPT, Muse, Usling123 & Rackneh  \\
//  Applies to a single object and it's children  \\
//               Made for MGS3 VR                 \\
//                                                \\
//                     GG ez                      \\
//                                                \\
///////////////////////// \\\\\\\\\\\\\\\\\\\\\\\\\\
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MGS3
{

    public class SceneMaster : MonoBehaviour
    {
        public bool UseFixer = false;
        public List<MeshFixer> meshFixers;



        void OnValidate()
        {
            UseFixer = false;
            if (UseFixer == false)
            {
                foreach (MeshFixer flipNormals in meshFixers)
                {
                    Debug.Log("Tried to fix " + flipNormals);
                    FlipNormals2(flipNormals.transform);
                }
                UseFixer = true;
            }
        }

        public void FlipNormals2(Transform parent)
        {
            MeshFixer flipNormals = parent.GetComponent<MeshFixer>();
            if (flipNormals != null)
            {
                Debug.Log(parent);
                flipNormals.FixNormals(parent, 0.01f);
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                // Check if the child has a MeshRenderer or SkinnedMeshRenderer
                if (child.GetComponent<MeshRenderer>() != null || child.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    FlipNormals2(child);
                    flipNormals.FixNormals(child, 0.01f);
                    Debug.Log(child);
                }
            }
        }
    }
}
