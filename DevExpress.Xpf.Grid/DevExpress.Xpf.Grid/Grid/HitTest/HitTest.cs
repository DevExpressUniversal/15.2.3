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
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
namespace DevExpress.Xpf.Grid {
	public abstract class GridViewHitTestVisitorBase : DataViewHitTestVisitorBase {
		internal GridViewHitTestVisitorBase(IDataViewHitInfo hitInfo) 
			: base(hitInfo) {
		}
		public virtual void VisitGroupRow(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.GroupRow);
			hitInfo.SetRowHandle(rowHandle);
			StopHitTestingCore();
		}
		public virtual void VisitGroupFooterSummaryRow(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.GroupFooterRow);
			hitInfo.SetRowHandle(rowHandle);
		}
		public virtual void VisitGroupFooterSummary(int rowHandle, ColumnBase column) {
			hitInfo.SetHitTest(TableViewHitTest.GroupFooterSummary);
			hitInfo.SetRowHandle(rowHandle);
			hitInfo.SetColumn(column);
		}
		public virtual void VisitGroupRowButton(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.GroupRowButton);
			hitInfo.SetRowHandle(rowHandle);
		}
		public virtual void VisitMasterRowButton(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.MasterRowButton);
			hitInfo.SetRowHandle(rowHandle);
		}
		public virtual void VisitGroupRowCheckBox(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.GroupRowCheckBox);
			hitInfo.SetRowHandle(rowHandle);
		}
		public virtual void VisitGroupPanel() {
			hitInfo.SetHitTest(TableViewHitTest.GroupPanel);
		}
		public virtual void VisitGroupPanelColumnHeader(GridColumn column) {
			hitInfo.SetHitTest(TableViewHitTest.GroupPanelColumnHeader);
			hitInfo.SetColumn(column);
		}
		public virtual void VisitGroupPanelColumnHeaderFilterButton(GridColumn column) {
			hitInfo.SetHitTest(TableViewHitTest.GroupPanelColumnHeaderFilterButton);
		}
		public virtual void VisitGroupValue(int rowHandle, GridColumnData columnData) {
			hitInfo.SetHitTest(TableViewHitTest.GroupValue);
		}
		public virtual void VisitGroupSummary(int rowHandle, GridGroupSummaryData summaryData) {
			hitInfo.SetHitTest(TableViewHitTest.GroupSummary);
		}
	}
	public abstract class TableViewHitTestVisitorBase : GridViewHitTestVisitorBase {
		protected TableViewHitTestVisitorBase()
			: base(TableViewHitInfo.Instance) {
		}
		internal TableViewHitTestVisitorBase(GridViewHitInfoBase hitInfo) 
			: base(hitInfo) {
		}
		#region IHitTestVisitor Members
		public virtual void VisitColumnButton() {
			hitInfo.SetHitTest(TableViewHitTest.ColumnButton);
		}
		public virtual void VisitBandButton() {
			hitInfo.SetHitTest(TableViewHitTest.BandButton);
		}
		public virtual void VisitColumnEdge(BaseColumn column) {
			hitInfo.SetHitTest(TableViewHitTest.ColumnEdge);
		}
		public virtual void VisitBandEdge(BandBase band) {
			hitInfo.SetHitTest(TableViewHitTest.BandEdge);
		}
		public virtual void VisitFixedLeftDiv(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.FixedLeftDiv);
		}
		public virtual void VisitFixedRightDiv(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.FixedRightDiv);
		}
		public virtual void VisitRowIndicator(int rowHandle, IndicatorState indicatorState) {
			hitInfo.SetHitTest(TableViewHitTest.RowIndicator);
			hitInfo.SetRowHandle(rowHandle);
			StopHitTestingCore();
		}
		#endregion
	}
	public sealed class HitInfoTableViewHitTestVisitor : TableViewHitTestVisitorBase {
		public HitInfoTableViewHitTestVisitor(TableViewHitInfo hitInfo)
			: base(hitInfo) {
		}
		internal override void StopHitTestingCore() {
			StopHitTesting();
		}
	}
#if !SL
	public abstract class CardViewHitTestVisitorBase : GridViewHitTestVisitorBase {
		CardViewHitInfo CardHitInfo { get { return (CardViewHitInfo)hitInfo; } }
		protected CardViewHitTestVisitorBase()
			: base(CardViewHitInfo.Instance) {
		}
		internal CardViewHitTestVisitorBase(GridViewHitInfoBase hitInfo)
			: base(hitInfo) {
		}
		#region IHitTestVisitor Members
		public virtual void VisitFieldCaption(int rowHandle, GridColumn column) {
			CardHitInfo.SetCardHitTest(CardViewHitTest.FieldCaption);
			hitInfo.SetColumn(column);
		}
		public virtual void VisitCardHeader(int rowHandle) {
			CardHitInfo.SetCardHitTest(CardViewHitTest.CardHeader);
		}
		public virtual void VisitCardHeaderButton(int rowHandle) {
			CardHitInfo.SetCardHitTest(CardViewHitTest.CardHeaderButton);
		}
		public virtual void VisitSeparator() {
			CardHitInfo.SetCardHitTest(CardViewHitTest.Separator);
		}
		public virtual void VisitColumnPanelShowButton() {
			CardHitInfo.SetCardHitTest(CardViewHitTest.ColumnPanelShowButton);
		}
		#endregion
		public sealed override void VisitRowCell(int rowHandle, ColumnBase column) {
			base.VisitRowCell(rowHandle, column);
			VisitFieldValue(rowHandle, column);
		}
		public sealed override void VisitRow(int rowHandle) {
			base.VisitRow(rowHandle);
			VisitCard(rowHandle);
		}
		public virtual void VisitFieldValue(int rowHandle, ColumnBase column) {
		}
		public virtual void VisitCard(int rowHandle) {
		}
	}
	public sealed class HitInfoCardViewHitTestVisitor : CardViewHitTestVisitorBase {
		public HitInfoCardViewHitTestVisitor(CardViewHitInfo hitInfo)
			: base(hitInfo) {
		}
		internal override void StopHitTestingCore() {
			StopHitTesting();
		}
	}
#endif
#if !SL
	public enum CardViewHitTest {
		#region common
		None,
		FieldValue,
		Card,
		GroupRow,
		GroupRowButton,
		ColumnHeaderPanel,
		ColumnHeader,
		ColumnHeaderFilterButton,
		GroupPanel,
		GroupPanelColumnHeader,
		GroupPanelColumnHeaderFilterButton,
		VerticalScrollBar,
		HorizontalScrollBar,
		FilterPanel,
		FilterPanelCloseButton,
		FilterPanelCustomizeButton,
		FilterPanelActiveButton,
		FilterPanelText,
		MRUFilterListComboBox,
		TotalSummaryPanel,
		FixedTotalSummary,
		TotalSummary,
		DataArea,
		GroupValue,
		GroupSummary,
		#endregion
		FieldCaption,
		CardHeader,
		CardHeaderButton,
		Separator,
		ColumnPanelShowButton,
	}
#endif
}
