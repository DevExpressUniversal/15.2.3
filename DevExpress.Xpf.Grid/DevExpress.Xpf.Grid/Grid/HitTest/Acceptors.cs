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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid.HitTest {
	public abstract class GridViewHitTestAcceptorBase : DataViewHitTestAcceptorBase {
		public abstract void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor);
		public sealed override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			Accept(element, (GridViewHitTestVisitorBase)visitor);
		}
	}
	#region common
	public class RowCellTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			IGridCellEditorOwner cell = (IGridCellEditorOwner)element;
			visitor.VisitRowCell(GetRowHandleByElement(element), cell.AssociatedColumn);
		}
	}
	public class RowTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitRow(GetRowHandleByElement(element));
		}
	}
	public class ColumnHeaderTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitColumnHeader((ColumnBase)BaseGridColumnHeader.GetGridColumn(LayoutHelper.FindParentObject<BaseGridColumnHeader>(element)));
		}
	}
	public class ColumnHeaderFilterButtonTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitColumnHeaderFilterButton((ColumnBase)BaseGridColumnHeader.GetGridColumn(LayoutHelper.FindParentObject<BaseGridColumnHeader>(element)));
		}
	}
	public class ColumnHeaderPanelTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitColumnHeaderPanel();
		}
	}
	public class VerticalScrollBarTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitVerticalScrollBar();
		}
	}
	public class HorizontalScrollBarTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitHorizontalScrollBar();
		}
	}
	public class FilterPanelTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitFilterPanel();
		}
	}
	public class FilterPanelCloseButtonTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitFilterPanelCloseButton();
		}
	}
	public class FilterPanelCustomizeButtonTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitFilterPanelCustomizeButton();
		}
	}
	public class FilterPanelActiveButtonTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitFilterPanelActiveButton();
		}
	}
	public class FilterPanelTextTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitFilterPanelText();
		}
	}
	public class TotalSummaryPanelTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitTotalSummaryPanel();
		}
	}
	public class TotalSummaryTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			GridColumnData data = element.DataContext as GridColumnData;
			if(data == null)
				return;
			ColumnBase column = data.Column as ColumnBase;
			if(column != null && column.HasTotalSummaries)
				visitor.VisitTotalSummary(column);
		}
	}
	public class FixedTotalSummaryTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			IFixedTotalSummary fixedTotalSummaryProvider = element as IFixedTotalSummary;
			if(fixedTotalSummaryProvider == null)
				return;
			visitor.VisitFixedTotalSummary(fixedTotalSummaryProvider.SummaryText);
		}
	}
	public class MRUFilterListComboBoxHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitMRUFilterListComboBox();
		}
	}
	public class DataAreaTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitDataArea();
		}
	}
	public class GroupFooterRowTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupFooterSummaryRow(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	public class GroupFooterSummaryTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			GridGroupSummaryColumnData data = element.DataContext as GridGroupSummaryColumnData;
			if(data == null)
				return;
			ColumnBase column = data.Column as ColumnBase;
			if(column != null && data.HasSummary)
				visitor.VisitGroupFooterSummary(data.GroupRowData.RowHandle.Value, column);
		}
	}
	public class GroupRowTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupRow(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	public class GroupRowButtonTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupRowButton(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	public class GroupRowCheckBoxTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupRowCheckBox(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	public class GroupPanelTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupPanel();
		}
	}
	public class GroupPanelColumnHeaderTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupPanelColumnHeader((GridColumn)GridColumnHeader.GetGridColumn(LayoutHelper.FindParentObject<GridColumnHeader>(element)));
		}
	}
	public class GroupPanelColumnHeaderFilterButtonTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupPanelColumnHeaderFilterButton((GridColumn)GridColumnHeader.GetGridColumn(LayoutHelper.FindParentObject<GridColumnHeader>(element)));
		}
	}
	public class GroupValueTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupValue(DataViewHitTestAcceptorBase.GetRowHandleByElement(element), ((IGroupValuePresenter)element).ValueData);
		}
	}
	public class GroupSummaryTableViewHitTestAcceptor : GridViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			visitor.VisitGroupSummary(DataViewHitTestAcceptorBase.GetRowHandleByElement(element), ((IDefaultGroupSummaryItem)element).ValueData);
		}
	}
	#endregion
	#region TableView
	public abstract class TableViewHitTestAcceptorBase : GridViewHitTestAcceptorBase {
		public sealed override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			AcceptTableVisitor(element, (TableViewHitTestVisitorBase)visitor);
		}
		public abstract void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor);
	}
	public class ColumnEdgeTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitColumnEdge((BaseColumn)GridColumnHeader.GetGridColumn(LayoutHelper.FindParentObject<BaseGridHeader>(element)));
		}
	}
	public class BandEdgeTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitBandEdge((BandBase)GridColumnHeader.GetGridColumn(LayoutHelper.FindParentObject<BandHeaderControl>(element)));
		}
	}
	public class FixedLeftDivTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitFixedLeftDiv(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	public class FixedRightDivTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitFixedRightDiv(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	public class RowIndicatorTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			RowData data = RowData.FindRowData(element);
			visitor.VisitRowIndicator(DataViewHitTestAcceptorBase.GetRowHandleByElement(element), data.IndicatorState);
		}
	}
	public class ColumnButtonTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitColumnButton();
		}
	}
	public class BandHeaderTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitBandHeader((BandBase)BandHeaderControl.GetGridColumn(LayoutHelper.FindParentObject<BandHeaderControl>(element)));
		}
	}
	public class BandHeaderPanelTableViewHitTestAcceptor : DataViewHitTestAcceptorBase {
		public override void Accept(FrameworkElement element, DataViewHitTestVisitorBase visitor) {
			visitor.VisitBandHeaderPanel();
		}
	}
	public class BandButtonTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitBandButton();
		}
	}
	public class MasterRowButtonTableViewHitTestAcceptor : TableViewHitTestAcceptorBase {
		public override void AcceptTableVisitor(FrameworkElement element, TableViewHitTestVisitorBase visitor) {
			visitor.VisitMasterRowButton(DataViewHitTestAcceptorBase.GetRowHandleByElement(element));
		}
	}
	#endregion
