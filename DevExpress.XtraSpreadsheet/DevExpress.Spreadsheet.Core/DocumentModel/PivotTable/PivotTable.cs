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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTableCache
	public class PivotTableCache : UniqueItemsCache<PivotTableInfo> {
		public PivotTableCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override PivotTableInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new PivotTableInfo();
		}
	}
	#endregion
	#region PivotTableInfo
	public class PivotTableInfo : ICloneable<PivotTableInfo>, ISupportsCopyFrom<PivotTableInfo>, ISupportsSizeOf {
		#region Fields
		byte createdVersion;
		byte minRefreshableVersion;
		byte updatedVersion;
		int autoFormatId;
		int chartFormat;
		int dataPosition;
		int indent;
		int pageWrap;
		uint packedValues;
		uint packedValues1;
		const uint maskAsteriskTotals = 1;
		const uint maskDataOnRows = 2;
		const uint maskDisableFieldList = 4;
		const uint maskEditData = 8;
		const uint maskFieldListSortAscending = 0x10;
		const uint maskFieldPrintTitles = 0x20;
		const uint maskGridDropZones = 0x40;
		const uint maskItemPrintTitles = 0x80;
		const uint maskMdxSubqueries = 0x100;
		const uint maskMergeItem = 0x200;
		const uint maskOutline = 0x400;
		const uint maskOutlineData = 0x800;
		const uint maskPageOverThenDown = 0x1000;
		const uint maskPrintDrill = 0x2000;
		const uint maskPublished = 0x4000;
		const uint maskShowEmptyColumn = 0x8000;
		const uint maskShowEmptyRow = 0x10000;
		const uint maskShowError = 0x20000;
		const uint maskSubtotalHiddenItems = 0x40000;
		const uint maskUseAutoFormatting = 0x80000;
		const uint maskApplyAlignmentFormats = 0x100000;
		const uint maskApplyBorderFormats = 0x200000;
		const uint maskApplyFontFormats = 0x400000;
		const uint maskApplyNumberFormats = 0x800000;
		const uint maskApplyPatternFormats = 0x1000000;
		const uint maskApplyWidthHeightFormats = 0x2000000;
		const uint maskHasAlignmentFormats = 0x4000000;
		const uint maskHasBorderFormats = 0x8000000;
		const uint maskHasFontFormats = 0x10000000;
		const uint maskHasNumberFormats = 0x20000000;
		const uint maskHasPatternFormats = 0x40000000;
		const uint maskHasWidthHeightFormats = 0x80000000;
		const uint maskColumnGrandTotals = 1;
		const uint maskCompact = 2;
		const uint maskCompactData = 4;
		const uint maskCustomListSort = 8;
		const uint maskEnableDrill = 0x10;
		const uint maskEnableFieldProperties = 0x20;
		const uint maskEnableWizard = 0x40;
		const uint maskImmersive = 0x80;
		const uint maskMultipleFieldFilters = 0x100;
		const uint maskPreserveFormatting = 0x200;
		const uint maskRowGrandTotals = 0x400;
		const uint maskShowCalcMbrs = 0x800;
		const uint maskShowDataDropDown = 0x1000;
		const uint maskShowDataTips = 0x2000;
		const uint maskShowDrill = 0x4000;
		const uint maskShowDropZones = 0x8000;
		const uint maskShowHeaders = 0x10000;
		const uint maskShowItems = 0x20000;
		const uint maskShowMemberPropertyTips = 0x40000;
		const uint maskShowMissing = 0x80000;
		const uint maskShowMultipleLabel = 0x100000;
		const uint maskVisualTotals = 0x200000;
		const uint maskHideValuesRow = 0x400000;
		#endregion
		public PivotTableInfo() {
			indent = 1;
			pageWrap = 0;
			chartFormat = 0;
			createdVersion = 0;
			updatedVersion = 0;
			minRefreshableVersion = 0;
			packedValues = 0;
			packedValues1 = 0xFFFFFFFF;
			ApplyWidthHeightFormats = true;
			ShowValuesRow = true;
			dataPosition = -1;
			Outline = true;
			OutlineData = true;
			MultipleFieldFilters = false;
			UseAutoFormatting = true;
		}
		#region Properties
		public byte CreatedVersion { get { return createdVersion; } set { createdVersion = value; } }
		public byte MinRefreshableVersion { get { return minRefreshableVersion; } set { minRefreshableVersion = value; } }
		public byte UpdatedVersion { get { return updatedVersion; } set { updatedVersion = value; } }
		public int AutoFormatId { get { return autoFormatId; } set { autoFormatId = value; } }
		public int ChartFormat { get { return chartFormat; } set { chartFormat = value; } }
		public int DataPosition { get { return dataPosition; } set { dataPosition = value; } }
		public int Indent { get { return indent; } set { indent = value; } }
		public int PageWrap { get { return pageWrap; } set { pageWrap = value; } }
		public bool AsteriskTotals {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskAsteriskTotals); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskAsteriskTotals, value); }
		}
		public bool DataOnRows {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDataOnRows); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDataOnRows, value); }
		}
		public bool DisableFieldList {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDisableFieldList); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDisableFieldList, value); }
		}
		public bool EditData {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskEditData); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskEditData, value); }
		}
		public bool FieldListSortAscending {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskFieldListSortAscending); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskFieldListSortAscending, value); }
		}
		public bool FieldPrintTitles {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskFieldPrintTitles); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskFieldPrintTitles, value); }
		}
		public bool GridDropZones {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskGridDropZones); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskGridDropZones, value); }
		}
		public bool ItemPrintTitles {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskItemPrintTitles); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskItemPrintTitles, value); }
		}
		public bool MdxSubqueries {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskMdxSubqueries); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskMdxSubqueries, value); }
		}
		public bool MergeItem {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskMergeItem); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskMergeItem, value); }
		}
		public bool Outline {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskOutline); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskOutline, value); }
		}
		public bool OutlineData {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskOutlineData); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskOutlineData, value); }
		}
		public bool PageOverThenDown {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskPageOverThenDown); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskPageOverThenDown, value); }
		}
		public bool PrintDrill {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskPrintDrill); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskPrintDrill, value); }
		}
		public bool Published {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskPublished); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskPublished, value); }
		}
		public bool ShowEmptyColumn {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowEmptyColumn); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowEmptyColumn, value); }
		}
		public bool ShowEmptyRow {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowEmptyRow); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowEmptyRow, value); }
		}
		public bool ShowError {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowError); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowError, value); }
		}
		public bool SubtotalHiddenItems {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskSubtotalHiddenItems); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskSubtotalHiddenItems, value); }
		}
		public bool UseAutoFormatting {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskUseAutoFormatting); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskUseAutoFormatting, value); }
		}
		public bool ColumnGrandTotals {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskColumnGrandTotals); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskColumnGrandTotals, value); }
		}
		public bool Compact {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskCompact); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskCompact, value); }
		}
		public bool CompactData {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskCompactData); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskCompactData, value); }
		}
		public bool CustomListSort {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskCustomListSort); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskCustomListSort, value); }
		}
		public bool EnableDrill {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskEnableDrill); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskEnableDrill, value); }
		}
		public bool EnableFieldProperties {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskEnableFieldProperties); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskEnableFieldProperties, value); }
		}
		public bool EnableWizard {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskEnableWizard); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskEnableWizard, value); }
		}
		public bool Immersive {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskImmersive); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskImmersive, value); }
		}
		public bool MultipleFieldFilters {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskMultipleFieldFilters); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskMultipleFieldFilters, value); }
		}
		public bool PreserveFormatting {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskPreserveFormatting); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskPreserveFormatting, value); }
		}
		public bool RowGrandTotals {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskRowGrandTotals); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskRowGrandTotals, value); }
		}
		public bool ShowCalcMbrs {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowCalcMbrs); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowCalcMbrs, value); }
		}
		public bool ShowDataDropDown {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowDataDropDown); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowDataDropDown, value); }
		}
		public bool ShowDataTips {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowDataTips); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowDataTips, value); }
		}
		public bool ShowDrill {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowDrill); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowDrill, value); }
		}
		public bool ShowDropZones {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowDropZones); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowDropZones, value); }
		}
		public bool ShowHeaders {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowHeaders); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowHeaders, value); }
		}
		public bool ShowItems {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowItems); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowItems, value); }
		}
		public bool ShowMemberPropertyTips {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowMemberPropertyTips); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowMemberPropertyTips, value); }
		}
		public bool ShowMissing {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowMissing); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowMissing, value); }
		}
		public bool ShowMultipleLabel {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskShowMultipleLabel); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskShowMultipleLabel, value); }
		}
		public bool VisualTotals {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskVisualTotals); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskVisualTotals, value); }
		}
		public bool ApplyAlignmentFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyAlignmentFormats); }
			set {
				PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyAlignmentFormats, value);
				this.HasAlignmentFormats = true;
			}
		}
		public bool HasAlignmentFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasAlignmentFormats); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasAlignmentFormats, value); }
		}
		public bool ApplyBorderFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyBorderFormats); }
			set {
				PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyBorderFormats, value);
				this.HasBorderFormats = true;
			}
		}
		public bool HasBorderFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasBorderFormats); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasBorderFormats, value); }
		}
		public bool ApplyFontFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyFontFormats); }
			set {
				PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyFontFormats, value);
				this.HasFontFormats = true;
			}
		}
		public bool HasFontFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasFontFormats); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasFontFormats, value); }
		}
		public bool ApplyNumberFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyNumberFormats); }
			set {
				PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyNumberFormats, value);
				this.HasNumberFormats = true;
			}
		}
		public bool HasNumberFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasNumberFormats); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasNumberFormats, value); }
		}
		public bool ApplyPatternFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyPatternFormats); }
			set {
				PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyPatternFormats, value);
				this.HasPatternFormats = true;
			}
		}
		public bool HasPatternFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasPatternFormats); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasPatternFormats, value); }
		}
		public bool ApplyWidthHeightFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyWidthHeightFormats); }
			set {
				PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyWidthHeightFormats, value);
				this.HasWidthHeightFormats = true;
			}
		}
		public bool HasWidthHeightFormats {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasWidthHeightFormats); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasWidthHeightFormats, value); }
		}
		public bool ShowValuesRow {
			get { return PackedValues.GetBoolBitValue(this.packedValues1, maskHideValuesRow); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues1, maskHideValuesRow, value); }
		}
		#endregion
		#region ICloneable<PivotTableInfo> Members
		public PivotTableInfo Clone() {
			PivotTableInfo result = new PivotTableInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PivotTableInfo> Members
		public void CopyFrom(PivotTableInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.createdVersion = value.createdVersion;
			this.minRefreshableVersion = value.minRefreshableVersion;
			this.updatedVersion = value.updatedVersion;
			this.autoFormatId = value.autoFormatId;
			this.chartFormat = value.chartFormat;
			this.dataPosition = value.dataPosition;
			this.indent = value.indent;
			this.pageWrap = value.pageWrap;
			this.packedValues = value.packedValues;
			this.packedValues1 = value.packedValues1;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			PivotTableInfo info = obj as PivotTableInfo;
			if (info == null)
				return false;
			return this.createdVersion == info.createdVersion &&
					this.minRefreshableVersion == info.minRefreshableVersion &&
					this.updatedVersion == info.updatedVersion &&
					this.autoFormatId == info.autoFormatId &&
					this.chartFormat == info.chartFormat &&
					this.dataPosition == info.dataPosition &&
					this.indent == info.indent &&
					this.pageWrap == info.pageWrap &&
					this.packedValues == info.packedValues &&
					this.packedValues1 == info.packedValues1;
		}
		public override int GetHashCode() {
			CombinedHashCode hashCode = new CombinedHashCode();
			hashCode.AddInt(this.createdVersion);
			hashCode.AddInt(this.minRefreshableVersion);
			hashCode.AddInt(this.updatedVersion);
			hashCode.AddInt(this.autoFormatId);
			hashCode.AddInt(this.chartFormat);
			hashCode.AddInt(this.dataPosition);
			hashCode.AddInt(this.indent);
			hashCode.AddInt(this.pageWrap);
			hashCode.AddInt((int)this.packedValues);
			hashCode.AddInt((int)this.packedValues1);
			return hashCode.CombinedHash32;
		}
	}
	#endregion
	#region PivotTable
	public class PivotTable : SpreadsheetUndoableIndexBasedObject<PivotTableInfo>, ITableBase, ISpreadsheetNamedObject, ISpreadsheetRangeObject {
		#region Static
		public static PivotTable Create(CellRange location, IPivotCacheSource source, IErrorHandler errorHandler) {
			return Create(location, source, string.Empty, errorHandler);
		}
		public static PivotTable Create(CellRange location, IPivotCacheSource source, string name, IErrorHandler errorHandler) {
			PivotCreateCommand command = new PivotCreateCommand(errorHandler, source, location, name);
			command.Execute();
			return command.Result as PivotTable;
		}
		public static PivotTable Create(CellRange location, CellRange sourceRange, IErrorHandler errorHandler) { 
			return Create(location, sourceRange, string.Empty, errorHandler);
		}
		public static PivotTable Create(CellRange location, CellRange sourceRange, string name, IErrorHandler errorHandler) { 
			Worksheet sheet = location.Worksheet as Worksheet;
			IPivotCacheSource source = PivotCacheSourceWorksheet.CreateInstance(sourceRange.ToString(true), sheet.DataContext);
			return Create(location, source, name, errorHandler);
		}
		#endregion
		#region Fields
		public const int ValuesFieldFakeIndex = -2;
		public const int Excel2013Version = 5;
		readonly DocumentModel documentModel;
		PivotCache cache;
		readonly PivotTableLocation location;
		readonly PivotFilterCollection filters;
		readonly PivotFieldCollection fields;
		readonly PivotPageFieldCollection pageFields;
		readonly PivotTableColumnRowFieldIndices columnFields;
		readonly PivotTableColumnRowFieldIndices rowFields;
		readonly PivotDataFieldCollection dataFields;
		readonly PivotFormatCollection formats;
		readonly PivotConditionalFormatCollection conditionalFormats;
		readonly PivotChartFormatsCollection chartFormats;
		readonly PivotHierarchyCollection hierarchies;
		readonly UndoableCollection<int> rowHierarchiesUsage;
		readonly UndoableCollection<int> colHierarchiesUsage;
		readonly PivotCalculationInfo calculationInfo;
		readonly PivotTableStyleInfo styleInfo;
		readonly PivotLayoutCellCache layoutCellCache;
		string name;
		string dataCaption;
		string colHeaderCaption;
		string errorCaption;
		string grandTotalCaption;
		string missingCaption;
		string pageStyle;
		string pivotTableStyle;
		string rowHeaderCaption;
		string tag;
		string vacatedStyle;
		string altText;
		string altTextSummary;
		string defaultDataCaption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotDefaultDataCaption);
		#endregion
		#region Constructors
		protected internal PivotTable(DocumentModel documentModel, string name, PivotTableLocation location)
			: base(documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(location, "Location");
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			this.name = name;
			this.documentModel = documentModel;
			this.location = location;
			this.fields = new PivotFieldCollection(documentModel);
			this.formats = new PivotFormatCollection(documentModel);
			this.filters = new PivotFilterCollection(documentModel);
			this.dataFields = new PivotDataFieldCollection(documentModel);
			this.pageFields = new PivotPageFieldCollection(documentModel);
			this.columnFields = new PivotTableColumnRowFieldIndices(documentModel);
			this.rowFields = new PivotTableColumnRowFieldIndices(documentModel);
			this.conditionalFormats = new PivotConditionalFormatCollection(documentModel);
			this.chartFormats = new PivotChartFormatsCollection(documentModel);
			this.hierarchies = new PivotHierarchyCollection(documentModel);
			this.rowHierarchiesUsage = new UndoableCollection<int>(documentModel);
			this.colHierarchiesUsage = new UndoableCollection<int>(documentModel);
			this.calculationInfo = new PivotCalculationInfo(this);
			this.layoutCellCache = new PivotLayoutCellCache(this);
			this.styleInfo = new PivotTableStyleInfo(this);
			this.dataCaption = DefaultDataCaption;
			location.RangeChanged += Location_RangeChanged;
			location.WholeRangeChanged += Location_WholeRangeChanged;
		}
		public PivotTable(DocumentModel documentModel, string name, PivotTableLocation location, PivotCache cache)
			: this(documentModel, name, location) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
		}
		#endregion
		#region Properties
		#region Cache
		public PivotCache Cache { get { return cache; } }
		internal void SetCache(PivotCache value) {
			HistoryHelper.SetValue(DocumentModel, cache, value, SetCacheCore);
		}
		internal void SetCacheCore(PivotCache cache) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
			calculationInfo.InvalidateCalculatedCache();
			RaiseCacheChanged(cache);
		}
		#endregion
		public string DefaultDataCaption { get { return defaultDataCaption; } }
		public PivotTableLocation Location { get { return location; } }
		public CellRange Range { get { return location.Range; } }
		public CellRangeBase WholeRange { get { return location.WholeRange; } }
		public Worksheet Worksheet { get { return (Worksheet)Range.Worksheet; } }
		public PivotFilterCollection Filters { get { return filters; } }
		public PivotFieldCollection Fields { get { return fields; } }
		public PivotPageFieldCollection PageFields { get { return pageFields; } }
		public PivotTableColumnRowFieldIndices ColumnFields { get { return columnFields; } }
		public PivotTableColumnRowFieldIndices RowFields { get { return rowFields; } }
		public PivotDataFieldCollection DataFields { get { return dataFields; } }
		public PivotFormatCollection Formats { get { return formats; } }
		public PivotConditionalFormatCollection ConditionalFormats { get { return conditionalFormats; } }
		public PivotChartFormatsCollection ChartFormats { get { return chartFormats; } }
		public PivotHierarchyCollection Hierarchies { get { return hierarchies; } }
		public UndoableCollection<int> RowHierarchiesUsage { get { return rowHierarchiesUsage; } }
		public UndoableCollection<int> ColHierarchiesUsage { get { return colHierarchiesUsage; } }
		public PivotCalculationInfo CalculationInfo { get { return calculationInfo; } }
		public PivotTableStyleInfo StyleInfo { get { return styleInfo; } }
		public PivotLayoutCellCache LayoutCellCache { get { return layoutCellCache; } }
		public PivotLayoutItems RowItems { get { return CalculationInfo.RowItems; } }
		public PivotLayoutItems ColumnItems { get { return CalculationInfo.ColumnItems; } }
		public PivotCalculatedCache CalculatedCache { get { return CalculationInfo.CalculatedCache; } }
		#region Name
		public string Name {
			get { return name; }
			set { HistoryHelper.SetValue(documentModel, Name, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetNameCore); }
		}
		protected internal void SetNameCore(string value) {
			string oldName = this.name;
			this.name = value;
			RaiseNameChanged(name, oldName);
		}
		#endregion
		#region DataCaption
		public string DataCaption { get { return dataCaption; } set { SetDataCaption(value); } }
		public void SetDataCaption(string value) {
			if (string.Compare(value, DataCaption, false) == 0)
				return;
			CheckActiveTransaction();
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, DataCaption, value, SetDataCaptionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetDataCaptionCore(string value) {
			dataCaption = value;
			calculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region ColHeaderCaption
		public string ColHeaderCaption { get { return colHeaderCaption; } set { SetColHeaderCaption(value); } }
		public void SetColHeaderCaption(string value) {
			if (string.Compare(value, ColHeaderCaption, false) == 0)
				return;
			CheckActiveTransaction();
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, ColHeaderCaption, value, SetColHeaderCaptionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetColHeaderCaptionCore(string value) {
			colHeaderCaption = value;
			calculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region ErrorCaption
		public string ErrorCaption { get { return errorCaption; } set { SetErrorCaption(value); } }
		public void SetErrorCaption(string value) {
			if (string.Compare(value, ErrorCaption, false) == 0)
				return;
			CheckActiveTransaction();
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, ErrorCaption, value, SetErrorCaptionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetErrorCaptionCore(string value) {
			errorCaption = value;
			calculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region GrandTotalCaption
		public string GrandTotalCaption { get { return grandTotalCaption; } set { SetGrandTotalCaption(value); } }
		void SetGrandTotalCaption(string value) {
			if (string.Compare(value, GrandTotalCaption, false) == 0)
				return;
			CheckActiveTransaction();
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, GrandTotalCaption, value, SetGrandTotalCaptionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetGrandTotalCaptionCore(string value) {
			grandTotalCaption = value;
			calculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region MissingCaption
		public string MissingCaption { get { return missingCaption; } set { SetMissingCaption(value); } }
		void SetMissingCaption(string value) {
			if (string.Compare(value, MissingCaption, false) == 0)
				return;
			CheckActiveTransaction();
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, MissingCaption, value, SetMissingCaptionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetMissingCaptionCore(string value) {
			missingCaption = value;
			calculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region PageStyle
		public string PageStyle { get { return pageStyle; } set { SetPageStyle(value); } }
		void SetPageStyle(string value) {
			if (string.Compare(value, PageStyle, false) == 0)
				return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, PageStyle, value, SetPageStyleCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPageStyleCore(string value) {
			pageStyle = value;
		}
		#endregion
		#region PivotTableStyle
		public string PivotTableStyle { get { return pivotTableStyle; } set { SetPivotTableStyle(value); } }
		void SetPivotTableStyle(string value) {
			if (string.Compare(value, PivotTableStyle, false) == 0)
				return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, PivotTableStyle, value, SetPivotTableStyleCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPivotTableStyleCore(string value) {
			pivotTableStyle = value;
		}
		#endregion
		#region RowHeaderCaption
		public string RowHeaderCaption { get { return rowHeaderCaption; } set { SetRowHeaderCaption(value); } }
		void SetRowHeaderCaption(string value) {
			if (string.Compare(value, RowHeaderCaption, false) == 0)
				return;
			CheckActiveTransaction();
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, RowHeaderCaption, value, SetRowHeaderCaptionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetRowHeaderCaptionCore(string value) {
			rowHeaderCaption = value;
			calculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region Tag
		public string Tag { get { return tag; } set { SetTag(value); } }
		void SetTag(string value) {
			if (string.Compare(value, Tag, false) == 0)
				return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, Tag, value, SetTagCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetTagCore(string value) {
			tag = value;
		}
		#endregion
		#region VacatedStyle
		public string VacatedStyle { get { return vacatedStyle; } set { SetVacatedStyle(value); } }
		void SetVacatedStyle(string value) {
			if (string.Compare(value, VacatedStyle, false) == 0)
				return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, VacatedStyle, value, SetVacatedStyleCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetVacatedStyleCore(string value) {
			vacatedStyle = value;
		}
		#endregion
		#region AltText
		public string AltText { get { return altText; } set { SetAltText(value); } }
		void SetAltText(string value) {
			if (string.Compare(value, AltText, false) == 0)
				return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, AltText, value, SetAltTextCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetAltTextCore(string value) {
			altText = value;
		}
		#endregion
		#region AltTextSummary
		public string AltTextSummary { get { return altTextSummary; } set { SetAltTextSummary(value); } }
		void SetAltTextSummary(string value) {
			if (string.Compare(value, AltTextSummary, false) == 0)
				return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(documentModel, AltTextSummary, value, SetAltTextSummaryCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetAltTextSummaryCore(string value) {
			altTextSummary = value;
		}
		#endregion
		#region CreatedVersion
		public byte CreatedVersion {
			get { return Info.CreatedVersion; }
			set {
				if (CreatedVersion != value)
					SetPropertyValue(SetCreatedVersionCore, value);
			}
		}
		DocumentModelChangeActions SetCreatedVersionCore(PivotTableInfo info, byte value) {
			info.CreatedVersion = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MinRefreshableVersion
		public byte MinRefreshableVersion {
			get { return Info.MinRefreshableVersion; }
			set {
				if (MinRefreshableVersion != value)
					SetPropertyValue(SetMinRefreshableVersionCore, value);
			}
		}
		DocumentModelChangeActions SetMinRefreshableVersionCore(PivotTableInfo info, byte value) {
			info.MinRefreshableVersion = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UpdatedVersion
		public byte UpdatedVersion {
			get { return Info.UpdatedVersion; }
			set {
				if (UpdatedVersion != value)
					SetPropertyValue(SetUpdatedVersionCore, value);
			}
		}
		DocumentModelChangeActions SetUpdatedVersionCore(PivotTableInfo info, byte value) {
			info.UpdatedVersion = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AutoFormatId
		public int AutoFormatId {
			get { return Info.AutoFormatId; }
			set {
				if (AutoFormatId != value)
					SetPropertyValue(SetAutoFormatIdCore, value);
			}
		}
		DocumentModelChangeActions SetAutoFormatIdCore(PivotTableInfo info, int value) {
			info.AutoFormatId = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ChartFormat
		public int ChartFormat {
			get { return Info.ChartFormat; }
			set {
				if (ChartFormat != value)
					SetPropertyValue(SetChartFormatCore, value);
			}
		}
		DocumentModelChangeActions SetChartFormatCore(PivotTableInfo info, int value) {
			info.ChartFormat = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DataPosition
		public int DataPosition {
			get { return Info.DataPosition; }
			set {
				if (DataPosition != value)
					SetPropertyValue(SetDataPositionCore, value);
			}
		}
		DocumentModelChangeActions SetDataPositionCore(PivotTableInfo info, int value) {
			info.DataPosition = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Indent
		public int Indent {
			get { return Info.Indent; }
			set {
				if (Indent != value) {
					CheckActiveTransaction();
					SetIndent(value);
				}
			}
		}
		public void SetIndent(int value) {
			SetPropertyValue(SetIndentCore, value);
		}
		DocumentModelChangeActions SetIndentCore(PivotTableInfo info, int value) {
			info.Indent = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PageWrap
		public int PageWrap {
			get { return Info.PageWrap; }
			set {
				if (PageWrap != value) {
					CheckActiveTransaction();
					SetPageWrap(value);
				}
			}
		}
		public void SetPageWrap(int value) {
			SetPropertyValue(SetPageWrapCore, value);
		}
		DocumentModelChangeActions SetPageWrapCore(PivotTableInfo info, int value) {
			info.PageWrap = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowHeaders
		public bool ShowHeaders {
			get { return Info.ShowHeaders; }
			set {
				if (ShowHeaders != value) {
					CheckActiveTransaction();
					SetShowHeaders(value);
				}
			}
		}
		public void SetShowHeaders(bool value) {
			SetPropertyValue(SetShowHeadersCore, value);
		}
		DocumentModelChangeActions SetShowHeadersCore(PivotTableInfo info, bool value) {
			info.ShowHeaders = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AsteriskTotals
		public bool AsteriskTotals {
			get { return Info.AsteriskTotals; }
			set {
				if (AsteriskTotals != value) {
					CheckActiveTransaction();
					SetAsteriskTotals(value);
				}
			}
		}
		public void SetAsteriskTotals(bool value) {
			SetPropertyValue(SetAsteriskTotalsCore, value);
		}
		DocumentModelChangeActions SetAsteriskTotalsCore(PivotTableInfo info, bool value) {
			info.AsteriskTotals = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DataOnRows
		public bool DataOnRows {
			get { return Info.DataOnRows; }
			set {
				if (DataOnRows != value)
					SetPropertyValue(SetDataOnRowsCore, value);
			}
		}
		DocumentModelChangeActions SetDataOnRowsCore(PivotTableInfo info, bool value) {
			info.DataOnRows = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DisableFieldList
		public bool DisableFieldList {
			get { return Info.DisableFieldList; }
			set {
				if (DisableFieldList != value)
					SetPropertyValue(SetDisableFieldListCore, value);
			}
		}
		DocumentModelChangeActions SetDisableFieldListCore(PivotTableInfo info, bool value) {
			info.DisableFieldList = value;
			return DocumentModelChangeActions.RaiseUpdateUI;
		}
		#endregion
		#region EditData
		public bool EditData {
			get { return Info.EditData; }
			set {
				if (EditData != value)
					SetPropertyValue(SetEditDataCore, value);
			}
		}
		DocumentModelChangeActions SetEditDataCore(PivotTableInfo info, bool value) {
			info.EditData = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FieldListSortAscending
		public bool FieldListSortAscending {
			get { return Info.FieldListSortAscending; }
			set {
				if (FieldListSortAscending != value)
					SetPropertyValue(SetFieldListSortAscendingCore, value);
			}
		}
		DocumentModelChangeActions SetFieldListSortAscendingCore(PivotTableInfo info, bool value) {
			info.FieldListSortAscending = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FieldPrintTitles
		public bool FieldPrintTitles {
			get { return Info.FieldPrintTitles; }
			set {
				if (FieldPrintTitles != value)
					SetPropertyValue(SetFieldPrintTitlesCore, value);
			}
		}
		DocumentModelChangeActions SetFieldPrintTitlesCore(PivotTableInfo info, bool value) {
			info.FieldPrintTitles = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region GridDropZones
		public bool GridDropZones {
			get { return Info.GridDropZones; }
			set {
				if (GridDropZones != value)
					SetPropertyValue(SetGridDropZonesCore, value);
			}
		}
		DocumentModelChangeActions SetGridDropZonesCore(PivotTableInfo info, bool value) {
			info.GridDropZones = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ItemPrintTitles
		public bool ItemPrintTitles {
			get { return Info.ItemPrintTitles; }
			set {
				if (ItemPrintTitles != value)
					SetPropertyValue(SetItemPrintTitlesCore, value);
			}
		}
		DocumentModelChangeActions SetItemPrintTitlesCore(PivotTableInfo info, bool value) {
			info.ItemPrintTitles = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MdxSubqueries
		public bool MdxSubqueries {
			get { return Info.MdxSubqueries; }
			set {
				if (MdxSubqueries != value)
					SetPropertyValue(SetMdxSubqueriesCore, value);
			}
		}
		DocumentModelChangeActions SetMdxSubqueriesCore(PivotTableInfo info, bool value) {
			info.MdxSubqueries = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MergeItem
		public bool MergeItem {
			get { return Info.MergeItem; }
			set {
				if (MergeItem != value) {
					CheckActiveTransaction();
					SetMergeItem(value);
				}
			}
		}
		public void SetMergeItem(bool value) {
			SetPropertyValue(SetMergeItemCore, value);
		}
		DocumentModelChangeActions SetMergeItemCore(PivotTableInfo info, bool value) {
			info.MergeItem = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Outline
		public bool Outline {
			get { return Info.Outline; }
			set {
				if (Outline != value)
					SetPropertyValue(SetOutlineCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineCore(PivotTableInfo info, bool value) {
			info.Outline = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OutlineData
		public bool OutlineData {
			get { return Info.OutlineData; }
			set {
				if (OutlineData != value) {
					CheckActiveTransaction();
					SetOutlineData(value);
				}
			}
		}
		public void SetOutlineData(bool value) {
			SetPropertyValue(SetOutlineDataCore, value);
		}
		DocumentModelChangeActions SetOutlineDataCore(PivotTableInfo info, bool value) {
			info.OutlineData = value;
			calculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PageOverThenDown
		public bool PageOverThenDown {
			get { return Info.PageOverThenDown; }
			set {
				if (PageOverThenDown != value) {
					CheckActiveTransaction();
					SetPageOverThenDown(value);
				}
			}
		}
		public void SetPageOverThenDown(bool value) {
			SetPropertyValue(SetPageOverThenDownCore, value);
		}
		DocumentModelChangeActions SetPageOverThenDownCore(PivotTableInfo info, bool value) {
			info.PageOverThenDown = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PrintDrill
		public bool PrintDrill {
			get { return Info.PrintDrill; }
			set {
				if (PrintDrill != value)
					SetPropertyValue(SetPrintDrillCore, value);
			}
		}
		DocumentModelChangeActions SetPrintDrillCore(PivotTableInfo info, bool value) {
			info.PrintDrill = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Published
		public bool Published {
			get { return Info.Published; }
			set {
				if (Published != value)
					SetPropertyValue(SetPublishedCore, value);
			}
		}
		DocumentModelChangeActions SetPublishedCore(PivotTableInfo info, bool value) {
			info.Published = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowEmptyColumn
		public bool ShowEmptyColumn {
			get { return Info.ShowEmptyColumn; }
			set {
				if (ShowEmptyColumn != value) {
					CheckActiveTransaction();
					SetShowEmptyColumn(value);
				}
			}
		}
		public void SetShowEmptyColumn(bool value) {
			SetPropertyValue(SetShowEmptyColCore, value);
		}
		DocumentModelChangeActions SetShowEmptyColCore(PivotTableInfo info, bool value) {
			info.ShowEmptyColumn = value;
			calculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowEmptyRow
		public bool ShowEmptyRow {
			get { return Info.ShowEmptyRow; }
			set {
				if (ShowEmptyRow != value) {
					CheckActiveTransaction();
					SetShowEmptyRow(value);
				}
			}
		}
		public void SetShowEmptyRow(bool value) {
			SetPropertyValue(SetShowEmptyRowCore, value);
		}
		DocumentModelChangeActions SetShowEmptyRowCore(PivotTableInfo info, bool value) {
			info.ShowEmptyRow = value;
			calculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowError
		public bool ShowError {
			get { return Info.ShowError; }
			set {
				if (ShowError != value) {
					CheckActiveTransaction();
					SetShowError(value);
				}
			}
		}
		public void SetShowError(bool value) {
			SetPropertyValue(SetShowErrorCore, value);
		}
		DocumentModelChangeActions SetShowErrorCore(PivotTableInfo info, bool value) {
			info.ShowError = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SubtotalHiddenItems
		public bool SubtotalHiddenItems {
			get { return Info.SubtotalHiddenItems; }
			set {
				if (SubtotalHiddenItems != value) {
					CheckActiveTransaction();
					SetSubtotalHiddenItems(value);
				}
			}
		}
		public void SetSubtotalHiddenItems(bool value) {
			SetPropertyValue(SetSubtotalHiddenItemsCore, value);
		}
		DocumentModelChangeActions SetSubtotalHiddenItemsCore(PivotTableInfo info, bool value) {
			info.SubtotalHiddenItems = value;
			calculationInfo.InvalidateCalculatedCache();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UseAutoFormatting
		public bool UseAutoFormatting {
			get { return Info.UseAutoFormatting; }
			set {
				if (UseAutoFormatting != value) {
					CheckActiveTransaction();
					SetUseAutoFormatting(value);
				}
			}
		}
		public void SetUseAutoFormatting(bool value) {
			SetPropertyValue(SetUseAutoFormattingCore, value);
		}
		DocumentModelChangeActions SetUseAutoFormattingCore(PivotTableInfo info, bool value) {
			info.UseAutoFormatting = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ColumnGrandTotals
		public bool ColumnGrandTotals {
			get { return Info.ColumnGrandTotals; }
			set {
				if (ColumnGrandTotals != value) {
					CheckActiveTransaction();
					PivotChangeColumnGrandTotalsCommand command = new PivotChangeColumnGrandTotalsCommand(calculationInfo.Transaction.ErrorHandler, this, value);
					command.Execute();
				}
			}
		}
		public void SetColumnGrandTotals(bool value) {
			SetPropertyValue(SetColumnGrandTotalsCore, value);
		}
		protected internal DocumentModelChangeActions SetColumnGrandTotalsCore(PivotTableInfo info, bool value) {
			info.ColumnGrandTotals = value;
			calculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Compact
		public bool Compact {
			get { return Info.Compact; }
			set {
				if (Compact != value)
					SetPropertyValue(SetCompactCore, value);
			}
		}
		DocumentModelChangeActions SetCompactCore(PivotTableInfo info, bool value) {
			info.Compact = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CompactData
		public bool CompactData {
			get { return Info.CompactData; }
			set {
				if (CompactData != value) {
					CheckActiveTransaction();
					SetCompactData(value);
				}
			}
		}
		public void SetCompactData(bool value) {
			SetPropertyValue(SetCompactDataCore, value);
		}
		DocumentModelChangeActions SetCompactDataCore(PivotTableInfo info, bool value) {
			info.CompactData = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CustomListSort
		public bool CustomListSort {
			get { return Info.CustomListSort; }
			set {
				if (CustomListSort != value)
					SetPropertyValue(SetCustomListSortCore, value);
			}
		}
		DocumentModelChangeActions SetCustomListSortCore(PivotTableInfo info, bool value) {
			info.CustomListSort = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EnableDrill
		public bool EnableDrill {
			get { return Info.EnableDrill; }
			set {
				if (EnableDrill != value)
					SetPropertyValue(SetEnableDrillCore, value);
			}
		}
		DocumentModelChangeActions SetEnableDrillCore(PivotTableInfo info, bool value) {
			info.EnableDrill = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EnableFieldProperties
		public bool EnableFieldProperties {
			get { return Info.EnableFieldProperties; }
			set {
				if (EnableFieldProperties != value)
					SetPropertyValue(SetEnableFieldPropertiesCore, value);
			}
		}
		DocumentModelChangeActions SetEnableFieldPropertiesCore(PivotTableInfo info, bool value) {
			info.EnableFieldProperties = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EnableWizard
		public bool EnableWizard {
			get { return Info.EnableWizard; }
			set {
				if (EnableWizard != value)
					SetPropertyValue(SetEnableWizardCore, value);
			}
		}
		DocumentModelChangeActions SetEnableWizardCore(PivotTableInfo info, bool value) {
			info.EnableWizard = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Immersive
		public bool Immersive {
			get { return Info.Immersive; }
			set {
				if (Immersive != value)
					SetPropertyValue(SetImmersiveCore, value);
			}
		}
		DocumentModelChangeActions SetImmersiveCore(PivotTableInfo info, bool value) {
			info.Immersive = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MultipleFieldFilters
		public bool MultipleFieldFilters {
			get { return Info.MultipleFieldFilters; }
			set {
				if (MultipleFieldFilters != value)
					SetPropertyValue(SetMultipleFieldFiltersCore, value);
			}
		}
		DocumentModelChangeActions SetMultipleFieldFiltersCore(PivotTableInfo info, bool value) {
			info.MultipleFieldFilters = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PreserveFormatting
		public bool PreserveFormatting {
			get { return Info.PreserveFormatting; }
			set {
				if (PreserveFormatting != value)
					SetPropertyValue(SetPreserveFormattingCore, value);
			}
		}
		DocumentModelChangeActions SetPreserveFormattingCore(PivotTableInfo info, bool value) {
			info.PreserveFormatting = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RowGrandTotals
		public bool RowGrandTotals {
			get { return Info.RowGrandTotals; }
			set {
				if (RowGrandTotals != value) {
					CheckActiveTransaction();
					PivotChangeRowGrandTotalsCommand command = new PivotChangeRowGrandTotalsCommand(calculationInfo.Transaction.ErrorHandler, this, value);
					command.Execute();
				}
			}
		}
		public void SetRowGrandTotals(bool value) {
			SetPropertyValue(SetRowGrandTotalsCore, value);
		}
		DocumentModelChangeActions SetRowGrandTotalsCore(PivotTableInfo info, bool value) {
			info.RowGrandTotals = value;
			calculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowCalcMbrs
		public bool ShowCalcMbrs {
			get { return Info.ShowCalcMbrs; }
			set {
				if (ShowCalcMbrs != value)
					SetPropertyValue(SetShowCalcMbrsCore, value);
			}
		}
		DocumentModelChangeActions SetShowCalcMbrsCore(PivotTableInfo info, bool value) {
			info.ShowCalcMbrs = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDataDropDown
		public bool ShowDataDropDown {
			get { return Info.ShowDataDropDown; }
			set {
				if (ShowDataDropDown != value)
					SetPropertyValue(SetShowDataDropDownCore, value);
			}
		}
		DocumentModelChangeActions SetShowDataDropDownCore(PivotTableInfo info, bool value) {
			info.ShowDataDropDown = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDataTips
		public bool ShowDataTips {
			get { return Info.ShowDataTips; }
			set {
				if (ShowDataTips != value)
					SetPropertyValue(SetShowDataTipsCore, value);
			}
		}
		DocumentModelChangeActions SetShowDataTipsCore(PivotTableInfo info, bool value) {
			info.ShowDataTips = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDrill
		public bool ShowDrill {
			get { return Info.ShowDrill; }
			set {
				if (ShowDrill != value) {
					CheckActiveTransaction();
					SetShowDrill(value);
				}
			}
		}
		public void SetShowDrill(bool value) {
			SetPropertyValue(SetShowDrillCore, value);
		}
		DocumentModelChangeActions SetShowDrillCore(PivotTableInfo info, bool value) {
			info.ShowDrill = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDropZones
		public bool ShowDropZones {
			get { return Info.ShowDropZones; }
			set {
				if (ShowDropZones != value)
					SetPropertyValue(SetShowDropZonesCore, value);
			}
		}
		DocumentModelChangeActions SetShowDropZonesCore(PivotTableInfo info, bool value) {
			info.ShowDropZones = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowItems
		public bool ShowItems {
			get { return Info.ShowItems; }
			set {
				if (ShowItems != value)
					SetPropertyValue(SetShowItemsCore, value);
			}
		}
		DocumentModelChangeActions SetShowItemsCore(PivotTableInfo info, bool value) {
			info.ShowItems = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowMemberPropertyTips
		public bool ShowMemberPropertyTips {
			get { return Info.ShowMemberPropertyTips; }
			set {
				if (ShowMemberPropertyTips != value)
					SetPropertyValue(SetShowMemberPropertyTipsCore, value);
			}
		}
		DocumentModelChangeActions SetShowMemberPropertyTipsCore(PivotTableInfo info, bool value) {
			info.ShowMemberPropertyTips = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowMissing
		public bool ShowMissing {
			get { return Info.ShowMissing; }
			set {
				if (ShowMissing != value) {
					CheckActiveTransaction();
					SetShowMissing(value);
				}
			}
		}
		public void SetShowMissing(bool value) {
			SetPropertyValue(SetShowMissingCore, value);
		}
		DocumentModelChangeActions SetShowMissingCore(PivotTableInfo info, bool value) {
			info.ShowMissing = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowMultipleLabel
		public bool ShowMultipleLabel {
			get { return Info.ShowMultipleLabel; }
			set {
				if (ShowMultipleLabel != value)
					SetPropertyValue(SetShowMultipleLabelCore, value);
			}
		}
		DocumentModelChangeActions SetShowMultipleLabelCore(PivotTableInfo info, bool value) {
			info.ShowMultipleLabel = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region VisualTotals
		public bool VisualTotals {
			get { return Info.VisualTotals; }
			set {
				if (VisualTotals != value)
					SetPropertyValue(SetVisualTotalsCore, value);
			}
		}
		DocumentModelChangeActions SetVisualTotalsCore(PivotTableInfo info, bool value) {
			info.VisualTotals = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyAlignmentFormats
		public bool ApplyAlignmentFormats {
			get { return Info.ApplyAlignmentFormats; }
			set {
				HasAlignmentFormats = true;
				if (ApplyAlignmentFormats != value)
					SetPropertyValue(SetApplyAlignmentFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetApplyAlignmentFormatsCore(PivotTableInfo info, bool value) {
			info.ApplyAlignmentFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasAlignmentFormats
		public bool HasAlignmentFormats {
			get { return Info.HasAlignmentFormats; }
			set {
				if (HasAlignmentFormats != value)
					SetPropertyValue(SetHasAlignmentFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetHasAlignmentFormatsCore(PivotTableInfo info, bool value) {
			info.HasAlignmentFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyBorderFormats
		public bool ApplyBorderFormats {
			get { return Info.ApplyBorderFormats; }
			set {
				HasBorderFormats = true;
				if (ApplyBorderFormats != value)
					SetPropertyValue(SetApplyBorderFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetApplyBorderFormatsCore(PivotTableInfo info, bool value) {
			info.ApplyBorderFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasBorderFormats
		public bool HasBorderFormats {
			get { return Info.HasBorderFormats; }
			set {
				if (HasBorderFormats != value)
					SetPropertyValue(SetHasBorderFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetHasBorderFormatsCore(PivotTableInfo info, bool value) {
			info.HasBorderFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyFontFormats
		public bool ApplyFontFormats {
			get { return Info.ApplyFontFormats; }
			set {
				HasFontFormats = true;
				if (ApplyFontFormats != value)
					SetPropertyValue(SetApplyFontFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFontFormatsCore(PivotTableInfo info, bool value) {
			info.ApplyFontFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasFontFormats
		public bool HasFontFormats {
			get { return Info.HasFontFormats; }
			set {
				if (HasFontFormats != value)
					SetPropertyValue(SetHasFontFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetHasFontFormatsCore(PivotTableInfo info, bool value) {
			info.HasFontFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyNumberFormats
		public bool ApplyNumberFormats {
			get { return Info.ApplyNumberFormats; }
			set {
				HasNumberFormats = true;
				if (ApplyNumberFormats != value)
					SetPropertyValue(SetApplyNumberFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetApplyNumberFormatsCore(PivotTableInfo info, bool value) {
			info.ApplyNumberFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasNumberFormats
		public bool HasNumberFormats {
			get { return Info.HasNumberFormats; }
			set {
				if (HasNumberFormats != value)
					SetPropertyValue(SetHasNumberFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetHasNumberFormatsCore(PivotTableInfo info, bool value) {
			info.HasNumberFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyPatternFormats
		public bool ApplyPatternFormats {
			get { return Info.ApplyPatternFormats; }
			set {
				HasPatternFormats = true;
				if (ApplyPatternFormats != value)
					SetPropertyValue(SetApplyPatternFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetApplyPatternFormatsCore(PivotTableInfo info, bool value) {
			info.ApplyPatternFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasPatternFormats
		public bool HasPatternFormats {
			get { return Info.HasPatternFormats; }
			set {
				if (HasPatternFormats != value)
					SetPropertyValue(SetHasPatternFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetHasPatternFormatsCore(PivotTableInfo info, bool value) {
			info.HasPatternFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyWidthHeightFormats
		public bool ApplyWidthHeightFormats {
			get { return Info.ApplyWidthHeightFormats; }
			set {
				HasWidthHeightFormats = true;
				if (ApplyWidthHeightFormats != value)
					SetPropertyValue(SetApplyWidthHeightFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetApplyWidthHeightFormatsCore(PivotTableInfo info, bool value) {
			info.ApplyWidthHeightFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasWidthHeightFormats
		public bool HasWidthHeightFormats {
			get { return Info.HasWidthHeightFormats; }
			set {
				if (HasWidthHeightFormats != value)
					SetPropertyValue(SetHasWidthHeightFormatsCore, value);
			}
		}
		DocumentModelChangeActions SetHasWidthHeightFormatsCore(PivotTableInfo info, bool value) {
			info.HasWidthHeightFormats = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HideValuesRow
		public bool ShowValuesRow {
			get { return Info.ShowValuesRow; }
			set {
				if (ShowValuesRow != value) {
					CheckActiveTransaction();
					SetShowValuesRow(value);
				}
			}
		}
		public void SetShowValuesRow(bool value) {
			SetPropertyValue(SetShowValuesRowCore, value);
		}
		DocumentModelChangeActions SetShowValuesRowCore(PivotTableInfo info, bool value) {
			info.ShowValuesRow = value;
			calculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region Transaction
		public void BeginTransaction(IErrorHandler errorHandler) {
			documentModel.BeginUpdate();
			calculationInfo.BeginUpdate(errorHandler);
		}
		public bool EndTransaction() {
			try {
				return calculationInfo.EndUpdate();
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal void CheckActiveTransaction() {
			if (!calculationInfo.HasActiveTransaction)
				throw new InvalidOperationException("This operation requires active transation. ");
		}
		#endregion
		#region Events
		#region NameChanged
		NameChangedEventHandler onNameChanged;
		public event NameChangedEventHandler NameChanged { add { onNameChanged += value; } remove { onNameChanged -= value; } }
		protected internal virtual void RaiseNameChanged(string name, string oldName) {
			if (onNameChanged != null) {
				NameChangedEventArgs args = new NameChangedEventArgs(name, oldName);
				onNameChanged(this, args);
			}
		}
		#endregion
		#region RangeChanged
		void Location_RangeChanged(object sender, CellRangeChangedEventArgs e) {
			if (onRangeChanged != null) {
				onRangeChanged(this, e);
			}
		}
		CellRangeChangedEventHandler onRangeChanged;
		public event CellRangeChangedEventHandler RangeChanged { add { onRangeChanged += value; } remove { onRangeChanged -= value; } }
		#endregion
		#region FieldsChanged
		EventHandler onFieldsChanged;
		public event EventHandler FieldsChanged { add { onFieldsChanged += value; } remove { onFieldsChanged -= value; } }
		protected internal virtual void RaiseFieldsChanged() {
			if (onFieldsChanged != null)
				onFieldsChanged(this, EventArgs.Empty);
		}
		#endregion
		#region OnCacheChanged
		CacheChangedEventHandler onCacheChanged;
		public event CacheChangedEventHandler OnCacheChanged { add { onCacheChanged += value; } remove { onCacheChanged -= value; } }
		protected internal virtual void RaiseCacheChanged(PivotCache cache) {
			if (onCacheChanged != null) {
				CacheChangedEventArgs args = new CacheChangedEventArgs(cache);
				onCacheChanged(this, args);
			}
		}
		#endregion
		#endregion
		#region Notification
		public IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			IModelErrorInfo info = null;
			info = location.CanRangeInsert(range, mode);
			if (info != null)
				return info;
			return fields.CanRangeInsert(range, mode);
		}
		public IModelErrorInfo CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			IModelErrorInfo info = null;
			info = location.CanRangeRemove(range, mode);
			if (info != null)
				return info;
			return fields.CanRangeRemove(range, mode);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			location.OnRangeInserting(context);
			fields.OnRangeInserting(context);
		}
		public bool OnRangeRemoving(RemoveRangeNotificationContext context) {
			fields.OnRangeRemoving(context);
			return location.OnRangeRemoving(context);
		}
		#endregion
		#region WholeRangeChanged
		void Location_WholeRangeChanged(object sender, CellRangeBaseChangedEventArgs e) {
			if (onWholeRangeChanged != null) {
				onWholeRangeChanged(this, e);
			}
		}
		CellRangeBaseChangedEventHandler onWholeRangeChanged;
		public event CellRangeBaseChangedEventHandler WholeRangeChanged { add { onWholeRangeChanged += value; } remove { onWholeRangeChanged -= value; } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<PivotTableInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.PivotTableCache;
		}
		#endregion
		public List<int> GetKeyFieldIndices() {
			List<int> items = new List<int>();
			items.AddRange(rowFields.GetKeyIndicesEnumerable());
			items.AddRange(columnFields.GetKeyIndicesEnumerable());
			return items;
		}
		#region CalculationInfo methods
		public bool AddFieldToKeyFields(int fieldIndex, PivotTableAxis axis, IErrorHandler errorHandler) {
			return CalculationInfo.AddFieldToKeyFields(fieldIndex, axis, errorHandler);
		}
		public bool InsertFieldToKeyFields(int fieldIndex, PivotTableAxis axis, int insertIndex, IErrorHandler errorHandler) {
			return CalculationInfo.InsertFieldToKeyFields(fieldIndex, axis, insertIndex, errorHandler);
		}
		public bool AddDataField(int fieldIndex, IErrorHandler errorHandler) {
			return AddDataField(fieldIndex, null, errorHandler);
		}
		public bool AddDataField(int fieldIndex, string caption, IErrorHandler errorHandler) {
			return CalculationInfo.AddDataField(fieldIndex, caption, errorHandler);
		}
		public bool AddDataField(int fieldIndex, PivotDataConsolidateFunction function, IErrorHandler errorHandler) {
			return AddDataField(fieldIndex, null, function, errorHandler);
		}
		public bool AddDataField(int fieldIndex, string caption, PivotDataConsolidateFunction function, IErrorHandler errorHandler) {
			return CalculationInfo.AddDataField(fieldIndex, caption, function, errorHandler);
		}
		public bool InsertDataField(int fieldIndex, int insertIndex, IErrorHandler errorHandler) {
			return InsertDataField(fieldIndex, null, insertIndex, errorHandler);
		}
		public bool InsertDataField(int fieldIndex, string caption, int insertIndex, IErrorHandler errorHandler) {
			return CalculationInfo.InsertDataField(fieldIndex, caption, insertIndex, errorHandler);
		}
		public bool InsertDataField(int fieldIndex, PivotDataConsolidateFunction function, int insertIndex, IErrorHandler errorHandler) {
			return InsertDataField(fieldIndex, null, function, insertIndex, errorHandler);
		}
		public bool InsertDataField(int fieldIndex, string caption, PivotDataConsolidateFunction function, int insertIndex, IErrorHandler errorHandler) {
			return CalculationInfo.InsertDataField(fieldIndex, caption, function, insertIndex, errorHandler);
		}
		public void RemoveKeyField(int fieldReferenceIndex, PivotTableAxis axis, IErrorHandler errorHandler) {
			CalculationInfo.RemoveKeyField(fieldReferenceIndex, axis, errorHandler);
		}
		public void RemoveFieldFromKeyFields(int fieldIndex, IErrorHandler errorHandler) {
			CalculationInfo.RemoveFieldFromKeyFields(fieldIndex, errorHandler);
		}
		public void ClearAllKeyFields(IErrorHandler errorHandler) {
			CalculationInfo.ClearAllKeyFields(errorHandler);
		}
		public void ClearKeyFields(PivotTableAxis axisType, IErrorHandler errorHandler) {
			CalculationInfo.ClearKeyFields(axisType, errorHandler);
		}
		public void MoveKeyField(PivotTableAxis sourceAxis, int sourceIndex, PivotTableAxis targetAxis, int targetIndex, IErrorHandler errorHandler) {
			CalculationInfo.MoveKeyField(sourceAxis, sourceIndex, targetAxis, targetIndex, errorHandler);
		}
		#endregion
		#region ITableBase Members
		ActualTableStyleCellFormatting ITableBase.GetActualCellFormatting(CellPosition cellPosition) {
			TableStyle style = DocumentModel.StyleSheet.TableStyles[StyleInfo.StyleName];
			return CalculationInfo.StyleFormatCache.GetActualCellFormatting(cellPosition, style.Cache);
		}
		TableStyle ITableBase.Style { get { return styleInfo.Style; } set { styleInfo.StyleName = value.Name.Name; } }
		#endregion
		public void ShowAllSubtotals(bool atTopOfGroup, IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				for (int i = 0; i < Fields.Count; ++i) {
					PivotField field = Fields[i];
					if (field.Subtotal == PivotFieldItemType.Blank)
						field.Subtotal = PivotFieldItemType.DefaultValue;
					field.SubtotalTop = atTopOfGroup;
					CalculationInfo.InvalidateLayout();
				}
			}
			finally {
				EndTransaction();
			}
		}
		public void HideAllSubtotals(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				for (int i = 0; i < Fields.Count; ++i)
					Fields[i].Subtotal = PivotFieldItemType.Blank;
				CalculationInfo.InvalidateLayout();
			}
			finally {
				EndTransaction();
			}
		}
		public void InsertBlankRows(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				for (int i = 0; i < Fields.Count; ++i)
					Fields[i].InsertBlankRow = true;
				CalculationInfo.InvalidateLayout();
			}
			finally {
				EndTransaction();
			}
		}
		public void RemoveBlankRows(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				for (int i = 0; i < Fields.Count; ++i)
					Fields[i].InsertBlankRow = false;
				CalculationInfo.InvalidateLayout();
			}
			finally {
				EndTransaction();
			}
		}
		public void SetCompactForm(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				Compact = true;
				Outline = true;
				CompactData = true;
				OutlineData = true;
				for (int i = 0; i < Fields.Count; ++i) {
					PivotField field = Fields[i];
					field.Compact = true;
					field.Outline = true;
				}
				CalculationInfo.InvalidateCalculatedCache();
			}
			finally {
				EndTransaction();
			}
		}
		public void SetOutlineForm(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				Compact = false;
				Outline = true;
				CompactData = false;
				OutlineData = true;
				for (int i = 0; i < Fields.Count; ++i) {
					PivotField field = Fields[i];
					field.Compact = false;
					field.Outline = true;
				}
				CalculationInfo.InvalidateCalculatedCache();
			}
			finally {
				EndTransaction();
			}
		}
		public void SetTabularForm(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				Compact = false;
				Outline = false;
				CompactData = false;
				OutlineData = false;
				for (int i = 0; i < Fields.Count; ++i) {
					PivotField field = Fields[i];
					field.Compact = false;
					field.Outline = false;
				}
				CalculationInfo.InvalidateCalculatedCache();
			}
			finally {
				EndTransaction();
			}
		}
		public void RepeatAllItemLabels(bool repeat, IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			try {
				for (int i = 0; i < Fields.Count; i++)
					Fields[i].FillDownLabels = repeat;
			}
			finally {
				EndTransaction();
			}
		}
		public void Recalculate(IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			calculationInfo.State = PivotTableOutOfDateState.CalculatedCache;
			EndTransaction();
		}
		public void RefreshCacheAndTable(IErrorHandler errorHandler) {
			Cache.Refresh(errorHandler);
		}
		#region FieldProperties
		internal string GetFieldCaption(int fieldIndex) {
			PivotField field = Fields[fieldIndex];
			if (!string.IsNullOrEmpty(field.Name))
				return field.Name;
			IPivotCacheField cacheField = cache.CacheFields[fieldIndex];
			if (!string.IsNullOrEmpty(cacheField.Caption))
				return cacheField.Caption;
			return cacheField.Name;
		}
		internal bool FieldIsCompact(int fieldIndex) {
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return CompactData;
			return Fields[fieldIndex].Compact;
		}
		internal bool FieldIsOutline(int fieldIndex) {
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return OutlineData;
			return Fields[fieldIndex].Outline;
		}
		internal bool FieldIsCompactForm(int fieldIndex) {
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return CompactData && OutlineData;
			return Fields[fieldIndex].CompactForm;
		}
		#endregion
		#region HasHiddenItems
		internal bool HasHiddenItems() {
			foreach (PivotPageField pageField in pageFields)
				if (pageField.ItemIndex >= 0)
					return true;
			for (int i = 0; i < fields.Count; i++)
				if (FieldHasHiddenItemsCore(i))
					return true;
			return false;
		}
		internal bool FieldHasHiddenItems(int fieldIndex) {
			PivotField field = Fields[fieldIndex];
			if (field.Axis == PivotTableAxis.Page) {
				foreach (PivotPageField pageField in pageFields)
					if (pageField.FieldIndex == fieldIndex && pageField.ItemIndex >= 0)
						return true;
			}
			return field.Items.HiddenItemsCount > 0;
		}
		internal bool FieldHasHiddenItemsCore(int fieldIndex) {
			PivotField field = Fields[fieldIndex];
			return field.Items.HiddenItemsCount > 0;
		}
		#endregion
		public void SetItemIsCollapsed(int fieldIndex, int itemIndex, bool value, IErrorHandler errorHandler) {
			BeginTransaction(errorHandler);
			Fields[fieldIndex].Items[itemIndex].HideDetails = value;
			EndTransaction();
		}
		public bool ItemIsCollapsed(int fieldIndex, int itemIndex) {
			return Fields[fieldIndex].Items[itemIndex].HideDetails;
		}
		internal void RemoveMeasureFilters(int dataFieldIndex) {
			for (int i = filters.Count - 1; i >= 0; i--) {
				PivotFilter filter = filters[i];
				if (!filter.IsMeasureFilter)
					continue;
				int measureFieldIndex = filter.MeasureFieldIndex.Value;
				if (measureFieldIndex == dataFieldIndex) {
					filters.Remove(filter);
					fields[filter.FieldIndex].MeasureFilter = false;
				}
				else
					if (measureFieldIndex > dataFieldIndex)
						filter.MeasureFieldIndex--;
			}
		}
		public void Remove(IErrorHandler errorHandler) {
			RemoveRangeCommand command = new RemoveRangeCommand(Worksheet, WholeRange, RemoveCellMode.Default, false, false, errorHandler);
			command.SuppressPivotTableChecks = true;
			command.Execute();
		}
		public void Clear(IErrorHandler errorHandler) {
			PivotClearCommand command = new PivotClearCommand(this, errorHandler);
			command.Execute();
		}
		public void ChangeDataSource(CellRange range, IErrorHandler errorHandler) {
			Guard.ArgumentNotNull(range, "range");
			ParsedExpression expression = new Model.ParsedExpression();
			WorkbookDataContext context = range.Worksheet.Workbook.DataContext;
			IParsedThing ptg = BasicExpressionCreator.CreateCellSingleRangePtg(range, BasicExpressionCreatorParameter.ShouldCreate3d, context);
			expression.Add(ptg);
			PivotCacheSourceWorksheet source = PivotCacheSourceWorksheet.CreateInstance(expression, context);
			ChangeDataSourceCore(source, errorHandler);
		}
		protected internal bool ChangeDataSourceCore(IPivotCacheSource source, IErrorHandler errorHandler) {
			PivotChangeDataSourceCommand command = new PivotChangeDataSourceCommand(this, source, errorHandler);
			return command.Execute();
		}
		public void ClearFilters(bool shouldUnHideItems, IErrorHandler errorHandler) {
			PivotClearFiltersCommand command = new PivotClearFiltersCommand(this, shouldUnHideItems, errorHandler);
			command.Execute();
		}
		public void ClearFieldFilters(int fieldIndex, PivotFilterClearType clearType, IErrorHandler errorHandler) {
			PivotClearFieldFiltersCommand command = new PivotClearFieldFiltersCommand(this, fieldIndex, clearType, errorHandler);
			command.Execute();
		}
		internal PivotField CreateField() {
			PivotField result = new PivotField(this);
			result.Outline = Outline;
			result.Compact = Compact;
			return result;
		}
		public void CopyFromNoHistory(PivotTable source, CellPositionOffset offset) {
			CopyFrom(source);
			location.CopyFromNoHistory(source.location, offset);
			fields.CopyFromNoHistory(this, offset, source.fields);
			columnFields.CopyFromNoHistory(source.columnFields);
			rowFields.CopyFromNoHistory(source.rowFields);
			dataFields.CopyFromNoHistory(this, source.dataFields);
			pageFields.CopyFromNoHistory(this, source.pageFields);
			formats.CopyFromNoHistory(this, source.formats);
			conditionalFormats.CopyFromNoHistory(this, offset, source.conditionalFormats);
			chartFormats.CopyFromNoHistory(this, offset, source.chartFormats);
			hierarchies.CopyFromNoHistory(source.hierarchies);
			rowHierarchiesUsage.InnerList.Clear();
			rowHierarchiesUsage.InnerList.AddRange(source.rowHierarchiesUsage);
			colHierarchiesUsage.InnerList.Clear();
			colHierarchiesUsage.InnerList.AddRange(source.colHierarchiesUsage);
			calculationInfo.CopyFromNoHistory(source.calculationInfo);
			layoutCellCache.CopyFromNoHistory(offset, source.layoutCellCache);
			styleInfo.CopyFromNoHistory(source.styleInfo);
			filters.CopyFromNoHistory(source.filters);
			dataCaption = source.dataCaption;
			colHeaderCaption = source.colHeaderCaption;
			errorCaption = source.errorCaption;
			grandTotalCaption = source.grandTotalCaption;
			missingCaption = source.missingCaption;
			pageStyle = source.pageStyle;
			rowHeaderCaption = source.rowHeaderCaption;
			tag = source.tag;
			vacatedStyle = source.vacatedStyle; 
			altText = source.altText;
			altTextSummary = source.altTextSummary;
			defaultDataCaption = source.defaultDataCaption;
		}
	}
	#endregion
}
