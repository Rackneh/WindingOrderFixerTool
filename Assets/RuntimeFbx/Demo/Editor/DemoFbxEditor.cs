using UnityEditor;
using UnityEngine;

namespace RuntimeFbx.Demo
{
    public class DemoFbxEditor : EditorWindow
    {
        [MenuItem("Tools/RuntimeFbx/EditorDemo")]
        public static void ShowWindow()
        {
            var window = GetWindow<DemoFbxEditor>(true, "RuntimeFbx Editor Demo");
            const int width = 400;
            const int height = 240;
            window.minSize = new Vector2(width, height);
            int x = (Screen.currentResolution.width - width) / 2;
            int y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);
            window.Show();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnGUI()
        {
            GUI.enabled = Selection.activeGameObject && Selection.activeGameObject.activeInHierarchy;
            if (GUILayout.Button("1. Export Selected Mesh To /Assets"))
            {
                ExportSelectedMesh();
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("2. Export a Cube Mesh To /Assets"))
            {
                ExportCubeFbx();
            }
        }

        private static void ExportSelectedMesh()
        {
            var obj = Selection.activeGameObject;
            if (!obj)
            {
                Debug.LogWarning("No GameObject is selected.");
                return;
            }
            var meshFilter = obj.GetComponent<MeshFilter>();
            if (!meshFilter)
            {
                Debug.LogWarning($"No MeshFilter found in selected GameObject<{obj.name}>");
                return;
            }

            var mesh = meshFilter.sharedMesh;
            if (!mesh)
            {
                Debug.LogWarning("Mesh is invalid.");
                return;
            }
            Util.SaveMesh(mesh, Application.dataPath + "/export_mesh.fbx");
            AssetDatabase.Refresh();
        }

        private static void ExportCubeFbx()
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.hideFlags = HideFlags.HideAndDontSave;
            var meshFilter = obj.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            Util.SaveMesh(mesh, Application.dataPath + "/cube.fbx");
            DestroyImmediate(obj);
            AssetDatabase.Refresh();
        }
    }
}

