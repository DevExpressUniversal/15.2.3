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

using DevExpress.Data;
using System.ComponentModel;
using System;
using System.Collections;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public abstract class TreeListDataHelperBase : IDisposable {
		int loading = 0;
		public TreeListDataHelperBase(TreeListDataProvider provider) {
			DataProvider = provider;
		}
		public abstract Type ItemType { get; }
		public virtual bool IsReady { get { return true; } }
		public virtual bool IsUnboundMode { get { return false; } }
		protected TreeListDataProvider DataProvider { get; private set; }
		protected TreeListView View { get { return DataProvider.View; } }
		protected DataColumnInfoCollection Columns { get { return DataProvider.Columns; } }
		protected virtual bool CanPopulate(PropertyDescriptor descriptor) { return true; }
		public virtual bool RequiresReloadDataOnEndUpdate { get { return false; } internal set { } }
		protected virtual DataColumnInfo CreateDataColumn(PropertyDescriptor descriptor) {
			DataColumnInfo column = new DataColumnInfo(descriptor);
			column.Visible = IsColumnVisible(column);
			return column;
		}
		protected virtual bool IsColumnVisible(DataColumnInfo column) {
			return column.Browsable;
		}
		protected virtual void PopulateColumn(PropertyDescriptor descriptor) {
			if(!CanPopulate(descriptor)) return;
			Columns.Add(CreateDataColumn(descriptor));
		}
		public virtual void NodeExpandingCollapsing(TreeListNode node) { }
		public virtual void Dispose() { }
		public object GetValue(TreeListNode node, string fieldName) {
			if(node == null)
				return null;
			return GetValue(node, Columns[fieldName]);
		}
		public object GetValue(TreeListNode node, DataColumnInfo columnInfo) {
			if(node == null || columnInfo == null)
				return null;
			return GetValue(node, columnInfo.PropertyDescriptor);
		}
		protected internal object GetValue(TreeListNode node, PropertyDescriptor descriptor) {
			object item = descriptor is TreeListUnboundPropertyDescriptor || descriptor is TreeListDisplayTextPropertyDescriptor || descriptor is TreeListSearchDisplayTextPropertyDescriptor ? node : node.Content;
			return descriptor.GetValue(item);
		}
		public void SetValue(TreeListNode node, string fieldName, object value) {
			SetValue(node, Columns[fieldName], value);
		}
		public void SetValue(TreeListNode node, DataColumnInfo columnInfo, object value) {
			if(node == null || columnInfo == null || columnInfo.ReadOnly) return;
			SetValue(node, columnInfo.PropertyDescriptor, columnInfo.ConvertValue(value, true));
		}
		protected void SetValue(TreeListNode node, PropertyDescriptor descriptor, object value) {
			object item = descriptor is TreeListUnboundPropertyDescriptor ? node : node.Content;
			if(item == null) return;
			descriptor.SetValue(item, value);
		}
		protected void RemoveTreeListNode(TreeListNode node) {
			TreeListNode parent = node.ParentNode;
			if(parent == null)
				DataProvider.Nodes.Remove(node);
			else
				parent.Nodes.Remove(node);
		}
		protected int GetRootParentIndex(TreeListNode node) {
			TreeListNode rootParent = node.ParentNode;
			if(rootParent == null)
				rootParent = node;
			else
				while(rootParent.ParentNode != null)
					rootParent = rootParent.ParentNode;
			return DataProvider.Nodes.IndexOf(rootParent);
		}
		protected internal virtual void UpdateNodeId(TreeListNode node) { }
		protected internal virtual void RecalcNodeIdsIfNeeded() { }
		protected virtual void CalcNodeIds() { }
		public virtual void DeleteNode(TreeListNode node, bool deleteChildren, bool modifySource) { }
		public abstract void PopulateColumns();
		public abstract bool AllowEdit { get; }
		public abstract bool AllowRemove  { get; }
		public abstract bool IsLoaded { get; }
		public bool IsLoading { get { return loading > 0; } }
		public abstract void LoadData();
		protected virtual void BeginLoad() { loading++; }
		protected virtual void EndLoad() { loading--; }
		protected virtual object GetValue(object item, string fieldName) {
			DataColumnInfo columnInfo = Columns[fieldName];
			if(columnInfo != null)
				return columnInfo.PropertyDescriptor.GetValue(item);
			return null;
		}
		protected virtual TreeListUnboundPropertyDescriptor CreateUnboundPropertyDescriptor(UnboundColumnInfo info) {
			return new TreeListUnboundPropertyDescriptor(DataProvider, info);
		}
		protected virtual void PopulateUnboundColumns() {
			UnboundColumnInfoCollection unboundColumns = DataProvider.GetUnboundColumns();
			if(unboundColumns != null) {
				foreach(UnboundColumnInfo info in unboundColumns) {
					if(Columns[info.Name] != null) continue;
					PopulateColumn(CreateUnboundPropertyDescriptor(info));
				}
			}
		}
		public virtual object GetDataRowByListIndex(int listSourceRowIndex) {
			return null;
		}
		public virtual object GetCellValueByListIndex(int listSourceRowIndex, string fieldName) {
			return null;
		}
		public virtual int GetListIndexByDataRow(object row) {
			return -1;
		}
		public virtual void ReloadChildNodes(TreeListNode node, IEnumerable chidnren = null) { }
		protected internal virtual bool SupportNotifications { get { return false; } }
	}
}
