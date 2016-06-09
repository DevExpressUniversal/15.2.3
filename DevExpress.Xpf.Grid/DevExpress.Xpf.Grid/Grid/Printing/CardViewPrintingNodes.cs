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
using System.Windows;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid.Printing {
	public abstract class CardViewPrintingNodeBase : PrintingNodeBase {
		protected internal new CardViewPrintingDataTreeBuilder TreeBuilder { get { return base.TreeBuilder as CardViewPrintingDataTreeBuilder; } }
		protected CardViewPrintingNodeBase(CardViewPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index)
			: base(treeBuilder, parent, index) {
		}
		protected override bool IsDetailContainerCore {
			get { return false; }
		}
	}
	public abstract class CardViewContainerPrintingNodeBase : ContainerPrintingNodeBase {
		protected internal new CardViewPrintingDataTreeBuilder TreeBuilder { get { return base.TreeBuilder as CardViewPrintingDataTreeBuilder; } }
		public CardViewContainerPrintingNodeBase(DataNodeContainer nodeContainer, CardViewPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(nodeContainer, treeBuilder, parent, index, pageSize) {
		}
		protected override bool GetIsDetailContainerCore() {
			return !nodeContainer.PrintInfo.IsGroupRowsContainer;
		}
		protected override IDataNode CreateGroupChildNode(RowNode rowNode, int index) {
			return TreeBuilder.CreateGroupPrintingNode(nodeContainer, rowNode, this, index, pageSize);
		}
		protected override IDataNode CreateChildNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index) {
			return TreeBuilder.CreateDetailPrintingNode(nodeContainer, rowNode, this, index);
		}
	}
	public class CardViewGroupPrintingNode : CardViewContainerPrintingNodeBase, IVisualGroupNode {
		protected readonly GroupNode groupNode;
		protected readonly NodeContainer parentContainer;
		public CardViewGroupPrintingNode(NodeContainer parentContainer, GroupNode groupNode, CardViewPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(groupNode.NodesContainer, treeBuilder, parent, index, pageSize) {
			this.groupNode = groupNode;
			this.parentContainer = parentContainer;
		}
		public RowViewInfo GetFooter(bool allowContentReuse) { return null; }
		public RowViewInfo GetHeader(bool allowContentReuse) {
			TreeBuilder.reusingGroupRowData.AssignFromInternal(null, parentContainer, groupNode, true);
			return CreateRowElement(TreeBuilder.reusingGroupRowData, TreeBuilder.PrintGroupRowTemplate);
		}
		public bool RepeatHeaderEveryPage {
			get { return true; }
		}
		public GroupUnion Union {
			get { return GroupUnion.WithFirstDetail; }
		}
	}
	public class CardViewGroupRootPrintingNode : CardViewContainerPrintingNodeBase, IVisualGroupNodeFixedFooter {
		public CardViewGroupRootPrintingNode(CardViewPrintingDataTreeBuilder treeBuilder, IDataNode parent, Size pageSize)
			: base(treeBuilder.RootNodeContainer, treeBuilder, parent, -1, pageSize) {
		}
		RowViewInfo IVisualGroupNode.GetFooter(bool allowContentReuse) {
			if(TreeBuilder.View.PrintTotalSummary && TreeBuilder.View.ShowTotalSummary && TreeBuilder.PrintFooterTemplate != null)
				return new RowViewInfo(TreeBuilder.PrintFooterTemplate, TreeBuilder.HeadersData);
			return null;
		}
		RowViewInfo IVisualGroupNode.GetHeader(bool allowContentReuse) {
			if(TreeBuilder.View.PrintTotalSummary && TreeBuilder.View.ShowTotalSummary && TreeBuilder.PrintHeaderTemplate != null)
				return new RowViewInfo(TreeBuilder.PrintHeaderTemplate, TreeBuilder.HeadersData);
			return null;
		}
		GroupUnion IGroupNode.Union { get { return GroupUnion.WithFirstDetail; } }
		bool IGroupNode.RepeatHeaderEveryPage { get { return false; } }
		public RowViewInfo GetFixedFooter(bool allowContentReuse) {
			if(TreeBuilder.View.PrintFixedTotalSummary && TreeBuilder.View.ShowFixedTotalSummary && TreeBuilder.PrintFixedFooterTemplate != null)
				return new RowViewInfo(TreeBuilder.PrintFixedFooterTemplate, TreeBuilder.HeadersData);
			return null;
		}
	}
	public class CardViewRootPrintingNode : CardViewContainerPrintingNodeBase, IRootDataNode, IDisposable {
		public CardViewRootPrintingNode(CardViewPrintingDataTreeBuilder treeBuilder, Size pageSize)
			: base(null, treeBuilder, null, -1, pageSize) {
		}
		protected override bool CanGetChildCore(int index) {
			return index < 1;
		}
		protected override bool IsDetailContainerCore {
			get { return false; }
		}
		protected override IDataNode GetChildCore(int index) {
			return new CardViewGroupRootPrintingNode(TreeBuilder, this, pageSize);
		}
		int IRootDataNode.GetTotalDetailCount() {
			return GetMasterRowsCount();
		}
		void IDisposable.Dispose() {
			TreeBuilder.OnRootNodeDispose();
		}
		int GetMasterRowsCount() {
			VirtualItemsEnumerator en = new VirtualItemsEnumerator(TreeBuilder.RootNodeContainer);
			int count = 0;
			while(en.MoveNext())
				count++;
			return count;
		}
	}
	public class GridCardViewPrintingNode : CardViewPrintingNodeBase, IVisualDetailNode {
		readonly RowNode rowNode;
		readonly NodeContainer parentContainer;
		public GridCardViewPrintingNode(NodeContainer parentContainer, RowNode rowNode, CardViewPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index)
			: base(treeBuilder, parent, index) {
			this.rowNode = rowNode;
			this.parentContainer = parentContainer;
		}
		#region IVisualDetailNode Members
		RowViewInfo IVisualDetailNode.GetDetail(bool allowContentReuse) {
			TreeBuilder.ReusingRowData.AssignFromInternal(null, parentContainer, rowNode, false);
			TreeBuilder.ReusingRowData.UpdateCellDataEditorsDisplayText();
			return CreateRowElement(TreeBuilder.ReusingRowData, TreeBuilder.PrintRowTemplate);
		}
		#endregion
		protected override bool IsDetailContainerCore { get { return false; } }
		protected override IDataNode GetChildCore(int index) {
			throw new NotSupportedException();
		}
		protected override bool CanGetChildCore(int index) {
			return false;
		}
	}
}
