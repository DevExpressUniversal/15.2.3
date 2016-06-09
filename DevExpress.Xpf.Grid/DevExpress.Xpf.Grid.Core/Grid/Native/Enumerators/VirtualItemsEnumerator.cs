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
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
namespace DevExpress.Xpf.Grid.Native {
	public abstract class VirtualItemsEnumeratorBase : NestedObjectEnumeratorBase {
		protected static IEnumerable EmptyEnumerable = new object[0];
		public VirtualItemsEnumeratorBase(object obj)
			: base(obj) {
		}
		public override void Reset() {
			base.Reset();
			MoveNext();
		}
		protected override IEnumerator GetNestedObjects(object obj) {
			IEnumerator containerEnumerator = GetContainerEnumerator(obj);
			if(containerEnumerator != null)
				return containerEnumerator;
			return GetGroupEnumerable(obj).GetEnumerator();
		}
		protected abstract IEnumerator GetContainerEnumerator(object obj);
		protected abstract IEnumerable GetGroupEnumerable(object obj);
	}
	public class VirtualItemsEnumerator : VirtualItemsEnumeratorBase {
		readonly NodeContainer containerItem;
		public NodeContainer CurrentContainer {
			get {
				if(CurrentParent is NodeContainer)
					return (NodeContainer)CurrentParent;
				return ((RowNode)CurrentParent).NodesContainer;
			}
		}
		public RowNode Current { get { return (RowNode)Enumerator.Current; } }
		public RowDataBase CurrentData { get { return Current.GetRowData(); } }
		public VirtualItemsEnumerator(NodeContainer containerItem)
			: base(containerItem) {
			this.containerItem = containerItem;
		}
		protected sealed override IEnumerator GetContainerEnumerator(object obj) {
			NodeContainer dataItem = obj as NodeContainer;
			return (dataItem != null && dataItem.Initialized) ? dataItem.Items.GetEnumerator() : null;
		}
		protected sealed override IEnumerable GetGroupEnumerable(object obj) {
			RowNode rowNode = obj as RowNode;
			return rowNode != null ? GetGroupDataEnumerable(rowNode) : EmptyEnumerable;
		}
		protected virtual IEnumerable<RowNode> GetGroupDataEnumerable(RowNode groupData) {
			return groupData.GetChildItems();
		}
	}
	public class SkipCollapsedGroupVirtualItemsEnumerator : VirtualItemsEnumerator {
		public SkipCollapsedGroupVirtualItemsEnumerator(NodeContainer containerItem)
			: base(containerItem) {
		}
		protected override IEnumerable<RowNode> GetGroupDataEnumerable(RowNode groupData) {
			return groupData.GetSkipCollapsedChildItems();
		}
	}
	public class RowDataItemsEnumerator : VirtualItemsEnumeratorBase {
		public RowData Current { get { return (RowData)Enumerator.Current; } }
		public RowDataItemsEnumerator(RowsContainer containerItem)
			: base(containerItem) {
		}
		protected sealed override IEnumerator GetContainerEnumerator(object obj) {
			RowsContainer dataItem = obj as RowsContainer;
			return (dataItem != null) ? dataItem.GetEnumerable().GetEnumerator() : null;
		}
		protected sealed override IEnumerable GetGroupEnumerable(object obj) {
			RowData rowData = obj as RowData;
			return rowData != null ? rowData.GetCurrentViewChildItems() : EmptyEnumerable;
		}
	}
}
