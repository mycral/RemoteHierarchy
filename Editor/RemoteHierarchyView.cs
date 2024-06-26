using System.Collections.Generic;
using RemoteHierarchy.EditorGUIHelper;
using RemoteHierarchy.Proto;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace RemoteHierarchy
{
	public class TreeViewItem<T> : TreeViewItem
	{
		public T data;
		public bool isInActiveInHierarchy = true;
	}
	
	class RemoteHierarchyView : TreeView
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;

		private Proto.S2C_GameObjectTree m_kTreeData;

		private List<TreeViewItem> m_kTreeViewItemList;
		
		public event System.Action<Proto.GameObjectInfo>  OnEvtGameObjectActiveStateChange;
		public event System.Action<TreeViewItem>  OnEvtNodeClicked;
		
		public RemoteHierarchyView(TreeViewState treeViewState)
			: base(treeViewState)
		{
			Reload();
		}

		protected override void SingleClickedItem(int id)
		{
			base.SingleClickedItem(id);

			var treeViewItem = m_kTreeViewItemList.Find(o => o.id == id);
			if (OnEvtNodeClicked != null)
			{
				OnEvtNodeClicked(treeViewItem);
			}
		}

		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem) args.item;
			
			Rect toggleRect = args.rowRect;
			toggleRect.x += GetContentIndent(item);
			toggleRect.width = kToggleWidth;

			bool isGrayLabel = false;
			
			if (item is TreeViewItem<Proto.GameObjectInfo> goInfo)
			{
				var isActive = goInfo.data.IsActive;
				isActive = EditorGUI.Toggle(toggleRect, isActive);
				if (goInfo.data.IsActive != isActive)
				{
					if (OnEvtGameObjectActiveStateChange != null)
					{
						goInfo.data.IsActive = isActive;
						OnEvtGameObjectActiveStateChange(goInfo.data);
					}
				}
				isGrayLabel = !goInfo.isInActiveInHierarchy;
			}
			var restRect = args.rowRect;
			if (item is TreeViewItem<Proto.GameObjectInfo>)
			{
				restRect.x += kToggleWidth;
			}
			args.rowRect = restRect;

			if (isGrayLabel)
			{
				using (new GUIColor(Color.grey))
				{
					base.RowGUI(args);
				}
			}
			else
			{
				base.RowGUI(args);
			}
		}
		
		
		public void SetData(Proto.S2C_GameObjectTree tree)
		{
			m_kTreeData = tree;
			Reload();
		}

		void BuildTree(List<TreeViewItem> treeViewItem)
		{
			if (m_kTreeData == null)
				return;
			
			m_kTreeData.SceneList.ForEach(x =>
			{
				treeViewItem.Add(new TreeViewItem<Proto.SceneInfo> { id = x.Name.GetHashCode(), depth = 0, displayName = x.Name ,data = x });
				foreach (var goInfo in x.RootGameObjects)
				{
					BuildGameObjectTree(treeViewItem, goInfo,1);
				}
			});
		}

		void BuildGameObjectTree(List<TreeViewItem> treeViewItem,Proto.GameObjectInfo goInfo, int depth)
		{
			treeViewItem.Add(new TreeViewItem<Proto.GameObjectInfo> { id = goInfo.InstanceId, depth = depth, displayName = goInfo.Name, data = goInfo });
			if (goInfo.Children != null)
			{
				foreach (var child in goInfo.Children)
				{
					BuildGameObjectTree(treeViewItem, child, depth + 1);
				}
			}
		}
		
		protected override TreeViewItem BuildRoot ()
		{
			// BuildRoot is called every time Reload is called to ensure that TreeViewItems 
			// are created from data. Here we just create a fixed set of items, in a real world example
			// a data model should be passed into the TreeView and the items created from the model.

			// This section illustrates that IDs should be unique and that the root item is required to 
			// have a depth of -1 and the rest of the items increment from that.
			var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
			m_kTreeViewItemList = new List<TreeViewItem>();
			BuildTree(m_kTreeViewItemList);

			//标记真正未激活的对象
			int depth = 0;
			bool isFindInActive = false;
			for (int i = 0; i < m_kTreeViewItemList.Count; ++i)
			{
				var curItem = m_kTreeViewItemList[i];
				if (curItem is TreeViewItem<Proto.GameObjectInfo> goInfo)
				{
					if (!goInfo.data.IsActive && !isFindInActive)
					{
						isFindInActive = true;
						goInfo.isInActiveInHierarchy = false;
						depth = goInfo.depth;
						continue;
					}
					if (depth < goInfo.depth && isFindInActive)
					{
						goInfo.isInActiveInHierarchy = false;
					}
					else
					{
						isFindInActive = false;
					}
				}
			}
			// Utility method that initializes the TreeViewItem.children and -parent for all items.
			SetupParentsAndChildrenFromDepths (root, m_kTreeViewItemList);
			
			// Return root of the tree
			return root;
		}
	}
}

