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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ITableStyleViewInfo
	public interface ITableStyleViewInfo {
		int RowCount { get; }
		int ColumnCount { get; }
		IActualFillInfo GetActualFillInfo(CellPosition position);
		Color GetTextColor(CellPosition position);
		XlBorderLineStyle GetLeftBorderLineStyle(CellPosition position);
		Color GetLeftBorderColor(CellPosition position);
		XlBorderLineStyle GetRightBorderLineStyle(CellPosition position);
		Color GetRightBorderColor(CellPosition position);
		XlBorderLineStyle GetTopBorderLineStyle(CellPosition position);
		Color GetTopBorderColor(CellPosition position);
		XlBorderLineStyle GetBottomBorderLineStyle(CellPosition position);
		Color GetBottomBorderColor(CellPosition position);
	}
	#endregion
	#region TableStyleViewInfoBase (abstract class)
	public abstract class TableStyleViewInfoBase<TStyleOwner, TFormatCache> : ITableStyleViewInfo, ITableBase where TStyleOwner : class {
		#region Static members
		static TableStyleViewInfoFillPropertyAccessor accessor = new TableStyleViewInfoFillPropertyAccessor();
		#endregion
		#region Fields
		readonly CellPosition topLeft = new CellPosition(0, 0);
		readonly CellRange range;
		readonly TableStyleElementInfoCache styleCache;
		readonly TFormatCache formatCache;
		#endregion
		protected TableStyleViewInfoBase(DocumentModel documentModel, string styleName) {
			TableStyleElementInfoCache styleCache = documentModel.StyleSheet.TableStyles[styleName].Cache;
			this.styleCache = styleCache;
			Worksheet activeSheet = documentModel.ActiveSheet;
			this.range = new CellRange(activeSheet, topLeft, BottomRight);
			this.formatCache = GetInstanceFormatCache();
			TStyleOwner styleOwner = GetActiveStyleOwner(activeSheet);
			if (styleOwner != null)
				InitStyleOptions(styleOwner);
			PrepareFormatCache();
		}
		#region Properties
		protected CellRange Range { get { return range; } }
		protected DocumentModel DocumentModel { get { return range.Worksheet.Workbook.StyleSheet.Workbook; } }
		protected TableStyleElementInfoCache StyleCache { get { return styleCache; } }
		protected TFormatCache FormatCache { get { return formatCache; } }
		protected abstract CellPosition BottomRight { get; }
		#endregion
		protected abstract TFormatCache GetInstanceFormatCache();
		protected abstract TStyleOwner GetActiveStyleOwner(Worksheet activeSheet);
		protected abstract void InitStyleOptions(TStyleOwner styleOwner);
		protected abstract void PrepareFormatCache();
		#region ITableStyleViewInfo Members
		public int RowCount { get { return BottomRight.Row - topLeft.Row + 1; } }
		public int ColumnCount { get { return BottomRight.Column - topLeft.Column + 1; } }
		public IActualFillInfo GetActualFillInfo(CellPosition position) {
			return accessor.GetInfo(this, position);
		}
		Color GetColor(int colorIndex) {
			return DocumentModel.Cache.ColorModelInfoCache[colorIndex].ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		XlBorderLineStyle GetBorderLineStyle(DifferentialFormatDisplayBorderDescriptor descriptor, CellPosition position) {
			return TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(this, descriptor, position, XlBorderLineStyle.None);
		}
		Color GetBorderColor(DifferentialFormatDisplayBorderDescriptor descriptor, CellPosition position) {
			return GetColor(TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(this, descriptor, position, ColorModelInfoCache.DefaultItemIndex));
		}
		public Color GetTextColor(CellPosition position) {
			return GetColor(TableStyleFormatBuilderFactory.PropertyBuilder.Build(this, DifferentialFormatPropertyDescriptor.FontColorIndex, position, ColorModelInfoCache.DefaultItemIndex));
		}
		public XlBorderLineStyle GetLeftBorderLineStyle(CellPosition position) {
			return GetBorderLineStyle(DifferentialFormatDisplayBorderDescriptor.LeftLineStyle, position);
		}
		public Color GetLeftBorderColor(CellPosition position) {
			return GetBorderColor(DifferentialFormatDisplayBorderDescriptor.LeftColorIndex, position);
		}
		public XlBorderLineStyle GetRightBorderLineStyle(CellPosition position) {
			return GetBorderLineStyle(DifferentialFormatDisplayBorderDescriptor.RightLineStyle, position);
		}
		public Color GetRightBorderColor(CellPosition position) {
			return GetBorderColor(DifferentialFormatDisplayBorderDescriptor.RightColorIndex, position);
		}
		public XlBorderLineStyle GetTopBorderLineStyle(CellPosition position) {
			return GetBorderLineStyle(DifferentialFormatDisplayBorderDescriptor.TopLineStyle, position);
		}
		public Color GetTopBorderColor(CellPosition position) {
			return GetBorderColor(DifferentialFormatDisplayBorderDescriptor.TopColorIndex, position);
		}
		public XlBorderLineStyle GetBottomBorderLineStyle(CellPosition position) {
			return GetBorderLineStyle(DifferentialFormatDisplayBorderDescriptor.BottomLineStyle, position);
		}
		public Color GetBottomBorderColor(CellPosition position) {
			return GetBorderColor(DifferentialFormatDisplayBorderDescriptor.BottomColorIndex, position);
		}
		#endregion
		#region ITableBase Members
		public CellRangeBase WholeRange { get { return range; } }
		public TableStyle Style { get { return null; } set { } }
		public abstract ActualTableStyleCellFormatting GetActualCellFormatting(CellPosition cellPosition);
		#endregion
	}
	#endregion
	#region TableStyleViewInfo
	public class TableStyleViewInfo : TableStyleViewInfoBase<ITableStyleOwner, TableStyleFormatViewInfoCache>, ITableStyleOwner, ITableStyleOptions {
		#region Fields
		readonly CellPosition bottomRight = new CellPosition(4, 4);
		readonly TableStyleStripeInfo[] columnStripeInfoCache = new TableStyleStripeInfo[5];
		bool hasHeadersRow = true;
		bool showRowStripes = true;
		bool hasTotalsRow = false;
		bool showColumnStripes = false;
		bool showFirstColumn = false;
		bool showLastColumn = false;
		#endregion
		public TableStyleViewInfo(DocumentModel documentModel, string styleName)
			: base(documentModel, styleName) {
		}
		#region TableStyleViewInfoBase<ITableStyleOwner, TableStyleFormatViewInfoCache> members
		protected override CellPosition BottomRight { get { return bottomRight; } }
		protected override ITableStyleOwner GetActiveStyleOwner(Worksheet activeSheet) {
			return activeSheet.Selection.TryGetActiveTable();
		}
		public override ActualTableStyleCellFormatting GetActualCellFormatting(CellPosition cellPosition) {
			return FormatCache.GetActualCellFormatting(cellPosition, StyleCache);
		}
		protected override TableStyleFormatViewInfoCache GetInstanceFormatCache() {
			return new TableStyleFormatViewInfoCache(this);
		}
		protected override void InitStyleOptions(ITableStyleOwner styleOwner) {
			ITableStyleOptions options = styleOwner.Options;
			this.hasHeadersRow = options.HasHeadersRow;
			this.showRowStripes = options.ShowRowStripes;
			this.hasTotalsRow = options.HasTotalsRow;
			this.showColumnStripes = options.ShowColumnStripes;
			this.showFirstColumn = options.ShowFirstColumn;
			this.showLastColumn = options.ShowLastColumn;
		}
		protected override void PrepareFormatCache() {
			FormatCache.Prepare(StyleCache);
		}
		#endregion
		#region ITableStyleOptions members
		bool ITableStyleOptions.HasHeadersRow { get { return hasHeadersRow; } }
		bool ITableStyleOptions.ShowRowStripes { get { return showRowStripes; } }
		bool ITableStyleOptions.HasTotalsRow { get { return hasTotalsRow; } }
		bool ITableStyleOptions.ShowColumnStripes { get { return showColumnStripes; } }
		bool ITableStyleOptions.ShowFirstColumn { get { return showFirstColumn; } }
		bool ITableStyleOptions.ShowLastColumn { get { return showLastColumn; } }
		#endregion
		#region ITableStyleOwner Members
		CellRange ITableStyleOwner.Range { get { return Range; } }
		ITableStyleOptions ITableStyleOwner.Options { get { return this; } }
		TableStyleFormatCache ITableStyleOwner.Cache { get { return FormatCache; } }
		void ITableStyleOwner.CacheColumnStripeInfo(int index, TableStyleStripeInfo info) {
			columnStripeInfoCache[index] = info;
		}
		TableStyleStripeInfo ITableStyleOwner.GetColumnStripeInfo(int index) {
			return columnStripeInfoCache[index];
		}
		#endregion
	}
	#endregion
	#region PivotStyleViewInfo
	public class PivotStyleViewInfo : TableStyleViewInfoBase<IPivotStyleOwner, PivotStyleFormatViewInfoCache>, IPivotStyleOwner, IPivotStyleOptions, IPivotTableLocation {
		#region Fields
		readonly CellPosition bottomRight = new CellPosition(4, 7);
		readonly TableStyleStripeInfo[] columnStripeInfoCache = new TableStyleStripeInfo[5];
		readonly TableStyleStripeInfo[] rowStripeInfoCache = new TableStyleStripeInfo[8];
		readonly PivotTableLocationCache locationCache = new PivotTableLocationCache();
		readonly int columnFieldsCount = 1;
		readonly int rowFieldsCount = 2;
		readonly int dataFieldsCount = 1;
		readonly int pageFieldsCount = 0;
		readonly int firstDataColumn = 1;
		readonly int firstDataRow = 1;
		bool showColumnHeaders = true;
		bool showColumnStripes = false;
		bool showRowHeaders = true;
		bool showRowStripes = false;
		bool showLastColumn = true;
		bool hasColumnGrandTotals = true;
		bool hasRowGrandTotals = true;
		PivotZoneFormattingCache pivotZoneCache;
		#endregion
		public PivotStyleViewInfo(DocumentModel documentModel, string styleName)
			: base(documentModel, styleName) {
		}
		#region TableStyleViewInfoBase<IPivotStyleOwner, PivotStyleFormatViewInfoCache> members
		protected override CellPosition BottomRight { get { return bottomRight; } }
		protected override IPivotStyleOwner GetActiveStyleOwner(Worksheet activeSheet) {
			PivotTable table = activeSheet.TryGetPivotTable(activeSheet.Selection.ActiveCell);
			if (table == null)
				return null;
			return table.CalculationInfo;
		}
		public override ActualTableStyleCellFormatting GetActualCellFormatting(CellPosition cellPosition) {
			return FormatCache.GetActualCellFormatting(cellPosition, StyleCache);
		}
		protected override PivotStyleFormatViewInfoCache GetInstanceFormatCache() {
			return new PivotStyleFormatViewInfoCache(this);
		}
		protected override void InitStyleOptions(IPivotStyleOwner styleOwner) {
			IPivotStyleOptions options = styleOwner.Options;
			this.showColumnHeaders = options.ShowColumnHeaders;
			this.showColumnStripes = options.ShowColumnStripes;
			this.showRowHeaders = options.ShowRowHeaders;
			this.showRowStripes = options.ShowRowStripes;
			this.hasColumnGrandTotals = options.HasColumnGrandTotals;
			this.hasRowGrandTotals = options.HasRowGrandTotals;
		}
		protected override void PrepareFormatCache() {
			FormatCache.Prepare(StyleCache);
			locationCache.FirstHeaderCell = new PivotTableLocationZone(new CellPosition(0, 0), new CellPosition(0, 0)); 
		}
		#endregion
		#region IPivotStyleOwner Members
		IPivotStyleOptions IPivotStyleOwner.Options { get { return this; } }
		IPivotTableLocation IPivotStyleOwner.Location { get { return this; } }
		PivotStyleFormatCache IPivotStyleOwner.StyleFormatCache { get { return FormatCache; } }
		PivotZoneFormattingCache IPivotStyleOwner.PivotZoneCache { get { return pivotZoneCache; } set { pivotZoneCache = value; } }
		int IPivotStyleOwner.ColumnFieldsCount { get { return columnFieldsCount; } }
		int IPivotStyleOwner.RowFieldsCount { get { return rowFieldsCount; } }
		int IPivotStyleOwner.DataFieldsCount { get { return dataFieldsCount; } }
		int IPivotStyleOwner.PageFieldsCount { get { return pageFieldsCount; } }
		void IPivotStyleOwner.CacheColumnItemStripeInfo(int columnItemIndex, TableStyleStripeInfo info) {
			columnStripeInfoCache[columnItemIndex] = info;
		}
		TableStyleStripeInfo IPivotStyleOwner.GetColumnItemStripeInfo(int columnItemIndex) {
			return columnStripeInfoCache[columnItemIndex];
		}
		void IPivotStyleOwner.CacheRowItemStripeInfo(int rowItemIndex, TableStyleStripeInfo info) {
			rowStripeInfoCache[rowItemIndex] = info;
		}
		TableStyleStripeInfo IPivotStyleOwner.GetRowItemStripeInfo(int rowItemIndex) {
			return rowStripeInfoCache[rowItemIndex];
		}
		int IPivotStyleOwner.GetDataColumnItemIndex(int absoluteDataIndex) {
			return absoluteDataIndex - firstDataColumn;
		}
		int IPivotStyleOwner.GetDataRowItemIndex(int absoluteDataIndex) {
			return absoluteDataIndex - firstDataRow;
		}
		IPivotZone IPivotStyleOwner.GetPivotZoneByCellPosition(CellPosition cellPosition) {
			return PivotZoneViewInfo.TryCreate(cellPosition, this);
		}
		#endregion
		#region IPivotStyleOptions Members
		bool IPivotStyleOptions.ShowColumnHeaders { get { return showColumnHeaders; } }
		bool IPivotStyleOptions.ShowColumnStripes { get { return showColumnStripes; } }
		bool IPivotStyleOptions.ShowRowHeaders { get { return showRowHeaders; } }
		bool IPivotStyleOptions.ShowRowStripes { get { return showRowStripes; } }
		bool IPivotStyleOptions.ShowLastColumn { get { return showLastColumn; } }
		bool IPivotStyleOptions.HasColumnGrandTotals { get { return hasColumnGrandTotals; } }
		bool IPivotStyleOptions.HasRowGrandTotals { get { return hasRowGrandTotals; } }
		#endregion
		#region IPivotTableLocation Members
		int IPivotTableLocation.FirstDataColumn { get { return firstDataColumn; } }
		int IPivotTableLocation.FirstDataRow { get { return firstDataRow; } }
		PivotTableLocationCache IPivotTableLocation.Cache { get { return locationCache; } }
		#endregion
	}
	#endregion
	#region TableStyleViewInfoFillPropertyAccessor
	internal class TableStyleViewInfoFillPropertyAccessor : IActualFillInfo, IActualGradientFillInfo, IActualConvergenceInfo {
		CellPosition position;
		ITableBase table;
		public IActualFillInfo GetInfo(ITableBase table, CellPosition position) {
			this.position = position;
			this.table = table;
			return this;
		}
		DocumentModel DocumentModel { get { return table.WholeRange.Worksheet.Workbook.StyleSheet.Workbook; } }
		#region Helper Methods
		Color GetColor(int colorIndex) {
			return DocumentModel.Cache.ColorModelInfoCache[colorIndex].ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		int GetColorIndex(DifferentialFormatPropertyDescriptor descriptor) {
			return GetValue(descriptor, ColorModelInfoCache.DefaultItemIndex);
		}
		T GetValue<T>(DifferentialFormatPropertyDescriptor descriptor, T defaultValue) {
			return TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, descriptor, position, defaultValue);
		}
		bool GetApply(DifferentialFormatPropertyDescriptor descriptor) {
			return TableStyleFormatBuilderFactory.ApplyPropertyBuilder.Build(table, descriptor, position);
		}
		float GetConvergence(DifferentialFormatPropertyDescriptor descriptor) {
			return GetValue(descriptor, GradientFillInfo.DefaultConvergenceValue);
		}
		#endregion
		#region IActualFillInfo Members
		public XlPatternType PatternType { get { return GetValue(DifferentialFormatPropertyDescriptor.FillPatternType, XlPatternType.None); } }
		public Color ForeColor { get { return GetColor(ForeColorIndex); } }
		public Color BackColor { get { return GetColor(BackColorIndex); } }
		public int ForeColorIndex { get { return GetColorIndex(DifferentialFormatPropertyDescriptor.FillForeColorIndex); } }
		public int BackColorIndex { get { return GetColorIndex(DifferentialFormatPropertyDescriptor.FillBackColorIndex); } }
		public bool ApplyPatternType { get { return GetApply(DifferentialFormatPropertyDescriptor.FillPatternType); } }
		public bool ApplyBackColor { get { return GetApply(DifferentialFormatPropertyDescriptor.FillBackColorIndex); } }
		public bool ApplyForeColor { get { return GetApply(DifferentialFormatPropertyDescriptor.FillForeColorIndex); } }
		public bool IsDifferential { get { return true; } }
		public IActualGradientFillInfo GradientFill { get { return this; } }
		public ModelFillType FillType { get { return GetValue(DifferentialFormatPropertyDescriptor.FillType, ModelFillType.Pattern); } }
		#endregion
		#region IActualGradientFill
		public ModelGradientFillType Type { get { return GetValue(DifferentialFormatPropertyDescriptor.FillGradientFillType, ModelGradientFillType.Linear); } }
		public IActualConvergenceInfo Convergence { get { return this; } }
		public double Degree { get { return GetValue(DifferentialFormatPropertyDescriptor.FillGradientFillDegree, GradientFillInfo.DefaultDegree); } }
		public IActualGradientStopCollection GradientStops { get { return TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, DifferentialFormatPropertyDescriptor.FillGradientFillGradientStops, position, new GradientStopInfoCollection(DocumentModel)); } }
		#endregion
		#region IActualConvergenceInfo Members
		public float Left { get { return GetConvergence(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceLeft); } }
		public float Right { get { return GetConvergence(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceRight); } }
		public float Top { get { return GetConvergence(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceTop); } }
		public float Bottom { get { return GetConvergence(DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceBottom); } }
		#endregion
	}
	#endregion
	#region ITableStylePropertyChanger
	public interface ITableStylePropertyChanger : IDifferentialFormatPropertyChanger {
		#region StripeSize
		int GetStripeSize(int elementIndex);
		void SetStripeSize(int elementIndex, int value);
		#endregion
	}
	#endregion
	#region ITableStyle
	public interface ITableStyle {
		ITableStyleElementFormat this[int elementIndex] { get; }
		TableStyleName Name { get; set; }
		bool IsHidden { get; set; }
		TableStyleElementIndexTableType TableType { get; }
		bool IsRegistered { get; }
		bool IsPredefined { get; }
		ITableStyleElementFormat WholeTable { get; }
		ITableStyleElementFormat HeaderRow { get; }
		ITableStyleElementFormat TotalRow { get; }
		ITableStyleElementFormat FirstColumn { get; }
		ITableStyleElementFormat LastColumn { get; }
		ITableStyleElementFormat FirstRowStripe { get; }
		ITableStyleElementFormat SecondRowStripe { get; }
		ITableStyleElementFormat FirstColumnStripe { get; }
		ITableStyleElementFormat SecondColumnStripe { get; }
		ITableStyleElementFormat FirstHeaderCell { get; }
		ITableStyleElementFormat LastHeaderCell { get; }
		ITableStyleElementFormat FirstTotalCell { get; }
		ITableStyleElementFormat LastTotalCell { get; }
		ITableStyleElementFormat FirstSubtotalColumn { get; }
		ITableStyleElementFormat SecondSubtotalColumn { get; }
		ITableStyleElementFormat ThirdSubtotalColumn { get; }
		ITableStyleElementFormat FirstSubtotalRow { get; }
		ITableStyleElementFormat SecondSubtotalRow { get; }
		ITableStyleElementFormat ThirdSubtotalRow { get; }
		ITableStyleElementFormat BlankRow { get; }
		ITableStyleElementFormat FirstColumnSubheading { get; }
		ITableStyleElementFormat SecondColumnSubheading { get; }
		ITableStyleElementFormat ThirdColumnSubheading { get; }
		ITableStyleElementFormat FirstRowSubheading { get; }
		ITableStyleElementFormat SecondRowSubheading { get; }
		ITableStyleElementFormat ThirdRowSubheading { get; }
		ITableStyleElementFormat PageFieldLabels { get; }
		ITableStyleElementFormat PageFieldValues { get; }
	}
	#endregion
	#region TableStyleElementIndexTableType (enum) 2 bits
	public enum TableStyleElementIndexTableType {
		General,
		Pivot,
		Table,
	}
	#endregion
	#region TableStyleElementIndexAccessor
	public class TableStyleElementIndexAccessor : IIndexAccessor<TableStyle, TableStyleElementFormat, DocumentModelChangeActions> {
		readonly int elementIndex;
		public TableStyleElementIndexAccessor(int elementIndex) {
			this.elementIndex = elementIndex;
		}
		protected int ElementIndex { get { return elementIndex; } }
		#region IIndexAccessor Members
		public int GetIndex(TableStyle owner) {
			return owner.FormatIndexes[elementIndex];
		}
		public void SetIndex(TableStyle owner, int value) {
			owner.AssignFormatIndex(elementIndex, value);
		}
		public int GetDeferredInfoIndex(TableStyle owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public TableStyleElementFormat GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableStyleBatchUpdateHelper)helper).GetInfo(elementIndex);
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TableStyleElementFormat info) {
			((TableStyleBatchUpdateHelper)helper).SetInfo(elementIndex, info.Clone());
		}
		public int GetInfoIndex(TableStyle owner, TableStyleElementFormat value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public TableStyleElementFormat GetInfo(TableStyle owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(TableStyle owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<TableStyleElementFormat> GetInfoCache(TableStyle owner) {
			return owner.DocumentModel.Cache.TableStyleElementFormatCache;
		}
		public void InitializeDeferredInfo(TableStyle owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(TableStyle owner, TableStyle from) {
			TableStyleElementFormat cloneFrom = GetInfo(from);
			if (!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel)) {
				TableStyleElementFormat cloneOwner = new TableStyleElementFormat(owner.DocumentModel);
				cloneOwner.AssignDifferentialFormatIndex(ConvertDifferentialFormatIndex(from.DocumentModel, cloneFrom.DifferentialFormatIndex, owner.DocumentModel));
				SetDeferredInfo(owner.BatchUpdateHelper, cloneOwner);
			} else
				SetDeferredInfo(owner.BatchUpdateHelper, cloneFrom);
		}
		int ConvertDifferentialFormatIndex(DocumentModel source, int differentialFormatIndex, DocumentModel target) {
			DifferentialFormat sourceFormat = (DifferentialFormat)source.Cache.CellFormatCache[differentialFormatIndex];
			DifferentialFormat targetFormat = new DifferentialFormat(target);
			targetFormat.CopyFrom(sourceFormat);
			return target.Cache.CellFormatCache.GetItemIndex(targetFormat);
		}
		public bool ApplyDeferredChanges(TableStyle owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableStyle owner) {
			return new TableStyleHistoryItem(owner, elementIndex);
		}
		#endregion
	}
	#endregion
	#region TableStyleBatchUpdateHelper
	public class TableStyleBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		TableStyleElementFormat[] tableStyleElementFormats;
		public TableStyleBatchUpdateHelper(IBatchUpdateHandler handler, int elementsCount)
			: base(handler) {
			this.tableStyleElementFormats = new TableStyleElementFormat[elementsCount];
		}
		public TableStyleElementFormat GetInfo(int elementIndex) {
			return tableStyleElementFormats[elementIndex];
		}
		public void SetInfo(int elementIndex, TableStyleElementFormat value) {
			tableStyleElementFormats[elementIndex] = value;
		}
	}
	#endregion
	#region TableStyleBatchInitHelper
	public class TableStyleBatchInitHelper : FormatBaseBatchUpdateHelper {
		public TableStyleBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region TableStyle
	public class TableStyle : MultiIndexObject<TableStyle, DocumentModelChangeActions>, ITableStyle, ITableStylePropertyChanger {
		#region Static Members
		public static TableStyle CreateDefaultStyle(DocumentModel documentModel) {
			return new TableStyle(documentModel, TableStyleName.DefaultStyleName, TableStyleElementIndexTableType.General);
		}
		public static bool CheckDefaultStyle(TableStyle style) {
			bool result = style.IsGeneralStyle && !style.isHidden && style.name.IsDefault;
			if (!result)
				return false;
			return style.CheckDefaultFormatIndexes();
		}
		#region Create
		public static TableStyle CreatePivotPredefinedStyle(DocumentModel documentModel, PredefinedPivotStyleId id) {
			return CreatePivotPredefinedStyle(documentModel, TableStyleName.CreatePredefined(id));
		}
		public static TableStyle CreatePivotPredefinedStyle(DocumentModel documentModel, TableStyleName name) {
			return new TableStyle(documentModel, name, TableStyleElementIndexTableType.Pivot);
		}
		public static TableStyle CreateTablePredefinedStyle(DocumentModel documentModel, PredefinedTableStyleId id) {
			return CreateTablePredefinedStyle(documentModel, TableStyleName.CreatePredefined(id));
		}
		public static TableStyle CreateTablePredefinedStyle(DocumentModel documentModel, TableStyleName name) {
			return new TableStyle(documentModel, name, TableStyleElementIndexTableType.Table);
		}
		public static TableStyle CreatePivotCustomStyle(DocumentModel documentModel, string name) {
			TableStyleName styleName = TableStyleName.CreateCustom(name);
			return new TableStyle(documentModel, styleName, TableStyleElementIndexTableType.Pivot);
		}
		public static TableStyle CreateTableCustomStyle(DocumentModel documentModel, string name) {
			TableStyleName styleName = TableStyleName.CreateCustom(name);
			return new TableStyle(documentModel, styleName, TableStyleElementIndexTableType.Table);
		}
		public static TableStyle CreatePivotStyle(DocumentModel documentModel, string name) {
			TableStyleName styleName = TableStyleName.CreatePivotName(name);
			return new TableStyle(documentModel, styleName, TableStyleElementIndexTableType.Pivot);
		}
		public static TableStyle CreateTableStyle(DocumentModel documentModel, string name) {
			TableStyleName styleName = TableStyleName.CreateTableName(name);
			return new TableStyle(documentModel, styleName, TableStyleElementIndexTableType.Table);
		}
		public static TableStyle CreateCustomStyle(DocumentModel documentModel, string name, TableStyleElementIndexTableType category) {
			TableStyleName styleName = TableStyleName.CreateCustom(name);
			return new TableStyle(documentModel, styleName, category);
		}
		#endregion
		#region Accessors
		readonly static IIndexAccessorBase<TableStyle, DocumentModelChangeActions>[] indexAccessors = GetIndexAccessors();
		static IIndexAccessorBase<TableStyle, DocumentModelChangeActions>[] GetIndexAccessors() {
			IIndexAccessorBase<TableStyle, DocumentModelChangeActions>[] result = new IIndexAccessorBase<TableStyle, DocumentModelChangeActions>[ElementsCount];
			for (int i = 0; i < ElementsCount; i++)
				result[i] = new TableStyleElementIndexAccessor(i);
			return result;
		}
		public static IIndexAccessorBase<TableStyle, DocumentModelChangeActions>[] TableStyleIndexAccessors { get { return indexAccessors; } }
		#endregion
		#region ElementIndexToCategoryTranslationTable
		protected internal static Dictionary<int, TableStyleElementIndexTableType> ElementIndexToCategoryTranslationTable = GetElementIndexToCategoryTranslationTable();
		static Dictionary<int, TableStyleElementIndexTableType> GetElementIndexToCategoryTranslationTable() {
			Dictionary<int, TableStyleElementIndexTableType> result = new Dictionary<int, TableStyleElementIndexTableType>();
			result.Add(WholeTableIndex, TableStyleElementIndexTableType.General);
			result.Add(HeaderRowIndex, TableStyleElementIndexTableType.General);
			result.Add(TotalRowIndex, TableStyleElementIndexTableType.General);
			result.Add(FirstColumnIndex, TableStyleElementIndexTableType.General);
			result.Add(LastColumnIndex, TableStyleElementIndexTableType.General);
			result.Add(FirstRowStripeIndex, TableStyleElementIndexTableType.General);
			result.Add(SecondRowStripeIndex, TableStyleElementIndexTableType.General);
			result.Add(FirstColumnStripeIndex, TableStyleElementIndexTableType.General);
			result.Add(SecondColumnStripeIndex, TableStyleElementIndexTableType.General);
			result.Add(FirstHeaderCellIndex, TableStyleElementIndexTableType.General);
			result.Add(LastHeaderCellIndex, TableStyleElementIndexTableType.Table);
			result.Add(FirstTotalCellIndex, TableStyleElementIndexTableType.Table);
			result.Add(LastTotalCellIndex, TableStyleElementIndexTableType.Table);
			result.Add(FirstSubtotalColumnIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(SecondSubtotalColumnIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(ThirdSubtotalColumnIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(FirstSubtotalRowIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(SecondSubtotalRowIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(ThirdSubtotalRowIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(BlankRowIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(FirstColumnSubheadingIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(SecondColumnSubheadingIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(ThirdColumnSubheadingIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(FirstRowSubheadingIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(SecondRowSubheadingIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(ThirdRowSubheadingIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(PageFieldLabelsIndex, TableStyleElementIndexTableType.Pivot);
			result.Add(PageFieldValuesIndex, TableStyleElementIndexTableType.Pivot);
			return result;
		}
		#endregion
		#endregion
		#region Fields
		#region Const
		internal const int ElementsCount = 28;
		internal const int WholeTableIndex = 0;
		internal const int HeaderRowIndex = 1;
		internal const int TotalRowIndex = 2;
		internal const int FirstColumnIndex = 3;
		internal const int LastColumnIndex = 4;
		internal const int FirstRowStripeIndex = 5;
		internal const int SecondRowStripeIndex = 6;
		internal const int FirstColumnStripeIndex = 7;
		internal const int SecondColumnStripeIndex = 8;
		internal const int FirstHeaderCellIndex = 9;
		internal const int LastHeaderCellIndex = 10;
		internal const int FirstTotalCellIndex = 11;
		internal const int LastTotalCellIndex = 12;
		internal const int FirstSubtotalColumnIndex = 13;
		internal const int SecondSubtotalColumnIndex = 14;
		internal const int ThirdSubtotalColumnIndex = 15;
		internal const int FirstSubtotalRowIndex = 16;
		internal const int SecondSubtotalRowIndex = 17;
		internal const int ThirdSubtotalRowIndex = 18;
		internal const int BlankRowIndex = 19;
		internal const int FirstColumnSubheadingIndex = 20;
		internal const int SecondColumnSubheadingIndex = 21;
		internal const int ThirdColumnSubheadingIndex = 22;
		internal const int FirstRowSubheadingIndex = 23;
		internal const int SecondRowSubheadingIndex = 24;
		internal const int ThirdRowSubheadingIndex = 25;
		internal const int PageFieldLabelsIndex = 26;
		internal const int PageFieldValuesIndex = 27;
		#endregion
		readonly DocumentModel documentModel;
		readonly int[] formatIndexes;
		readonly TableStyleBaseElementChangeManager elementChangeManager;
		readonly TableStyleElementInfoCache cache;
		TableStyleName name;
		TableStyleElementIndexTableType tableType;
		bool isHidden;
		bool isRegistered;
		#endregion
		public TableStyle(DocumentModel documentModel, TableStyleName name, TableStyleElementIndexTableType tableType) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.tableType = tableType;
			formatIndexes = new int[ElementsCount];
			InitialFormatIndexes();
			elementChangeManager = new TableStyleBaseElementChangeManager(this);
			cache = new TableStyleElementInfoCache();
			this.name = name;
			CalculateProperties();
		}
		#region Properties
		protected internal int[] FormatIndexes { get { return formatIndexes; } }
		internal new TableStyleBatchUpdateHelper BatchUpdateHelper { get { return (TableStyleBatchUpdateHelper)base.BatchUpdateHelper; } }
		internal new DocumentModel DocumentModel { get { return documentModel; } }
		protected override IIndexAccessorBase<TableStyle, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		TableStyleCollection Styles { get { return DocumentModel.StyleSheet.TableStyles; } }
		public bool IsDefault { get { return CheckDefaultStyle(this); } }
		public bool IsTableStyle { get { return tableType == TableStyleElementIndexTableType.Table; } }
		public bool IsPivotStyle { get { return tableType == TableStyleElementIndexTableType.Pivot; } }
		public bool IsGeneralStyle { get { return tableType == TableStyleElementIndexTableType.General; } }
		protected internal TableStyleElementInfoCache Cache {
			get {
				if (cache.IsValid)
					return cache;
				cache.Prepare(this);
				return cache;
			}
		}
		#endregion
		#region ITableStyle Members
		public TableStyleName Name { get { return name; } set { Rename(value); } }
		public bool IsRegistered { get { return isRegistered; } set { isRegistered = value; } }
		public bool IsHidden { get { return isHidden; } set { isHidden = value; } }
		public bool IsPredefined { get { return name.IsPredefined; } }
		public TableStyleElementIndexTableType TableType { get { return tableType; } set { SetTableType(value); } }
		public ITableStyleElementFormat this[int elementIndex] { get { return GetTableStyleElementFormat(elementIndex); } }
		public ITableStyleElementFormat WholeTable { get { return GetTableStyleElementFormat(WholeTableIndex); } }
		public ITableStyleElementFormat HeaderRow { get { return GetTableStyleElementFormat(HeaderRowIndex); } }
		public ITableStyleElementFormat TotalRow { get { return GetTableStyleElementFormat(TotalRowIndex); } }
		public ITableStyleElementFormat FirstColumn { get { return GetTableStyleElementFormat(FirstColumnIndex); } }
		public ITableStyleElementFormat LastColumn { get { return GetTableStyleElementFormat(LastColumnIndex); } }
		public ITableStyleElementFormat FirstRowStripe { get { return GetTableStyleElementFormat(FirstRowStripeIndex); } }
		public ITableStyleElementFormat SecondRowStripe { get { return GetTableStyleElementFormat(SecondRowStripeIndex); } }
		public ITableStyleElementFormat FirstColumnStripe { get { return GetTableStyleElementFormat(FirstColumnStripeIndex); } }
		public ITableStyleElementFormat SecondColumnStripe { get { return GetTableStyleElementFormat(SecondColumnStripeIndex); } }
		public ITableStyleElementFormat FirstHeaderCell { get { return GetTableStyleElementFormat(FirstHeaderCellIndex); } }
		public ITableStyleElementFormat LastHeaderCell { get { return GetTableStyleElementFormat(LastHeaderCellIndex); } }
		public ITableStyleElementFormat FirstTotalCell { get { return GetTableStyleElementFormat(FirstTotalCellIndex); } }
		public ITableStyleElementFormat LastTotalCell { get { return GetTableStyleElementFormat(LastTotalCellIndex); } }
		public ITableStyleElementFormat FirstSubtotalColumn { get { return GetTableStyleElementFormat(FirstSubtotalColumnIndex); } }
		public ITableStyleElementFormat SecondSubtotalColumn { get { return GetTableStyleElementFormat(SecondSubtotalColumnIndex); } }
		public ITableStyleElementFormat ThirdSubtotalColumn { get { return GetTableStyleElementFormat(ThirdSubtotalColumnIndex); } }
		public ITableStyleElementFormat FirstSubtotalRow { get { return GetTableStyleElementFormat(FirstSubtotalRowIndex); } }
		public ITableStyleElementFormat SecondSubtotalRow { get { return GetTableStyleElementFormat(SecondSubtotalRowIndex); } }
		public ITableStyleElementFormat ThirdSubtotalRow { get { return GetTableStyleElementFormat(ThirdSubtotalRowIndex); } }
		public ITableStyleElementFormat BlankRow { get { return GetTableStyleElementFormat(BlankRowIndex); } }
		public ITableStyleElementFormat FirstColumnSubheading { get { return GetTableStyleElementFormat(FirstColumnSubheadingIndex); } }
		public ITableStyleElementFormat SecondColumnSubheading { get { return GetTableStyleElementFormat(SecondColumnSubheadingIndex); } }
		public ITableStyleElementFormat ThirdColumnSubheading { get { return GetTableStyleElementFormat(ThirdColumnSubheadingIndex); } }
		public ITableStyleElementFormat FirstRowSubheading { get { return GetTableStyleElementFormat(FirstRowSubheadingIndex); } }
		public ITableStyleElementFormat SecondRowSubheading { get { return GetTableStyleElementFormat(SecondRowSubheadingIndex); } }
		public ITableStyleElementFormat ThirdRowSubheading { get { return GetTableStyleElementFormat(ThirdRowSubheadingIndex); } }
		public ITableStyleElementFormat PageFieldLabels { get { return GetTableStyleElementFormat(PageFieldLabelsIndex); } }
		public ITableStyleElementFormat PageFieldValues { get { return GetTableStyleElementFormat(PageFieldValuesIndex); } }
		#endregion
		#region MultiIndexObject<TableStyleBase, DocumentModelChangeActions> Members
		protected override IDocumentModel GetDocumentModel() {
			return documentModel;
		}
		public override TableStyle GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new TableStyleBatchUpdateHelper(this, ElementsCount);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new TableStyleBatchInitHelper(this);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		#endregion
		protected internal TableStyleElementFormat GetElementFormat(int elementIndex) {
			if (BatchUpdateHelper != null)
				return BatchUpdateHelper.GetInfo(elementIndex);
			return DocumentModel.Cache.TableStyleElementFormatCache[formatIndexes[elementIndex]];
		}
		protected internal void ClearElementFormat(int elementIndex) {
			TableStyleElementIndexAccessor accessor = GetIndexAccessor(elementIndex);
			DocumentModel.BeginUpdate();
			try {
				ReplaceInfo(accessor, DocumentModel.Cache.TableStyleElementFormatCache.DefaultItem, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ClearBorders(int elementIndex) {
			TableStyleElementIndexAccessor accessor = GetIndexAccessor(elementIndex);
			DocumentModel.BeginUpdate();
			try {
				TableStyleElementFormat info = GetInfoForModification(accessor);
				info.RemoveBorders();
				ReplaceInfo(accessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ClearFont(int elementIndex) {
			TableStyleElementIndexAccessor accessor = GetIndexAccessor(elementIndex);
			DocumentModel.BeginUpdate();
			try {
				TableStyleElementFormat info = GetInfoForModification(accessor);
				info.ClearFont();
				ReplaceInfo(accessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ClearFill(int elementIndex) {
			TableStyleElementIndexAccessor accessor = GetIndexAccessor(elementIndex);
			DocumentModel.BeginUpdate();
			try {
				TableStyleElementFormat info = GetInfoForModification(accessor);
				info.ClearFill();
				ReplaceInfo(accessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal TableStyle CloneTo(DocumentModel documentModel) {
			TableStyle result = new TableStyle(documentModel, name, tableType);
			if (!IsPredefined) {
				result.CopyFrom(this);
				return result;
			}
			return result;
		}
		protected internal IList<byte> GetNonEmptyStyleElementIndexes() {
			IList<byte> result = new List<byte>();
			for (byte i = 0; i < ElementsCount; i++) {
				if (HasTableStyleElementFormatting(i)) 
					result.Add(i);
			}
			return result;
		}
		protected internal bool HasTableStyleElementFormatting(int elementIndex) {
			return formatIndexes[elementIndex] != TableStyleElementFormatCache.DefaultItemIndex;
		}
		protected internal ITableStyleElementFormat GetTableStyleElementFormat(int elementIndex) {
			return elementChangeManager.GetFormatInfo(elementIndex);
		}
		protected internal TableStyleElementIndexAccessor GetIndexAccessor(int elementIndex) {
			return (TableStyleElementIndexAccessor)IndexAccessors[elementIndex];
		}
		protected internal void AssignFormatIndex(int elementIndex, int value) {
			this.formatIndexes[elementIndex] = value;
		}
		protected override void SetPropertyValue<TInfo, U>(IIndexAccessor<TableStyle, TInfo, DocumentModelChangeActions> indexHolder, MultiIndexObject<TableStyle, DocumentModelChangeActions>.SetPropertyValueDelegate<TInfo, U> setter, U newValue) {
			cache.SetInvalid();
			base.SetPropertyValue<TInfo, U>(indexHolder, setter, newValue);
		}
		#region Internal
		void InitialFormatIndexes() {
			for (int i = 0; i < ElementsCount; i++)
				formatIndexes[i] = TableStyleElementFormatCache.DefaultItemIndex;
		}
		bool CheckDefaultFormatIndexes() {
			for (int i = 0; i < ElementsCount; i++)
				if (HasTableStyleElementFormatting(i))
					return false;
			return true;
		}
		void CalculateProperties() {
			if (IsPredefined) {
				PredefinedTableStyleCalculator calculator = new PredefinedTableStyleCalculator(this);
				calculator.CalculateProperties();
			}
		}
		DifferentialFormat GetDifferentialFormat(int elementIndex) {
			return GetElementFormat(elementIndex).FormatInfo;
		}
		#endregion
		#region FormatStringPropertyChanger
		string IDifferentialFormatPropertyChanger.GetFormatString(int elementIndex) {
			return GetElementFormat(elementIndex).FormatString;
		}
		void IDifferentialFormatPropertyChanger.SetFormatString(int elementIndex, string value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.FormatString == value && info.MultiOptionsInfo.ApplyNumberFormat)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFormatString, value);
		}
		DocumentModelChangeActions SetFormatString(TableStyleElementFormat info, string value) {
			info.FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontInfoPropertyChanger
		#region FontName
		string IDifferentialFormatPropertyChanger.GetFontName(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Name;
		}
		void IDifferentialFormatPropertyChanger.SetFontName(int elementIndex, string value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Name == value && info.MultiOptionsInfo.ApplyFontName)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontNameCore, value);
		}
		DocumentModelChangeActions SetFontNameCore(TableStyleElementFormat info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSize
		double IDifferentialFormatPropertyChanger.GetFontSize(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Size;
		}
		void IDifferentialFormatPropertyChanger.SetFontSize(int elementIndex, double value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Size == value && info.MultiOptionsInfo.ApplyFontSize)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontSizeCore, value);
		}
		DocumentModelChangeActions SetFontSizeCore(TableStyleElementFormat info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontBold
		bool IDifferentialFormatPropertyChanger.GetFontBold(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Bold;
		}
		void IDifferentialFormatPropertyChanger.SetFontBold(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Bold == value && info.MultiOptionsInfo.ApplyFontBold)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontBoldCore, value);
		}
		DocumentModelChangeActions SetFontBoldCore(TableStyleElementFormat info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontItalic
		bool IDifferentialFormatPropertyChanger.GetFontItalic(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Italic;
		}
		void IDifferentialFormatPropertyChanger.SetFontItalic(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Italic == value && info.MultiOptionsInfo.ApplyFontItalic)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontItalicCore, value);
		}
		DocumentModelChangeActions SetFontItalicCore(TableStyleElementFormat info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontStrikeThrough
		bool IDifferentialFormatPropertyChanger.GetFontStrikeThrough(int elementIndex) {
			return GetElementFormat(elementIndex).Font.StrikeThrough;
		}
		void IDifferentialFormatPropertyChanger.SetFontStrikeThrough(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.StrikeThrough == value && info.MultiOptionsInfo.ApplyFontStrikeThrough)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontStrikeThroughCore, value);
		}
		DocumentModelChangeActions SetFontStrikeThroughCore(TableStyleElementFormat info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontCondense
		bool IDifferentialFormatPropertyChanger.GetFontCondense(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Condense;
		}
		void IDifferentialFormatPropertyChanger.SetFontCondense(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Condense == value && info.MultiOptionsInfo.ApplyFontCondense)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontCondenseCore, value);
		}
		DocumentModelChangeActions SetFontCondenseCore(TableStyleElementFormat info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontExtend
		bool IDifferentialFormatPropertyChanger.GetFontExtend(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Extend;
		}
		void IDifferentialFormatPropertyChanger.SetFontExtend(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Extend == value && info.MultiOptionsInfo.ApplyFontExtend)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontExtendCore, value);
		}
		DocumentModelChangeActions SetFontExtendCore(TableStyleElementFormat info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontOutline
		bool IDifferentialFormatPropertyChanger.GetFontOutline(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Outline;
		}
		void IDifferentialFormatPropertyChanger.SetFontOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Outline == value && info.MultiOptionsInfo.ApplyFontOutline)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontOutlineCore, value);
		}
		DocumentModelChangeActions SetFontOutlineCore(TableStyleElementFormat info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontShadow
		bool IDifferentialFormatPropertyChanger.GetFontShadow(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Shadow;
		}
		void IDifferentialFormatPropertyChanger.SetFontShadow(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Shadow == value && info.MultiOptionsInfo.ApplyFontShadow)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontShadowCore, value);
		}
		DocumentModelChangeActions SetFontShadowCore(TableStyleElementFormat info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontCharset
		int IDifferentialFormatPropertyChanger.GetFontCharset(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Charset;
		}
		void IDifferentialFormatPropertyChanger.SetFontCharset(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Charset == value && info.MultiOptionsInfo.ApplyFontCharset)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontCharsetCore, value);
		}
		DocumentModelChangeActions SetFontCharsetCore(TableStyleElementFormat info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontFontFamily
		int IDifferentialFormatPropertyChanger.GetFontFontFamily(int elementIndex) {
			return GetElementFormat(elementIndex).Font.FontFamily;
		}
		void IDifferentialFormatPropertyChanger.SetFontFontFamily(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.FontFamily == value && info.MultiOptionsInfo.ApplyFontFamily)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontFontFamilyCore, value);
		}
		DocumentModelChangeActions SetFontFontFamilyCore(TableStyleElementFormat info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontUnderline
		XlUnderlineType IDifferentialFormatPropertyChanger.GetFontUnderline(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Underline;
		}
		void IDifferentialFormatPropertyChanger.SetFontUnderline(int elementIndex, XlUnderlineType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Underline == value && info.MultiOptionsInfo.ApplyFontUnderline)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontUnderlineCore, value);
		}
		DocumentModelChangeActions SetFontUnderlineCore(TableStyleElementFormat info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontColor
		Color IDifferentialFormatPropertyChanger.GetFontColor(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Color;
		}
		void IDifferentialFormatPropertyChanger.SetFontColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Color == value && info.MultiOptionsInfo.ApplyFontColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontColorCore, value);
		}
		DocumentModelChangeActions SetFontColorCore(TableStyleElementFormat info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontScript
		XlScriptType IDifferentialFormatPropertyChanger.GetFontScript(int elementIndex) {
			return GetElementFormat(elementIndex).Font.Script;
		}
		void IDifferentialFormatPropertyChanger.SetFontScript(int elementIndex, XlScriptType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Script == value && info.MultiOptionsInfo.ApplyFontScript)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontScriptCore, value);
		}
		DocumentModelChangeActions SetFontScriptCore(TableStyleElementFormat info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSchemeStyle
		XlFontSchemeStyles IDifferentialFormatPropertyChanger.GetFontSchemeStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Font.SchemeStyle;
		}
		void IDifferentialFormatPropertyChanger.SetFontSchemeStyle(int elementIndex, XlFontSchemeStyles value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.SchemeStyle == value && info.MultiOptionsInfo.ApplyFontSchemeStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFontScriptCore, value);
		}
		DocumentModelChangeActions SetFontScriptCore(TableStyleElementFormat info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region FillInfoPropertyChanger
		#region ClearFill
		void IDifferentialFormatPropertyChanger.ClearFill(int elementIndex) {
			if (!GetDifferentialFormat(elementIndex).MultiOptionsInfo.ApplyFillNone)
				ClearFill(elementIndex);
		}
		#endregion
		#region FillPatternType
		XlPatternType IDifferentialFormatPropertyChanger.GetFillPatternType(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.PatternType;
		}
		void IDifferentialFormatPropertyChanger.SetFillPatternType(int elementIndex, XlPatternType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.PatternType == value && info.MultiOptionsInfo.ApplyFillPatternType)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFillPatternTypeCore, value);
		}
		DocumentModelChangeActions SetFillPatternTypeCore(TableStyleElementFormat info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FillForeColor
		Color IDifferentialFormatPropertyChanger.GetFillForeColor(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.ForeColor;
		}
		void IDifferentialFormatPropertyChanger.SetFillForeColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.ForeColor == value && info.MultiOptionsInfo.ApplyFillForeColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFillForeColorCore, value);
		}
		DocumentModelChangeActions SetFillForeColorCore(TableStyleElementFormat info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FillBackColor
		Color IDifferentialFormatPropertyChanger.GetFillBackColor(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.BackColor;
		}
		void IDifferentialFormatPropertyChanger.SetFillBackColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.BackColor == value && info.MultiOptionsInfo.ApplyFillBackColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFillBackColorCore, value);
		}
		DocumentModelChangeActions SetFillBackColorCore(TableStyleElementFormat info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region GradientFillInfoPropertyChanger
		#region GradientStops
		IGradientStopCollection IDifferentialFormatPropertyChanger.GetGradientStops(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.GradientStops;
		}
		#endregion
		#region Type
		ModelGradientFillType IDifferentialFormatPropertyChanger.GetGradientFillType(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.Type;
		}
		void IDifferentialFormatPropertyChanger.SetGradientFillType(int elementIndex, ModelGradientFillType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Type == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetGradientTypeCore, value);
		}
		DocumentModelChangeActions SetGradientTypeCore(TableStyleElementFormat info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Degree
		double IDifferentialFormatPropertyChanger.GetDegree(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.Degree;
		}
		void IDifferentialFormatPropertyChanger.SetDegree(int elementIndex, double value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Degree == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetDegreeCore, value);
		}
		DocumentModelChangeActions SetDegreeCore(TableStyleElementFormat info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceLeft
		float IDifferentialFormatPropertyChanger.GetConvergenceLeft(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.Convergence.Left;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceLeft(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Left == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetConvergenceLeftCore, value);
		}
		DocumentModelChangeActions SetConvergenceLeftCore(TableStyleElementFormat info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceRight
		float IDifferentialFormatPropertyChanger.GetConvergenceRight(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.Convergence.Right;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceRight(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Right == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetConvergenceRightCore, value);
		}
		DocumentModelChangeActions SetConvergenceRightCore(TableStyleElementFormat info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceTop
		float IDifferentialFormatPropertyChanger.GetConvergenceTop(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.Convergence.Top;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceTop(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Top == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetConvergenceTopCore, value);
		}
		DocumentModelChangeActions SetConvergenceTopCore(TableStyleElementFormat info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceBottom
		float IDifferentialFormatPropertyChanger.GetConvergenceBottom(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.GradientFill.Convergence.Bottom;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceBottom(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Bottom == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetConvergenceBottomCore, value);
		}
		DocumentModelChangeActions SetConvergenceBottomCore(TableStyleElementFormat info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region FillTypePropertyChanger
		ModelFillType IDifferentialFormatPropertyChanger.GetFillType(int elementIndex) {
			return GetElementFormat(elementIndex).Fill.FillType;
		}
		void IDifferentialFormatPropertyChanger.SetFillType(int elementIndex, ModelFillType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.FillType == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetFillTypeCore, value);
		}
		DocumentModelChangeActions SetFillTypeCore(TableStyleElementFormat info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region BorderInfoPropertyChanger
		#region BorderLeftLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderLeftLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.LeftLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftLineStyle == value && info.BorderOptionsInfo.ApplyLeftLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderLeftLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderLeftLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderRightLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.RightLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightLineStyle == value && info.BorderOptionsInfo.ApplyRightLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderRightLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderRightLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderTopLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.TopLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopLineStyle == value && info.BorderOptionsInfo.ApplyTopLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderTopLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderTopLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderBottomLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.BottomLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomLineStyle == value && info.BorderOptionsInfo.ApplyBottomLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderBottomLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderBottomLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderHorizontalLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.HorizontalLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalLineStyle == value && info.BorderOptionsInfo.ApplyHorizontalLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderHorizontalLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderVerticalLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.VerticalLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalLineStyle == value && info.BorderOptionsInfo.ApplyVerticalLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderVerticalLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalUpLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderDiagonalUpLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.DiagonalUpLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalUpLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalUpLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderDiagonalUpLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalDownLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderDiagonalDownLineStyle(int elementIndex) {
			return GetElementFormat(elementIndex).Border.DiagonalDownLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalDownLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalDownLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderDiagonalDownLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyleCore(TableStyleElementFormat info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderLeftColor
		Color IDifferentialFormatPropertyChanger.GetBorderLeftColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.LeftColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftColor == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderLeftColorCore, value);
		}
		DocumentModelChangeActions SetBorderLeftColorCore(TableStyleElementFormat info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightColor
		Color IDifferentialFormatPropertyChanger.GetBorderRightColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.RightColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightColor == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderRightColorCore, value);
		}
		DocumentModelChangeActions SetBorderRightColorCore(TableStyleElementFormat info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopColor
		Color IDifferentialFormatPropertyChanger.GetBorderTopColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.TopColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopColor == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderTopColorCore, value);
		}
		DocumentModelChangeActions SetBorderTopColorCore(TableStyleElementFormat info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomColor
		Color IDifferentialFormatPropertyChanger.GetBorderBottomColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.BottomColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomColor == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderBottomColorCore, value);
		}
		DocumentModelChangeActions SetBorderBottomColorCore(TableStyleElementFormat info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalColor
		Color IDifferentialFormatPropertyChanger.GetBorderHorizontalColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.HorizontalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalColor == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderHorizontalColorCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalColorCore(TableStyleElementFormat info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalColor
		Color IDifferentialFormatPropertyChanger.GetBorderVerticalColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.VerticalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalColor == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderVerticalColorCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalColorCore(TableStyleElementFormat info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalColor
		Color IDifferentialFormatPropertyChanger.GetBorderDiagonalColor(int elementIndex) {
			return GetElementFormat(elementIndex).Border.DiagonalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalColor == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderDiagonalColorCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalColorCore(TableStyleElementFormat info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderLeftColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderLeftColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.LeftColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftColorIndex == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderLeftColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderLeftColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderRightColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.RightColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightColorIndex == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderRightColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderRightColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderTopColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.TopColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopColorIndex == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderTopColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderTopColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderBottomColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.BottomColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomColorIndex == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderBottomColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderBottomColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderHorizontalColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.HorizontalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalColorIndex == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderHorizontalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderVerticalColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.VerticalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalColorIndex == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderVerticalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalColorIndex
		int IDifferentialFormatPropertyChanger.GetBorderDiagonalColorIndex(int elementIndex) {
			return GetElementFormat(elementIndex).Border.DiagonalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalColorIndex == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderDiagonalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndexCore(TableStyleElementFormat info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderOutline
		bool IDifferentialFormatPropertyChanger.GetBorderOutline(int elementIndex) {
			return GetElementFormat(elementIndex).Border.Outline;
		}
		void IDifferentialFormatPropertyChanger.SetBorderOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.Outline == value && info.BorderOptionsInfo.ApplyOutline)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetBorderOutlineCore, value);
		}
		DocumentModelChangeActions SetBorderOutlineCore(TableStyleElementFormat info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region CellAlignmentPropertyChanger
		#region WrapText
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentWrapText(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.WrapText;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentWrapText(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.WrapText == value && info.MultiOptionsInfo.ApplyAlignmentWrapText)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentWrapTextCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentWrapTextCore(TableStyleElementFormat info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region JustifyLastLine
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentJustifyLastLine(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.JustifyLastLine;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentJustifyLastLine(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.JustifyLastLine == value && info.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentJustifyLastLineCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentJustifyLastLineCore(TableStyleElementFormat info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ShrinkToFit
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentShrinkToFit(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.ShrinkToFit;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentShrinkToFit(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.ShrinkToFit == value && info.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentShrinkToFitCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentShrinkToFitCore(TableStyleElementFormat info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region TextRotation
		int IDifferentialFormatPropertyChanger.GetCellAlignmentTextRotation(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.TextRotation;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentTextRotation(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.TextRotation == value && info.MultiOptionsInfo.ApplyAlignmentTextRotation)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentTextRotationCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentTextRotationCore(TableStyleElementFormat info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Indent
		byte IDifferentialFormatPropertyChanger.GetCellAlignmentIndent(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.Indent;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentIndent(int elementIndex, byte value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Indent == value && info.MultiOptionsInfo.ApplyAlignmentIndent)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentIndentCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentIndentCore(TableStyleElementFormat info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region RelativeIndent
		int IDifferentialFormatPropertyChanger.GetCellAlignmentRelativeIndent(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.RelativeIndent;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentRelativeIndent(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.RelativeIndent == value && info.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentRelativeIndentCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentRelativeIndentCore(TableStyleElementFormat info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Horizontal
		XlHorizontalAlignment IDifferentialFormatPropertyChanger.GetCellAlignmentHorizontal(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.Horizontal;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentHorizontal(int elementIndex, XlHorizontalAlignment value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Horizontal == value && info.MultiOptionsInfo.ApplyAlignmentHorizontal)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentHorizontalCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentHorizontalCore(TableStyleElementFormat info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Vertical
		XlVerticalAlignment IDifferentialFormatPropertyChanger.GetCellAlignmentVertical(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.Vertical;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentVertical(int elementIndex, XlVerticalAlignment value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Vertical == value && info.MultiOptionsInfo.ApplyAlignmentVertical)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentVerticalCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentVerticalCore(TableStyleElementFormat info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlReadingOrder
		XlReadingOrder IDifferentialFormatPropertyChanger.GetCellAlignmentReadingOrder(int elementIndex) {
			return GetElementFormat(elementIndex).Alignment.ReadingOrder;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentReadingOrder(int elementIndex, XlReadingOrder value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.ReadingOrder == value && info.MultiOptionsInfo.ApplyAlignmentReadingOrder)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellAlignmentReadingOrderCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentReadingOrderCore(TableStyleElementFormat info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region CellProtectionPropertyChanger
		#region Locked
		bool IDifferentialFormatPropertyChanger.GetCellProtectionLocked(int elementIndex) {
			return GetElementFormat(elementIndex).Protection.Locked;
		}
		void IDifferentialFormatPropertyChanger.SetCellProtectionLocked(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Protection.Locked == value && info.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellProtectionLockedCore, value);
		}
		DocumentModelChangeActions SetCellProtectionLockedCore(TableStyleElementFormat info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Hidden
		bool IDifferentialFormatPropertyChanger.GetCellProtectionHidden(int elementIndex) {
			return GetElementFormat(elementIndex).Protection.Hidden;
		}
		void IDifferentialFormatPropertyChanger.SetCellProtectionHidden(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Protection.Hidden == value && info.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetCellProtectionHiddenCore, value);
		}
		DocumentModelChangeActions SetCellProtectionHiddenCore(TableStyleElementFormat info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region StripeSizeInfoPropertyChanger
		int ITableStylePropertyChanger.GetStripeSize(int elementIndex) {
			return GetElementFormat(elementIndex).StripeSize;
		}
		void ITableStylePropertyChanger.SetStripeSize(int elementIndex, int value) {
			if (GetElementFormat(elementIndex).StripeSize == value)
				return;
			SetPropertyValue(GetIndexAccessor(elementIndex), SetStripeSizeCore, value);
		}
		DocumentModelChangeActions SetStripeSizeCore(TableStyleElementFormat info, int value) {
			((ITableStyleElementFormat)info).StripeSize = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Operations
		#region Duplicate
		public TableStyle Duplicate() {
			return DuplicateCore(GetDuplicatedStyleName());
		}
		public TableStyle Duplicate(string name) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			if (Name.IsEquals(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDuplicateStyleName);
			TableStyleName newName = TableStyleName.CreateCustom(name);
			return DuplicateCore(name);
		}
		string GetDuplicatedStyleName() {
			if (IsPivotStyle)
				return DocumentModel.GetService<IPivotStyleNameCreationService>().GetDuplicatePivotStyleName(this);
			return DocumentModel.GetService<ITableStyleNameCreationService>().GetDuplicateTableStyleName(this);
		}
		TableStyle DuplicateCore(string name) {
			System.Diagnostics.Debug.Assert(!IsUpdateLocked);
			TableStyle result = TableStyle.CreateCustomStyle(DocumentModel, name, tableType);
			result.IsHidden = IsHidden;
			for (int i = 0; i < ElementsCount; i++)
				result.AssignFormatIndex(i, FormatIndexes[i]);
			return result;
		}
		#endregion
		#region Rename
		void Rename(TableStyleName name) {
			if (this.name.IsEquals(name))
				return;
			RenameCore(name);
		}
		void RenameCore(TableStyleName name) {
			if (IsRegistered)
				RenameRegisteredStyle(name);
			else
				SetName(name);
		}
		void RenameRegisteredStyle(TableStyleName name) {
			DocumentModel.BeginUpdate();
			try {
				Styles.Remove(Name.Name);
				TableStyleName oldName = Name;
				SetName(name);
				Styles.Add(this);
				string newName = Name.Name;
				if (IsTableStyle) {
					SetDefaultTableStyleName(newName, oldName);
					ApplyToTables(oldName);
				}
				if (IsPivotStyle) {
					SetDefaultPivotStyleName(newName, oldName);
					ApplyToPivots(oldName);
				}
				if (IsGeneralStyle) {
					SetDefaultTableStyleName(newName, oldName);
					SetDefaultPivotStyleName(newName, oldName);
					ApplyToTables(oldName);
					ApplyToPivots(oldName);
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		void SetDefaultTableStyleName(string newName, TableStyleName oldName) {
			if (oldName.IsEquals(Styles.CachedDefaultTableStyleName))
				Styles.SetDefaultTableStyleName(newName);
		}
		void ApplyToTables(TableStyleName oldName) {
			foreach (Worksheet sheet in DocumentModel.Sheets)
				foreach (Table table in sheet.Tables)
					if (oldName.IsEquals(table.TableInfo.StyleName))
						table.Style = this;
		}
		void SetDefaultPivotStyleName(string newName, TableStyleName oldName) {
			if (oldName.IsEquals(Styles.CachedDefaultPivotStyleName))
				Styles.SetDefaultPivotStyleName(newName);
		}
		void ApplyToPivots(TableStyleName oldName) {
		}
		#endregion
		#region Remove
		public void Remove() {
			DocumentModel.BeginUpdate();
			try {
				string defaultStyleName = TableStyleName.DefaultStyleName.Name;
				if (IsTableStyle) {
					SetDefaultTableStyleName(defaultStyleName, Name);
					Styles.DefaultTableStyle.ApplyToTables(Name);
				}
				if (IsPivotStyle) {
					SetDefaultPivotStyleName(defaultStyleName, Name);
					Styles.DefaultPivotStyle.ApplyToPivots(Name);
				}
				if (IsGeneralStyle) {
					SetDefaultTableStyleName(defaultStyleName, Name);
					SetDefaultPivotStyleName(defaultStyleName, Name);
					Styles.DefaultTableStyle.ApplyToTables(Name);
					Styles.DefaultPivotStyle.ApplyToPivots(Name);
				}
				Styles.Remove(Name.Name);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#endregion
		#region RangeHelper
		protected internal CellRangeInfo TryGetRange(int elementIndex, CellPosition cellPosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow, bool showColumnStripes, bool showRowStripes, bool showFirstColumn, bool showLastColumn) {
			if (elementIndex == TableStyle.WholeTableIndex)
				return tableRange;
			if (elementIndex == TableStyle.FirstColumnStripeIndex)
				return TryGetFirstColumnStripeRange(cellPosition, tableRange, hasHeaderRow, hasTotalsRow, showColumnStripes);
			if (elementIndex == TableStyle.SecondColumnStripeIndex)
				return TryGetSecondColumnStripeRange(cellPosition, tableRange, hasHeaderRow, hasTotalsRow, showColumnStripes);
			if (elementIndex == TableStyle.FirstRowStripeIndex)
				return TryGetFirstRowStripeRange(cellPosition, tableRange, hasHeaderRow, hasTotalsRow, showRowStripes);
			if (elementIndex == TableStyle.SecondRowStripeIndex)
				return TryGetSecondRowStripeRange(cellPosition, tableRange, hasHeaderRow, hasTotalsRow, showRowStripes);
			if (elementIndex == TableStyle.LastColumnIndex)
				return TryGetLastColumnRange(tableRange, showLastColumn);
			if (elementIndex == TableStyle.FirstColumnIndex)
				return TryGetFirstColumnRange(tableRange, showFirstColumn);
			if (elementIndex == TableStyle.HeaderRowIndex)
				return TryGetHeadersRowRange(tableRange, hasHeaderRow);
			if (elementIndex == TableStyle.TotalRowIndex)
				return TryGetTotalRowRange(tableRange, hasTotalsRow);
			if (elementIndex == TableStyle.FirstHeaderCellIndex)
				return TryGetFirstHeaderRange(tableRange, hasHeaderRow, showFirstColumn);
			if (elementIndex == TableStyle.LastHeaderCellIndex)
				return TryGetLastHeaderRange(tableRange, hasHeaderRow, showLastColumn);
			if (elementIndex == TableStyle.FirstTotalCellIndex)
				return TryGetFirstTotalRange(tableRange, hasTotalsRow, showFirstColumn);
			if (elementIndex == TableStyle.LastTotalCellIndex)
				return TryGetLastTotalRange(tableRange, hasTotalsRow, showLastColumn);
			return null;
		}
		protected internal bool IsInsideTargetRange(int elementIndex, CellPosition cellPosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow, bool showColumnStripes, bool showRowStripes, bool showFirstColumn, bool showLastColumn) {
			CellRangeInfo targetCellRange = TryGetRange(elementIndex, cellPosition, tableRange, hasHeaderRow, hasTotalsRow, showColumnStripes, showRowStripes, showFirstColumn, showLastColumn);
			if (targetCellRange == null)
				return false;
			return targetCellRange.ContainsCell(cellPosition);
		}
		protected internal bool IsHeadersRow(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasHeaderRow) {
			return hasHeaderRow && IsFirstRow(absolutePosition, tableRange);
		}
		protected internal bool IsDataRow(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow) {
			return
				!IsHeadersRow(absolutePosition, tableRange, hasHeaderRow) &&
				!IsTotalsRow(absolutePosition, tableRange, hasTotalsRow);
		}
		bool IsFirstRow(CellPosition absolutePosition, CellRangeInfo tableRange) {
			return absolutePosition.Row == tableRange.First.Row;
		}
		bool IsTotalsRow(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasTotalsRow) {
			return hasTotalsRow && IsLastRow(absolutePosition, tableRange);
		}
		bool IsLastRow(CellPosition absolutePosition, CellRangeInfo tableRange) {
			return absolutePosition.Row == tableRange.Last.Row;
		}
		CellRangeInfo TryGetFirstColumnStripeRange(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow, bool showColumnStripes) {
			if (!IsDataRow(absolutePosition, tableRange, hasHeaderRow, hasTotalsRow) || !showColumnStripes)
				return null;
			CellRangeInfo dataRange = GetDataRange(tableRange, hasHeaderRow, hasTotalsRow);
			int dataLeftColumn = dataRange.First.Column;
			int relativePosition = absolutePosition.Column - dataLeftColumn;
			int firstStripeSize = GetElementFormat(TableStyle.FirstColumnStripeIndex).StripeSize;
			int secondStripeSize = GetElementFormat(TableStyle.SecondColumnStripeIndex).StripeSize;
			int stripeSize = firstStripeSize + secondStripeSize;
			int countStripe = relativePosition / stripeSize;
			int left = dataLeftColumn + countStripe * stripeSize;
			int right = left + firstStripeSize - 1;
			int dataRightColumn = dataRange.Last.Column;
			if (right > dataRightColumn)
				right = dataRightColumn;
			if (absolutePosition.Column > right)
				return null;
			return GetRange(left, dataRange.First.Row, right, dataRange.Last.Row);
		}
		CellRangeInfo TryGetSecondColumnStripeRange(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow, bool showColumnStripes) {
			if (!IsDataRow(absolutePosition, tableRange, hasHeaderRow, hasTotalsRow) || !showColumnStripes)
				return null;
			CellRangeInfo dataRange = GetDataRange(tableRange, hasHeaderRow, hasTotalsRow);
			int dataLeftColumn = dataRange.First.Column;
			int relativePosition = absolutePosition.Column - dataLeftColumn;
			int firstStripeSize = GetElementFormat(TableStyle.FirstColumnStripeIndex).StripeSize;
			int secondStripeSize = GetElementFormat(TableStyle.SecondColumnStripeIndex).StripeSize;
			int stripeSize = firstStripeSize + secondStripeSize;
			int countStripe = relativePosition / stripeSize;
			int left = dataLeftColumn + countStripe * stripeSize + firstStripeSize;
			if (absolutePosition.Column < left)
				return null;
			int right = left + secondStripeSize - 1;
			int dataRightColumn = dataRange.Last.Column;
			if (right > dataRightColumn)
				right = dataRightColumn;
			return GetRange(left, dataRange.First.Row, right, dataRange.Last.Row);
		}
		CellRangeInfo TryGetFirstRowStripeRange(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow, bool showRowStripes) {
			if (!IsDataRow(absolutePosition, tableRange, hasHeaderRow, hasTotalsRow) || !showRowStripes)
				return null;
			CellRangeInfo dataRange = GetDataRange(tableRange, hasHeaderRow, hasTotalsRow);
			int dataTopRow = dataRange.First.Row;
			int relativePosition = absolutePosition.Row - dataTopRow;
			int firstStripeSize = GetElementFormat(TableStyle.FirstRowStripeIndex).StripeSize;
			int secondStripeSize = GetElementFormat(TableStyle.SecondRowStripeIndex).StripeSize;
			int stripeSize = firstStripeSize + secondStripeSize;
			int countStripe = relativePosition / stripeSize;
			int top = dataTopRow + countStripe * stripeSize;
			int bottom = top + firstStripeSize - 1;
			int dataBottomRow = dataRange.Last.Row;
			if (bottom > dataBottomRow)
				bottom = dataBottomRow;
			if (absolutePosition.Row > bottom)
				return null;
			return GetRange(dataRange.First.Column, top, dataRange.Last.Column, bottom);
		}
		CellRangeInfo TryGetSecondRowStripeRange(CellPosition absolutePosition, CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow, bool showRowStripes) {
			if (!IsDataRow(absolutePosition, tableRange, hasHeaderRow, hasTotalsRow) || !showRowStripes)
				return null;
			CellRangeInfo dataRange = GetDataRange(tableRange, hasHeaderRow, hasTotalsRow);
			int dataTopRow = dataRange.First.Row;
			int relativePosition = absolutePosition.Row - dataTopRow;
			int firstStripeSize = GetElementFormat(TableStyle.FirstRowStripeIndex).StripeSize;
			int secondStripeSize = GetElementFormat(TableStyle.SecondRowStripeIndex).StripeSize;
			int stripeSize = firstStripeSize + secondStripeSize;
			int countStripe = relativePosition / stripeSize;
			int top = dataTopRow + countStripe * stripeSize + firstStripeSize;
			if (absolutePosition.Row < top)
				return null;
			int bottom = top + secondStripeSize - 1;
			int dataBottomRow = dataRange.Last.Row;
			if (bottom > dataBottomRow)
				bottom = dataBottomRow;
			return GetRange(dataRange.First.Column, top, dataRange.Last.Column, bottom);
		}
		CellRangeInfo TryGetFirstColumnRange(CellRangeInfo tableRange, bool showFirstColumn) {
			if (!showFirstColumn)
				return null;
			CellPosition topLeft = tableRange.First;
			return GetRange(topLeft.Column, topLeft.Row, topLeft.Column, tableRange.Last.Row);
		}
		CellRangeInfo TryGetLastColumnRange(CellRangeInfo tableRange, bool showLastColumn) {
			if (!showLastColumn)
				return null;
			CellPosition bottomRight = tableRange.Last;
			return GetRange(bottomRight.Column, tableRange.First.Row, bottomRight.Column, bottomRight.Row);
		}
		CellRangeInfo TryGetHeadersRowRange(CellRangeInfo tableRange, bool hasHeaderRow) {
			if (!hasHeaderRow)
				return null;
			CellPosition topLeft = tableRange.First;
			return GetRange(topLeft.Column, topLeft.Row, tableRange.Last.Column, topLeft.Row);
		}
		CellRangeInfo TryGetTotalRowRange(CellRangeInfo tableRange, bool hasTotalsRow) {
			if (!hasTotalsRow)
				return null;
			CellPosition bottomRight = tableRange.Last;
			return GetRange(tableRange.First.Column, bottomRight.Row, bottomRight.Column, bottomRight.Row);
		}
		CellRangeInfo TryGetFirstHeaderRange(CellRangeInfo tableRange, bool hasHeaderRow, bool showFirstColumn) {
			if (!hasHeaderRow || !showFirstColumn)
				return null;
			int top = tableRange.First.Row;
			int left = tableRange.First.Column;
			return GetRange(left, top, left, top);
		}
		CellRangeInfo TryGetLastHeaderRange(CellRangeInfo tableRange, bool hasHeaderRow, bool showLastColumn) {
			if (!hasHeaderRow || !showLastColumn)
				return null;
			int top = tableRange.First.Row;
			int right = tableRange.Last.Column;
			return GetRange(right, top, right, top);
		}
		CellRangeInfo TryGetFirstTotalRange(CellRangeInfo tableRange, bool hasTotalsRow, bool showFirstColumn) {
			if (!hasTotalsRow || !showFirstColumn)
				return null;
			int bottom = tableRange.Last.Row;
			int left = tableRange.First.Column;
			return GetRange(left, bottom, left, bottom);
		}
		CellRangeInfo TryGetLastTotalRange(CellRangeInfo tableRange, bool hasTotalsRow, bool showLastColumn) {
			if (!hasTotalsRow || !showLastColumn)
				return null;
			int bottom = tableRange.Last.Row;
			int right = tableRange.Last.Column;
			return GetRange(right, bottom, right, bottom);
		}
		CellRangeInfo GetDataRange(CellRangeInfo tableRange, bool hasHeaderRow, bool hasTotalsRow) {
			int top = tableRange.First.Row;
			if (hasHeaderRow)
				top++;
			int bottom = tableRange.Last.Row;
			if (hasTotalsRow)
				bottom--;
			return GetRange(tableRange.First.Column, top, tableRange.Last.Column, bottom);
		}
		CellRangeInfo GetRange(int left, int top, int right, int bottom) {
			CellPosition topLeft = new CellPosition(left, top);
			CellPosition bottomRight = new CellPosition(right, bottom);
			return new CellRangeInfo(topLeft, bottomRight);
		}
		#endregion
		void SetName(TableStyleName name) {
			ApplyHistoryItem(new TableStyleChangeNameHistoryItem(this, name));
		}
		protected internal void SetNameCore(TableStyleName name) {
			this.name = name;
		}
		void SetTableType(TableStyleElementIndexTableType tableType) {
			if (this.tableType == tableType)
				return;
			if (IsDefault || IsPredefined)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CannotChangeDefaultOrPredefinedStyleType);
			if (IsReferenced(tableType))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CannotChangeAppliedStyleType);
			ApplyHistoryItem(new TableStyleChangeTableTypeHistoryItem(this, tableType));
		}
		protected internal void SetTableTypeCore(TableStyleElementIndexTableType tableType) {
			this.tableType = tableType;
		}
		bool IsReferenced(TableStyleElementIndexTableType tableType) {
			if (tableType == TableStyleElementIndexTableType.General)
				return false;
			WorksheetCollection sheets = DocumentModel.Sheets;
			if (tableType == TableStyleElementIndexTableType.Table) {
				for (int i = 0; i < sheets.Count; i++) {
					PivotTableCollection pivotTables = sheets[i].PivotTables;
					for (int j = 0; j < pivotTables.Count; j++) {
						if (Object.ReferenceEquals(this, pivotTables[j].StyleInfo.Style))
							return true;
					}
				}
			}
			else {
				for (int i = 0; i < sheets.Count; i++) {
					TableCollection tables = sheets[i].Tables;
					for (int j = 0; j < tables.Count; j++) {
						if (Object.ReferenceEquals(this, tables[j].Style))
							return true;
					}
				}
			}
			return false;
		}
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region Equals
		public override bool Equals(object obj) {
			TableStyle other = obj as TableStyle;
			if (other == null)
				return false;
			return name.IsEquals(other.name) && base.Equals(other);
		}
		public bool EqualsByFormatting(TableStyle obj) {
			TableStyle other = obj as TableStyle;
			return other != null && base.Equals(other);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), Name.GetHashCode());
		}
		#endregion
		public override void CopyFrom(MultiIndexObject<TableStyle, DocumentModelChangeActions> obj) {
			try {
				TableStyle sourceStyle = obj as TableStyle;
				if (sourceStyle == null)
					throw new ArgumentException("obj is not TableStyle");
				base.CopyFrom(obj);
				isHidden = sourceStyle.isHidden;
				tableType = sourceStyle.tableType;
			} finally {
			}
		}
#if DEBUGTEST
		public bool CheckDefault2(int without) {
			bool result = true;
			int defaultFormatIndex = TableStyleElementFormatCache.DefaultItemIndex;
			int[] formatIndexes = this.FormatIndexes;
			for (int i = 0; i < TableStyle.ElementsCount; i++)
				if (without != i)
					result &= defaultFormatIndex == formatIndexes[i];
			return true;
		}
#endif
	}
	#endregion
	#region TableStyleBaseElementChangeManager
	public class TableStyleBaseElementChangeManager : DifferentialFormatPropertyChangeManager, ITableStyleElementFormat {
		public TableStyleBaseElementChangeManager(ITableStylePropertyChanger info)
			: base(info) {
		}
		#region Properties
		public int StripeSize {
			get { return ((ITableStylePropertyChanger)Info).GetStripeSize(ElementIndex); }
			set { ((ITableStylePropertyChanger)Info).SetStripeSize(ElementIndex, value); }
		}
		#endregion
		protected internal new ITableStyleElementFormat GetFormatInfo(int elementIndex) {
			ElementIndex = elementIndex;
			return this;
		}
	}
	#endregion
	#region TableStyleCategory
	public enum TableStyleCategory {
		Custom = 0,
		Light = 1,
		Medium = 2,
		Dark = 3
	}
	#endregion
} 
