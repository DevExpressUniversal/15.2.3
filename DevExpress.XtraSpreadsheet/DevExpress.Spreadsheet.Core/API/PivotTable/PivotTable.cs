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
	#region PivotViewOptions
	public interface PivotViewOptions {
		string ColumnHeaderCaption { get; set; }
		string DataCaption { get; set; }
		string ErrorCaption { get; set; }
		string GrandTotalCaption { get; set; }
		string MissingCaption { get; set; }
		string RowHeaderCaption { get; set; }
		bool ShowDrillIndicators { get; set; }
		bool ShowError { get; set; }
		bool ShowFieldHeaders { get; set; }
		bool ShowMissing { get; set; }
		bool ShowMultipleLabels { get; set; }
		string AltTextTitle { get; set; }
		string AltTextDescription { get; set; }
	}
	#endregion
	#region PivotBehaviorOptions
	public interface PivotBehaviorOptions {
		bool AllowMultipleFieldFilters { get; set; }
		bool EnableFieldList { get; set; }
		bool AutoFitColumns { get; set; }
	}
	#endregion
	#region PivotTable
	public interface PivotTable {
		string Name { get; set; }
		PivotCache Cache { get; }
		PivotLocation Location { get; }
		PivotFilterCollection Filters { get; }
		PivotFieldCollection Fields { get; }
		PivotPageFieldCollection PageFields { get; }
		PivotFieldReferenceCollection ColumnFields { get; }
		PivotFieldReferenceCollection RowFields { get; }
		PivotDataFieldCollection DataFields { get; }
		PivotLayout Layout { get; }
		PivotViewOptions View { get; }
		PivotBehaviorOptions Behavior { get; }
		TableStyle Style { get; set; }
		bool ShowColumnHeaders { get; set; }
		bool ShowRowHeaders { get; set; }
		bool BandedColumns { get; set; }
		bool BandedRows { get; set; }
		void BeginUpdate();
		void EndUpdate();
		void ChangeDataSource(Range sourceRange);
		void ChangeDataSource(PivotCache cache);
		void Clear();
		PivotTable MoveTo(Range location);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using DevExpress.Spreadsheet;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Office.Utils;
	#region NativePivotTable
	partial class NativePivotTable : NativeObjectBase, PivotTable, PivotViewOptions,  PivotBehaviorOptions, PivotLocation, PivotLayout {
		#region Fields
		readonly Model.PivotTable modelPivotTable;
		readonly NativeWorksheet nativeWorksheet;
		NativePivotCache nativeCache;
		NativePivotFieldCollection nativeFields;
		NativePivotPageFieldCollection nativePageFields;
		NativePivotFieldReferenceCollection nativeColumnFields;
		NativePivotFieldReferenceCollection nativeRowFields;
		NativePivotDataFieldCollection nativeDataFields;
		NativePivotFilterCollection nativeFilters;
		#endregion
		public NativePivotTable(Model.PivotTable modelPivotTable, NativeWorksheet nativeWorksheet, NativePivotCache nativeCache) {
			Guard.ArgumentNotNull(modelPivotTable, "modelPivotTable");
			Guard.ArgumentNotNull(modelPivotTable.Location, "modelLocation");
			Guard.ArgumentNotNull(modelPivotTable.StyleInfo, "modelStyleInfo");
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			Guard.ArgumentNotNull(nativeCache, "nativeCache");
			this.modelPivotTable = modelPivotTable;
			this.nativeWorksheet = nativeWorksheet;
			this.nativeCache = nativeCache;
			modelPivotTable.OnCacheChanged += OnCacheChanged;
		}
		#region Properties
		protected internal IWorkbook Workbook { get { return nativeWorksheet.Workbook; } }
		protected internal Model.PivotTable ModelItem { get { return modelPivotTable; } }
		protected Model.DocumentModel ModelWorkbook { get { return modelPivotTable.Worksheet.Workbook; } }
		protected Model.PivotTableStyleInfo ModelStyleInfo { get { return modelPivotTable.StyleInfo; } }
		ApiErrorHandler ErrorHandler { get { return ApiErrorHandler.Instance; } }
		public PivotCache Cache {
			get {
				CheckValid();
				return nativeCache;
			}
		}
		#region Name
		public string Name {
			get {
				CheckValid();
				return modelPivotTable.Name;
			}
			set {
				CheckValid();
				Model.PivotTableRenameCommand command = new Model.PivotTableRenameCommand(modelPivotTable, value, ErrorHandler);
				command.Execute();
			}
		}
		#endregion
		public PivotFieldCollection Fields {
			get {
				CheckValid();
				if (nativeFields == null)
					nativeFields = new NativePivotFieldCollection(this);
				return nativeFields;
			}
		}
		public PivotPageFieldCollection PageFields {
			get {
				CheckValid();
				if (nativePageFields == null)
					nativePageFields = new NativePivotPageFieldCollection(this, modelPivotTable.PageFields);
				return nativePageFields;
			}
		}
		public PivotFieldReferenceCollection ColumnFields {
			get {
				CheckValid();
				if (nativeColumnFields == null)
					nativeColumnFields = new NativePivotFieldReferenceCollection(this, modelPivotTable.ColumnFields, false);
				return nativeColumnFields;
			}
		}
		public PivotFieldReferenceCollection RowFields {
			get {
				CheckValid();
				if (nativeRowFields == null)
					nativeRowFields = new NativePivotFieldReferenceCollection(this, modelPivotTable.RowFields, true);
				return nativeRowFields;
			}
		}
		public PivotDataFieldCollection DataFields {
			get {
				CheckValid();
				if (nativeDataFields == null)
					nativeDataFields = new NativePivotDataFieldCollection(this, modelPivotTable.DataFields);
				return nativeDataFields;
			}
		}
		public PivotFilterCollection Filters {
			get {
				CheckValid();
				if (nativeFilters == null)
					nativeFilters = new NativePivotFilterCollection(this);
				return nativeFilters;
			}
		}
		#region PivotViewOptions Members
		public PivotViewOptions View { get { return this; } }
		#region ColumnHeaderCaption
		string PivotViewOptions.ColumnHeaderCaption {
			get {
				CheckValid();
				return modelPivotTable.ColHeaderCaption;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ColHeaderCaption = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region DataCaption
		string PivotViewOptions.DataCaption {
			get {
				CheckValid();
				return modelPivotTable.DataCaption;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.DataCaption = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ErrorCaption
		string PivotViewOptions.ErrorCaption {
			get {
				CheckValid();
				return modelPivotTable.ErrorCaption;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ErrorCaption = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region GrandTotalCaption
		string PivotViewOptions.GrandTotalCaption {
			get {
				CheckValid();
				return modelPivotTable.GrandTotalCaption;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.GrandTotalCaption = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region MissingCaption
		string PivotViewOptions.MissingCaption {
			get {
				CheckValid();
				return modelPivotTable.MissingCaption;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.MissingCaption = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region RowHeaderCaption
		string PivotViewOptions.RowHeaderCaption {
			get {
				CheckValid();
				return modelPivotTable.RowHeaderCaption;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.RowHeaderCaption = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowDrillIndicators
		bool PivotViewOptions.ShowDrillIndicators {
			get {
				CheckValid();
				return modelPivotTable.ShowDrill;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ShowDrill = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowError
		bool PivotViewOptions.ShowError {
			get {
				CheckValid();
				return modelPivotTable.ShowError;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ShowError = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowFieldHeaders
		bool PivotViewOptions.ShowFieldHeaders {
			get {
				CheckValid();
				return modelPivotTable.ShowHeaders;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ShowHeaders = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowMissing
		bool PivotViewOptions.ShowMissing {
			get {
				CheckValid();
				return modelPivotTable.ShowMissing;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ShowMissing = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowMultipleLabels
		bool PivotViewOptions.ShowMultipleLabels {
			get {
				CheckValid();
				return modelPivotTable.ShowMultipleLabel;
			}
			set {
				CheckValid();
				modelPivotTable.ShowMultipleLabel = value;
			}
		}
		#endregion
		#region AltTextTitle
		string PivotViewOptions.AltTextTitle {
			get {
				CheckValid();
				return modelPivotTable.AltText;
			}
			set {
				CheckValid();
				modelPivotTable.AltText = value;
			}
		}
		#endregion
		#region AltTextDescription
		string PivotViewOptions.AltTextDescription {
			get {
				CheckValid();
				return modelPivotTable.AltTextSummary;
			}
			set {
				CheckValid();
				modelPivotTable.AltTextSummary = value;
			}
		}
		#endregion
		#endregion
		#region PivotBehaviorOptions Members
		public PivotBehaviorOptions Behavior { get { return this; } }
		#region AllowMultipleFieldFilters
		bool PivotBehaviorOptions.AllowMultipleFieldFilters {
			get {
				CheckValid();
				return modelPivotTable.MultipleFieldFilters;
			}
			set {
				CheckValid();
				if (!value) {
					if (modelPivotTable.MultipleFieldFilters != value) {
						Model.TurnOffPivotMultipleFiltersCommand command = new Model.TurnOffPivotMultipleFiltersCommand(modelPivotTable, ErrorHandler);
						command.Execute();
					}
				}
				else
					modelPivotTable.MultipleFieldFilters = true;
			}
		}
		#endregion
		#region EnableFieldList
		bool PivotBehaviorOptions.EnableFieldList {
			get {
				CheckValid();
				return !modelPivotTable.DisableFieldList;
			}
			set {
				CheckValid();
				modelPivotTable.DisableFieldList = !value;
			}
		}
		#endregion
		#region AutoFitColumns
		bool PivotBehaviorOptions.AutoFitColumns {
			get {
				CheckValid();
				return modelPivotTable.UseAutoFormatting;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.UseAutoFormatting = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#endregion
		#region Style
		public TableStyle Style {
			get {
				CheckValid();
				return new NativeTableStyle(nativeWorksheet.NativeWorkbook, ModelWorkbook.StyleSheet.TableStyles[ModelStyleInfo.StyleName]);
			}
			set {
				CheckValid();
				if (!value.IsPivotStyle)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorInvalidTableStyleType);
				ModelStyleInfo.StyleName = value.Name;
			}
		}
		#endregion
		#region ShowColumnHeaders
		public bool ShowColumnHeaders {
			get {
				CheckValid();
				return ModelStyleInfo.ShowColumnHeaders;
			}
			set {
				CheckValid();
				ModelStyleInfo.ShowColumnHeaders = value;
			}
		}
		#endregion
		#region ShowRowHeaders
		public bool ShowRowHeaders {
			get {
				CheckValid();
				return ModelStyleInfo.ShowRowHeaders;
			}
			set {
				CheckValid();
				ModelStyleInfo.ShowRowHeaders = value;
			}
		}
		#endregion
		#region BandedColumns
		public bool BandedColumns {
			get {
				CheckValid();
				return ModelStyleInfo.ShowColumnStripes;
			}
			set {
				CheckValid();
				ModelStyleInfo.ShowColumnStripes = value;
			}
		}
		#endregion
		#region BandedRows
		public bool BandedRows {
			get {
				CheckValid();
				return ModelStyleInfo.ShowRowStripes;
			}
			set {
				CheckValid();
				ModelStyleInfo.ShowRowStripes = value;
			}
		}
		#endregion
		#endregion
		public void BeginUpdate() {
			CheckValid();
			modelPivotTable.BeginTransaction(ApiErrorHandler.Instance);
		}
		public void EndUpdate() {
			CheckValid();
			modelPivotTable.EndTransaction();
		}
		public void ChangeDataSource(Range sourceRange) {
			CheckValid();
			modelPivotTable.ChangeDataSource(GetModelRange(sourceRange), ErrorHandler);
		}
		public void ChangeDataSource(PivotCache cache) {
			CheckValid();
			if (cache == null)
				HandleError(Model.ModelErrorType.UsingInvalidObject);
			modelPivotTable.ChangeDataSource(GetModelRange(cache.SourceRange), ErrorHandler);
		}
		public void Clear() {
			CheckValid();
			modelPivotTable.Clear(ErrorHandler);
		}
		public PivotTable MoveTo(Range location) {
			CheckValid();
			Model.MovePivotCommand command = new Model.MovePivotCommand(modelPivotTable, GetModelRange(location), ErrorHandler);
			command.Execute();
			if (IsValid)
				return this;
			return location.Worksheet.PivotTables.Last;
		}
		Model.CellRange GetModelRange(Range range) {
			if (range == null)
				HandleError(Model.ModelErrorType.ErrorInvalidRange);
			Model.CellRangeBase modelRange = ((NativeWorksheet)range.Worksheet).GetModelRange(range);
			if (modelRange == null)
				HandleError(Model.ModelErrorType.ErrorInvalidRange);
			if (modelRange.RangeType == Model.CellRangeType.UnionRange)
				HandleError(Model.ModelErrorType.UnionRangeNotAllowed);
			return (Model.CellRange)modelRange;
		}
		void HandleError(Model.ModelErrorType errorType) {
			ErrorHandler.HandleError(errorType);
		}
		void OnCacheChanged(object sender, EventArgs e) {
			Model.CacheChangedEventArgs args = e as Model.CacheChangedEventArgs;
			if (args != null)
				nativeCache = ((NativePivotCacheCollection)Workbook.PivotCaches).Find(args.Cache);
		}
		internal bool AddFieldToKeyFields(int fieldIndex, PivotAxisType axisType) {
			return modelPivotTable.AddFieldToKeyFields(fieldIndex, (Model.PivotTableAxis)axisType, ErrorHandler);
		}
		internal bool InsertFieldToKeyFields(int fieldIndex, PivotAxisType axisType, int insertIndex) {
			return modelPivotTable.InsertFieldToKeyFields(fieldIndex, (Model.PivotTableAxis)axisType, insertIndex, ErrorHandler);
		}
		internal bool AddDataField(int fieldIndex) {
			return modelPivotTable.AddDataField(fieldIndex, ErrorHandler);
		}
		internal bool InsertDataField(int fieldIndex, int insertIndex) {
			return modelPivotTable.InsertDataField(fieldIndex, insertIndex, ErrorHandler);
		}
		internal void RemoveKeyField(int fieldRefenceIndex, PivotAxisType axisType) {
			modelPivotTable.RemoveKeyField(fieldRefenceIndex, (Model.PivotTableAxis)axisType, ErrorHandler);
		}
		internal void ClearKeyFields(PivotAxisType axisType) {
			modelPivotTable.ClearKeyFields((Model.PivotTableAxis)axisType, ErrorHandler);
		}
		#region SetIsValid
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (!value)
				modelPivotTable.OnCacheChanged -= OnCacheChanged;
			if (nativeFields != null)
				nativeFields.IsValid = value;
			if (nativePageFields != null)
				nativePageFields.IsValid = value;
			if (nativeColumnFields != null)
				nativeColumnFields.IsValid = value;
			if (nativeRowFields != null)
				nativeRowFields.IsValid = value;
			if (nativeDataFields != null)
				nativeDataFields.IsValid = value;
			if (nativeFilters != null)
				nativeFilters.IsValid = value;
		}
		#endregion
	}
	#endregion
}
