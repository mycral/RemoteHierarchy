using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteHierarchy.Proto
{
    public class MessageId
    {
        public const int S2C_GameObjectTree = 1;
        public const int C2S_SetGameObjectActiveState = 2;
        public const int C2S_GetGameObjectTree = 3;
    }

    [Serializable]
    public class GameObjectActive
    {
        public int InstanceId;
        public bool IsActive;
    }

    [Serializable]
    public class GameObjectInfo
    {
        public GameObjectInfo() { Children = new List<GameObjectInfo>(); }

        public string Name;
        /// <summary>
        /// 实例Id方便查找单位
        /// </summary>
        public int InstanceId;
        public bool IsActive;
        public List<GameObjectInfo> Children;
    }

    [Serializable]
    public class SceneInfo
    {
        public SceneInfo() { RootGameObjects = new List<GameObjectInfo>(); }
        public string Name;
        public List<GameObjectInfo> RootGameObjects;
    }

    [Serializable]
    public class S2C_GameObjectTree
    {
        public S2C_GameObjectTree() { SceneList = new List<SceneInfo>(); }
        public List<SceneInfo> SceneList;
    }
}



