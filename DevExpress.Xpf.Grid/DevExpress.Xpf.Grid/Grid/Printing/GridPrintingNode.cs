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
	public abstract class PrintingNodeBase : IDataNode { 
		readonly IDataNode parent;
		protected readonly int fIndex;
		readonly DataTreeBuilder _treeBuilder;
		protected internal DataTreeBuilder TreeBuilder { get { return _treeBuilder; } }
		protected PrintingNodeBase(DataTreeBuilder treeBuilder, IDataNode parent, int index) {
			_treeBuilder = treeBuilder;
			this.parent = parent;
			this.fIndex = index;
		}
		#region IDataNode Members
		bool IDataNode.CanGetChild(int index) {
			return CanGetChildCore(index);
		}
		IDataNode IDataNode.GetChild(int index) {
			return GetChildCore(index);
		}
		int IDataNode.Index { get { return fIndex; } }
		IDataNode IDataNode.Parent { get { return parent; } }
		bool IDataNode.IsDetailContainer { get { return IsDetailContainerCore; } }
		bool IDataNode.PageBreakBefore { get { return false; } }
		bool IDataNode.PageBreakAfter { get { return false; } }
		#endregion
		protected abstract bool IsDetailContainerCore { get; }
		protected abstract IDataNode GetChildCore(int index);
		protected abstract bool CanGetChildCore(int index);
		protected RowViewInfo CreateRowElement(RowData rowData, DataTemplate template) {
			if(template == null) return null;
			return new RowViewInfo(template, rowData);
		}
	}
	public abstract class GridPrintingNodeBase : PrintingNodeBase {
		protected internal new PrintingDataTreeBuilder TreeBuilder { get { return base.TreeBuilder as PrintingDataTreeBuilder; } }
		protected GridPrintingNodeBase(PrintingDataTreeBuilderBase treeBuilder, IDataNode parent, int index) 
			: base(treeBuilder, parent, index) {
		}
	}
	public abstract class ContainerPrintingNodeBase : PrintingNodeBase { 
		protected readonly Size pageSize;
		protected readonly DataNodeContainer nodeContainer;
		public ContainerPrintingNodeBase(DataNodeContainer nodeContainer, DataTreeBuilder treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(treeBuilder, parent, index) {
			this.pageSize = pageSize;
			this.nodeContainer = nodeContainer;
		}
		protected override bool IsDetailContainerCore { get { return GetIsDetailContainerCore(); } }
		protected abstract bool GetIsDetailContainerCore();
		protected override IDataNode GetChildCore(int index) {
			RowNode rowNode = nodeContainer.Items[index];
			return DataTreeBuilder.CreateRowElement<IDataNode>(
					!IsDetailContainerCore,
					() => CreateGroupChildNode(rowNode, index),
					() => CreateChildNode(nodeContainer, rowNode, this, index)
				);
		}
		protected abstract IDataNode CreateGroupChildNode(RowNode rowNode, int index);
		protected abstract IDataNode CreateChildNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index);
		protected override bool CanGetChildCore(int index) {
			if(0 <= index && index < nodeContainer.Items.Count) {
				DataRowNode dataRowNode = nodeContainer.Items[index] as DataRowNode;
				if(dataRowNode == null || dataRowNode.RowHandle.Value != GridControl.NewItemRowHandle)
					return true;
			}
			return false;
		}
	}
	public abstract class GridContainerPrintingNodeBase : ContainerPrintingNodeBase {
		protected internal new PrintingDataTreeBuilder TreeBuilder { get { return base.TreeBuilder as PrintingDataTreeBuilder; } }
		public GridContainerPrintingNodeBase(DataNodeContainer nodeContainer, PrintingDataTreeBuilderBase treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(nodeContainer, treeBuilder, parent, index, pageSize) {
		}
		protected override bool GetIsDetailContainerCore() {
			return !nodeContainer.PrintInfo.IsGroupRowsContainer && !(TreeBuilder.View.DataControl.DetailDescriptorCore is DataControlDetailDescriptor || TreeBuilder.View.DataControl.DetailDescriptorCore is TabViewDetailDescriptor);
		}
		protected override IDataNode CreateGroupChildNode(RowNode rowNode, int index) {
			return rowNode is GroupNode ? TreeBuilder.CreateGroupPrintingNode(nodeContainer, rowNode, this, index, pageSize) : TreeBuilder.CreateMasterDetailPrintingNode(nodeContainer, rowNode, this, index, pageSize);
		}
		protected override IDataNode CreateChildNode(NodeContainer container, RowNode rowNode, IDataNode parentNode, int index) {
			return TreeBuilder.CreateDetailPrintingNode(nodeContainer, rowNode, this, index);
		}
	}
	public class GridGroupRootPrintingNode : GridContainerPrintingNodeBase, IVisualGroupNodeFixedFooter {
		public GridGroupRootPrintingNode(PrintingDataTreeBuilderBase treeBuilder, IDataNode parent, Size pageSize)
			: base(treeBuilder.RootNodeContainer, treeBuilder, parent, -1, pageSize) {
		}
		RowViewInfo IVisualGroupNode.GetFooter(bool allowContentReuse) {
			if(TreeBuilder.PrintView.PrintTotalSummary && TreeBuilder.View.ShowTotalSummary && TreeBuilder.PrintFooterTemplate != null)
				return new RowViewInfo(TreeBuilder.PrintFooterTemplate, TreeBuilder.HeadersData);
			return null;
		}
		RowViewInfo IVisualGroupNode.GetHeader(bool allowContentReuse) {
			if(TreeBuilder.PrintHeaderTemplate != null) {
				return new RowViewInfo(TreeBuilder.PrintHeaderTemplate, TreeBuilder.HeadersData);
			}
			return null;
		}
		RowViewInfo IVisualGroupNodeFixedFooter.GetFixedFooter(bool allowContentReuse) {
			if(TreeBuilder.PrintView.PrintFixedTotalSummary && TreeBuilder.View.ShowFixedTotalSummary && TreeBuilder.PrintFixedFooterTemplate != null)
				return new RowViewInfo(TreeBuilder.PrintFixedFooterTemplate, TreeBuilder.HeadersData);
			return null;
		}
		GroupUnion IGroupNode.Union { get { return GroupUnion.WithFirstDetail; } }
		bool IGroupNode.RepeatHeaderEveryPage { get { return true; } }
	}
	public class GridRootPrintingNode : GridContainerPrintingNodeBase, IRootDataNode, IDisposable {
		public GridRootPrintingNode(PrintingDataTreeBuilderBase treeBuilder, Size pageSize)
			: base(null, treeBuilder, null, -1, pageSize) { }
		protected override bool CanGetChildCore(int index) {
			return index < 1;
		}
		protected override bool IsDetailContainerCore {
			get { return false; }
		}
		protected override IDataNode GetChildCore(int index) {
			return new GridGroupRootPrintingNode(TreeBuilder, this, pageSize);
		}
		int GetMasterRowsCount() {
			VirtualItemsEnumerator en = new VirtualItemsEnumerator(TreeBuilder.RootNodeContainer);
			int count = 0;
			while(en.MoveNext())
				count++;
			return count;
		}
		int GetExpandedDetailsRowsCount() {
			int result = 0;
			GridPrintingDataTreeBuilder treeBuilder = TreeBuilder as GridPrintingDataTreeBuilder;
			if(treeBuilder == null)
				return result;
			if(!treeBuilder.View.AllowPrintDetails.ToBoolean(GridPrintingHelper.DefaultAllowPrintDetails))
				return result;
			return GetExpandedDetailsRowsCountCore(treeBuilder.View.Grid);
		}
		int GetExpandedDetailsRowsCountCore(GridControl grid) {
			int result = 0;
			for(int i = 0; i < grid.VisibleRowCount; i++) {
				int rowHandle = grid.GetRowHandleByVisibleIndex(i);
				if(!grid.IsMasterRowExpanded(rowHandle))
					continue;
				DataControlDetailDescriptor descriptor = MasterDetailPrintHelper.GetActiveDetailDescriptor((GridPrintingDataTreeBuilder)TreeBuilder, rowHandle, grid, false);
				if(descriptor == null)
					continue;
				GridControl detailgrid = grid.MasterDetailProvider.FindDetailDataControl(rowHandle, descriptor) as GridControl;
				if(detailgrid == null || !(detailgrid.View is TableView))
					continue;
				result += detailgrid.VisibleRowCount;
				if(!((TableView)detailgrid.View).AllowPrintDetails.ToBoolean(GridPrintingHelper.DefaultAllowPrintDetails))
					continue;
				result += GetExpandedDetailsRowsCountCore(detailgrid);
			}
			return result;
		}
		bool IsPrintAllDetails() {
			GridPrintingDataTreeBuilder treeBuilder = TreeBuilder as GridPrintingDataTreeBuilder;
			if(treeBuilder == null)
				return false;
			return treeBuilder.View.PrintAllDetails.ToBoolean(false);
		}
		#region IRootDataNode Members
		int IRootDataNode.GetTotalDetailCount() {
			if(IsPrintAllDetails())
				return -1;
			int masterRowsCount = GetMasterRowsCount();
			int expandedDetailsRowsCount = GetExpandedDetailsRowsCount();
			return masterRowsCount + expandedDetailsRowsCount;
		}
		void IDisposable.Dispose() {
			TreeBuilder.OnRootNodeDispose(); 
		}
		#endregion
	}
	public class MasterDetailPrintHelper {
		public static bool IsMasterRowExpanded(GridPrintingDataTreeBuilder treeBuilder, int rowHandle) {
			return treeBuilder.View.Grid.IsMasterRowExpanded(treeBuilder.GetOriginalRowHandle(rowHandle));
		}
		public static bool IsDetailContainsRows(GridPrintingDataTreeBuilder treeBuilder, int masterRowHandle, DataControlBase detailDataControl) {
			if(!((TableView)detailDataControl.DataView).PrintSelectedRowsOnly)
				return detailDataControl.VisibleRowCount > 0;
			PrintSelectedRowsInfo info = null;
			return PrintSelectedRowsHelper.GetSelectedRows(detailDataControl.DataProviderBase, detailDataControl.viewCore, out info, null).Count > 0;
		}
		public static RowDetailInfoBase GetRowDetailInfoForPrinting(GridPrintingDataTreeBuilder treeBuilder, int rowHandle) {
			return ((MasterDetailProvider)treeBuilder.View.Grid.MasterDetailProvider).GetRowDetailInfoForPrinting(treeBuilder.GetOriginalRowHandle(rowHandle));
		}
		public static DataControlBase FindDetailDataControl(GridPrintingDataTreeBuilder treeBuilder, int rowHandle, DataControlDetailDescriptor descriptor) {
			return treeBuilder.View.Grid.MasterDetailProvider.FindDetailDataControl(treeBuilder.GetOriginalRowHandle(rowHandle), descriptor);
		}
		public static DataControlDetailDescriptor GetActiveDetailDescriptor(GridPrintingDataTreeBuilder treeBuilder, int rowHandle, GridControl grid = null, bool useOriginalRowHandle = true) {
			if(grid == null) grid = treeBuilder.View.Grid;
			if(grid.DetailDescriptor is DataControlDetailDescriptor)
				return grid.DetailDescriptor as DataControlDetailDescriptor;
			if(grid.DetailDescriptor is TabViewDetailDescriptor)
				return GetActiveDetailDescriptor(grid, grid.DetailDescriptor as TabViewDetailDescriptor, useOriginalRowHandle ? treeBuilder.GetOriginalRowHandle(rowHandle) : rowHandle);
			return null;
		}
		static DataControlDetailDescriptor GetActiveDetailDescriptor(GridControl grid, TabViewDetailDescriptor tabDescriptor, int rowHandle) {
			TabsDetailInfo tabsDetailInfo = (TabsDetailInfo)((MasterDetailProvider)grid.MasterDetailProvider).GetRowDetailInfo(rowHandle);
			DetailDescriptorBase descriptorBase = tabsDetailInfo.FindVisibleDetailDescriptor();
			if(descriptorBase is DataControlDetailDescriptor)
				return descriptorBase as DataControlDetailDescriptor;
			if(descriptorBase is TabViewDetailDescriptor)
				return GetActiveDetailDescriptor(grid, descriptorBase as TabViewDetailDescriptor, rowHandle);
			return null;
		}
		public static List<DataControlDetailDescriptor> GetAllDetailDescriptors(DataControlBase grid) {
			GridControl gridControl = grid as GridControl;
			List<DataControlDetailDescriptor> result = new List<DataControlDetailDescriptor>();
			if(gridControl.DetailDescriptor is DataControlDetailDescriptor) {
				result.Add(gridControl.DetailDescriptor as DataControlDetailDescriptor);
				return result;
			}
			if(gridControl.DetailDescriptor is TabViewDetailDescriptor)
				GetAllDetailDescriptorsFromTab((TabViewDetailDescriptor)gridControl.DetailDescriptor, result);
			return result;
		}
		static void GetAllDetailDescriptorsFromTab(TabViewDetailDescriptor tabDescriptor, List<DataControlDetailDescriptor> result) {
			foreach(var descriptor in tabDescriptor.DetailDescriptors) {
				if(descriptor is DataControlDetailDescriptor) {
					result.Add(descriptor as DataControlDetailDescriptor);
					continue;
				}
				if(descriptor is TabViewDetailDescriptor)
					GetAllDetailDescriptorsFromTab((TabViewDetailDescriptor)descriptor, result);
			}
		}
		public static MasterDetailPrintInfo GetInheritedPrintInfo(TableView view, GridPrintingDataTreeBuilder TreeBuilder, PrintDetailType printDetailType
#if DEBUGTEST
, int forceDetailGroupLevel = -1
#endif
) {
			DefaultBoolean allowPrintDetails = view.AllowPrintDetails == DefaultBoolean.Default ? TreeBuilder.MasterDetailPrintInfo.AllowPrintDetails : view.AllowPrintDetails;
			DefaultBoolean allowPrintEmptyDetails = view.AllowPrintEmptyDetails == DefaultBoolean.Default ? TreeBuilder.MasterDetailPrintInfo.AllowPrintEmptyDetails : view.AllowPrintEmptyDetails;
			DefaultBoolean printAllDetails = view.PrintAllDetails == DefaultBoolean.Default ? TreeBuilder.MasterDetailPrintInfo.PrintAllDetails : view.PrintAllDetails;
#if DEBUGTEST
			int detailGroupLevel = forceDetailGroupLevel == -1 ? TreeBuilder.MasterDetailPrintInfo.DetailGroupLevel + TreeBuilder.ReusingRowData.Level : forceDetailGroupLevel;
#else
			int detailGroupLevel = TreeBuilder.MasterDetailPrintInfo.DetailGroupLevel + TreeBuilder.ReusingRowData.Level;
#endif
			return new MasterDetailPrintInfo(allowPrintDetails, allowPrintEmptyDetails, printAllDetails, TreeBuilder.MasterDetailPrintInfo.RootPrintingDataTreeBuilder, printDetailType, detailGroupLevel);
		}
	}
	public class DescriptorAndGridControl : IDescriptorAndDataControlBase {
		DataControlDetailDescriptor descriptor;
		BindingBase itemsSourceBinding;
		GridControl grid;
		int currentRowHandle = DataControlBase.InvalidRowHandle;
		public DataControlBase Grid { get { return grid; } }
		public DetailDescriptorBase Descriptor {get { return descriptor; } }
		internal DescriptorAndGridControl(DataControlDetailDescriptor descriptor) {
			this.descriptor = descriptor;
		}
		void InitializeGrid(DataControlDetailInfo detailInfo, PrintingDataTreeBuilderBase treeBuilder) {
			if(detailInfo.DataControl == null)
				detailInfo.UpdateDataControl();
			itemsSourceBinding = descriptor.GetItemsSourceBinding();
			grid = (GridControl)descriptor.DataControl.CloneDetailForPrint(treeBuilder.View.MasterRootNodeContainer, treeBuilder.View.MasterRootRowsContainer);
			descriptor.DataControl.PopulateColumnsIfNeeded(detailInfo.DataControl.DataProviderBase);
			descriptor.DataControl.CopyToDetail(grid);
			grid.LockUpdateLayout = true;
		}
		void UpdateClonedDetail(DataControlDetailInfo detailInfo, PrintingDataTreeBuilderBase treeBuilder) {
			grid.DataContext = treeBuilder.ReusingRowData.Row;
			grid.DataControlParent = detailInfo;
			if(itemsSourceBinding != null)
				grid.SetBinding(DataControlBase.ItemsSourceProperty,
#if SL
				(Binding)
#endif
			itemsSourceBinding);
			grid.PopulateColumnsIfNeeded(grid.GridDataProvider);
			grid.UpdateTotalSummary();
			grid.View.UpdateColumnsViewInfo();
		}
		public DataControlBase GetDetailGridControl(PrintingDataTreeBuilderBase treeBuilder, out bool isGenerated) {
			isGenerated = true;
			if(treeBuilder.ReusingRowData.RowHandle.Value == currentRowHandle)
				return grid;
			currentRowHandle = treeBuilder.ReusingRowData.RowHandle.Value;
			RowDetailInfoBase detailInfoBase = MasterDetailPrintHelper.GetRowDetailInfoForPrinting((GridPrintingDataTreeBuilder)treeBuilder, treeBuilder.ReusingRowData.RowHandle.Value);
			DataControlDetailInfo detailInfo = detailInfoBase as DataControlDetailInfo;
			if(detailInfo == null)
				detailInfo = GetDetailInfo(detailInfoBase);
			if(grid == null)
				InitializeGrid(detailInfo, treeBuilder);
			UpdateClonedDetail(detailInfo, treeBuilder);
			return grid;
		}
		DataControlDetailInfo GetDetailInfo(RowDetailInfoBase detailInfoBase) {
			TabsDetailInfo tabsDetailInfo = (TabsDetailInfo)detailInfoBase;
			return descriptor.CreateRowDetailInfo(tabsDetailInfo.container) as DataControlDetailInfo;
		}
	}
	public class StubNode : PrintingNodeBase, IVisualDetailNode {
		public StubNode(DataTreeBuilder treeBuilder, IDataNode parent, int index)
			: base(treeBuilder, parent, index) { 
		}
		protected override bool IsDetailContainerCore {
			get { return false; }
		}
		protected override IDataNode GetChildCore(int index) {
			return null;
		}
		protected override bool CanGetChildCore(int index) {
			return false;
		}
		public RowViewInfo GetDetail(bool allowContentReuse) {
			return null;
		}
	}
	public class GridMasterDetailPrintingNode : GridContainerPrintingNodeBase, IVisualGroupNode {
		readonly RowNode rowNode;
		readonly NodeContainer parentContainer;
		DataControlBase detailDataControl;
		List<KeyValuePair<DataControlBase, bool>> details;
		protected internal new GridPrintingDataTreeBuilder TreeBuilder { get { return base.TreeBuilder as GridPrintingDataTreeBuilder; } }
		public GridMasterDetailPrintingNode(NodeContainer parentContainer, RowNode rowNode, GridPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(treeBuilder.RootNodeContainer, treeBuilder, parent, index, pageSize) {
			this.rowNode = rowNode;
			this.parentContainer = parentContainer;
		}
		#region IVisualGroupNode Members
		RowViewInfo IVisualGroupNode.GetFooter(bool allowContentReuse) {
			return null;
		}
		protected override bool GetIsDetailContainerCore() {
			return printStub;
		}
		protected void UpdateReusingRowData() {
			TreeBuilder.ReusingRowData.AssignFromInternal(null, parentContainer, rowNode, true);
		}
		RowViewInfo IVisualGroupNode.GetHeader(bool allowContentReuse) {
			UpdateReusingRowData();
			return CreateRowElement(TreeBuilder.ReusingRowData, TreeBuilder.PrintRowTemplate);
		}
		bool printStub = false;
		protected override bool CanGetChildCore(int index) {
			if(!TreeBuilder.GetAllowPrintDetailsValue() || index > GetPrintDeailsLimit()) {
				return false;
			}
			UpdateReusingRowData();
			if(details == null)
				ActualizeDetails();
			if(!TreeBuilder.GetPrintAllDetailsValue()) {
				if(!IsMasterRowExpanded()) {
					if(index == 0) {
						printStub = true;
						return true;
					}
					return false;
				}
				DataControlDetailDescriptor descriptor = MasterDetailPrintHelper.GetActiveDetailDescriptor(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value);
				if(descriptor == null)
					return false;
				detailDataControl = MasterDetailPrintHelper.FindDetailDataControl(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value, descriptor);
				if(MasterDetailPrintHelper.IsDetailContainsRows(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value, detailDataControl))
					return true;
				return CanPrintEmptyDetails(detailDataControl);
			}
			if(details.Count == 0) {
				if(index == 0) {
					printStub = true;
					return true;
				}
				return false;
			}
			if(TreeBuilder.View.PrintSelectedRowsOnly) {
				foreach(var detail in details) {
					if(MasterDetailPrintHelper.IsDetailContainsRows(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value, detail.Key)) return true;
				}
				return false;
			}
			return true;
		}
		void ActualizeDetails() {
			List<IDescriptorAndDataControlBase> descriptors = new List<IDescriptorAndDataControlBase>();
			MasterDetailPrintHelper.GetAllDetailDescriptors(TreeBuilder.View.Grid).ForEach(descr => descriptors.Add(TreeBuilder.MasterDetailPrintInfo.RootPrintingDataTreeBuilder.GetDescriptorAndGridControl(descr)));
			details = new List<KeyValuePair<DataControlBase, bool>>();
			foreach(var descr in descriptors) {
				bool isDescriptorGenerated = true;
				DataControlBase detailGrid = MasterDetailPrintHelper.FindDetailDataControl(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value, (DataControlDetailDescriptor)descr.Descriptor);
				if(detailGrid == null || !((TableView)detailGrid.viewCore).PrintSelectedRowsOnly)
					detailGrid = descr.GetDetailGridControl(TreeBuilder, out isDescriptorGenerated);
				if(!MasterDetailPrintHelper.IsDetailContainsRows(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value, detailGrid) && !CanPrintEmptyDetails(detailGrid))
					continue;
				details.Add(new KeyValuePair<DataControlBase,bool>(detailGrid, isDescriptorGenerated));
			}
		}
		int GetPrintDeailsLimit() {
			if(details == null || !TreeBuilder.GetPrintAllDetailsValue())
				return 0;
			return details == null ? 0 : details.Count - 1;
		}
		bool IsMasterRowExpanded() {
			return MasterDetailPrintHelper.IsMasterRowExpanded(TreeBuilder, TreeBuilder.ReusingRowData.RowHandle.Value);
		}
		bool CanPrintEmptyDetails(DataControlBase dataControl) {
			if(!TreeBuilder.GetAllowPrintEmptyDetailsValue())
				return false;
			TableView view = (TableView)((GridControl)dataControl).View;
			return TreeBuilder.IsGridHeaderFooterVisible(view);
		}
		protected override IDataNode GetChildCore(int index) {
			if(index == 0 && printStub) {
				return new StubNode(TreeBuilder, this, 0);
			}
			UpdateReusingRowData();
			if(!TreeBuilder.GetPrintAllDetailsValue())
				return GetDetailPrintNode((TableView)detailDataControl.viewCore, PrintDetailType.Last, false);
			return GetDetailPrintNode((TableView)(details[index].Key).DataView, index == details.Count - 1 ? PrintDetailType.Last : PrintDetailType.None, details[index].Value);
		}
		IDataNode GetDetailPrintNode(TableView view, PrintDetailType printDetailType, bool isDescriptorGenerated) {
			double totalIndent = 0;
			DataControlBase originationDataControl = TreeBuilder.View.DataControl.GetOriginationDataControl();
			originationDataControl.EnumerateThisAndParentDataControls(dataControl => { totalIndent += GridPrintingHelper.GroupIndent; });
			totalIndent += TreeBuilder.ReusingRowData.Level * GridPrintingHelper.GroupIndent;
			IRootDataNode rootNode = GridPrintingHelper.CreatePrintingTreeNode(
				view, 
				new Size(pageSize.Width - totalIndent, pageSize.Height), 
				MasterDetailPrintHelper.GetInheritedPrintInfo(view, TreeBuilder, printDetailType),
				null
				);
			return rootNode.GetChild(-1);
		}
		DataControlDetailInfo GetDetailInfo(DataControlDetailDescriptor dcdd, RowDetailInfoBase detailInfoBase) {
			TabsDetailInfo tabsDetailInfo = (TabsDetailInfo)detailInfoBase;
			return dcdd.CreateRowDetailInfo(tabsDetailInfo.container) as DataControlDetailInfo;
		}
		GroupUnion IGroupNode.Union { get { return GroupUnion.WithFirstDetail; } }
		bool IGroupNode.RepeatHeaderEveryPage { get { return true; } }
		#endregion
	}
	public class GridDetailPrintingNode : GridPrintingNodeBase, IVisualDetailNode {
		readonly RowNode rowNode;
		readonly NodeContainer parentContainer;
		public GridDetailPrintingNode(NodeContainer parentContainer, RowNode rowNode, PrintingDataTreeBuilderBase treeBuilder, IDataNode parent, int index)
			: base(treeBuilder, parent, index) {
			this.rowNode = rowNode;
			this.parentContainer = parentContainer;
		}
		#region IVisualDetailNode Members
		RowViewInfo IVisualDetailNode.GetDetail(bool allowContentReuse) {
			TreeBuilder.ReusingRowData.AssignFromInternal(null, parentContainer, rowNode, false);
			TreeBuilder.ReusingRowData.UpdateCellDataEditorsDisplayText();
			TreeBuilder.ReusingRowData.UpdatePrintingMergeValue();
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
	public class GridGroupPrintingNode : GridContainerPrintingNodeBase, IVisualGroupNode {
		protected readonly GroupNode groupNode;
		protected readonly NodeContainer parentContainer;
		protected internal GridPrintingDataTreeBuilder GridTreeBuilder { get { return base.TreeBuilder as GridPrintingDataTreeBuilder; } }
		public GridGroupPrintingNode(NodeContainer parentContainer, GroupNode groupNode, GridPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(groupNode.NodesContainer, treeBuilder, parent, index, pageSize) {
			this.groupNode = groupNode;
			this.parentContainer = parentContainer;
		}
		#region IVisualGroupNode Members
		RowViewInfo IVisualGroupNode.GetFooter(bool allowContentReuse) {
			return null;
		}
		RowViewInfo IVisualGroupNode.GetHeader(bool allowContentReuse) {
			GridTreeBuilder.reusingGroupRowData.AssignFromInternal(null, parentContainer, groupNode, true);
			return CreateRowElement(GridTreeBuilder.reusingGroupRowData, GridTreeBuilder.PrintGroupRowTemplate);
		}
		GroupUnion IGroupNode.Union { get { return GroupUnion.WithFirstDetail; } }
		bool IGroupNode.RepeatHeaderEveryPage { get { return true; } }
		protected override bool CanGetChildCore(int index) {
			if(0 <= index && index < nodeContainer.Items.Count) {
				DataRowNode dataRowNode = nodeContainer.Items[index] as DataRowNode;
				if(dataRowNode == null || dataRowNode.RowHandle.Value != GridControl.NewItemRowHandle)
					return true;
			}
			return false;
		}
		#endregion
	}
	public class GridGroupSummaryPrintingNode : GridGroupPrintingNode, IVisualGroupNode {
		GroupSummaryRowNode SummaryRowNode { get { return groupNode as GroupSummaryRowNode; } }
		public GridGroupSummaryPrintingNode(NodeContainer parentContainer, GroupNode groupNode, GridPrintingDataTreeBuilder treeBuilder, IDataNode parent, int index, Size pageSize)
			: base(parentContainer, groupNode, treeBuilder, parent, index, pageSize) { }
		RowViewInfo IVisualGroupNode.GetFooter(bool allowContentReuse) {
			GroupSummaryRowData rowData = (GroupSummaryRowData)SummaryRowNode.CreateRowData();
			rowData.AssignFromInternal(null, parentContainer, groupNode, true);
			return CreateRowElement(rowData, GridTreeBuilder.PrintGroupFooterTemplate);
		}
		RowViewInfo IVisualGroupNode.GetHeader(bool allowContentReuse) {
			return null;
		}
		protected override bool CanGetChildCore(int index) {
			return false;
		}
	}
}
