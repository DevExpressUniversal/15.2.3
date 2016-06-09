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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region PivotAxisType
	public enum PivotAxisType {
		None = DevExpress.XtraSpreadsheet.Model.PivotTableAxis.None,
		Row = DevExpress.XtraSpreadsheet.Model.PivotTableAxis.Row,
		Column = DevExpress.XtraSpreadsheet.Model.PivotTableAxis.Column,
		Page = DevExpress.XtraSpreadsheet.Model.PivotTableAxis.Page,
		Value = DevExpress.XtraSpreadsheet.Model.PivotTableAxis.Value
	}
	#endregion
	#region PivotFieldSortType
	public enum PivotFieldSortType {
		Ascending = DevExpress.XtraSpreadsheet.Model.PivotTableSortTypeField.Ascending,
		Descending = DevExpress.XtraSpreadsheet.Model.PivotTableSortTypeField.Descending,
		Manual = DevExpress.XtraSpreadsheet.Model.PivotTableSortTypeField.Manual
	}
	#endregion
	#region PivotSubtotalsType
	public enum PivotSubtotalType {
		Automatic,
		None,
		Custom
	}
	#endregion
	#region PivotSubtotalsFunctions
	[Flags]
	public enum PivotSubtotalFunctions {
		None = 0,
		Average = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Avg,
		Count = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.CountA,
		CountNumbers = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Count,
		Max = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Max,
		Min = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Min,
		Product = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Product,
		StdDev = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.StdDev,
		StdDevp = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.StdDevP,
		Sum = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Sum,
		Var = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.Var,
		Varp = DevExpress.XtraSpreadsheet.Model.PivotFieldItemType.VarP
	}
	#endregion
	#region PivotFieldBehaviorOptions
	public interface PivotFieldBehaviorOptions {
		bool DragOffAllowed { get; set; }
		bool DragToColumnAxisAllowed { get; set; }
		bool DragToRowAxisAllowed { get; set; }
		bool DragToDataAllowed { get; set; }
		bool DragToPageAllowed { get; set; }
		bool MultipleItemSelectionAllowed { get; set; }
	}
	#endregion
	#region PivotFieldLayout
	public interface PivotFieldLayout {
		bool Compact { get; set; }
		bool Outline { get; set; }
		bool SubtotalOnTop { get; set; }
		bool InsertBlankRowAfterEachItem { get; set; }
		bool RepeatItemLabels { get; set; } 
		bool ShowItemsWithNoData { get; set; } 
	}
	#endregion
	#region PivotField
	public interface PivotField {
		string Name { get; set; }
		string SourceName { get; }
		PivotAxisType Axis { get; }
		PivotItemCollection Items { get; }
		PivotFieldBehaviorOptions Behavior { get; }
		PivotFieldLayout Layout { get; }
		string NumberFormat { get; set; }
		bool IncludeNewItemsInFilter { get; set; }
		PivotFieldSortType SortType { get; set; }
		PivotSubtotalType SubtotalType { get; }
		PivotSubtotalFunctions SubtotalFunctions { get; }
		string SubtotalCaption { get; set; }
		void SetSubtotal(PivotSubtotalFunctions type);
		void SetSubtotalAutomatic();
		void ShowAllItems();
		void ShowSingleItem(int itemIndex);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	partial class NativePivotField : NativeObjectBase, PivotField, PivotFieldBehaviorOptions, PivotFieldLayout {
		#region Fields
		readonly Model.PivotField modelPivotField;
		readonly NativePivotTable nativePivotTable;
		NativePivotItemCollection nativeItems;
		int fieldIndex;
		#endregion
		public NativePivotField(Model.PivotField modelPivotField, NativePivotTable nativePivotTable) {
			Guard.ArgumentNotNull(modelPivotField, "modelPivotField");
			Guard.ArgumentNotNull(nativePivotTable, "nativePivotTable");
			this.modelPivotField = modelPivotField;
			this.nativePivotTable = nativePivotTable;
		}
		#region Properties
		protected internal NativePivotTable ParentTable { get { return nativePivotTable; } }
		protected internal Model.PivotField ModelItem { get { return modelPivotField; } }
		protected internal Model.PivotTable ModelPivotTable { get { return nativePivotTable.ModelItem; } }
		protected internal int FieldIndex { get { return fieldIndex; } set { fieldIndex = value; } } 
		protected internal Model.IPivotCacheField CacheField { get { return ModelPivotTable.Cache.CacheFields[fieldIndex]; } }
		ApiErrorHandler ErrorHandler { get { return ApiErrorHandler.Instance; } }
		#region PivotField Members
		public string Name {
			get {
				CheckValid();
				return ModelPivotTable.GetFieldCaption(fieldIndex);
			}
			set {
				CheckValid();
				Model.PivotFieldRenameCommand command = new Model.PivotFieldRenameCommand(modelPivotField, value, ErrorHandler);
				command.Execute();
			}
		}
		public string SourceName {
			get {
				CheckValid();
				return CacheField.Name;
			}
		}
		public PivotAxisType Axis {
			get {
				CheckValid();
				PivotAxisType axisType = (PivotAxisType)modelPivotField.Axis;
				if (axisType != PivotAxisType.Column && axisType != PivotAxisType.Row && axisType != PivotAxisType.Page) {
					foreach (Model.PivotDataField dataField in ModelPivotTable.DataFields)
						if (dataField.FieldIndex == FieldIndex)
							return PivotAxisType.Value;
					return PivotAxisType.None;
				}
				return axisType;
			}
		}
		public PivotItemCollection Items {
			get {
				CheckValid();
				if (nativeItems == null) {
					if (ModelItem.Items.Count == 0)
						throw new InvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeBuiltFromEmptyCache));
					nativeItems = new NativePivotItemCollection(this);
				}
				return nativeItems;
			}
		}
		#region PivotFieldBehaviorOptions Members
		public PivotFieldBehaviorOptions Behavior { get { return this; } }
		#region DragOffAllowed
		bool PivotFieldBehaviorOptions.DragOffAllowed {
			get {
				CheckValid();
				return modelPivotField.DragOff;
			}
			set {
				CheckValid();
				modelPivotField.DragOff = value;
			}
		}
		#endregion
		#region DragToColumnAxisAllowed
		bool PivotFieldBehaviorOptions.DragToColumnAxisAllowed {
			get {
				CheckValid();
				return modelPivotField.DragToCol;
			}
			set {
				CheckValid();
				modelPivotField.DragToCol = value;
			}
		}
		#endregion
		#region DragToRowAxisAllowed
		bool PivotFieldBehaviorOptions.DragToRowAxisAllowed {
			get {
				CheckValid();
				return modelPivotField.DragToRow;
			}
			set {
				CheckValid();
				modelPivotField.DragToRow = value;
			}
		}
		#endregion
		#region DragToDataAllowed
		bool PivotFieldBehaviorOptions.DragToDataAllowed {
			get {
				CheckValid();
				return modelPivotField.DragToData;
			}
			set {
				CheckValid();
				modelPivotField.DragToData = value;
			}
		}
		#endregion
		#region DragToPageAllowed
		bool PivotFieldBehaviorOptions.DragToPageAllowed {
			get {
				CheckValid();
				return modelPivotField.DragToPage;
			}
			set {
				CheckValid();
				modelPivotField.DragToPage = value;
			}
		}
		#endregion
		#region MultipleItemSelectionAllowed
		bool PivotFieldBehaviorOptions.MultipleItemSelectionAllowed {
			get {
				CheckValid();
				return modelPivotField.MultipleItemSelectionAllowed;
			}
			set {
				CheckValid();
				if (modelPivotField.MultipleItemSelectionAllowed == value)
					return;
				ModelPivotTable.DocumentModel.BeginUpdate();
				try {
					modelPivotField.MultipleItemSelectionAllowed = value;
					if (value) {
						Model.TurnOffShowSingleItemCommand command = new Model.TurnOffShowSingleItemCommand(modelPivotField, ErrorHandler);
						command.Execute();
					}
				}
				finally {
					ModelPivotTable.DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
		#endregion
		#region PivotFieldLayout Members
		public PivotFieldLayout Layout { get { return this; } }
		#region Compact
		bool PivotFieldLayout.Compact {
			get {
				CheckValid();
				return modelPivotField.Compact;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.Compact = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region Outline
		bool PivotFieldLayout.Outline {
			get {
				CheckValid();
				return modelPivotField.Outline;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.Outline = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region SubtotalOnTop
		bool PivotFieldLayout.SubtotalOnTop {
			get {
				CheckValid();
				return modelPivotField.SubtotalTop;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.SubtotalTop = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region InsertBlankRowAfterEachItem
		bool PivotFieldLayout.InsertBlankRowAfterEachItem {
			get {
				CheckValid();
				return modelPivotField.InsertBlankRow;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.InsertBlankRow = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region RepeatItemLabels
		bool PivotFieldLayout.RepeatItemLabels {
			get {
				CheckValid();
				return modelPivotField.FillDownLabels;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.FillDownLabels = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowItemsWithNoData
		bool PivotFieldLayout.ShowItemsWithNoData {
			get {
				CheckValid();
				return modelPivotField.ShowItemsWithNoData;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.ShowItemsWithNoData = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#endregion
		#region NumberFormat
		public string NumberFormat {
			get {
				CheckValid();
				return modelPivotField.FormatString;
			}
			set {
				CheckValid();
				modelPivotField.SetFormatString(value, ErrorHandler);
			}
		}
		#endregion
		#region IncludeNewItemsInFilter
		public bool IncludeNewItemsInFilter {
			get {
				CheckValid();
				return modelPivotField.IncludeNewItemsInFilter;
			}
			set {
				CheckValid();
				modelPivotField.IncludeNewItemsInFilter = value;
			}
		}
		#endregion
		#region SortType
		public PivotFieldSortType SortType {
			get {
				CheckValid();
				return (PivotFieldSortType)modelPivotField.SortType;
			}
			set {
				CheckValid();
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.SortType = (Model.PivotTableSortTypeField)value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region SubtotalType
		public PivotSubtotalType SubtotalType {
			get {
				CheckValid();
				if (modelPivotField.DefaultSubtotal)
					return PivotSubtotalType.Automatic;
				else if (SubtotalFunctions == PivotSubtotalFunctions.None)
					return PivotSubtotalType.None;
				return PivotSubtotalType.Custom;
			}
		}
		#endregion
		#region SubtotalFunctions
		public PivotSubtotalFunctions SubtotalFunctions {
			get {
				CheckValid();
				Model.PivotFieldItemType subtotalType = modelPivotField.Subtotal;
				if (subtotalType == Model.PivotFieldItemType.DefaultValue) {
					if (CacheField.SharedItems.ContainsOnlyNumbers)
						return PivotSubtotalFunctions.Sum;
					return PivotSubtotalFunctions.Count;
				}
				return (PivotSubtotalFunctions)subtotalType;
			}
		}
		#endregion
		#region SubtotalCaption
		public string SubtotalCaption {
			get {
				CheckValid();
				return modelPivotField.SubtotalCaption;
			}
			set {
				CheckValid();
				if (StringExtensions.CompareInvariantCultureIgnoreCase(SubtotalCaption, value) == 0)
					return;
				ModelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotField.SubtotalCaption = value;
				}
				finally {
					ModelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#endregion
		#endregion
		public void SetSubtotal(PivotSubtotalFunctions type) {
			CheckValid();
			modelPivotField.SetSubtotal((Model.PivotFieldItemType)type, ErrorHandler);
		}
		public void SetSubtotalAutomatic() {
			CheckValid();
			modelPivotField.SetSubtotal(Model.PivotFieldItemType.DefaultValue, ErrorHandler);
		}
		public void ShowAllItems() {
			CheckValid();
			modelPivotField.UnHideItems(ErrorHandler);
		}
		public void ShowSingleItem(int itemIndex) {
			CheckValid();
			ApiValueChecker.CheckIndex(itemIndex, Items.Count - 1);
			Model.ShowSinglePivotItemCommand command = new Model.ShowSinglePivotItemCommand(modelPivotField, itemIndex, ErrorHandler);
			command.Execute();
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeItems != null)
				nativeItems.IsValid = value;
		}
	}
}
