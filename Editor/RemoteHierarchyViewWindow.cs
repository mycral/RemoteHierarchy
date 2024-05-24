using UnityEngine;
using UnityEditor.IMGUI.Controls;
using RemoteHierarchy;
using UnityEditor;
namespace RemoteHierarchy
{
	class RemoteHierarchyViewWindow : EditorWindow
	{
		private Client m_kClient = new Client();
		// We are using SerializeField here to make sure view state is written to the window 
		// layout file. This means that the state survives restarting Unity as long as the window
		// is not closed. If omitting the attribute then the state just survives assembly reloading 
		// (i.e. it still gets serialized/deserialized)
		[SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        RemoteHierarchyView m_TreeView;
		SearchField m_SearchField;
		private string m_kHost;
		void OnEnable ()
		{
			m_kHost = EditorPrefs.GetString("RemoteHierarchyViewWindow_Host", "127.0.0.1");
			m_kClient.OnEvtNewGameObjectTree -= OnEvtNewGameObjectTree;
			m_kClient.OnEvtNewGameObjectTree += OnEvtNewGameObjectTree;
			
			// Check if we already had a serialized view state (state 
			// that survived assembly reloading)
			if (m_TreeViewState == null)
				m_TreeViewState = new TreeViewState ();

			m_TreeView = new RemoteHierarchyView(m_TreeViewState);
			m_SearchField = new SearchField ();
			m_SearchField.downOrUpArrowKeyPressed -= m_TreeView.SetFocusAndEnsureSelectedItem;
			m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

			m_TreeView.OnEvtGameObjectActiveStateChange -= OnEvtGameObjectInfoChange;
			m_TreeView.OnEvtGameObjectActiveStateChange += OnEvtGameObjectInfoChange;
		}

		void OnEvtNewGameObjectTree(Proto.S2C_GameObjectTree tree)
		{
			m_TreeView.SetData(tree);
			Repaint();
		}

		void OnEvtGameObjectInfoChange(Proto.GameObjectInfo goInfo)
		{
			m_kClient.SendSetGameObjectActive(goInfo.InstanceId, goInfo.IsActive);
			Repaint();
		}

		void OnGUI ()
		{
			DoToolbar ();
			DoTreeView ();
		}
		void DoToolbar()
		{
			GUILayout.BeginHorizontal (EditorStyles.toolbar);
			GUILayout.Label("客户端地址");
			m_kHost = GUILayout.TextField(m_kHost,GUILayout.Width(100));
			GUILayout.Label($"{(m_kClient.IsConnected() ? "已经连接" : "未连接")}");
			if(m_kClient.IsConnected())
			{
				if(GUILayout.Button("断开"))
				{
					m_kClient.StopConnect();
				}
			}
			else
			{
				m_TreeView.SetData(null);
                if (GUILayout.Button("连接"))
                {
                    m_kClient.ConnectToTcpServer(m_kHost);
                    EditorPrefs.SetString("RemoteHierarchyViewWindow_Host", m_kHost);
                }
            }
            if (GUILayout.Button("刷新"))
            {
                m_kClient.SendGetGameObjectList();
            }
            GUILayout.Space (100);
			GUILayout.FlexibleSpace();
			m_TreeView.searchString = m_SearchField.OnToolbarGUI (m_TreeView.searchString);
			GUILayout.EndHorizontal();
		}

		void DoTreeView()
		{
			Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
			m_TreeView.OnGUI(rect);
		}

		// Add menu named "My Window" to the Window menu
		[MenuItem ("TreeView Examples/RemoteHierarchyViewWindow")]
		static void ShowWindow ()
		{
			// Get existing open window or if none, make a new one:
			var window = GetWindow<RemoteHierarchyViewWindow> ();
			window.titleContent = new GUIContent ("RemoteHierarchyViewWindow");
			window.Show ();
		}
	}
}
