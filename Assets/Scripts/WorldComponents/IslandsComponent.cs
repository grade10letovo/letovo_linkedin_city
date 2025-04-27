using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsComponent : MonoBehaviour, IWorldComponent
#if UNITY_EDITOR
    , IEditorUploadable
#endif
{
    public void LoadRuntimeState(object data)
    {
    }
    public object SaveRuntimeState()
    {
        return null;
    }
    public IEnumerable<Object> CollectAssets()
    {
        return null;
    }
    public object BuildServerPayload()
    {
        return null;
    }
    public int ComponentVersion { 
        get
        {
            return 0;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
