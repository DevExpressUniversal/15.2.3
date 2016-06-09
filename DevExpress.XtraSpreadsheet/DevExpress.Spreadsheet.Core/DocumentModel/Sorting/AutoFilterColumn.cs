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
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region DynamicFilterType
	public enum DynamicFilterType { 
		Null = 0,
		AboveAverage = 1,
		BelowAverage = 2,
		Tomorrow = 3,
		Today = 4,
		Yesterday = 5,
		NextWeek = 6,
		ThisWeek = 7,
		LastWeek = 8,
		NextMonth = 9,
		ThisMonth = 10,
		LastMonth = 11,
		NextQuarter = 12,
		ThisQuarter = 13,
		LastQuarter = 14,
		NextYear = 15,
		ThisYear = 16,
		LastYear = 17,
		YearToDate = 18,
		Q1 = 19,
		Q2 = 20,
		Q3 = 21,
		Q4 = 22,
		M1 = 23,
		M2 = 24,
		M3 = 25,
		M4 = 26,
		M5 = 27,
		M6 = 28,
		M7 = 29,
		M8 = 30,
		M9 = 31,
		M10 = 32,
		M11 = 33,
		M12 = 34
	}
	#endregion
	#region FilterComparisonOperator
	public enum FilterComparisonOperator { 
		LessThan = 1,
		Equal = 2,
		LessThanOrEqual = 3,
		GreaterThan = 4,
		NotEqual = 5,
		GreaterThanOrEqual = 6
	}
	#endregion
	#region CalendarType
	public enum CalendarType { 
		None = 0,
		Gregorian = 1,
		GregorianUs = 2,
		Japan = 3,
		Taiwan = 4,
		Korea = 5,
		Hijri = 6,
		Thai = 7,
		Hebrew = 8,
		GregorianMeFrench = 9,
		GregorianArabic = 10,
		GregorianXlitEnglish = 11,
		GregorianXlitFrench = 12
	}
	#endregion
	#region AutoFilterColumnInfo
	public class AutoFilterColumnInfo : ICloneable<AutoFilterColumnInfo>, ISupportsCopyFrom<AutoFilterColumnInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskHiddenAutoFilterButton = 0x00000001; 
		const uint MaskShowFilterButton = 0x00000002;	   
		const uint MaskFilterByTopOrder = 0x00000004;	   
		const uint MaskDynamicFilterType = 0x000003F0;	  
		const uint MaskFilterByCellFill = 0x00000400;	   
		const uint MaskIconSetType = 0x0000F800;			
		const uint MaskTop10FilterType = 0x00030000;		
		uint packedValues;
		int iconId;
		double topOrBottomDoubleValue;
		double filterDoubleValue;
		double dynamicMaxValue;
		double dynamicMinValue;
		#endregion
		#region Properties
		#region DynamicFilterType
		public DynamicFilterType DynamicFilterType {
			get { return (DynamicFilterType)((packedValues & MaskDynamicFilterType) >> 4); }
			set {
				packedValues &= ~MaskDynamicFilterType;
				packedValues |= ((uint)value << 4) & MaskDynamicFilterType;
			}
		}
		#endregion
		#region Top10FilterType
		public Top10FilterType Top10FilterType {
			get { return (Top10FilterType)((packedValues & MaskTop10FilterType) >> 16); }
			set {
				packedValues &= ~MaskTop10FilterType;
				packedValues |= ((uint)value << 16) & MaskTop10FilterType;
			}
		}
		#endregion
		#region IconSetType
		public IconSetType IconSetType {
			get { return (IconSetType)((packedValues & MaskIconSetType) >> 11); }
			set {
				packedValues &= ~MaskIconSetType;
				packedValues |= ((uint)value << 11) & MaskIconSetType;
			}
		}
		#endregion
		public bool HiddenAutoFilterButton { get { return GetBooleanVal(MaskHiddenAutoFilterButton); } set { SetBooleanVal(MaskHiddenAutoFilterButton, value); } }
		public bool ShowFilterButton { get { return GetBooleanVal(MaskShowFilterButton); } set { SetBooleanVal(MaskShowFilterButton, value); } }
		public bool FilterByCellFill { get { return GetBooleanVal(MaskFilterByCellFill); } set { SetBooleanVal(MaskFilterByCellFill, value); } }
		public bool FilterByTopOrder { get { return GetBooleanVal(MaskFilterByTopOrder); } set { SetBooleanVal(MaskFilterByTopOrder, value); } }
		public int IconId { get { return iconId; } set { iconId = value; } }
		public double TopOrBottomDoubleValue { get { return topOrBottomDoubleValue; } set { topOrBottomDoubleValue = value; } }
		public double FilterDoubleValue { get { return filterDoubleValue; } set { filterDoubleValue = value; } }
		public double DynamicMaxValue { get { return dynamicMaxValue; } set { dynamicMaxValue = value; } }
		public double DynamicMinValue { get { return dynamicMinValue; } set { dynamicMinValue = value; } }
		#endregion
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ICloneable<AutoFilterColumnInfo> Members
		public AutoFilterColumnInfo Clone() {
			AutoFilterColumnInfo clone = new AutoFilterColumnInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<AutoFilterColumnInfo> Members
		public void CopyFrom(AutoFilterColumnInfo value) {
			this.packedValues = value.packedValues;
			this.dynamicMaxValue = value.DynamicMaxValue;
			this.dynamicMinValue = value.DynamicMinValue;
			this.filterDoubleValue = value.FilterDoubleValue;
			this.iconId = value.IconId;
			this.topOrBottomDoubleValue = value.TopOrBottomDoubleValue;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			AutoFilterColumnInfo info = obj as AutoFilterColumnInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues &&
				this.dynamicMaxValue == info.DynamicMaxValue &&
				this.dynamicMinValue == info.DynamicMinValue &&
				this.filterDoubleValue == info.FilterDoubleValue &&
				this.iconId == info.IconId &&
				this.topOrBottomDoubleValue == info.TopOrBottomDoubleValue;
		}
		public override int GetHashCode() {
			Office.Utils.CombinedHashCode result = new Office.Utils.CombinedHashCode();
			result.AddInt((int)packedValues);
			result.AddInt(IconId);
			result.AddInt(DynamicMaxValue.GetHashCode());
			result.AddInt(DynamicMinValue.GetHashCode());
			result.AddInt(FilterDoubleValue.GetHashCode());
			result.AddInt(TopOrBottomDoubleValue.GetHashCode());
			return result.CombinedHash32;
		}
	}
	#endregion
	#region AutoFilterColumnInfoCache
	public class AutoFilterColumnInfoCache : UniqueItemsCache<AutoFilterColumnInfo> {
		public const int DefaultItemIndex = 0;
		public AutoFilterColumnInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override AutoFilterColumnInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			AutoFilterColumnInfo info = new AutoFilterColumnInfo();
			info.DynamicFilterType = DynamicFilterType.Null;
			info.DynamicMaxValue = -1.0f;
			info.DynamicMinValue = -1.0f;
			info.FilterByCellFill = true;
			info.Top10FilterType = Top10FilterType.Count;
			info.FilterByTopOrder = true;
			info.FilterDoubleValue = -1.0f;
			info.HiddenAutoFilterButton = false;
			info.IconId = -1;
			info.IconSetType = IconSetType.None;
			info.ShowFilterButton = true;
			info.TopOrBottomDoubleValue = -1.0f;
			return info;
		}
	}
	#endregion
	#region AutoFilterColumnBatchUpdateHelper
	public class AutoFilterColumnBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		DifferentialFormat differentialFormat;
		AutoFilterColumnInfo autoFilterColumnInfo;
		public AutoFilterColumnBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public AutoFilterColumnInfo AutoFilterColumnInfo { get { return autoFilterColumnInfo; } set { autoFilterColumnInfo = value; } }
		public DifferentialFormat DifferentialFormat { get { return differentialFormat; } set { differentialFormat = value; } }
	}
	#endregion
	#region AutoFilterColumnBatchInitHelper
	public class AutoFilterColumnBatchInitHelper : AutoFilterColumnBatchUpdateHelper {
		public AutoFilterColumnBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region AutoFilterColumnDifferentialFormatIndexAccessor
	public class AutoFilterColumnDifferentialFormatIndexAccessor : IIndexAccessor<AutoFilterColumn, FormatBase, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetInfoIndex(AutoFilterColumn owner, FormatBase value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public FormatBase GetInfo(AutoFilterColumn owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((AutoFilterColumnBatchUpdateHelper)helper).DifferentialFormat;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			((AutoFilterColumnBatchUpdateHelper)helper).DifferentialFormat = (DifferentialFormat)info.Clone();
		}
		public int GetIndex(AutoFilterColumn owner) {
			return owner.FormatIndex;
		}
		public void SetIndex(AutoFilterColumn owner, int value) {
			owner.AssignFormatIndex(value);
		}
		public bool IsIndexValid(AutoFilterColumn owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		public int GetDeferredInfoIndex(AutoFilterColumn owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void InitializeDeferredInfo(AutoFilterColumn owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(AutoFilterColumn owner, AutoFilterColumn from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(AutoFilterColumn owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(AutoFilterColumn owner) {
			return new AutoFilterColumnDifferentialFormatIndexChangeHistoryItem(owner);
		}
		#endregion
		UniqueItemsCache<FormatBase> GetInfoCache(AutoFilterColumn owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
	}
	#endregion
	#region AutoFilterColumnInfoIndexAccessor
	public class AutoFilterColumnInfoIndexAccessor : IIndexAccessor<AutoFilterColumn, AutoFilterColumnInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<AutoFilterColumn,AutoFilterColumnInfo,DocumentModelChangeActions> Members
		public int GetInfoIndex(AutoFilterColumn owner, AutoFilterColumnInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public AutoFilterColumnInfo GetInfo(AutoFilterColumn owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public AutoFilterColumnInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((AutoFilterColumnBatchUpdateHelper)helper).AutoFilterColumnInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, AutoFilterColumnInfo info) {
			((AutoFilterColumnBatchUpdateHelper)helper).AutoFilterColumnInfo = info.Clone();
		}
		#endregion
		#region IIndexAccessorBase<AutoFilterColumn,DocumentModelChangeActions> Members
		public int GetIndex(AutoFilterColumn owner) {
			return owner.InfoIndex;
		}
		public void SetIndex(AutoFilterColumn owner, int value) {
			owner.AssignInfoIndex(value);
		}
		public bool IsIndexValid(AutoFilterColumn owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		public int GetDeferredInfoIndex(AutoFilterColumn owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void InitializeDeferredInfo(AutoFilterColumn owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(AutoFilterColumn owner, AutoFilterColumn from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(AutoFilterColumn owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(AutoFilterColumn owner) {
			return new AutoFilterColumnInfoIndexChangeHistoryItem(owner);
		}
		#endregion
		AutoFilterColumnInfoCache GetInfoCache(AutoFilterColumn owner) {
			return owner.DocumentModel.Cache.AutoFilterColumnInfoCache;
		}
	}
	#endregion
	#region Top10FilterType
	public enum Top10FilterType {
		Count = 0,
		Percent = 1,
		Sum = 2,
	}
	#endregion
	#region AutoFilterColumn
	public class AutoFilterColumn : MultiIndexObject<AutoFilterColumn, DocumentModelChangeActions>, IDifferentialFormat, IFormatBase, IRunFontInfo, IFillInfo, IBorderInfo, ICellAlignmentInfo, ICellProtectionInfo, IGradientFillInfo, IConvergenceInfo {
		#region Static Members
		readonly static AutoFilterColumnDifferentialFormatIndexAccessor differentialFormatIndexAccessor = new AutoFilterColumnDifferentialFormatIndexAccessor();
		readonly static AutoFilterColumnInfoIndexAccessor autoFilterColumnInfoIndexAccessor = new AutoFilterColumnInfoIndexAccessor();
		readonly static IIndexAccessorBase<AutoFilterColumn, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<AutoFilterColumn, DocumentModelChangeActions>[] {
			differentialFormatIndexAccessor, 
			autoFilterColumnInfoIndexAccessor
		};
		public static AutoFilterColumnDifferentialFormatIndexAccessor DifferentialFormatIndexAccessor { get { return differentialFormatIndexAccessor; } }
		public static AutoFilterColumnInfoIndexAccessor AutoFilterColumnInfoIndexAccessor { get { return autoFilterColumnInfoIndexAccessor; } }
		#endregion
		#region Fields
		Worksheet sheet;
		CustomFilterCollection customFilters;
		FilterCriteria filterCriteria;
		int columnId;
		int formatIndex = CellFormatCache.DefaultDifferentialFormatIndex;
		int infoIndex = AutoFilterColumnInfoCache.DefaultItemIndex;
		double top10FilterValue;
		#endregion
		public AutoFilterColumn(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "worksheet");
			this.sheet = sheet;
			this.customFilters = new CustomFilterCollection(sheet);
			this.filterCriteria = new FilterCriteria(sheet);
		}
		#region Properties
		public CustomFilterCollection CustomFilters { get { return customFilters; } }
		public FilterCriteria FilterCriteria { get { return filterCriteria; } }
		internal int FormatIndex { get { return formatIndex; } }
		internal int InfoIndex { get { return infoIndex; } }
		internal new AutoFilterColumnBatchUpdateHelper BatchUpdateHelper { get { return (AutoFilterColumnBatchUpdateHelper)base.BatchUpdateHelper; } }
		AutoFilterColumnInfo InfoCore { get { return (AutoFilterColumnInfo)AutoFilterColumnInfoIndexAccessor.GetInfo(this); } }
		DifferentialFormat FormatInfoCore { get { return (DifferentialFormat)DifferentialFormatIndexAccessor.GetInfo(this); } }
		public Worksheet Sheet { get { return sheet; } }
		protected internal AutoFilterColumnInfo Info {
			get { return BatchUpdateHelper != null ? BatchUpdateHelper.AutoFilterColumnInfo : InfoCore; }
		}
		protected internal DifferentialFormat FormatInfo {
			get { return BatchUpdateHelper != null ? BatchUpdateHelper.DifferentialFormat : FormatInfoCore; }
		}
		public new DocumentModel DocumentModel { get { return sheet.Workbook; } }
		#region ColumnId
		public int ColumnId {
			get { return columnId; }
			set {
				if (columnId == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				AutoFilterColumnIdHistoryItem item = new AutoFilterColumnIdHistoryItem(this, value, columnId);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetColumnIdCore(int value) {
			columnId = value;
		}
		#endregion
		#region HiddenAutoFilterButton
		public bool HiddenAutoFilterButton {
			get { return Info.HiddenAutoFilterButton; }
			set {
				if (HiddenAutoFilterButton == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetHiddenAutoFilterButtonCore, value);
			}
		}
		DocumentModelChangeActions SetHiddenAutoFilterButtonCore(AutoFilterColumnInfo info, bool value) {
			info.HiddenAutoFilterButton = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowFilterButton
		public bool ShowFilterButton {
			get { return Info.ShowFilterButton; }
			set {
				if (ShowFilterButton == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetShowFilterButtonCore, value);
			}
		}
		DocumentModelChangeActions SetShowFilterButtonCore(AutoFilterColumnInfo info, bool value) {
			info.ShowFilterButton = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FilterByCellFill
		public bool FilterByCellFill {
			get { return Info.FilterByCellFill; }
			set {
				if (FilterByCellFill == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetFilterByCellFillCore, value);
			}
		}
		DocumentModelChangeActions SetFilterByCellFillCore(AutoFilterColumnInfo info, bool value) {
			info.FilterByCellFill = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DynamicFilterType
		public DynamicFilterType DynamicFilterType {
			get { return Info.DynamicFilterType; }
			set {
				if (DynamicFilterType == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetDynamicFilterTypeCore, value);
			}
		}
		DocumentModelChangeActions SetDynamicFilterTypeCore(AutoFilterColumnInfo info, DynamicFilterType value) {
			info.DynamicFilterType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DynamicMaxValue
		public double DynamicMaxValue {
			get { return Info.DynamicMaxValue; }
			set {
				if (DynamicMaxValue == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetDynamicMaxValueCore, value);
			}
		}
		DocumentModelChangeActions SetDynamicMaxValueCore(AutoFilterColumnInfo info, double value) {
			info.DynamicMaxValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DynamicMinValue
		public double DynamicMinValue {
			get { return Info.DynamicMinValue; }
			set {
				if (DynamicMinValue == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetDynamicMinValueCore, value);
			}
		}
		DocumentModelChangeActions SetDynamicMinValueCore(AutoFilterColumnInfo info, double value) {
			info.DynamicMinValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IconSetType
		public IconSetType IconSetType {
			get { return Info.IconSetType; }
			set {
				if (IconSetType == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetIconSetTypeCore, value);
			}
		}
		DocumentModelChangeActions SetIconSetTypeCore(AutoFilterColumnInfo info, IconSetType value) {
			info.IconSetType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IconId
		public int IconId {
			get { return Info.IconId; }
			set {
				if (IconId == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetIconIdCore, value);
			}
		}
		DocumentModelChangeActions SetIconIdCore(AutoFilterColumnInfo info, int value) {
			info.IconId = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FilterByTopOrder
		public bool FilterByTopOrder {
			get { return Info.FilterByTopOrder; }
			set {
				if (FilterByTopOrder == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetFilterByTopOrderCore, value);
			}
		}
		DocumentModelChangeActions SetFilterByTopOrderCore(AutoFilterColumnInfo info, bool value) {
			info.FilterByTopOrder = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Top10FilterType
		public Top10FilterType Top10FilterType {
			get { return Info.Top10FilterType; }
			set {
				if (Top10FilterType == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetTop10FilterTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTop10FilterTypeCore(AutoFilterColumnInfo info, Top10FilterType value) {
			info.Top10FilterType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TopOrBottomDoubleValue
		public double TopOrBottomDoubleValue {
			get { return Info.TopOrBottomDoubleValue; }
			set {
				if (TopOrBottomDoubleValue == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetTopOrBottomDoubleValueCore, value);
			}
		}
		DocumentModelChangeActions SetTopOrBottomDoubleValueCore(AutoFilterColumnInfo info, double value) {
			info.TopOrBottomDoubleValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FilterDoubleValue
		public double FilterDoubleValue {
			get { return Info.FilterDoubleValue; }
			set {
				if (FilterDoubleValue == value)
					return;
				SetPropertyValue(AutoFilterColumnInfoIndexAccessor, SetFilterDoubleValueCore, value);
			}
		}
		DocumentModelChangeActions SetFilterDoubleValueCore(AutoFilterColumnInfo info, double value) {
			info.FilterDoubleValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool ShouldWriteAutoFilter12 {
			get {
				int filterCriteriaCount = FilterCriteria.Filters.Count;
				bool shouldWriteFilterCriteria = FilterCriteria.DateGroupings.Count > 0 || filterCriteriaCount > 2 ||
												 (FilterCriteria.FilterByBlank && filterCriteriaCount > 1);
				return IsDynamicFilter || IsIconFilter || IsColorFilter || shouldWriteFilterCriteria;
			}
		}
		public AutoFilterColumnInfo DefaultInfo { get { return DocumentModel.Cache.AutoFilterColumnInfoCache.DefaultItem; } }
		public bool IsTop10Filter { get { return TopOrBottomDoubleValue != DefaultInfo.TopOrBottomDoubleValue; } }
		public bool IsDynamicFilter { get { return DynamicFilterType != DefaultInfo.DynamicFilterType; } }
		public bool IsIconFilter { get { return IconSetType != DefaultInfo.IconSetType || IconId != DefaultInfo.IconId; } }
		public bool IsColorFilter { get { return FormatIndex != CellFormatCache.DefaultDifferentialFormatIndex || FilterByCellFill != DefaultInfo.FilterByCellFill; } }
		public bool IsNonDefault {
			get {
				return formatIndex != CellFormatCache.DefaultDifferentialFormatIndex ||
					infoIndex != AutoFilterColumnInfoCache.DefaultItemIndex ||
					CustomFilters.Count > 0 || FilterCriteria.HasFilter();
			}
		}
		#region IDifferentialFormat Members
		public IRunFontInfo Font { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		public string FormatString {
			get { return FormatInfo.FormatString; }
			set {
				if ((FormatInfo.FormatString == value) && FormatInfo.MultiOptionsInfo.ApplyNumberFormat)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			((DifferentialFormat)info).FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#region IRunFontInfo Members
		#region string IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return FormatInfo.Font.Name; }
			set {
				if ((FormatInfo.Font.Name == value) && FormatInfo.MultiOptionsInfo.ApplyFontName)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return FormatInfo.Font.Color; }
			set {
				if ((FormatInfo.Font.Color == value) && FormatInfo.MultiOptionsInfo.ApplyFontColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return FormatInfo.Font.Bold; }
			set {
				if ((FormatInfo.Font.Bold == value) && FormatInfo.MultiOptionsInfo.ApplyFontBold)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return FormatInfo.Font.Condense; }
			set {
				if ((FormatInfo.Font.Condense) == value && FormatInfo.MultiOptionsInfo.ApplyFontCondense)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return FormatInfo.Font.Extend; }
			set {
				if ((FormatInfo.Font.Extend == value) && FormatInfo.MultiOptionsInfo.ApplyFontExtend)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return FormatInfo.Font.Italic; }
			set {
				if ((FormatInfo.Font.Italic == value) && FormatInfo.MultiOptionsInfo.ApplyFontItalic)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return FormatInfo.Font.Outline; }
			set {
				if ((FormatInfo.Font.Outline == value) && FormatInfo.MultiOptionsInfo.ApplyFontOutline)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return FormatInfo.Font.Shadow; }
			set {
				if ((FormatInfo.Font.Shadow == value) && FormatInfo.MultiOptionsInfo.ApplyFontShadow)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FormatInfo.Font.StrikeThrough; }
			set {
				if ((FormatInfo.Font.StrikeThrough == value) && FormatInfo.MultiOptionsInfo.ApplyFontStrikeThrough)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return FormatInfo.Font.Charset; }
			set {
				if ((FormatInfo.Font.Charset == value) && FormatInfo.MultiOptionsInfo.ApplyFontCharset)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FormatInfo.Font.FontFamily; }
			set {
				if (FormatInfo.Font.FontFamily == value && FormatInfo.MultiOptionsInfo.ApplyFontFamily)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region double IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return FormatInfo.Font.Size; }
			set {
				if ((FormatInfo.Font.Size == value) && FormatInfo.MultiOptionsInfo.ApplyFontSize)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSchemeStyles IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FormatInfo.Font.SchemeStyle; }
			set {
				if ((FormatInfo.Font.SchemeStyle == value) && FormatInfo.MultiOptionsInfo.ApplyFontSchemeStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ModelScriptType IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return FormatInfo.Font.Script; }
			set {
				if ((FormatInfo.Font.Script == value) && FormatInfo.MultiOptionsInfo.ApplyFontScript)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ModelUnderlineType IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FormatInfo.Font.Underline; }
			set {
				if ((FormatInfo.Font.Underline == value) && FormatInfo.MultiOptionsInfo.ApplyFontUnderline)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IBorderInfo Members
		#region XlBorderLineStyle IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return FormatInfo.Border.LeftLineStyle; }
			set {
				if ((FormatInfo.Border.LeftLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyLeftLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderLeftLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return FormatInfo.Border.RightLineStyle; }
			set {
				if ((FormatInfo.Border.RightLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyRightLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderRightLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderRightLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return FormatInfo.Border.TopLineStyle; }
			set {
				if ((FormatInfo.Border.TopLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyTopLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderTopLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderTopLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return FormatInfo.Border.BottomLineStyle; }
			set {
				if ((FormatInfo.Border.BottomLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyBottomLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderBottomLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return FormatInfo.Border.DiagonalUpLineStyle; }
			set {
				if ((FormatInfo.Border.DiagonalUpLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalUpLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return FormatInfo.Border.DiagonalDownLineStyle; }
			set {
				if ((FormatInfo.Border.DiagonalDownLineStyle) == value && FormatInfo.BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalDownLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return FormatInfo.Border.HorizontalLineStyle; }
			set {
				if ((FormatInfo.Border.HorizontalLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyHorizontalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderHorizontalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlBorderLineStyle IBorderInfo.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return FormatInfo.Border.VerticalLineStyle; }
			set {
				if ((FormatInfo.Border.VerticalLineStyle == value) && FormatInfo.BorderOptionsInfo.ApplyVerticalLineStyle)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderVerticalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region bool IBorderInfo.Outline
		bool IBorderInfo.Outline {
			get { return FormatInfo.Border.Outline; }
			set {
				if ((FormatInfo.Border.Outline == value) && FormatInfo.BorderOptionsInfo.ApplyOutline)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.LeftColor
		Color IBorderInfo.LeftColor {
			get { return FormatInfo.Border.LeftColor; }
			set {
				if ((FormatInfo.Border.LeftColor == value) && FormatInfo.BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderLeftColor, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColor(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.RightColor
		Color IBorderInfo.RightColor {
			get { return FormatInfo.Border.RightColor; }
			set {
				if ((FormatInfo.Border.RightColor == value) && FormatInfo.BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderRightColor, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColor(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.TopColor
		Color IBorderInfo.TopColor {
			get { return FormatInfo.Border.TopColor; }
			set {
				if ((FormatInfo.Border.TopColor == value) && FormatInfo.BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderTopColor, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColor(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.BottomColor
		Color IBorderInfo.BottomColor {
			get { return FormatInfo.Border.BottomColor; }
			set {
				if ((FormatInfo.Border.BottomColor == value) && FormatInfo.BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderBottomColor, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColor(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return FormatInfo.Border.DiagonalColor; }
			set {
				if ((FormatInfo.Border.DiagonalColor == value) && FormatInfo.BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColor(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return FormatInfo.Border.HorizontalColor; }
			set {
				if ((FormatInfo.Border.HorizontalColor == value) && FormatInfo.BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderHorizontalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColor(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IBorderInfo.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return FormatInfo.Border.VerticalColor; }
			set {
				if ((FormatInfo.Border.VerticalColor == value) && FormatInfo.BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderVerticalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColor(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return FormatInfo.Border.LeftColorIndex; }
			set {
				if ((FormatInfo.Border.LeftColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderLeftColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColorIndex(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return FormatInfo.Border.RightColorIndex; }
			set {
				if ((FormatInfo.Border.RightColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderRightColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColorIndex(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return FormatInfo.Border.TopColorIndex; }
			set {
				if ((FormatInfo.Border.TopColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderTopColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColorIndex(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return FormatInfo.Border.BottomColorIndex; }
			set {
				if ((FormatInfo.Border.BottomColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderBottomColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColorIndex(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return FormatInfo.Border.DiagonalColorIndex; }
			set {
				if ((FormatInfo.Border.DiagonalColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderDiagonalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndex(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return FormatInfo.Border.HorizontalColorIndex; }
			set {
				if ((FormatInfo.Border.HorizontalColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderHorizontalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndex(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region int IBorderInfo.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return FormatInfo.Border.VerticalColorIndex; }
			set {
				if ((FormatInfo.Border.VerticalColorIndex == value) && FormatInfo.BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetBorderVerticalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColorIndex(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IFillInfo Members
		#region IFillInfo.ClearFill
		void IFillInfo.Clear() {
			if (!FormatInfo.MultiOptionsInfo.ApplyFillNone)
				ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				DifferentialFormat info = (DifferentialFormat)GetInfoForModification(DifferentialFormatIndexAccessor);
				info.Fill.Clear();
				ReplaceInfo(DifferentialFormatIndexAccessor, info, DocumentModelChangeActions.None);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region ModelPatternType IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FormatInfo.Fill.PatternType; }
			set {
				if ((FormatInfo.Fill.PatternType == value) && FormatInfo.MultiOptionsInfo.ApplyFillPatternType)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFillPatternType, value);
			}
		}
		DocumentModelChangeActions SetFillPatternType(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IFillInfo.ForeColor
		Color IFillInfo.ForeColor {
			get { return FormatInfo.Fill.ForeColor; }
			set {
				if ((FormatInfo.Fill.ForeColor == value) && FormatInfo.MultiOptionsInfo.ApplyFillForeColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFillForeColor, value);
			}
		}
		DocumentModelChangeActions SetFillForeColor(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Color IFillInfo.BackColor
		Color IFillInfo.BackColor {
			get { return FormatInfo.Fill.BackColor; }
			set {
				if ((FormatInfo.Fill.BackColor == value) && FormatInfo.MultiOptionsInfo.ApplyFillBackColor)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetFillBackColor, value);
			}
		}
		DocumentModelChangeActions SetFillBackColor(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ModelFillType IFillInfo.FillType
		ModelFillType IFillInfo.FillType {
			get { return FormatInfo.Fill.FillType; }
			set {
				if (FormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetModelFillType, value);
			}
		}
		DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo IFillInfo.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region IGradientFillInfo Members
		#region IGradientFillInfo.Convergence
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		#endregion
		#region IGradientFillInfo.GradientStops
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return FormatInfo.Fill.GradientFill.GradientStops; } }
		#endregion
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return FormatInfo.Fill.GradientFill.Type; }
			set {
				if (FormatInfo.Fill.GradientFill.Type == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoType, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoType(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return FormatInfo.Fill.GradientFill.Degree; }
			set {
				if (FormatInfo.Fill.GradientFill.Degree == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoDegree, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoDegree(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return FormatInfo.Fill.GradientFill.Convergence.Left; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Left == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoLeft, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoLeft(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return FormatInfo.Fill.GradientFill.Convergence.Right; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Right == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoRight, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoRight(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return FormatInfo.Fill.GradientFill.Convergence.Top; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Top == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoTop, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoTop(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return FormatInfo.Fill.GradientFill.Convergence.Bottom; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Bottom == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#region ICellAlignmentInfo Members
		#region ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return FormatInfo.Alignment.WrapText; }
			set {
				if (FormatInfo.Alignment.WrapText == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentWrapText)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentWrapText, value);
			}
		}
		DocumentModelChangeActions SetAlignmentWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return FormatInfo.Alignment.JustifyLastLine; }
			set {
				if (FormatInfo.Alignment.JustifyLastLine == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetAlignmentJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return FormatInfo.Alignment.ShrinkToFit; }
			set {
				if (FormatInfo.Alignment.ShrinkToFit == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetAlignmentShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return FormatInfo.Alignment.TextRotation; }
			set {
				if (FormatInfo.Alignment.TextRotation == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentTextRotation)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentTextRotation, value);
			}
		}
		DocumentModelChangeActions SetAlignmentTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return FormatInfo.Alignment.Indent; }
			set {
				if (FormatInfo.Alignment.Indent == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentIndent)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return FormatInfo.Alignment.RelativeIndent; }
			set {
				if (FormatInfo.Alignment.RelativeIndent == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return FormatInfo.Alignment.Horizontal; }
			set {
				if (FormatInfo.Alignment.Horizontal == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentHorizontal)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentHorizontal, value);
			}
		}
		DocumentModelChangeActions SetAlignmentHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return FormatInfo.Alignment.Vertical; }
			set {
				if (FormatInfo.Alignment.Vertical == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentVertical)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentVertical, value);
			}
		}
		DocumentModelChangeActions SetAlignmentVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return FormatInfo.Alignment.ReadingOrder; }
			set {
				if (FormatInfo.Alignment.ReadingOrder == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentReadingOrder)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetAlignmentReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetAlignmentReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region ICellProtectionInfo Members
		#region ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return FormatInfo.Protection.Locked; }
			set {
				if (FormatInfo.Protection.Locked == value && FormatInfo.MultiOptionsInfo.ApplyProtectionLocked)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetProtectionLocked, value);
			}
		}
		DocumentModelChangeActions SetProtectionLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return FormatInfo.Protection.Hidden; }
			set {
				if (FormatInfo.Protection.Hidden == value && FormatInfo.MultiOptionsInfo.ApplyProtectionHidden)
					return;
				SetPropertyValue(DifferentialFormatIndexAccessor, SetProtectionHidden, value);
			}
		}
		DocumentModelChangeActions SetProtectionHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#endregion
		public void Clear() {
			DocumentModel.BeginUpdate();
			try {
				ReplaceInfos();
				this.CustomFilters.Clear();
				this.FilterCriteria.Clear();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void ReplaceInfos() {
			FormatBase defaultFormatInfo = Sheet.Workbook.Cache.CellFormatCache.DefaultDifferentialFormatItem;
			if (IsUpdateLocked) {
				AutoFilterColumnInfoIndexAccessor.GetDeferredInfo(BatchUpdateHelper).CopyFrom(DefaultInfo);
				DifferentialFormatIndexAccessor.GetDeferredInfo(BatchUpdateHelper).CopyFrom(defaultFormatInfo);
			}
			else {
				ReplaceInfo(AutoFilterColumnInfoIndexAccessor, DefaultInfo, DocumentModelChangeActions.None);
				ReplaceInfo(DifferentialFormatIndexAccessor, defaultFormatInfo, DocumentModelChangeActions.None);
			}
		}
		public void CopyFrom(AutoFilterColumn source) {
			base.CopyFrom(source);
			this.ColumnId = source.ColumnId;
			this.CustomFilters.CopyFrom(source.CustomFilters);
			this.FilterCriteria.CopyFrom(source.FilterCriteria, sheet);
		}
		public override bool Equals(object obj) {
			AutoFilterColumn otherColumn = obj as AutoFilterColumn;
			if (otherColumn == null)
				return false;
			if (otherColumn.ColumnId != ColumnId)
				return false;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ ColumnId.GetHashCode();
		}
		#region MultiIndexObject<AutoFilterColumn, DocumentModelChangeActions> Members
		protected override IDocumentModel GetDocumentModel() {
			return sheet.Workbook;
		}
		protected override IIndexAccessorBase<AutoFilterColumn, DocumentModelChangeActions>[] IndexAccessors {
			get { return indexAccessors; }
		}
		public override AutoFilterColumn GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new AutoFilterColumnBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new AutoFilterColumnBatchInitHelper(this);
		}
		protected  override void ApplyChanges(DocumentModelChangeActions changeActions) {
			sheet.ApplyChanges(changeActions);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Index Management
		internal void AssignFormatIndex(int index) {
			formatIndex = index;
		}
		internal void AssignInfoIndex(int index) {
			infoIndex = index;
		}
		#endregion
		internal bool IsRowVisible(CellRange range, int rowIndex, IFilteringBehaviour filteringBehaviour) {
			ICell cell = Sheet.TryGetCell(range.TopLeft.Column + ColumnId, rowIndex);
			return IsValueVisible(cell, filteringBehaviour);
		}
		internal bool IsValueVisible(IAutoFilterValue value, IFilteringBehaviour filteringBehaviour) {
			if (DynamicFilterType != DynamicFilterType.Null)
				return DynamicFilterIsCellVisible(value);
			else if (IsTop10Filter)
				return Top10FilterIsCellVisible(value);
			else if (FilterCriteria.HasFilter())
				return FilterCriteria.IsValueVisible(value);
			else if (CustomFilters.Count > 0)
				return CustomFilters.IsCellVisible(value, filteringBehaviour);
			else
				return true;
		}
		internal void Update(IAutoFilterValueProvider valueProvider, IFilteringBehaviour filteringBehaviour) {
			if (IsTop10Filter)
				UpdateTop10FilterValue(valueProvider, filteringBehaviour);
			else if (IsDynamicFilter)
				UpdateDynamicFilterValues(valueProvider);
		}
		void UpdateDynamicFilterValues(IAutoFilterValueProvider valueProvider) {
			if (DynamicFilterType == DynamicFilterType.BelowAverage || DynamicFilterType == DynamicFilterType.AboveAverage)
				this.DynamicMinValue = CalculateAverage(valueProvider);
			else
				CalculateDateTimeDynamicFilterValues(DateTime.Now.Date);
		}
		void UpdateTop10FilterValue(IAutoFilterValueProvider valueProvider, IFilteringBehaviour filteringBehaviour) {
			this.FilterDoubleValue = CalculateTop10Value(valueProvider, filteringBehaviour);
		}
		internal CellRange GetColumnRange(AutoFilterBase autoFilter) {
			CellPosition topLeft = autoFilter.Range.TopLeft;
			return autoFilter.GetFilterColumnDataRange(topLeft.Column + ColumnId);
		}
		internal void CalculateDateTimeDynamicFilterValues(DateTime now) {
			if (DynamicFilterType <= DynamicFilterType.BelowAverage && DynamicFilterType >= DynamicFilterType.Q1)
				return;
			switch (DynamicFilterType) {
				case DynamicFilterType.Tomorrow:
					UpdateDynamicMinMaxValue(now.AddDays(1), now.AddDays(2));
					break;
				case DynamicFilterType.Today:
					UpdateDynamicMinMaxValue(now, now.AddDays(1));
					break;
				case DynamicFilterType.Yesterday:
					UpdateDynamicMinMaxValue(now.AddDays(-1), now);
					break;
				case DynamicFilterType.NextWeek:
					UpdateDynamicMinMaxValue(BeginOfWeek(now).AddDays(7), BeginOfWeek(now).AddDays(14));
					break;
				case DynamicFilterType.ThisWeek:
					UpdateDynamicMinMaxValue(BeginOfWeek(now), BeginOfWeek(now).AddDays(7));
					break;
				case DynamicFilterType.LastWeek:
					UpdateDynamicMinMaxValue(BeginOfWeek(now).AddDays(-7), BeginOfWeek(now));
					break;
				case DynamicFilterType.NextMonth:
					UpdateDynamicMinMaxValue(BeginOfMonth(now).AddMonths(1), BeginOfMonth(now).AddMonths(2));
					break;
				case DynamicFilterType.ThisMonth:
					UpdateDynamicMinMaxValue(BeginOfMonth(now), BeginOfMonth(now).AddMonths(1));
					break;
				case DynamicFilterType.LastMonth:
					UpdateDynamicMinMaxValue(BeginOfMonth(now).AddMonths(-1), BeginOfMonth(now));
					break;
				case DynamicFilterType.NextQuarter:
					UpdateDynamicMinMaxValue(BeginOfQuarter(now).AddMonths(3), BeginOfQuarter(now).AddMonths(6));
					break;
				case DynamicFilterType.ThisQuarter:
					UpdateDynamicMinMaxValue(BeginOfQuarter(now), BeginOfQuarter(now).AddMonths(3));
					break;
				case DynamicFilterType.LastQuarter:
					UpdateDynamicMinMaxValue(BeginOfQuarter(now).AddMonths(-3), BeginOfQuarter(now));
					break;
				case DynamicFilterType.NextYear:
					UpdateDynamicMinMaxValue(BeginOfYear(now).AddYears(1), BeginOfYear(now).AddYears(2));
					break;
				case DynamicFilterType.ThisYear:
					UpdateDynamicMinMaxValue(BeginOfYear(now), BeginOfYear(now).AddYears(1));
					break;
				case DynamicFilterType.LastYear:
					UpdateDynamicMinMaxValue(BeginOfYear(now).AddYears(-1), BeginOfYear(now));
					break;
				case DynamicFilterType.YearToDate:
					UpdateDynamicMinMaxValue(BeginOfYear(now), now.AddDays(1));
					break;
			}
		}
		DateTime BeginOfMonth(DateTime value) {
			return new DateTime(value.Year, value.Month, 1);
		}
		DateTime BeginOfYear(DateTime value) {
			return new DateTime(value.Year, 1, 1);
		}
		DateTime BeginOfQuarter(DateTime value) {
			return new DateTime(value.Year, (value.Month - 1) / 3 * 3 + 1, 1);
		}
		DateTime BeginOfWeek(DateTime value) {
			return new DateTime(value.Year, value.Month, value.Day).AddDays(-(int)value.DayOfWeek);
		}
		void UpdateDynamicMinMaxValue(DateTime minValue, DateTime maxValue) {
			this.DynamicMinValue = DocumentModel.DataContext.ToDateTimeSerialDouble(minValue);
			this.DynamicMaxValue = DocumentModel.DataContext.ToDateTimeSerialDouble(maxValue);
		}
		bool DynamicFilterIsCellVisible(IAutoFilterValue autoFilterValue) {
			if (autoFilterValue == null)
				return false;
			VariantValue value = autoFilterValue.Value;
			if (!value.IsNumeric)
				return false;
			if (DynamicFilterType == DynamicFilterType.BelowAverage)
				return value.NumericValue < DynamicMinValue;
			else if (DynamicFilterType == DynamicFilterType.AboveAverage)
				return value.NumericValue > DynamicMinValue;
			if (!autoFilterValue.IsDateTime)
				return false;
			if (DynamicFilterType >= DynamicFilterType.Tomorrow && DynamicFilterType <= DynamicFilterType.YearToDate)
				return value.NumericValue >= DynamicMinValue && value.NumericValue < DynamicMaxValue;
			if (DynamicFilterType >= DynamicFilterType.Q1 && DynamicFilterType <= DynamicFilterType.Q4)
				return CalculateQuarter(value) == CalculateQuarter(DynamicFilterType);
			return CalculateMonth(value) == CalculateMonth(DynamicFilterType);
		}
		int CalculateMonth(VariantValue value) {
			return DocumentModel.DataContext.FromDateTimeSerial(value.NumericValue).Month;
		}
		int CalculateQuarter(VariantValue value) {
			return (CalculateMonth(value) - 1) / 3 + 1;
		}
		int CalculateMonth(DynamicFilterType type) {
			return Math.Min(12, Math.Max(1, type - DynamicFilterType.M1 + 1));
		}
		int CalculateQuarter(DynamicFilterType type) {
			return Math.Min(4, Math.Max(1, type - DynamicFilterType.Q1 + 1));
		}
		double CalculateAverage(IAutoFilterValueProvider valueProvider) {
			return CalculateFunctionValue(new FunctionAverage(), valueProvider, 0);
		}
		double CalculateTop10Value(IAutoFilterValueProvider valueProvider, IFilteringBehaviour filteringBehaviour) {
			if (filteringBehaviour.DefaultTop10Behaviour) {
				ISpreadsheetFunction function = CalculateTop10DefaultFunction();
				double order = CalculateTop10Order(valueProvider);
				top10FilterValue = CalculateFunctionValue(function, valueProvider, order, 0);
				return top10FilterValue;
			}
			else {
				ISpreadsheetFunction function = CalculateTop10PivotFunction();
				double order = CalculateTop10PivotOrder(valueProvider);
				top10FilterValue = CalculateFunctionValue(function, valueProvider, order, 0);
				return TopOrBottomDoubleValue;
			}
		}
		ISpreadsheetFunction CalculateTop10PivotFunction() {
			if (Top10FilterType == Model.Top10FilterType.Percent) {
				if (FilterByTopOrder)
					return new PivotTop10PercentFilterLarge();
				else
					return new PivotTop10PercentFilterSmall();
			}
			else if (Top10FilterType == Model.Top10FilterType.Sum) {
				if (FilterByTopOrder)
					return new PivotTop10SumFilterLarge();
				else
					return new PivotTop10SumFilterSmall();
			}
			else
				return CalculateTop10DefaultFunction();
		}
		ISpreadsheetFunction CalculateTop10DefaultFunction() {
			if (FilterByTopOrder)
				return new FunctionLarge();
			else
				return new FunctionSmall();
		}
		double CalculateTop10Order(IAutoFilterValueProvider valueProvider) {
			double count = GetNumericValuesCount(valueProvider);
			if (Top10FilterType == Model.Top10FilterType.Percent)
				return Math.Max(1, Math.Floor(TopOrBottomDoubleValue * count / 100));
			else
				return TopOrBottomDoubleValue > count ? count : TopOrBottomDoubleValue;
		}
		double CalculateTop10PivotOrder(IAutoFilterValueProvider valueProvider) {
			if (Top10FilterType == Model.Top10FilterType.Percent)
				return TopOrBottomDoubleValue / 100;
			else if (Top10FilterType == Model.Top10FilterType.Sum)
				return TopOrBottomDoubleValue;
			else {
				double count = GetNumericValuesCount(valueProvider);
				return TopOrBottomDoubleValue > count ? count : TopOrBottomDoubleValue;
			}
		}
		double CalculateFunctionValue(ISpreadsheetFunction function, IAutoFilterValueProvider valueProvider, double defaultValue) {
			VariantValue argument = VariantValue.FromArray(valueProvider.GetVariantArray());
			return CalculateFunctionValueCore(function, new VariantValue[] { argument }, defaultValue);
		}
		double CalculateFunctionValue(ISpreadsheetFunction function, IAutoFilterValueProvider valueProvider, double parameter, double defaultValue) {
			VariantValue argument = VariantValue.FromArray(valueProvider.GetVariantArray());
			return CalculateFunctionValueCore(function, new VariantValue[] { argument, parameter }, defaultValue);
		}
		double CalculateFunctionValueCore(ISpreadsheetFunction function, VariantValue[] arguments, double defaultValue) {
			VariantValue result = function.Evaluate(arguments, DocumentModel.DataContext, false);
			if (result.IsNumeric)
				return result.NumericValue;
			else
				return defaultValue;
		}
		double GetNumericValuesCount(IAutoFilterValueProvider valueProvider) {
			return valueProvider.GetNumericValuesCount();
		}
		bool Top10FilterIsCellVisible(IAutoFilterValue autoFilterValue) {
			if (autoFilterValue == null)
				return false;
			VariantValue value = autoFilterValue.Value;
			if (!value.IsNumeric)
				return false;
			if (FilterByTopOrder)
				return value.NumericValue >= top10FilterValue;
			else
				return value.NumericValue <= top10FilterValue;
		}
	}
	#endregion
	#region AutoFilterColumnCollection
	public class AutoFilterColumnCollection : IEnumerable<AutoFilterColumn> {
		List<AutoFilterColumn> innerList;
		Worksheet sheet;
		public AutoFilterColumnCollection(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			innerList = new List<AutoFilterColumn>();
		}
		public int Count { get { return innerList.Count; } }
		public AutoFilterColumn this[int index] { get { return innerList[index]; } }
		public Worksheet Sheet { get { return sheet; } }
		public int Add(AutoFilterColumn item) {
			Guard.ArgumentNotNull(item, "item");
			Debug.Assert(item.ColumnId == Count);
			Insert(innerList.Count, item);
			return innerList.Count - 1;
		}
		public int IndexOf(AutoFilterColumn item) {
			return innerList.IndexOf(item);
		}
		public void Insert(int index, AutoFilterColumn item) {
			Guard.ArgumentNotNull(item, "item");
			Debug.Assert(item.ColumnId == index);
			for (int i = index; i < Count; ++i)
				++innerList[i].ColumnId;
			DocumentHistory history = item.DocumentModel.History;
			AutoFilterColumnInsertHistoryItem historyItem = new AutoFilterColumnInsertHistoryItem(this, item, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public void RemoveAt(int index) {
			AutoFilterColumn deletedColumn = innerList[index];
			Remove(deletedColumn);
		}
		public void Remove(AutoFilterColumn item) {
			Guard.ArgumentNotNull(item, "item");
			for (int i = item.ColumnId + 1; i < Count; ++i)
				--innerList[i].ColumnId;
			DocumentHistory history = item.DocumentModel.History;
			AutoFilterColumnDeleteHistoryItem historyItem = new AutoFilterColumnDeleteHistoryItem(this, item);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void AddCore(AutoFilterColumn item) {
			Guard.ArgumentNotNull(item, "item");
			innerList.Add(item);
		}
		protected internal void ClearCore() {
			innerList.Clear();
		}
		protected internal virtual void RemoveCore(AutoFilterColumn item) {
			Guard.ArgumentNotNull(item, "item");
			innerList.Remove(item);
		}
		protected internal virtual void InsertCore(AutoFilterColumn item, int index) {
			Guard.ArgumentNotNull(item, "item");
			Debug.Assert(item.ColumnId == index);
			innerList.Insert(index, item);
		}
		protected internal void Clear() {
			for (int i = Count - 1; i >= 0; --i)
				RemoveAt(i);
		}
		public IEnumerator<AutoFilterColumn> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	#endregion
	#region IAutoFilterValueProvider
	public interface IAutoFilterValueProvider {
		IEnumerable<VariantValue> GetValuesEnumerable();
		IVariantArray GetVariantArray();
		IEnumerable<IAutoFilterValue> GetAutoFilterValuesEnumerable();
		double GetNumericValuesCount();
	}
	public class CellAutoFilterValueProvider : IAutoFilterValueProvider {
		readonly CellRange range;
		public CellAutoFilterValueProvider(CellRange range) {
			this.range = range;
		}
		#region IAutoFilterValueProvider Members
		public IEnumerable<VariantValue> GetValuesEnumerable() {
			int valuesCount = (int)range.CellCount;
			for (int i = 0; i < valuesCount; i++) {
				VariantValue value = range.GetCellValueByZOrder(i);
				yield return value;
			}
		}
		public IVariantArray GetVariantArray() {
			return new RangeVariantArray(range);
		}
		public IEnumerable<IAutoFilterValue> GetAutoFilterValuesEnumerable() {
			foreach (ICell cell in range.GetExistingCellsEnumerable())
				yield return cell;
		}
		public double GetNumericValuesCount() {
			int result = 0;
			foreach (VariantValue value in GetValuesEnumerable()) {
				if (value.IsNumeric)
					result++;
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region AutoFilterValue
	public interface IAutoFilterValue {
		VariantValue Value { get; }
		string Text { get; }
		bool IsDateTime { get; }
	}
	#endregion
}
