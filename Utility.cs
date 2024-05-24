using RemoteHierarchy.Proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RemoteHierarchy
{
    public class Utility
    {
        public static S2C_GameObjectTree BuildGameObjectTree()
        {
            S2C_GameObjectTree ret = new S2C_GameObjectTree();

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                var curScene = SceneManager.GetSceneAt(i);
                ret.SceneList.Add(BuildSceneInfo(curScene));
            }

            return ret;
        }

        public static SceneInfo BuildSceneInfo(Scene scene)
        {
            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.Name = scene.name;
            var rootGos = scene.GetRootGameObjects();
            foreach (var go in rootGos)
            {
                sceneInfo.RootGameObjects.Add(BuildGameObjectInfo(go));
            }
            return sceneInfo;
        }

        public static GameObjectInfo BuildGameObjectInfo(GameObject go)
        {
            GameObjectInfo ret = new GameObjectInfo();
            ret.Name = go.name;
            ret.InstanceId = go.GetInstanceID();
            ret.IsActive = go.activeSelf;
            for(int i=0;i < go.transform.childCount;++i)
            {
                ret.Children.Add( BuildGameObjectInfo(go.transform.GetChild(i).gameObject));
            }
            return ret;
        }
        public static byte[] BuildMessageBytes(int messageId)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(4 * 2));
            bytes.AddRange(BitConverter.GetBytes(messageId));
            return bytes.ToArray();
        }

        public static byte[] BuildMessageBytes(int messageId,object msgObject)
        {
            List<byte> bytes = new List<byte>();

            string jsonStr = JsonUtility.ToJson(msgObject);
            byte[] buff = System.Text.UTF8Encoding.UTF8.GetBytes(jsonStr);
            bytes.AddRange(BitConverter.GetBytes(buff.Length + 4 * 2));
            bytes.AddRange(BitConverter.GetBytes(messageId));
            bytes.AddRange(buff);
            return bytes.ToArray();
        }

        public static (int , byte[]) ResolveMessageBytes(byte[] messageBytes)
        {
            int messageId = BitConverter.ToInt32(messageBytes, 4);
            byte[] bytes = new byte[messageBytes.Length - 8];
            Array.Copy(messageBytes,8,bytes,0, bytes.Length);
            return (messageId, bytes);
        }

        public static T ResoveMessageBody<T>(byte[] bytes,int length)
        {
            string str = System.Text.UTF8Encoding.UTF8.GetString(bytes, 8, length - 8);
            return JsonUtility.FromJson<T>(str);
        }

        public static GameObject GetGameObjectFromInstanceId(int id)
        {
            var gos = GameObject.FindObjectsOfType<GameObject>(true);
            foreach(var go in gos)
            {
                if (go.GetInstanceID() == id)
                    return go;
            }
            return null;
        }


        public static void WaitExecOnMainThread(Action action)
        {
            bool isExec = false;
            Action act = () =>
            {
                try
                {
                    action();
                }
                catch(Exception ex)
                {
                    Debug.LogException(ex);
                }
                isExec = true;
            };
            UnityMainThreadDispatcher.Instance.AddAction(act);
            while (isExec == false)
            {
                Thread.Sleep(0);
            }
        }



    }
}
