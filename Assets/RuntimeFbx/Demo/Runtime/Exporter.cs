using UnityEngine;

public class Exporter : MonoBehaviour
{
    private Selector selector;

    void Start()
    {
        selector = FindObjectOfType<Selector>();
    }

    public void OnClickExportButton()
    {
        if (!selector.selectedTransform)
        {
            Debug.LogWarning("No Mesh GameObject selected");
            return;
        }

        var meshFilter = selector.selectedTransform.GetComponent<MeshFilter>();
        Debug.Assert(meshFilter);
        var mesh = meshFilter.sharedMesh;
        var fileName = ToValidFileName(mesh.name);
        var filePath = UnifySlash($"{Application.persistentDataPath}/{fileName}.fbx");
        RuntimeFbx.Util.SaveMesh(mesh, filePath);
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogWarning($"Failed to save mesh<{mesh.name}> to path<{filePath}>");
            return;
        }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        string argument = $"/select, \"{filePath}\"";
        System.Diagnostics.Process.Start("explorer.exe", argument);
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        var argument = $"-R \"{filePath}\"";
        System.Diagnostics.Process.Start("open", argument);
#endif
    }
    
    private static string ToValidFileName(string name)
    {   
        var invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
        foreach (char c in invalidFileNameChars)
        {
            name = name.Replace(c, '_');
        }

        name = name.Trim();
        
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name))
        {
            name = "default";
            Debug.LogWarning("Name is empty or white space, set to <default> instead.");
        }
        return name;
    }

    private static string UnifySlash(string str)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return str.Replace('/', '\\');
#else
        return str.Replace('\\', '/');
#endif
    }
}
