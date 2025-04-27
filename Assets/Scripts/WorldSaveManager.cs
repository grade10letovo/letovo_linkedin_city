using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
#endif
using UnityEngine;

public class WorldSaveManager : MonoBehaviour
{
#if UNITY_EDITOR
    [HideInInspector] public List<IEditorUploadable> uploadableObjects = new List<IEditorUploadable>();

    // Called to find all objects that implement IEditorUploadable
    public void FindUploadableObjects()
    {
        uploadableObjects.Clear();
        var objects = FindObjectsOfType<MonoBehaviour>(true);
        foreach (var obj in objects)
        {
            if (obj is IEditorUploadable uploadable)
            {
                uploadableObjects.Add(uploadable);
            }
        }
    }
    // Prepare and save world data (called from Editor button)
    public void SaveWorld()
    {
        FindUploadableObjects();

        // Collect assets by tag (used for different asset bundles)
        Dictionary<string, HashSet<Object>> assetsByTag = CollectAssetsByTag();

        // Prepare snapshot data
        var snapshotData = PrepareSnapshot(uploadableObjects);

        // Process and upload data to server
        StartCoroutine(UploadDataToServer(assetsByTag, snapshotData));
    }


    private Dictionary<string, HashSet<Object>> CollectAssetsByTag()
    {
        var assetsByTag = new Dictionary<string, HashSet<Object>>();

        foreach (var uploadable in uploadableObjects)
        {
            var assets = uploadable.CollectAssets();
            //var tag = uploadable.GetComponent<MonoBehaviour>().gameObject.tag;

            if (!assetsByTag.ContainsKey(tag))
                assetsByTag[tag] = new HashSet<Object>();

            foreach (var asset in assets)
            {
                assetsByTag[tag].Add(asset);
            }
        }

        return assetsByTag;
    }
    private object PrepareSnapshot(List<IEditorUploadable> uploadableObjects)
    {
        var snapshotData = new List<ObjectSnapshot>();

        foreach (var obj in uploadableObjects)
        {
            var payload = obj.BuildServerPayload();
            snapshotData.Add(new ObjectSnapshot
            {
                //objectId = obj.GetInstanceID(),
                type = obj.GetType().Name,
                data = payload
            });
        }

        return snapshotData;
    }

    private IEnumerator UploadDataToServer(Dictionary<string, HashSet<Object>> assetsByTag, object snapshotData)
    {
        // Upload the asset bundles for each tag
        /*foreach (var tag in assetsByTag.Keys)
        {
            var bundlePath = await PrepareAssetBundles(assetsByTag[tag], tag);
            // Simulate sending the bundle to server
            yield return StartCoroutine(SendBundleToServer(bundlePath));
        }*/

        // Upload the snapshot data
        yield return StartCoroutine(SendSnapshotDataToServer(snapshotData));
    }

    private async Task<string> PrepareAssetBundles(HashSet<Object> assets, string tag)
    {
        // Create the asset bundle using Addressables or AssetBundles
        List<string> bundlePaths = new List<string>();

        foreach (var asset in assets)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!string.IsNullOrEmpty(assetPath))
            {
                // Example of adding assets to Addressables (can be adjusted for specific bundles)
                //AddressableAssetSettings.DefaultGroup.AddAssetEntry(assetPath);
            }
        }

        // Build Addressables
        AddressableAssetSettings.BuildPlayerContent();

        // Assuming bundles are saved and we know the path
        string bundleDirectory = "Assets/AddressableAssets/" + tag;
        /*bundlePaths = AssetDatabase.FindAssets("t:AssetBundle", new[] { bundleDirectory })
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToList();*/

        return string.Join(",", bundlePaths);
    }
#endif


    private IEnumerator SendBundleToServer(string bundlePath)
    {
        // Simulate sending bundle to the server
        Debug.Log($"Sending bundle {bundlePath} to server...");
        yield return null; // Simulate a delay for sending
    }

    private IEnumerator SendSnapshotDataToServer(object snapshotData)
    {
        // Simulate sending snapshot data to server
        Debug.Log("Sending snapshot data to server...");
        yield return null; // Simulate a delay for sending
    }
}

public class ObjectSnapshot
{
    public int objectId;
    public string type;
    public object data;
}
