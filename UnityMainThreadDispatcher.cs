using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteHierarchy
{
    class UnityMainThreadDispatcher :MonoBehaviour
    {
        private static UnityMainThreadDispatcher _Instance;
        public static UnityMainThreadDispatcher Instance {
            get{
                if(_Instance == null)
                {
                    var go = new GameObject(nameof(UnityMainThreadDispatcher));
                    _Instance = go.AddComponent<UnityMainThreadDispatcher>();
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
        }
    }
}
