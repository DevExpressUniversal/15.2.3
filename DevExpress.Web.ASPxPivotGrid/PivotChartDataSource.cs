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

using System.Web.UI;
using System.Collections;
using DevExpress.XtraPivotGrid.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using DevExpress.XtraPivotGrid;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Data.ChartDataSources;
using DevExpress.Utils;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.Charts.Native;
using System.Drawing;
using System.Security;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotChartDataSourceView : DataSourceView, IPivotGrid {
		ASPxPivotGrid pivotGrid;
		PivotChartDataSource chartDataSource;
		public PivotChartDataSource ChartDataSource { get { return chartDataSource; } }
		public PivotChartDataSourceView(ASPxPivotGrid pivotGrid) : base(pivotGrid, string.Empty) {
			this.pivotGrid = pivotGrid;
			this.chartDataSource = CreateChartDataSource(pivotGrid);
		}
		ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected IPivotGrid IPivotGrid { get { return (IPivotGrid)ChartDataSource; } }
		protected virtual PivotChartDataSource CreateChartDataSource(ASPxPivotGrid pivot) {
			return (PivotChartDataSource)pivot.Data.ChartDataSource;
		}
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
			return chartDataSource;
		}
		public virtual void InvalidateChartData() {
			chartDataSource.InvalidateChartData();
		}
		#region IPivotGrid Members
		event DataChangedEventHandler IChartDataSource.DataChanged {
			add { ((IChartDataSource)IPivotGrid).DataChanged += value; }
			remove { ((IChartDataSource)IPivotGrid).DataChanged -= value; }
		}
		IList<string> IPivotGrid.ArgumentColumnNames {
			get { return IPivotGrid.ArgumentColumnNames; }
		}
		IList<string> IPivotGrid.ValueColumnNames {
			get { return IPivotGrid.ValueColumnNames; }
		}
		bool IPivotGrid.RetrieveDataByColumns {
			get { return IPivotGrid.RetrieveDataByColumns; }
			set {
				if(IPivotGrid.RetrieveDataByColumns == value) 
					return;
				IPivotGrid.RetrieveDataByColumns = value;
				OnChanged();
			}
		}
		bool IPivotGrid.SinglePageSupported { get { return true; } }
		bool IPivotGrid.SinglePageOnly { 
			get { return PivotGrid.OptionsChartDataSource.CurrentPageOnly; } 
			set {
				if(PivotGrid.OptionsChartDataSource.CurrentPageOnly == value) return;
				PivotGrid.OptionsChartDataSource.CurrentPageOnly = value; 
			} 
		}
		bool IPivotGrid.SelectionSupported { get { return IPivotGrid.SelectionSupported; } }
		bool IPivotGrid.SelectionOnly { 
			get { return IPivotGrid.SelectionOnly; }
			set {
				if(IPivotGrid.SelectionOnly == value) return;
				IPivotGrid.SelectionOnly = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveColumnTotals {
			get { return IPivotGrid.RetrieveColumnTotals; }
			set {
				if(IPivotGrid.RetrieveColumnTotals == value) return;
				IPivotGrid.RetrieveColumnTotals = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveColumnGrandTotals {
			get { return IPivotGrid.RetrieveColumnGrandTotals; }
			set {
				if(IPivotGrid.RetrieveColumnGrandTotals == value) return;
				IPivotGrid.RetrieveColumnGrandTotals = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveColumnCustomTotals {
			get { return IPivotGrid.RetrieveColumnCustomTotals; }
			set {
				if(IPivotGrid.RetrieveColumnCustomTotals == value) return;
				IPivotGrid.RetrieveColumnCustomTotals = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveRowTotals {
			get { return IPivotGrid.RetrieveRowTotals; }
			set {
				if(IPivotGrid.RetrieveRowTotals == value) return;
				IPivotGrid.RetrieveRowTotals = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveRowGrandTotals {
			get { return IPivotGrid.RetrieveRowGrandTotals; }
			set {
				if(IPivotGrid.RetrieveRowGrandTotals == value) return;
				IPivotGrid.RetrieveRowGrandTotals = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveRowCustomTotals {
			get { return IPivotGrid.RetrieveRowCustomTotals; }
			set {
				if(IPivotGrid.RetrieveRowCustomTotals == value) return;
				IPivotGrid.RetrieveRowCustomTotals = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveEmptyCells {
			get { return IPivotGrid.RetrieveEmptyCells; }
			set {
				if(IPivotGrid.RetrieveEmptyCells == value) return;
				IPivotGrid.RetrieveEmptyCells = value;
				OnChanged();
			}
		}
		bool IPivotGrid.RetrieveDateTimeValuesAsMiddleValues {
			get { return IPivotGrid.RetrieveDateTimeValuesAsMiddleValues; }
			set { IPivotGrid.RetrieveDateTimeValuesAsMiddleValues = value; }
		}
		int IPivotGrid.MaxAllowedSeriesCount {
			get { return IPivotGrid.MaxAllowedSeriesCount; }
			set {
				if(IPivotGrid.MaxAllowedSeriesCount == value) return;
				IPivotGrid.MaxAllowedSeriesCount = value;
				OnChanged();
			}
		}
		int IPivotGrid.MaxAllowedPointCountInSeries {
			get { return IPivotGrid.MaxAllowedPointCountInSeries; }
			set {
				if(IPivotGrid.MaxAllowedPointCountInSeries == value) return;
				IPivotGrid.MaxAllowedPointCountInSeries = value;
				OnChanged();
			}
		}
		int IPivotGrid.UpdateDelay { 
			get { return IPivotGrid.UpdateDelay; } 
			set {
				if(IPivotGrid.UpdateDelay == value) return;
				IPivotGrid.UpdateDelay = value; 
			} 
		}
		string IChartDataSource.ValueDataMember { get { return IPivotGrid.ValueDataMember; } }
		string IChartDataSource.ArgumentDataMember { get { return IPivotGrid.ArgumentDataMember; } }
		string IChartDataSource.SeriesDataMember { get { return IPivotGrid.SeriesDataMember; } }
		DefaultBoolean IPivotGrid.RetrieveFieldValuesAsText {
			get { return IPivotGrid.RetrieveFieldValuesAsText; }
			set {
				if(IPivotGrid.RetrieveFieldValuesAsText == value) return;
				IPivotGrid.RetrieveFieldValuesAsText = value;
				OnChanged();
			}
		}
		DateTimeMeasureUnitNative? IChartDataSource.DateTimeArgumentMeasureUnit { get { return IPivotGrid.DateTimeArgumentMeasureUnit; } }
		IDictionary<DateTime, DateTimeMeasureUnitNative> IPivotGrid.DateTimeMeasureUnitByArgument { get { return IPivotGrid.DateTimeMeasureUnitByArgument; } }
		int IPivotGrid.AvailableSeriesCount { get { return IPivotGrid.AvailableSeriesCount; } }
		IDictionary<object, int> IPivotGrid.AvailablePointCountInSeries { get { return IPivotGrid.AvailablePointCountInSeries; } }
		[SecuritySafeCritical]
		protected virtual void OnChanged() {
			if(PivotGrid.DesignMode && PivotGrid.Site != null && !ChartDataSource.IsListChangedLocked) {
				IDesignerHost Host = (IDesignerHost)PivotGrid.Site.GetService(typeof(IDesignerHost));
				ControlDesigner Designer = (ControlDesigner)Host.GetDesigner(PivotGrid);
				Designer.OnComponentChanged(PivotGrid, new ComponentChangedEventArgs(PivotGrid, null, null, null));
			}
		}
		void IPivotGrid.LockListChanged() {
			IPivotGrid.LockListChanged();
		}
		void IPivotGrid.UnlockListChanged() {
			IPivotGrid.UnlockListChanged();
		}
		#endregion
	}
	public class PivotChartDataSource : PivotChartDataSourceBase {
		public PivotChartDataSource(PivotGridWebData data) : base(data) { }
		PivotGridWebData data;
		new PivotGridWebData Data {
			get {
				if(data == null)
					data = (PivotGridWebData)base.Data;
				return data;
			} 
		}
		public override PivotGridOptionsChartDataSourceBase Options {
			get {
				return Data.OptionsChartDataSource;
			}
		}
		public override string GetListName(PropertyDescriptor[] listAccessors) {
			return Data.PivotGrid.ID;
		}
		bool IsUnpagedMode { get { return !Data.OptionsChartDataSource.CurrentPageOnly && Data.IsDataBound; } }
		protected override void EnsureIsCalculated() {
			if(Data.EventsImplementor != null) {
				if(Data.EventsImplementor.IsDataBindNecessary)
					Data.PivotGrid.DataBind();
				else
					((DevExpress.Web.Internal.IASPxWebControl)Data.PivotGrid).EnsureChildControls();
			}
			base.EnsureIsCalculated();
		}
		protected override PivotChartDataSourceRowBase CreateDataSourceRow(Point cell) {
			return new PivotChartDataSourceRow(this, cell);
		}
		#region VisualItems wrapper
		protected new PivotWebVisualItems VisualItems { get { return (PivotWebVisualItems)base.VisualItems; } }
		protected override PivotFieldValueItem GetParentItem(bool isColumn, PivotFieldValueItem item) {
			return VisualItems.GetParentItem(isColumn, item, !IsUnpagedMode);
		}
		protected override object GetCellValue(int columnIndex, int rowIndex) {
			return VisualItems.GetCellValue(columnIndex, rowIndex, !IsUnpagedMode);
		}
		protected override PivotFieldValueItem GetLastLevelItem(bool isColumn, int lastLevelIndex) {
			return VisualItems.GetLastLevelItem(isColumn, lastLevelIndex, !IsUnpagedMode);
		}
		protected override PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex) {
			return VisualItems.CreateCellItem(columnIndex, rowIndex, !IsUnpagedMode, false);
		}
		protected override int GetCount(bool isColumn) {
			if(isColumn)
				return base.GetCount(true);
			return IsUnpagedMode ? VisualItems.UnpagedRowCount : VisualItems.RowCount;
		}
		#endregion
	}
	public class PivotChartDataSourceRow : PivotChartDataSourceRowBase {
		PivotFieldValueEventArgs columnValueInfo;
		PivotFieldValueEventArgs rowValueInfo;
		PivotCellBaseEventArgs cellInfo;
		internal PivotChartDataSourceRow(PivotChartDataSourceBase ds, Point cell)
			: base(ds, cell) { }
		internal PivotChartDataSourceRow(PivotChartDataSourceBase ds)
			: base(ds) { }
		public PivotFieldValueEventArgs ColumnValueInfo {
			get {
				if(ColumnItem != null)
					columnValueInfo = new PivotFieldValueEventArgs(ColumnItem);
				return columnValueInfo;
			}
		}
		public PivotFieldValueEventArgs RowValueInfo {
			get {
				if(RowItem != null)
					rowValueInfo = new PivotFieldValueEventArgs(RowItem);
				return rowValueInfo;
			}
		}
		public PivotCellBaseEventArgs CellInfo {
			get {
				if(CellItem != null)
					cellInfo = new PivotCellBaseEventArgs(CellItem);
				return cellInfo;
			}
		}
	}
}
