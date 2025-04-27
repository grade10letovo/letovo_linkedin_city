using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldComponent
{
    void LoadRuntimeState(object data);
    object SaveRuntimeState();
    int ComponentVersion { get; }
}

#if UNITY_EDITOR
public interface IEditorUploadable
{
    IEnumerable<Object> CollectAssets();
    object BuildServerPayload();
}
#endif

public interface IVersionedLoader
{
    object Migrate(object oldData, int fromVersion);
}

