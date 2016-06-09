#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native.Data;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardExport {
	public class ExportGridControl : IDisposable, IGridControl {
		readonly PivotGridData pivotData;
		readonly IList<IGridColumn> columns = new List<IGridColumn>();
		PivotGridThinClientData thinClientData;
		event EventHandler<GridCustomDrawCellEventArgsBase> customDrawCell;
		public PivotGridData PivotData { get { return pivotData; } }
		public PivotGridThinClientData Data {
			get { return thinClientData; }
			set { thinClientData = value; }
		}
		public ReadOnlyTypedList DataSource { get; set; }
		public event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell {
			add { customDrawCell += value; }
			remove { customDrawCell -= value; }
		}
		public ExportGridControl() {
			this.pivotData = new PivotGridData();
			pivotData.Fields.Add("RowIndex", PivotArea.RowArea);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		IGridColumn IGridControl.AddColumn(GridColumnViewModel columnModel) {
			return ((IGridControl)this).AddColumn(columnModel, null, null);
		}
		IGridColumn IGridControl.AddColumn(GridColumnViewModel columnModel, ValueFormatViewModel format, GridViewerDataController dataController) {
			PivotGridFieldBase field = new PivotGridFieldBase(columnModel.DataId, PivotArea.DataArea);
			PivotFieldExportHelper.SetupFieldFormat(field, format);
			field.Caption = columnModel.Caption;
			pivotData.Fields.Add(field);
			ExportGridControlColumnWrapper column = new ExportGridControlColumnWrapper(field);
			columns.Add(column);
			return column;
		}
		bool IGridControl.AllowCellMerge {
			get { return false; }
			set { ; }
		}
		bool IGridControl.AllowIncrementalSearch {
			get { return false; }
			set { ; }
		}
		void IGridControl.ClearColumns() {
			columns.Clear();
			List<PivotGridFieldBase> dataFields = pivotData.GetFieldsByArea(PivotArea.DataArea, false);
			foreach(PivotGridFieldBase field in dataFields)
				pivotData.Fields.Remove(field);
		}
		IList<IGridColumn> IGridControl.Columns {
			get {
				return columns;
			}
		}
		int IGridControl.ColumnsCount { get { return pivotData.GetFieldsByArea(PivotArea.DataArea, false).Count; } }
		List<PivotGridFieldBase> DataColumnFields {
			get { return pivotData.GetFieldsByArea(PivotArea.DataArea, false); }
		}
		void OnRequestFieldValueDisplayText(object sender, ExportFieldValueDisplayTextEventArgsWrapper e) {
			e.SetDisplayText(DataColumnFields[e.Field.AreaIndex].Caption);
		}
		bool IGridControl.EnableAppearanceEvenRow {
			get { return false; }
			set { }
		}
		bool IGridControl.ShowColumnHeaders {
			get { return false; }
			set { }
		}
		GridColumnWidthMode IGridControl.ColumnWidthMode {
			get;
			set;
		}
		bool IGridControl.WordWrap {
			get;
			set;
		}
		bool IGridControl.ShowFooter { get; set; } 
		bool IGridControl.ShowHorizontalLines {
			get { return pivotData.OptionsPrint.PrintHorzLines.ToBoolean(true); }
			set { pivotData.OptionsPrint.PrintHorzLines = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		bool IGridControl.ShowVerticalLines {
			get { return pivotData.OptionsPrint.PrintVertLines.ToBoolean(true); }
			set { pivotData.OptionsPrint.PrintVertLines = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		bool IGridControl.AllColumnsFixed {
			get;
			set;
		}
		event EventHandler<CustomColumnDisplayTextEventArgsBase> IGridControl.GridViewCustomColumnDisplayText {
			add { }
			remove { }
		}
		event EventHandler<CustomGridDisplayTextEventArgs> IGridControl.CustomGridDisplayText {
			add { }
			remove { }
		}
		void IGridControl.SetData(ReadOnlyTypedList data) {
			DataSource = data;
			if(data == null) {
				thinClientData = null;
				return;
			}
			IList listSource = (IList)data;
			ITypedList typedList = (ITypedList)data;
			PropertyDescriptorCollection properties = typedList.GetItemProperties(null);
			List<ThinClientFieldValueItem> rowValues = new List<ThinClientFieldValueItem>();
			for(int i = 0; i < listSource.Count; i++)
				rowValues.Add(new ThinClientFieldValueItem(new ThinClientValueItem(i)));
			thinClientData = new PivotGridThinClientData(null, rowValues);
			foreach(PivotGridFieldBase dataField in DataColumnFields) {
				PropertyDescriptor pd = properties[dataField.FieldName];
				for(int i = 0; i < listSource.Count; i++) {
					object sourceValue = pd.GetValue(listSource[i]);
					thinClientData.AddCell(null, rowValues[i], dataField.AreaIndex, new ThinClientValueItem(sourceValue));
				}
			}
		}
		void IGridControl.ResetClientState() { }
		void IGridControl.BeginUpdate() {
			pivotData.BeginUpdate();
		}
		void IGridControl.EndUpdate() {
			pivotData.EndUpdate();
		}
		public void ScrollTo(int index) {
			if(index < 0 || index > ((IGridControl)this).ColumnsCount - 1)
				throw new ArgumentException();
			List<PivotGridFieldBase> dataFields = pivotData.GetFieldsByArea(PivotArea.DataArea, false);
			for(int i = 0; i < index; i++)
				pivotData.Fields.Remove(dataFields[i]);
		}
		public void DataBind() {
			PivotGridThinClientDataSource oldDataSource = pivotData.PivotDataSource as PivotGridThinClientDataSource;
			if(oldDataSource != null)
				((IDisposable)oldDataSource).Dispose();
			if(thinClientData == null) {
				pivotData.PivotDataSource = null;
				return;
			}
			pivotData.PivotDataSource = new PivotGridThinClientDataSource(thinClientData);
			ExportPivotDataEventsImplementor implementor = new ExportPivotDataEventsImplementor(pivotData);
			implementor.RequestFieldValueDisplayText += OnRequestFieldValueDisplayText;
			pivotData.EventsImplementor = implementor;
		}
		public void OnRequestCustomDrawCell(object sender, GridCustomDrawCellEventArgsBase e) {
			if(customDrawCell != null)
				customDrawCell(thinClientData, e);
		}
	}
	public class ExportGridControlColumnWrapper : IGridColumn {
		readonly PivotGridFieldBase field;
		public ExportGridControlColumnWrapper(PivotGridFieldBase field) {
			this.field = field;
		}
		bool IGridColumn.AllowGroup {
			get { return false; }
			set { ; }
		}
		bool IGridColumn.AllowMove {
			get { return false; }
			set { ; }
		}
		bool IGridColumn.AllowCellMerge {
			get;
			set;
		}
		string IGridColumn.Caption {
			get { return field.Caption; }
			set { field.Caption = value; }
		}
		string IGridColumn.FieldName {
			get { return field.FieldName; }
			set { field.FieldName = value; }
		}
		GridColumnFilterMode IGridColumn.FilterMode {
			get { return GridColumnFilterMode.Value; }
			set { ; }
		}
		bool IGridColumn.Visible {
			get { return field.Visible; }
			set { field.Visible = value; }
		}
		int IGridColumn.VisibleIndex {
			get { return field.AreaIndex; }
			set { field.AreaIndex = value; }
		}
		GridColumnDisplayMode IGridColumn.DisplayMode { get; set; }
		double IGridColumn.DefaultBestCharacterCount { get; set; }
		int IGridColumn.ActualIndex { get; set; }
		double IGridColumn.Weight { get; set; }
		double IGridColumn.FixedWidth { get; set; }
		GridColumnFixedWidthType IGridColumn.WidthType { get; set; }
		bool IGridColumn.IgnoreDeltaIndication { get; set; }
		int IGridColumn.MaxIconStyleImageWidth { get; set; }
		bool IGridColumn.TextIsHidden { get; set; }
	}
}
