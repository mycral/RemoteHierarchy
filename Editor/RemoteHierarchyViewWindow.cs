using UnityEngine;
using UnityEditor.IMGUI.Controls;
using RemoteHierarchy;
using RemoteHierarchy.Proto;
using RemoteHierarchy.RemoteHierarchy.Editor;
using UnityEditor;
namespace RemoteHierarchy
{
	class RemoteHierarchyViewWindow : EditorWindow
	{
		private Client m_kClient = new Client();
		[SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        RemoteHierarchyView m_TreeView;
		SearchField m_SearchField;
		private string m_kHost;
		private EditorGUISplitView m_kHorizontalSplitView = new EditorGUISplitView (EditorGUISplitView.Direction.Horizontal);
		void OnEnable ()
		{
			
			m_kHost = EditorPrefs.GetString(StringTable.RemoteHierarchyViewWindowHost, "127.0.0.1");
			m_kClient.OnEvtNewGameObjectTree -= OnEvtNewGameObjectTree;
			m_kClient.OnEvtNewGameObjectTree += OnEvtNewGameObjectTree;
			
			m_kClient.OnEvtResponseGameObjectDetail -= OnEvtResponseGameObjectDetail;
			m_kClient.OnEvtResponseGameObjectDetail += OnEvtResponseGameObjectDetail;
			
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

			m_TreeView.OnEvtNodeClicked -= OnEvtTreeViewItemClicked;
			m_TreeView.OnEvtNodeClicked += OnEvtTreeViewItemClicked;

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

		void OnEvtTreeViewItemClicked(TreeViewItem item)
		{
			if (item is TreeViewItem<GameObjectInfo> goInfo)
			{
				m_kClient.SendGetGameObjectDetail(goInfo.data.InstanceId);
			}
		}

		void OnGUI ()
		{
			DoToolbar ();
			m_kHorizontalSplitView.BeginSplitView();
			DoTreeView ();
			m_kHorizontalSplitView.Split();
			DoInspectorView();
			m_kHorizontalSplitView.EndSplitView();
			Repaint();
		}
		void DoToolbar()
		{
			GUILayout.BeginHorizontal (EditorStyles.toolbar);
			GUILayout.Label(StringTable.ClientAddress);
			m_kHost = GUILayout.TextField(m_kHost,GUILayout.Width(100));
			GUILayout.Label($"{(m_kClient.IsConnected() ? StringTable.Connected : StringTable.NotConnected)}");
			if(m_kClient.IsConnected())
			{
				if(GUILayout.Button(StringTable.Disconnect))
				{
					m_kClient.StopConnect();
				}
				if (GUILayout.Button(StringTable.Refresh))
				{
					m_kClient.SendGetGameObjectList();
				}
			}
			else
			{
				m_TreeView.SetData(null);
				if (GUILayout.Button(StringTable.Connect))
				{
					m_kClient.ConnectToTcpServer(m_kHost);
					EditorPrefs.SetString(StringTable.RemoteHierarchyViewWindowHost, m_kHost);
				}
			}
            GUILayout.Space (100);
			GUILayout.FlexibleSpace();
			m_TreeView.searchString = m_SearchField.OnToolbarGUI (m_TreeView.searchString);
			
			// Add a button for language selection
			if (GUILayout.Button(StringTable.Language))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent(StringTable.English), StringTable.GetCurrentLanguage() == StringTable.LanguageType.English, () => StringTable.SetLanguage(StringTable.LanguageType.English));
				menu.AddItem(new GUIContent(StringTable.Chinese), StringTable.GetCurrentLanguage() == StringTable.LanguageType.Chinese, () => StringTable.SetLanguage(StringTable.LanguageType.Chinese));
				menu.ShowAsContext();
			}
			
			GUILayout.EndHorizontal();
		}

		void DoTreeView()
		{
			Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
			m_TreeView.OnGUI(rect);
		}

		private Proto.S2C_ResponseGameObjectDetail m_kCurDetail;
		
		void OnEvtResponseGameObjectDetail(Proto.S2C_ResponseGameObjectDetail detail)
		{
			m_kCurDetail = detail;
		}

		void DoInspectorView()
		{
			if (m_kCurDetail == null || m_kCurDetail.JsonData == null)
				return;
			
			using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
			{
				GUILayout.Label(m_kCurDetail.JsonData);
			}
			//EditorGUILayout.HelpBox(new GUIContent("DoInspectorView"));
		}

		// Add menu named "My Window" to the Window menu
		[MenuItem ("Window/RemoteHierarchyViewWindow")]
		static void ShowWindow ()
		{
			// Get existing open window or if none, make a new one:
			var window = GetWindow<RemoteHierarchyViewWindow> ();
			window.titleContent = new GUIContent ("RemoteHierarchyViewWindow");
			window.Show ();
		}
	}
}
