using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Utils
{
    [MenuItem("Tools/Storage/Open %SO")]
    private static void OpenGameStorage()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Tools/Storage/Clear %SR")]
    private static void ClearGameStorage()
    {
        PlayerPrefs.DeleteAll();
        foreach (var file in Directory.GetFiles(Application.persistentDataPath))
        {
            File.Delete(file);
        }
    }
}