#if !SL
	#region CardView
	public abstract class CardViewHitTestAcceptorBase : GridViewHitTestAcceptorBase {
		public sealed override void Accept(FrameworkElement element, GridViewHitTestVisitorBase visitor) {
			AcceptCardVisitor(element, (CardViewHitTestVisitorBase)visitor);
		}
		public abstract void AcceptCardVisitor(FrameworkElement element, CardViewHitTestVisitorBase visitor);
	}
	public class FieldCaptionCardViewHitTestAcceptor : CardViewHitTestAcceptorBase {
		public override void AcceptCardVisitor(FrameworkElement element, CardViewHitTestVisitorBase visitor) {
			GridCellData data = element.DataContext as GridCellData;
			visitor.VisitFieldCaption(GetRowHandleByElement(element), data != null ? (GridColumn)data.Column : null);
		}
	}
	public class CardHeaderCardViewHitTestAcceptor : CardViewHitTestAcceptorBase {
		public override void AcceptCardVisitor(FrameworkElement element, CardViewHitTestVisitorBase visitor) {
			visitor.VisitCardHeader(GetRowHandleByElement(element));
		}
	}
	public class CardHeaderButtonCardViewHitTestAcceptor : CardViewHitTestAcceptorBase {
		public override void AcceptCardVisitor(FrameworkElement element, CardViewHitTestVisitorBase visitor) {
			visitor.VisitCardHeaderButton(GetRowHandleByElement(element));
		}
	}
	public class SeparatorCardViewHitTestAcceptor : CardViewHitTestAcceptorBase {
		public override void AcceptCardVisitor(FrameworkElement element, CardViewHitTestVisitorBase visitor) {
			visitor.VisitSeparator();
		}
	}
	public class ColumnPanelShowButtonCardViewHitTestAcceptor : CardViewHitTestAcceptorBase {
		public override void AcceptCardVisitor(FrameworkElement element, CardViewHitTestVisitorBase visitor) {
			visitor.VisitColumnPanelShowButton();
		}
	}
	#endregion
#endif
}
