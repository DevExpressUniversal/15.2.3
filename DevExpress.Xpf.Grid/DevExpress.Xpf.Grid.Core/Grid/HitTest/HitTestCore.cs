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
	public abstract class DataViewHitTestVisitorBase : HitTestVisitorBase {
		internal readonly IDataViewHitInfo hitInfo;
		internal DataViewHitTestVisitorBase(IDataViewHitInfo hitInfo) {
			this.hitInfo = hitInfo;
		}
		#region IHitTestVisitor Members
		public virtual void VisitRowCell(int rowHandle, ColumnBase column) {
			hitInfo.SetHitTest(TableViewHitTest.RowCell);
			hitInfo.SetRowHandle(rowHandle);
			hitInfo.SetColumn(column);
			StopHitTestingCore();
		}
		public virtual void VisitRow(int rowHandle) {
			hitInfo.SetHitTest(TableViewHitTest.Row);
			hitInfo.SetRowHandle(rowHandle);
			StopHitTestingCore();
		}
		public virtual void VisitColumnHeader(ColumnBase column) {
			hitInfo.SetHitTest(TableViewHitTest.ColumnHeader);
			hitInfo.SetColumn(column);
		}
		public virtual void VisitColumnHeaderFilterButton(ColumnBase column) {
			hitInfo.SetHitTest(TableViewHitTest.ColumnHeaderFilterButton);
		}
		public virtual void VisitColumnHeaderPanel() {
			hitInfo.SetHitTest(TableViewHitTest.ColumnHeaderPanel);
		}
		public virtual void VisitBandHeader(BandBase band) {
			hitInfo.SetHitTest(TableViewHitTest.BandHeader);
			hitInfo.SetBand(band);
		}
		public virtual void VisitBandHeaderPanel() {
			hitInfo.SetHitTest(TableViewHitTest.BandHeaderPanel);
		}
		public virtual void VisitVerticalScrollBar() {
			hitInfo.SetHitTest(TableViewHitTest.VerticalScrollBar);
		}
		public virtual void VisitHorizontalScrollBar() {
			hitInfo.SetHitTest(TableViewHitTest.HorizontalScrollBar);
		}
		public virtual void VisitFilterPanel() {
			hitInfo.SetHitTest(TableViewHitTest.FilterPanel);
		}
		public virtual void VisitFilterPanelCloseButton() {
			hitInfo.SetHitTest(TableViewHitTest.FilterPanelCloseButton);
		}
		public virtual void VisitFilterPanelActiveButton() {
			hitInfo.SetHitTest(TableViewHitTest.FilterPanelActiveButton);
		}
		public virtual void VisitFilterPanelText() {
			hitInfo.SetHitTest(TableViewHitTest.FilterPanelText);
		}
		public virtual void VisitFilterPanelCustomizeButton() {
			hitInfo.SetHitTest(TableViewHitTest.FilterPanelCustomizeButton);
		}
		public virtual void VisitMRUFilterListComboBox() {
			hitInfo.SetHitTest(TableViewHitTest.MRUFilterListComboBox);
		}
		public virtual void VisitTotalSummaryPanel() {
			hitInfo.SetHitTest(TableViewHitTest.TotalSummaryPanel);
		}
		public virtual void VisitTotalSummary(ColumnBase column) {
			hitInfo.SetHitTest(TableViewHitTest.TotalSummary);
			hitInfo.SetColumn(column);
		}
		public virtual void VisitFixedTotalSummary(string summaryText) {
			hitInfo.SetHitTest(TableViewHitTest.FixedTotalSummary);
		}
		public virtual void VisitDataArea() {
			hitInfo.SetHitTest(TableViewHitTest.DataArea);
		}
		#endregion
		internal virtual void StopHitTestingCore() { }
	}
	public enum TableViewHitTest {
		#region common
		None,
		RowCell,
		Row,
		GroupRow,
		GroupRowButton,
		GroupRowCheckBox,
		ColumnHeaderPanel,
		ColumnHeader,
		ColumnHeaderFilterButton,
		BandHeaderPanel,
		BandHeader,
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
		TotalSummary,
		FixedTotalSummary,
		DataArea,
		GroupValue,
		GroupSummary,
		#endregion
		ColumnButton,
		BandButton,
		ColumnEdge,
		BandEdge,
		FixedLeftDiv,
		FixedRightDiv,
		RowIndicator,
		GroupFooterRow,
		GroupFooterSummary,
		MasterRowButton
	}
}
