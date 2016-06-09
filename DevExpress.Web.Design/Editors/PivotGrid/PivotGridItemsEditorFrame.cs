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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Data.Browsing.Design;
using DevExpress.Web.Design;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class PivotGridItemsEditorFrame : ItemsEditorFrame {
		PivotGridItemsOwner FieldsOwner { get { return (PivotGridItemsOwner)ItemsOwner; } }
		ColumnNameEditorPicker TreeViewOlapFields { get; set; }
		protected override void CreateRetrieveFieldsPopup() {
			base.CreateRetrieveFieldsPopup();
			if(FieldsOwner.HasOLAPDataSource) {
				CheckAll.Visible = false;
				RetrieveFieldsCheckedListBox.Visible = false;
			}
		}
		protected override void BeforeShowRetrieveFieldsPopup() {
			if(!FieldsOwner.HasOLAPDataSource) {
				base.BeforeShowRetrieveFieldsPopup();
				return;
			}
			CreateRetrieveFieldsDataSourceTreeView();
			DisplayFields.Clear();
			DisplayFields.AddRange(DataOwner.GetUsedFieldNames());
			IterateTreeViewOlapFieldsNodes(TreeViewOlapFields.Nodes, (TreeNode node) => { node.Checked = false; });
			TreeViewOlapFields.CollapseAll();
			DisplayFields.ForEach(f => UpdateRetrieveFieldItemChecked(f));
		}
		void CreateRetrieveFieldsDataSourceTreeView() {
			if(TreeViewOlapFields != null)
				return;
			TreeViewOlapFields = new ColumnNameEditorPicker(FieldsOwner.GetOLAPDataSource(), string.Empty, FieldsOwner.ServiceProvider);
			TreeViewOlapFields.Name = "TreeViewOlapDataSource";
			RetrieveFieldsPopup.Controls.Add(TreeViewOlapFields);
			TreeViewOlapFields.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			TreeViewOlapFields.Location = new Point(0, 30);
			TreeViewOlapFields.CheckBoxes = true;
			TreeViewOlapFields.Size = new Size(RetrieveFieldsPopup.Width, RetrieveFieldsPopup.Height - RetrieveFieldsCheckedListBox.Top - 35);
			TreeViewOlapFields.AfterCheck += treeView_AfterCheck;
			TreeViewOlapFields.AfterExpand += treeView_AfterExpand;
			TreeViewOlapFields.TabIndex = 1;
		}
		void UpdateRetrieveFieldItemChecked(string fieldName) {
			if(string.IsNullOrEmpty(fieldName))
				return;
			IterateTreeViewOlapFieldsNodes(TreeViewOlapFields.Nodes, (TreeNode node) => {
				var nodeText = GetOLAPNodeFieldName(node);
				if(node.Nodes.Count != 0) {
					if(fieldName.IndexOf(nodeText) != -1)
						node.Expand();
					return;
				}
				if(node.Checked)
					return;
				TreeViewOlapFields.AfterCheck -= treeView_AfterCheck;
				node.Checked = fieldName == nodeText;
				UpdateParentsCheck(node);
				TreeViewOlapFields.AfterCheck += treeView_AfterCheck;
			});
		}
		void treeView_AfterCheck(object sender, TreeViewEventArgs e) {
			TreeViewOlapFields.AfterCheck -= treeView_AfterCheck;
			UpdateChildCheck(e.Node);
			UpdateParentsCheck(e.Node);
			TreeViewOlapFields.AfterCheck += treeView_AfterCheck;
		}
		void treeView_AfterExpand(object sender, TreeViewEventArgs e) {
			treeView_AfterCheck(sender, e);
		}
		void UpdateChildCheck(TreeNode node) {
			var isChecked = node.Checked;
			foreach(TreeNode childNode in node.Nodes) {
				childNode.Checked = isChecked;
				UpdateChildCheck(childNode);
			}
		}
		void UpdateParentsCheck(TreeNode node) {
			if(node == null) 
				return;
			var parent = node.Parent;
			if(parent == null) 
				return;
			parent.Checked = node.Checked && !parent.Nodes.OfType<TreeNode>().ToList().Exists(n => !n.Checked);
			UpdateParentsCheck(parent);
		}
		void UpdateParentNodesChecking(TreeNode node) {
			if(node.Parent != null && node.Parent.Checked)
				node.Parent.Checked = node.Checked;
		}
		protected override void UpdateItemsFromFieldsSelector() {
			if(FieldsOwner.HasOLAPDataSource)
				RetrieveFieldsFromOlap();
			else
				base.UpdateItemsFromFieldsSelector();
		}
		List<string> checkedRetrieveFields;
		List<string> CheckedRetrieveFields {
			get {
				if(checkedRetrieveFields == null)
					checkedRetrieveFields = new List<string>();
				return checkedRetrieveFields;
			}
		}
		void RetrieveFieldsFromOlap() {
			CheckedRetrieveFields.Clear();
			IterateTreeViewOlapFieldsNodes(TreeViewOlapFields.Nodes, RetrieveFieldFromOlap);
			FieldsOwner.CreateDataFields(CheckedRetrieveFields);
		}
		void RetrieveFieldFromOlap(TreeNode node) {
			var fieldName = GetOLAPNodeFieldName(node);
			if(node.Checked) {
				if(node.Nodes.Count != 0) {
					node.Expand();
					return;
				}
				if(!DisplayFields.Contains(fieldName))
					CheckedRetrieveFields.Add(fieldName);
			} else {
				FieldsOwner.RemoveOlapDataItem(fieldName);
			}
		}
		void IterateTreeViewOlapFieldsNodes(TreeNodeCollection nodes, Action<TreeNode> iterateAction) { 
			if(nodes == null)
				return;
			foreach(TreeNode node in nodes) {
				iterateAction(node);
				IterateTreeViewOlapFieldsNodes(node.Nodes, iterateAction);
			}
		}
		string GetOLAPNodeFieldName(TreeNode node) {
			var fieldName = string.Empty;
			foreach(Match match in Regex.Matches(node.FullPath, @"\[(.+?)\]"))
				fieldName += string.IsNullOrEmpty(fieldName) ? match.Value : string.Format(".{0}", match.Value);
			return fieldName;
		}
		protected override void ProcessMenuItemAction(DesignEditorDescriptorItem rootItem, DesignEditorDescriptorItem descriptorItem) {
			if(rootItem.ActionType == DesignEditorMenuRootItemActionType.MoveToNewGroup)
				FieldsOwner.MoveSelectedFieldsToNewGroup();
			else
				base.ProcessMenuItemAction(rootItem, descriptorItem);
		}
	}
}
