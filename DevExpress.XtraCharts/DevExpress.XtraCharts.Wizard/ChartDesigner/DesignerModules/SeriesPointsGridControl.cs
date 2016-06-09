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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
namespace DevExpress.XtraCharts.Designer {
	public partial class SeriesPointsGridControl : XtraUserControl {
		#region Nested classes
		interface IPointValuesAccessor {
			object this[int index] { get; set; }
		}
		abstract class PointValuesAccessor<TValues> : IPointValuesAccessor {
			readonly SeriesPointModel pointModel;
			protected abstract TValues[] Values { get; set; }
			protected SeriesPointModel PointModel { get { return pointModel; } }
			public object this[int index] {
				get { return Values[index]; }
				set { SetPointValue(value, index); }
			}
			protected PointValuesAccessor(SeriesPointModel pointModel) {
				this.pointModel = pointModel;
			}
			void SetPointValue(object value, int valueIndex) {
				int valuesCount = Values.Length;
				TValues[] valuesCopy = new TValues[valuesCount];
				Array.Copy(Values, valuesCopy, valuesCount);
				valuesCopy[valueIndex] = ConvertValue(value);
				Values = valuesCopy;
			}
			protected abstract TValues ConvertValue(object value);
		}
		class DateTimePointValuesAccessor : PointValuesAccessor<DateTime> {
			protected override DateTime[] Values {
				get { return PointModel.DateTimeValues; }
				set { PointModel.DateTimeValues = value; }
			}
			public DateTimePointValuesAccessor(SeriesPointModel pointModel) : base(pointModel) {
			}
			protected override DateTime ConvertValue(object value) {
				return Convert.ToDateTime(value);
			}
		}
		class NumericPointValuesAccessor : PointValuesAccessor<double> {
			protected override double[] Values {
				get { return PointModel.Values; }
				set { PointModel.Values = value; }
			}
			public NumericPointValuesAccessor(SeriesPointModel pointModel) : base(pointModel) {
			}
			protected override double ConvertValue(object value) {
				return Convert.ToDouble(value);
			}
		}
		#endregion
		SeriesPointCollectionModel model;
		ScaleType argumentScaleType;
		bool canRefreshWholeData = true;
		Series Series {
			get {
				SeriesPointCollection pointsCollection = (SeriesPointCollection)model.ChartCollection;
				return pointsCollection.Owner;
			}
		}
		IList PointList { get { return (IList)model; } }
		bool AllowEdit { get { return((IBindingList)model.ChartCollection).AllowEdit; } }
		internal SeriesPointCollectionModel SelectedModel {
			get { return model; }
			set {
				model = value;
				ModelChanged();
			}
		}
		public SeriesPointsGridControl() {
			InitializeComponent();
			this.Disposed += SeriesPointsGridControl_Disposed;
		}
		void SeriesPointsGridControl_Disposed(object sender, EventArgs e) {
			this.Disposed -= SeriesPointsGridControl_Disposed;			
			gcPoints.DataSource = null;
			pointsGridView.Dispose();
			gcPoints.Dispose();
		}
		RepositoryItemDateEdit CreateDateTimeEdit() {
			RepositoryItemDateEdit dateEdit = new RepositoryItemDateEdit() { CalendarTimeEditing = DefaultBoolean.True, EditMask = "G", ValidateOnEnterKey = true };
			dateEdit.EditValueChanged += gridEdit_EditValueChanged;
			return dateEdit;
		}
		GridColumn CreateArgumentColumn(ScaleType argumentScaleType) {
			string argumentCaption = ChartLocalizer.GetString(ChartStringId.ArgumentMember);
			GridColumn argumentColumn = new GridColumn() { Caption = argumentCaption, Visible = true };
			UpdateArgumentColumnEditor(argumentColumn, argumentScaleType);
			return argumentColumn;
		}
		GridColumn CreateColorColumn() {
			string colorCaption = ChartLocalizer.GetString(ChartStringId.ColumnColor);
			GridColumn colorColumn = new GridColumn() { Caption = colorCaption, FieldName = "Color", Visible = true };
			RepositoryItemColorPickEdit colorEdit = new RepositoryItemColorPickEdit();
			colorEdit.AutomaticColor = Color.Empty;
			colorEdit.EditValueChanged += gridEdit_EditValueChanged;
			colorColumn.ColumnEdit = colorEdit;
			return colorColumn;
		}
		void UpdateArgumentColumnEditor(GridColumn argumentColumn, ScaleType newArgumentScaleType) {
			if (newArgumentScaleType == ScaleType.DateTime) {
				argumentColumn.FieldName = "DateTimeArgument";
				argumentColumn.ColumnEdit = CreateDateTimeEdit(); ;
			}
			else {
				argumentColumn.FieldName = "Argument";
				argumentColumn.ColumnEdit = null;
			}
		}
		void gridEdit_EditValueChanged(object sender, EventArgs e) {
			pointsGridView.PostEditor();
		}
		GridColumn[] CreateValuesColumns(Series series) {
			SeriesViewBase seriesView = series.View;
			GridColumn[] columns = new GridColumn[seriesView.ValuesCount];
			for (int valueIndex = 0; valueIndex < seriesView.ValuesCount; valueIndex++) {
				string caption = seriesView.GetValueCaption(valueIndex);
				GridColumn column = new GridColumn() { Caption = caption, FieldName = caption, Visible = true, Tag = valueIndex };
				if (series.ValueScaleType == ScaleType.DateTime) {
					column.UnboundType = UnboundColumnType.DateTime;
					column.ColumnEdit = CreateDateTimeEdit();
				}
				else
					column.UnboundType = UnboundColumnType.Decimal;
				columns[valueIndex] = column;
			}
			return columns;
		}
		GridColumn CreateLinkOptionsColumn() {
			string linkCaption = ChartLocalizer.GetString(ChartStringId.ColumnLinks);
			GridColumn linksColumn = new GridColumn() { Caption = linkCaption, Visible = true };
			RepositoryItemButtonEdit buttonEdit = new RepositoryItemButtonEdit() { TextEditStyle = XtraEditors.Controls.TextEditStyles.HideTextEditor };
			buttonEdit.ButtonClick += buttonEdit_ButtonClick;
			linksColumn.ColumnEdit = buttonEdit;
			return linksColumn;
		}
		void buttonEdit_ButtonClick(object sender, XtraEditors.Controls.ButtonPressedEventArgs e) {
			int[] selectedRows = pointsGridView.GetSelectedRows();
			if(selectedRows.Length < 0 || selectedRows[0] < 0 || selectedRows[0] >= model.Count)
				return;
			SeriesPointModel point = model[selectedRows[0]];
			using(TaskLinkCollectionModelEditorForm taskLinksEditor = new TaskLinkCollectionModelEditorForm(point.Relations)) {
				taskLinksEditor.Initialize(null);
				taskLinksEditor.ShowDialog();
			}
		}
		void PopulateColumns(Series series) {
			pointsGridView.Columns.Add(CreateArgumentColumn(argumentScaleType));
			pointsGridView.Columns.AddRange(CreateValuesColumns(series));
			pointsGridView.Columns.Add(CreateColorColumn());
			if(SeriesViewHelper.IsSupportedRelations(series.View))
				pointsGridView.Columns.Add(CreateLinkOptionsColumn());
		}
		void ModelChanged() {
			pointsGridView.BeginInit();
			ClearColumns();
			gcPoints.DataSource = model;
			if (model != null) {
				Series series = Series;
				argumentScaleType = series.ActualArgumentScaleType;
				PopulateColumns(series);
				pointsGridView.OptionsBehavior.Editable = AllowEdit;
				pointsGridView.OptionsView.NewItemRowPosition = AllowEdit ? NewItemRowPosition.Bottom : NewItemRowPosition.None;
			}
			pointsGridView.EndInit();
		}
		void ClearColumns() {
			foreach (GridColumn column in pointsGridView.Columns) {
				RepositoryItem columnEdit = column.ColumnEdit;
				if (columnEdit != null) {
					columnEdit.EditValueChanged -= gridEdit_EditValueChanged;
					if (columnEdit is RepositoryItemButtonEdit)
						((RepositoryItemButtonEdit)columnEdit).ButtonClick -= buttonEdit_ButtonClick;
				}
			}
			pointsGridView.Columns.Clear();
		}
		IPointValuesAccessor CreateValuesAccessor(SeriesPointModel pointModel, ScaleType valueScaleType) {
			if (valueScaleType == ScaleType.DateTime)
				return new DateTimePointValuesAccessor(pointModel);
			else
				return new NumericPointValuesAccessor(pointModel);
		}
		void GridViewCustomUnboundColumnData(object sender, CustomColumnDataEventArgs e) {
			SeriesPointModel point = (SeriesPointModel)e.Row;
			if (point != null) {
				int valueIndex = (int)e.Column.Tag;
				IPointValuesAccessor valuesAccessor = CreateValuesAccessor(point, Series.ValueScaleType);
				if (e.IsGetData) {
					if (point.IsEmpty)
						e.Value = string.Empty;
					else
						e.Value = valuesAccessor[valueIndex];
				}
				else if (e.IsSetData) {
					if (e.Value == null || e.Value.Equals(string.Empty))
						point.IsEmpty = true;
					else
						valuesAccessor[valueIndex] = e.Value;
				}
			}
		}
		void GridViewInitNewRow(object sender, XtraGrid.Views.Grid.InitNewRowEventArgs e) {
			model.CommandManager.BeginTransaction();
			canRefreshWholeData = false;
			model.AddNewElement(PointList[PointList.Count - 1]);
		}
		void UpdateArgumentColumn() {
			if (model == null)
				return;
			ScaleType newArgumentScaleType = PointList.Count == 0 ? ScaleType.Numerical : Series.ActualArgumentScaleType;
			if (argumentScaleType != newArgumentScaleType && pointsGridView.Columns.Count > 0) {
				this.argumentScaleType = newArgumentScaleType;
				UpdateArgumentColumnEditor(pointsGridView.Columns[0], newArgumentScaleType);
			}
		}
		void GridViewRowUpdated(object sender, RowObjectEventArgs e) {
			if ((model != null) && (e.RowHandle < 0)) {
				model.CommandManager.CommitTransaction();
				canRefreshWholeData = true;
			}
		}
		void GridViewCellValueChanged(object sender, CellValueChangedEventArgs e) {
			UpdateArgumentColumn();
		}
		void GridViewKeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Delete && AllowEdit) {
				object selectedRow = pointsGridView.GetRow(pointsGridView.FocusedRowHandle);
				model.DeleteElement(selectedRow);
			}
		}
		void GridViewRowCountChanged(object sender, EventArgs e) {
			UpdateArgumentColumn();
		}
		internal void UpdateData() {
			if (model != null) {
				if (canRefreshWholeData)
					pointsGridView.RefreshData();
				pointsGridView.RefreshRow(pointsGridView.FocusedRowHandle);
				UpdateArgumentColumn();
			}
		}
	}
}
