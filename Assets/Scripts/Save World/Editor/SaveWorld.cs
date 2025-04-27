using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveWorld : MonoBehaviour
{
    public static void ExecuteSave()
    {
        var uploadables = Object.FindObjectsOfType<MonoBehaviour>().OfType<IEditorUploadable>();
        foreach (var u in uploadables)
        {
            var assets = u.CollectAssets();
            var payload = u.BuildServerPayload();
            // Build bundles and snapshot
        }
        // Upload zip archive
    }
}
