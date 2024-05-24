using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace RemoteHierarchy
{
    class EditorUnityMainThreadDispatcher
    {
        private static EditorUnityMainThreadDispatcher _Instance;
        public static EditorUnityMainThreadDispatcher Instance {
            get{
                if(_Instance == null)
                {
                    _Instance = new EditorUnityMainThreadDispatcher();
                    EditorApplication.update -= _Instance.Update;
                    EditorApplication.update += _Instance.Update;
                }
                return _Instance;
            } 
        }

        private List<Action> m_kActions = new List<Action>();

        public void AddAction(Action action)
        {
            lock(this)
            {
                m_kActions.Add(action);
            }
        }

        private void Update()
        {
            lock (this)
            {
                foreach(var action in m_kActions)
                {
                    try
                    {
                        action();
                    }
                    catch(Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                m_kActions.Clear();
            }
            //Debug.Log("Update" + DateTimeOffset.Now);
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
            EditorUnityMainThreadDispatcher.Instance.AddAction(act);
            while (isExec == false)
            {
                System.Threading.Thread.Sleep(0);
            }
        }
        
    }
}
