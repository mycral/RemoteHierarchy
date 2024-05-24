using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace RemoteHierarchy
{
	public class TreeViewItem<T> : TreeViewItem
	{
		public T data;
	}
	
	class RemoteHierarchyView : TreeView
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;

		private Proto.S2C_GameObjectTree m_kTreeData;
		
		public event System.Action<Proto.GameObjectInfo>  OnEvtGameObjectActiveStateChange;
		
		public RemoteHierarchyView(TreeViewState treeViewState)
			: base(treeViewState)
		{
			Reload();
		}


		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem) args.item;
			
			Rect toggleRect = args.rowRect;
			toggleRect.x += GetContentIndent(item);
			toggleRect.width = kToggleWidth;
			//if (toggleRect.xMax < cellRect.xMax)
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
			}
			// Default icon and label
			var restRect = args.rowRect;
			restRect.x += kToggleWidth;
			args.rowRect = restRect;
			base.RowGUI(args);
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
			var allItems = new List<TreeViewItem>();
			BuildTree(allItems);
			// Utility method that initializes the TreeViewItem.children and -parent for all items.
			SetupParentsAndChildrenFromDepths (root, allItems);
			
			// Return root of the tree
			return root;
		}
	}
}

