using UnityEngine;

namespace RemoteHierarchy
{
    public class StartServer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Start()
        {
            var go = new GameObject("RemoteHierarchy.Server");
            go.AddComponent<Server>();
            go.AddComponent<UnityMainThreadDispatcher>();
            Object.DontDestroyOnLoad(go);
#if !UNITY_EDITOR
            go.hideFlags = HideFlags.HideAndDontSave;
#endif
        }
    }
}