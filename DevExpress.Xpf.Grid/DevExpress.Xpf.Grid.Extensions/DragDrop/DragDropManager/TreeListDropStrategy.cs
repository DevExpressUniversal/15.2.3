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
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Grid.Native;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#else
using System.Data;
#endif
namespace DevExpress.Xpf.Grid.DragDrop {
	public abstract class TreeListDropStrategy {
		public TreeListDropStrategy(TreeListView view) {
			this.TreeListView = view;
		}
		protected TreeListView TreeListView { get; set; }
		public virtual void DropObject(IList source, TreeListNode insertNode, DropTargetType dropTargetType, object obj) {
		}
		public virtual void MoveRows(TreeListDragDropManager treeListDragDropManager, DragDropManagerBase sourceManager, TreeListNode insertNode, DropTargetType dropTargetType) {}
		public virtual void AddRow(TreeListDragDropManager targetManager, DragDropManagerBase sourceManager, object row, int insertRowHandle) {}
		public virtual void UpdateSelection(TreeListDragDropManager targetManager) {
			if(TreeListView.DataControl.SelectionMode == MultiSelectMode.Row && targetManager.RestoreSelection) {
				int[] selectedHandles = TreeListView.DataControl.GetSelectedRowHandles();
				if(selectedHandles.Length > 0) {
					int startRowHandle = selectedHandles[0];
					int endRowHandle = selectedHandles[selectedHandles.Length - 1];
					TreeListView.DataControl.SelectRange(startRowHandle, endRowHandle);
				}
			}
			else TreeListView.DataControl.UnselectAll();
		}
	}
	public class UnboundDropStrategy : TreeListDropStrategy {
		public UnboundDropStrategy(TreeListView view)
			: base(view) {
		}
		public override void AddRow(TreeListDragDropManager targetManager, DragDropManagerBase sourceManager, object row, int insertRowHandle) {
			TreeListNode node = row as TreeListNode;
			if(ReferenceEquals(targetManager, sourceManager)) {
				TreeListNodeCollection sourceNodeCollection = GetParentCollection(node, TreeListView);
				sourceNodeCollection.Remove(node);
				TreeListView.Nodes.Add(node);
			}
			else {
				sourceManager.RemoveObject(row);
				TreeListView.Nodes.Add(new TreeListNode(row));
			}
		}
		public override void MoveRows(TreeListDragDropManager targetManager, DragDropManagerBase sourceManager, TreeListNode insertNode, DropTargetType dropTargetType) {
			object insertObject = insertNode.Content;
			targetManager.DataControl.BeginDataUpdate();
			TreeListNodeCollection targetNodeCollection = null;
			int insertIndex = 0;
			switch(dropTargetType) {
				case DropTargetType.InsertRowsAfter:
				case DropTargetType.InsertRowsBefore:
					targetNodeCollection = GetParentCollection(insertNode, targetManager.TreeListView);
					insertIndex = targetNodeCollection.IndexOf(insertNode)
						+ (dropTargetType == DropTargetType.InsertRowsAfter ? 1 : 0);
					break;
				case DropTargetType.InsertRowsIntoNode:
					targetNodeCollection = insertNode.Nodes;
					insertIndex = targetNodeCollection.Count;
					break;
				case DropTargetType.DataArea:
					targetNodeCollection = TreeListView.Nodes;
					insertIndex = targetNodeCollection.Count;
					break;
			}
			foreach(object obj in sourceManager.DraggingRows) {
				if(ReferenceEquals(targetManager, sourceManager)) {
					TreeListNodeCollection sourceNodeCollection = GetParentCollection(obj as TreeListNode, TreeListView);
					if(ReferenceEquals(targetNodeCollection, sourceNodeCollection)) {
						if(insertIndex > sourceNodeCollection.IndexOf(obj as TreeListNode))
							insertIndex--;
						if(ReferenceEquals(insertObject, sourceManager.GetObject(obj)))
							continue;
					}
					TreeListNode sourceNode = obj as TreeListNode;
					sourceNodeCollection.Remove(sourceNode);
					targetNodeCollection.Insert(insertIndex, sourceNode);
				}
				else {
					sourceManager.RemoveObject(obj);
					TreeListNode targetNode = new TreeListNode(sourceManager.GetObject(obj));
					targetNodeCollection.Insert(insertIndex, targetNode);
				}
				insertIndex++;
			}
			targetManager.DataControl.EndDataUpdate();
			UpdateSelection(targetManager);
			TreeListView.DataControl.CurrentItem = targetManager.GetObject(sourceManager.DraggingRows[0]);
		}
		public TreeListNodeCollection GetParentCollection(TreeListNode insertNode, TreeListView treeView) {
			if(insertNode == null)
				return null;
			if(insertNode.ParentNode == null)
				return treeView == null ? null : treeView.Nodes;
			else
				return insertNode.ParentNode.Nodes;
		}
	}
	public class BoundDropStrategy : TreeListDropStrategy {
		public BoundDropStrategy(TreeListView view)
			: base(view) {
		}
		public override void AddRow(TreeListDragDropManager targetManager, DragDropManagerBase sourceManager, object row, int insertRowHandle) {
			sourceManager.RemoveObject(row);
			DropObject(targetManager.ItemsSource, null, DropTargetType.DataArea, sourceManager.GetObject(row));
			targetManager.InsertObject(targetManager.ItemsSource, null, DropTargetType.DataArea, sourceManager.GetObject(row), -1);
		}
		public override void MoveRows(TreeListDragDropManager targetManager, DragDropManagerBase sourceManager, TreeListNode insertNode, DropTargetType dropTargetType) {
			object insertObject = insertNode.Content;
			if(dropTargetType == DropTargetType.InsertRowsIntoNode && !insertNode.IsExpanded)
				insertNode.SetNodeExpanded(true);
			targetManager.DataControl.BeginDataUpdate();
			IList targetList = targetManager.GetSource(insertNode);
			int insertIndex = targetList.IndexOf(insertNode.Content)
				+ (dropTargetType == DropTargetType.InsertRowsAfter ? 1 : 0);
#if !SL
			bool isDataTable = targetList is DataView;
#endif
			foreach(object obj in sourceManager.DraggingRows) {
				object rawObjSource = sourceManager.GetSource(obj);
				if(ReferenceEquals(targetList, rawObjSource) 
#if !SL
					&& !isDataTable
#endif
					) {
					if(insertIndex > targetList.IndexOf(targetManager.GetObject(obj)))
						insertIndex--;
					if(ReferenceEquals(insertObject, rawObjSource))
						continue;
				}
				MoveRow(targetManager, sourceManager, targetList, insertNode, dropTargetType, obj, insertIndex);
				insertIndex++;
			}
			targetManager.DataControl.EndDataUpdate();
			UpdateSelection(targetManager);
			TreeListView.DataControl.CurrentItem = targetManager.GetObject(sourceManager.DraggingRows[0]);
		}
		protected virtual void MoveRow(TreeListDragDropManager targetManager, DragDropManagerBase sourceManager, IList targetList, TreeListNode insertNode, DropTargetType dropTargetType, object obj, int insertIndex) {
			object sourceMngObj = sourceManager.GetObject(obj);
#if !SL
			DataView dataView = targetList as DataView;
			if(dataView != null) {
				DataRow row = Utils.CloneDataRow(dataView, sourceMngObj);
				sourceManager.RemoveObject(obj);
				DropObject(targetList, insertNode, dropTargetType, row);
				targetManager.InsertObject(dataView.Table.Rows, insertNode, dropTargetType, row, insertIndex);
				dataView.Table.AcceptChanges();
			}
			else {
#endif
				sourceManager.RemoveObject(obj);
				DropObject(targetList, insertNode, dropTargetType, sourceMngObj);
				targetManager.InsertObject(targetList, insertNode, dropTargetType, sourceMngObj, insertIndex);
#if !SL
			}
#endif
		}
	}
	public class SelfReferenceDropStrategy : BoundDropStrategy {
		public SelfReferenceDropStrategy(TreeListView view)
			: base(view) {
		}
		public override void DropObject(IList source, TreeListNode insertNode, DropTargetType dropTargetType, object obj) {
			switch(dropTargetType) {
				case DropTargetType.InsertRowsAfter:
				case DropTargetType.InsertRowsBefore:
					Utils.SetPropertyValue(obj, TreeListView.ParentFieldName,
						Utils.GetPropertyValue(insertNode.Content, TreeListView.ParentFieldName));
					break;
				case DropTargetType.InsertRowsIntoNode:
					Utils.SetPropertyValue(obj, TreeListView.ParentFieldName,
						Utils.GetPropertyValue(insertNode.Content, TreeListView.KeyFieldName));
					break;
				case DropTargetType.DataArea:
					if(TreeListView.RootValue != null)
						Utils.SetPropertyValue(obj, TreeListView.ParentFieldName, TreeListView.RootValue);
					break;
				default:
					break;
			}
		}
	}
	public class EmptyDropStrategy : BoundDropStrategy {
		public EmptyDropStrategy(TreeListView view) : base(view) { }
	}
}
