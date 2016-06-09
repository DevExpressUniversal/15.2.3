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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardCommon.Viewer {
	public enum PivotDashboardItemArea {
		RowArea = 0,
		ColumnArea = 1,
		DataArea = 2
	}
	public enum PivotDashboardItemValueType {
		Value = 0,
		Total = 1,
		GrandTotal = 2,
		CustomTotal = 3
	}
	public interface IPivotGridField {
		string FieldName { get; }
		int Width { get; set; }
		int AreaIndex { get; }
		PivotDashboardItemArea Area { get; }
	}
	public interface IPivotGridControl {
		IEnumerable<IPivotGridField> Fields { get; }
		bool ShowColumnGrandTotals { get; set; }
		bool ShowRowGrandTotals { get; set; }
		bool ShowColumnTotals { get; set; }
		bool ShowRowTotals { get; set; }
		event EventHandler<PivotFieldDisplayTextEventArgsBase> FieldValueDisplayText;
		event EventHandler<PivotCellDisplayTextEventArgsBase> CustomCellDisplayText;
		event EventHandler<PivotCustomDrawCellEventArgsBase> CustomDrawCell;
		event EventHandler<PivotFieldValueCollapseStateChangedEventArgs> FieldValueCollapseStateChanged;
		void SetData(PivotGridThinClientData data);
		void BeginUpdate();
		void EndUpdate();
		void ClearFields();
		IPivotGridField AddField(string fieldName, PivotColumnViewModel column, PivotDashboardItemArea area);
	}
	public abstract class PivotFieldDisplayTextEventArgsBase : EventArgs {
		public abstract IPivotGridField Field { get; }
		public abstract bool IsColumn { get; }
		public abstract object Value { get; }
		public abstract PivotDashboardItemValueType ValueType { get; }
		public abstract void SetDisplayText(string displayText);
		public abstract object GetDrillDownValue(string property);
	}
	public abstract class PivotFieldDisplayTextEventArgsWrapperBase : PivotFieldDisplayTextEventArgsBase {
		protected abstract PivotDrillDownDataSource CreateDrillDownDataSource();
		public override object GetDrillDownValue(string propName) {
			PivotDrillDownDataSource ds = CreateDrillDownDataSource();
			if(ds != null) {
				PropertyDescriptorCollection props = ds.GetItemProperties(null);
				propName = String.Format("{0}{1}", IsColumn ? "Column" : "Row", propName);
				PropertyDescriptor prop = props[propName];
				object value = prop.GetValue(ds[0]);
				if(value != null)
					return (string)value;
			}
			return null;
		}
	}
	public abstract class PivotCellDisplayTextEventArgsBase : EventArgs {
		public abstract IPivotGridField DataField { get; }
		public abstract object Value { get; }
		public abstract void SetDisplayText(string displayText);
	}
	public abstract class PivotCustomDrawCellEventArgsBase : CustomDrawCellEventArgsBase {
		readonly AxisPoint columnAxisPoint;
		readonly AxisPoint rowAxisPoint;
		readonly string valueFieldName;
		readonly bool isDataArea;
		public string ValueFieldName { get { return valueFieldName; } }
		public AxisPoint ColumnAxisPoint { get { return columnAxisPoint; } }
		public AxisPoint RowAxisPoint { get { return rowAxisPoint; } }
		public bool IsDataArea { get { return isDataArea; } }
		protected PivotCustomDrawCellEventArgsBase(PivotDrillDownDataSource drillDownDataSource, string valueFieldName, bool area, bool isDarkSkin, Color defaultBackColor)
			: base(isDarkSkin, false, defaultBackColor) {
			PropertyDescriptorCollection properties = drillDownDataSource.GetItemProperties(null);
			PropertyDescriptor columnTag = properties["ColumnTag"];
			if(columnTag!=null)
				this.columnAxisPoint = columnTag.GetValue(drillDownDataSource[0]) as AxisPoint;
			PropertyDescriptor rowTag = properties["RowTag"];
			if(rowTag != null)
				this.rowAxisPoint = rowTag.GetValue(drillDownDataSource[0]) as AxisPoint;
			this.valueFieldName = valueFieldName;
			this.isDataArea = area;
		}
		protected PivotCustomDrawCellEventArgsBase(AxisPoint columnAxisPoint, AxisPoint rowAxisPoint, string valueFieldName, bool area, bool isDarkSkin, Color defaultBackColor)
			: base(isDarkSkin, false, defaultBackColor) {
			this.columnAxisPoint = columnAxisPoint;
			this.rowAxisPoint = rowAxisPoint;
			this.valueFieldName = valueFieldName;
			this.isDataArea = area;
		}
	}
	public class PivotFieldValueCollapseStateChangedEventArgs : EventArgs {
		readonly bool isColumn;
		readonly bool expand;
		readonly object[] values;
		public bool IsColumn { get { return isColumn; } }
		public bool Collapse { get { return expand; } }
		public object[] Values { get { return values; } }
		public PivotFieldValueCollapseStateChangedEventArgs(bool isColumn, bool expand, object[] values) {
			this.isColumn = isColumn;
			this.expand = expand;
			this.values = values;
		}
	}
	public struct CollapseStateCacheKey {
		bool isColumn;
		object[] values;
		public CollapseStateCacheKey(bool isColumn, object[] values) {
			this.isColumn = isColumn;
			this.values = values;
		}
		public override int GetHashCode() {
			return HashcodeHelper.GetCompositeHashCode(values);
		}
		public override bool Equals(object obj) {
			CollapseStateCacheKey key = (CollapseStateCacheKey)obj;
			if(isColumn != key.isColumn || values.Length != key.values.Length)
				return false;
			for(int i = 0; i < values.Length; i++)
				if(!Object.Equals(values[i], key.values[i]))
					return false;
			return true;
		}
   }
	public class PivotDashboardItemViewControl {
		public static PivotDashboardItemArea GetPivotGridArea(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea:
					return PivotDashboardItemArea.ColumnArea;
				case PivotArea.RowArea:
					return PivotDashboardItemArea.RowArea;
				case PivotArea.DataArea:
				default:
					return PivotDashboardItemArea.DataArea;
			}
		}
		public static PivotArea GetPivotArea(PivotDashboardItemArea area) {
			switch(area) {
				case PivotDashboardItemArea.ColumnArea:
					return PivotArea.ColumnArea;
				case PivotDashboardItemArea.RowArea:
					return PivotArea.RowArea;
				case PivotDashboardItemArea.DataArea:
				default:
					return PivotArea.DataArea;
			}
		}
		static string LocalizeIfTotal(string displayText, PivotDashboardItemValueType valueType) {
			if(valueType == PivotDashboardItemValueType.Total)
				return String.Format(DashboardLocalizer.GetString(DashboardStringId.PivotGridTotal), displayText);
			return displayText;
		}
		static string GetFieldNamePrefix(PivotDashboardItemArea area) {
			switch(area) {
				case PivotDashboardItemArea.ColumnArea:
					return "Column";
				case PivotDashboardItemArea.RowArea:
					return "Row";
				case PivotDashboardItemArea.DataArea:
					return "Value";
				default:
					throw new InvalidOperationException();
			}
		}
		readonly IPivotGridControl pivot;
		PivotDashboardItemViewModel viewModel;
		ConditionalFormattingModel cfModel;
		readonly Dictionary<string, PivotColumnViewModel> columnViewModels = new Dictionary<string, PivotColumnViewModel>();
		PivotGridThinClientDataBuilder dataBuilder;
		public PivotDashboardItemViewControl(IPivotGridControl pivotGridControl) {
			pivot = pivotGridControl;
			pivot.FieldValueDisplayText+= OnPivotFieldValueDisplayText;
			pivot.CustomCellDisplayText += OnPivotCustomCellDisplayText;
			pivot.CustomDrawCell += OnPivotCustomDrawCell;
			pivot.FieldValueCollapseStateChanged += OnPivotFieldValueCollapseStateChanged;
		}
		void OnPivotFieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgsBase e) {
			IPivotGridField field = e.Field;
			if(e.ValueType == PivotDashboardItemValueType.GrandTotal && (field == null || field.Area != PivotDashboardItemArea.DataArea)) {
				e.SetDisplayText(DashboardLocalizer.GetString(DashboardStringId.PivotGridGrandTotal));
			}
			else {
				if(viewModel == null || field == null || field.Area == PivotDashboardItemArea.DataArea)
					return;
				object displayText = e.GetDrillDownValue("DisplayText");
				if(displayText != null) {
					e.SetDisplayText(LocalizeIfTotal((string)displayText, e.ValueType));
					return;
				}
				if(DashboardSpecialValues.IsOthersValue(e.Value))
					e.SetDisplayText(DashboardLocalizer.GetString(DashboardStringId.TopNOthersValue));
				else if(DashboardSpecialValues.IsNullValue(e.Value) && e.ValueType != PivotDashboardItemValueType.GrandTotal) {
					e.SetDisplayText(DashboardLocalizer.GetString(DashboardStringId.DashboardNullValue));
				}
				else {
					PivotColumnViewModel columnViewModel;
					if(columnViewModels.TryGetValue(field.FieldName, out columnViewModel)) {
						if(columnViewModel != null && columnViewModel.Format != null) {
							FormatterBase formatter = FormatterBase.CreateFormatter(columnViewModel.Format);
							e.SetDisplayText(LocalizeIfTotal(formatter.Format(e.Value), e.ValueType));
						}
					}
				}
			}
		}
		void OnPivotCustomCellDisplayText(object sender, PivotCellDisplayTextEventArgsBase e) {
			IPivotGridField dataField = e.DataField;
			if(dataField == null || viewModel == null)
				return;
			ValueFormatViewModel valueViewModel = viewModel.Values[dataField.AreaIndex].Format;
			if(valueViewModel != null) {
				FormatterBase formatter = FormatterBase.CreateFormatter(valueViewModel);
				e.SetDisplayText(formatter.Format(e.Value));
			}
		}
		void OnPivotCustomDrawCell(object sender, PivotCustomDrawCellEventArgsBase args) {
			PivotColumnViewModel valueViewModel = null;
			if(args.ValueFieldName != null)
				columnViewModels.TryGetValue(args.ValueFieldName, out valueViewModel);
			if(valueViewModel != null) {
				AxisPoint columnPoint = dataBuilder.CorrectAxisPoint(args.ColumnAxisPoint);
				AxisPoint rowPoint = dataBuilder.CorrectAxisPoint(args.RowAxisPoint);
				dataBuilder.FillStyleSettings(args, columnPoint, rowPoint, valueViewModel.DataId);
			}
		}
		void OnPivotFieldValueCollapseStateChanged(object sender, PivotFieldValueCollapseStateChangedEventArgs e) {
			dataBuilder.OnCollapseStateChanged(e.IsColumn, e.Values, e.Collapse);
		}
		public void Update(PivotDashboardItemViewModel viewModel, ConditionalFormattingModel cfModel, MultiDimensionalData data) {
			UpdateViewModel(viewModel);
			UpdateConditionalFormattingModel(cfModel);
			pivot.ShowColumnGrandTotals = viewModel.ShowColumnGrandTotals;
			pivot.ShowRowGrandTotals = viewModel.ShowRowGrandTotals;
			pivot.ShowColumnTotals = viewModel.ShowColumnTotals;
			pivot.ShowRowTotals = viewModel.ShowRowTotals;
			UpdateData(data, viewModel.MeasureIds);
		}
		public PivotGridThinClientDataBuilder RefreshDataBuilder(MultiDimensionalData mdData, AxisPoint columnAxisPoint, AxisPoint rowAxisPoint, string[] measureIds) {
			dataBuilder = new PivotGridThinClientDataBuilder(mdData, columnAxisPoint, rowAxisPoint, measureIds, cfModel);
			return dataBuilder;
		}
		void UpdateViewModel(PivotDashboardItemViewModel viewModel) {
			this.viewModel = viewModel;
			columnViewModels.Clear();
			Dictionary<string, int> widthCache = new Dictionary<string, int>();
			foreach(IPivotGridField field in pivot.Fields)
				widthCache.Add(field.FieldName, field.Width);
			pivot.BeginUpdate();
			pivot.ClearFields();
			if(viewModel.Columns != null)
				CreatePivotFields(viewModel.Columns, PivotDashboardItemArea.ColumnArea);
			if(viewModel.Rows != null)
				CreatePivotFields(viewModel.Rows, PivotDashboardItemArea.RowArea);
			if(viewModel.Values != null)
				CreatePivotFields(viewModel.Values, PivotDashboardItemArea.DataArea);
			foreach(IPivotGridField field in pivot.Fields) {
				int width;
				if(widthCache.TryGetValue(field.FieldName, out width))
					field.Width = width;
			}
			pivot.EndUpdate();
		}
		void UpdateConditionalFormattingModel(ConditionalFormattingModel cfModel) {
			this.cfModel = cfModel;
		}
		void CreatePivotFields(IList<PivotColumnViewModel> viewModels, PivotDashboardItemArea area) {
			string fieldNamePrefix = GetFieldNamePrefix(area);
			for(int index = 0; index < viewModels.Count; index++) {
				PivotColumnViewModel columnViewModel = viewModels[index];
				string fieldName = String.Format("{0}_{1}", fieldNamePrefix, index);
				pivot.AddField(fieldName, columnViewModel, area);
				columnViewModels[fieldName] = columnViewModel;
			}
		}
		void UpdateData(MultiDimensionalData data, string[] measureIds) {
			this.dataBuilder = new PivotGridThinClientDataBuilder(data, measureIds, cfModel);
			pivot.SetData(dataBuilder.GetClientData());
		}
	}
}
