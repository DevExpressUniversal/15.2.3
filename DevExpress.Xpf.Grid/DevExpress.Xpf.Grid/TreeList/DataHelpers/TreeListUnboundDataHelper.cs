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

using DevExpress.Xpf.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Data.Helpers;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Utils;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.GridData;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListUnboundDataHelper : TreeListDataHelperBase {
		public TreeListUnboundDataHelper(TreeListDataProvider provider) : base(provider) { }
		public readonly int MaxNodeId = int.MaxValue / 2;
		protected internal int NodeCounter { get; set; }
		public override bool RequiresReloadDataOnEndUpdate { get { return !string.IsNullOrEmpty(View.CheckBoxFieldName); } internal set { } }
		public override void PopulateColumns() {
			foreach(IColumnInfo column in DataProvider.View.GetColumns())
				if(column.UnboundType == UnboundColumnType.Bound && !string.IsNullOrEmpty(column.FieldName))
					PopulateColumn(new UnitypeDataPropertyDescriptor(column.FieldName, column.ReadOnly));
			PopulateUnboundColumns();
			InitAllNodesIsChecked();
		}
		public override Type ItemType { get { return null; } }
		public override bool AllowEdit { get { return true; } }
		public override bool AllowRemove { get { return true; } }
		public override bool IsReady { get { return IsLoaded; } }
		public override bool IsLoaded { get { return DataProvider.Nodes.Count > 0; } }
		public override bool IsUnboundMode { get { return true; } }
		public override void LoadData() {
			InitAllNodesIsChecked();
		}
		protected internal override void UpdateNodeId(TreeListNode node) {
			node.Id = NodeCounter++;
		}
		protected override void CalcNodeIds() {
			NodeCounter = 0;
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes))
				node.Id = NodeCounter++;
		}
		protected internal override void RecalcNodeIdsIfNeeded() {
			if(NodeCounter >= MaxNodeId)
				CalcNodeIds();
		}
		public override void DeleteNode(TreeListNode node, bool deleteChildren, bool modifySource) {
			if(!deleteChildren) {
				List<TreeListNode> list = node.Nodes.ToList<TreeListNode>();
				DataProvider.LockRecursiveNodesUpdate();
				int rootParentIndex = GetRootParentIndex(node);
				foreach(TreeListNode child in list) {
					rootParentIndex++;
					DataProvider.Nodes.Insert(rootParentIndex, child);
				}
				node.Nodes.Clear();
				DataProvider.UnlockRecursiveNodesUpdate();
			}
			RemoveTreeListNode(node);
		}
		void InitAllNodesIsChecked() {
			DataProvider.InitNodesIsChecked();
		}
	}
}
