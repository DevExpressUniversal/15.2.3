#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using DevExpress.Utils.Taskbar;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.Utils.Design.Taskbar {
	public class JumpListEditorCategoryCollectionForm : JumpListEditorForm {
		JumpListCategoryCollection collection;
		public JumpListEditorCategoryCollectionForm(ITypeDescriptorContext context, IServiceProvider provider, object value)
			: base(context, provider, value) {
			BtnAdd.Text = "Add Category";
			LblTasks.Text = "Custom Categories:";
			BtnAdd.Click += delegate {
				Context.OnComponentChanging();
				JumpListCategory category = new JumpListCategory("Category");
				collection.Add(category);
				RefreshCollection();
				SelectNode(category);
				TaskbarAssistant.Container.Add(category);
				AddedComponents.Add(category);
				Context.OnComponentChanged();
			};
		}
		void SelectNode(object value) {
			foreach(TreeNode nodeCategory in TreeView.Nodes) {
				if(nodeCategory.Tag.Equals(value)) SelectNode(nodeCategory);
				foreach (TreeNode nodeTask in nodeCategory.Nodes.Cast<TreeNode>().Where(nodeTask => nodeTask.Tag.Equals(value)))
					SelectNode(nodeTask);
			}
		}
		void SelectNode(TreeNode node) {
			TreeView.SelectedNode = node;
			TreeView.Select();
		}
		protected override void InitializeCollection(object list) {
			collection = (JumpListCategoryCollection)list;
			RefreshCollection();
			base.InitializeCollection(list);
		}
		protected override void RefreshCollection() {
			TreeView.Nodes.Clear();
			int value = 0;
			foreach(JumpListCategory category in collection) {
				TreeNode node = new TreeNode(category.Caption) { Tag = category };
				TreeView.Nodes.Add(node);
				foreach (TreeNode nodeTask in from JumpListItemTask task in category.JumpItems select new TreeNode(task.Caption) { Tag = task })
					TreeView.Nodes[value].Nodes.Add(nodeTask);
				value++;
			}
			TreeView.ExpandAll();
			base.RefreshCollection();
		}
		protected override void UpdateButtons() {
			BtnRemove.Enabled = BtnAddTask.Enabled = TreeView.SelectedNode != null;
			BtnUpCmd.Enabled = BtnRemove.Enabled && CanTreeNodeMoveUp();
			BtnDownCmd.Enabled = BtnRemove.Enabled && CanTreeNodeMoveDown();
		}
		bool CanTreeNodeMoveUp() {
			if(TreeView.SelectedNode == null) return false;
			if(TreeView.SelectedNode.Tag is JumpListCategory) return CanJumpListCategoryMoveUp((JumpListCategory)TreeView.SelectedNode.Tag);
			if(TreeView.SelectedNode.Tag is JumpListItemTask) return CanJumpListItemTaskMoveUp((JumpListItemTask)TreeView.SelectedNode.Tag);
			return false;
		}
		bool CanTreeNodeMoveDown() {
			if(TreeView.SelectedNode == null) return false;
			if(TreeView.SelectedNode.Tag is JumpListCategory) return CanJumpListCategoryMoveDown((JumpListCategory)TreeView.SelectedNode.Tag);
			if(TreeView.SelectedNode.Tag is JumpListItemTask) return CanJumpListItemTaskMoveDown((JumpListItemTask)TreeView.SelectedNode.Tag);
			return false;
		}
		bool CanJumpListCategoryMoveUp(JumpListCategory category) {
			return collection.IndexOf(category) != 0;
		}
		bool CanJumpListCategoryMoveDown(JumpListCategory category) {
			return collection.IndexOf(category) != collection.Count - 1;
		}
		bool CanJumpListItemTaskMoveUp(JumpListItemTask task) {
			JumpListCategory category = collection[TreeView.SelectedNode.Parent.Index];
			return category.JumpItems.IndexOf(task) != 0;
		}
		bool CanJumpListItemTaskMoveDown(JumpListItemTask task) {
			JumpListCategory category = collection[TreeView.SelectedNode.Parent.Index];
			return category.JumpItems.IndexOf(task) != category.JumpItems.Count - 1;
		}
		protected override void OnBtnAddTaskClick(object sender, EventArgs e) {
			if(TreeView.SelectedNode == null) return;
			Context.OnComponentChanging();
			JumpListItemTask task = new JumpListItemTask("Task");
			JumpListCategory category = (JumpListCategory)(TreeView.SelectedNode.Parent == null ? TreeView.SelectedNode.Tag : TreeView.SelectedNode.Parent.Tag);
			category.JumpItems.Add(task);
			RefreshCollection();
			SelectNode(task);
			TaskbarAssistant.Container.Add(task);
			AddedComponents.Add(task);
			Context.OnComponentChanged();
		}
		protected override void OnBtnRemoveClick(object sender, EventArgs e) {
			if(TreeView.SelectedNode == null) return;
			Context.OnComponentChanging();
			if(TreeView.SelectedNode.Parent != null) {
				JumpListItemTask task = (JumpListItemTask)TreeView.SelectedNode.Tag;
				collection[TreeView.SelectedNode.Parent.Index].JumpItems.Remove(task);
				TaskbarAssistant.Container.Remove(task);
				RemovedComponents.Add(task);
			}
			else {
				JumpListCategory category = (JumpListCategory)TreeView.SelectedNode.Tag;
				collection.Remove(category);
				TaskbarAssistant.Container.Remove(category);
				RemovedComponents.Add(category);
			}
			RefreshCollection();
			Context.OnComponentChanged();
			PropertyGrid.SelectedObject = null;
		}
		protected override void OnBtnUpCmdClick(object sender, EventArgs e) {
			TreeNodeMove(-1);
		}
		protected override void OnBtnDownCmdClick(object sender, EventArgs e) {
			TreeNodeMove(+1);
		}
		void TreeNodeMove(int delta) {
			if(TreeView.SelectedNode == null) return;
			Context.OnComponentChanging();
			Component component = (Component)TreeView.SelectedNode.Tag;
			if(component is JumpListCategory) TreeNodeCategoryMove((JumpListCategory)component, delta);
			if(component is JumpListItemTask) TreeNodeTaskMove((JumpListItemTask)component, delta);
			RefreshCollection();
			SelectNode(component);
			Context.OnComponentChanged();
		}
		void TreeNodeCategoryMove(JumpListCategory category, int delta) {
			int currentItemIndex = collection.IndexOf(category);
			collection.Remove(category);
			collection.Insert(currentItemIndex + delta, category);
		}
		void TreeNodeTaskMove(JumpListItemTask task, int delta) {
			JumpListCategory category = collection[TreeView.SelectedNode.Parent.Index];
			int currentItemIndex = category.JumpItems.IndexOf(task);
			category.JumpItems.Remove(task);
			category.JumpItems.Insert(currentItemIndex + delta, task);
		}
	}
	public class JumpListEditorTaskCollectionForm : JumpListEditorForm {
		JumpListCategoryItemCollection collection;
		public JumpListEditorTaskCollectionForm(ITypeDescriptorContext context, IServiceProvider provider, object value)
			: base(context, provider, value) {
			LblTasks.Text = "Tasks Category:";
			BtnAdd.Text = "Add Separator";
			BtnAdd.Click += delegate {
				Context.OnComponentChanging();
				JumpListItemSeparator separator = new JumpListItemSeparator();
				collection.Add(separator);
				RefreshCollection();
				UpdateButtons();
				SelectNode(separator);
				TaskbarAssistant.Container.Add(separator);
				AddedComponents.Add(separator);
				Context.OnComponentChanged();
			};
		}
		void SelectNode(object value) {
			foreach (TreeNode node in TreeView.Nodes.Cast<TreeNode>().Where(node => node.Tag.Equals(value))) {
				TreeView.SelectedNode = node;
				TreeView.Select();
				return;
			}
		}
		protected override void UpdateButtons() {
			BtnRemove.Enabled = TreeView.SelectedNode != null;
			BtnUpCmd.Enabled = BtnRemove.Enabled && TreeView.Nodes.IndexOf(TreeView.SelectedNode) != 0;
			BtnDownCmd.Enabled = BtnRemove.Enabled && TreeView.Nodes.IndexOf(TreeView.SelectedNode) != TreeView.Nodes.Count - 1;
		}
		protected override void InitializeCollection(object list) {
			collection = (JumpListCategoryItemCollection)list;
			RefreshCollection();
			base.InitializeCollection(list);
		}
		protected override void RefreshCollection() {
			TreeView.Nodes.Clear();
			foreach(IJumpListItem item in collection) {
				TreeNode node = null;
				if(item is JumpListItemSeparator)
					node = new TreeNode("Separator");
				if(item is JumpListItemTask)
					node = new TreeNode(((JumpListItemTask)item).Caption);
				node.Tag = item;
				TreeView.Nodes.Add(node);
			}
			TreeView.ExpandAll();
			base.RefreshCollection();
		}
		protected override void OnBtnAddTaskClick(object sender, EventArgs e) {
			Context.OnComponentChanging();
			JumpListItemTask task = new JumpListItemTask("Task");
			collection.Add(task);
			RefreshCollection();
			SelectNode(task);
			TaskbarAssistant.Container.Add(task);
			AddedComponents.Add(task);
			Context.OnComponentChanged();
		}
		protected override void OnBtnRemoveClick(object sender, EventArgs e) {
			if(TreeView.SelectedNode == null) return;
			Context.OnComponentChanging();
			IJumpListItem item = (IJumpListItem)TreeView.SelectedNode.Tag;
			collection.Remove(item);
			RefreshCollection();
			TaskbarAssistant.Container.Remove((Component)item);
			RemovedComponents.Add((Component)item);
			Context.OnComponentChanged();
		}
		protected override void OnBtnUpCmdClick(object sender, EventArgs e) {
			TreeNodeMove(-1);
		}
		protected override void OnBtnDownCmdClick(object sender, EventArgs e) {
			TreeNodeMove(+1);
		}
		void TreeNodeMove(int delta) {
			if(TreeView.SelectedNode == null) return;
			Context.OnComponentChanging();
			IJumpListItem item = (IJumpListItem)TreeView.SelectedNode.Tag;
			int currentItemIndex = collection.IndexOf(item);
			collection.Remove(item);
			collection.Insert(currentItemIndex + delta, item);
			RefreshCollection();
			SelectNode(item);
			Context.OnComponentChanged();
		}
	}
}
