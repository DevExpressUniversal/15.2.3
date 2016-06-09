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
using DevExpress.XtraSpreadsheet.Utils;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTableCache
	public class PivotFieldCache : UniqueItemsCache<PivotFieldInfo> {
		public PivotFieldCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override PivotFieldInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new PivotFieldInfo();
		}
	}
	#endregion
	#region PivotFieldInfo
	public class PivotFieldInfo : ICloneable<PivotFieldInfo>, ISupportsCopyFrom<PivotFieldInfo>, ISupportsSizeOf {
		#region Fields
		int itemPageCount;
		int? rankBy;
		int numberFormatIndex;
		uint firstPackedValues;
		uint secondPackedValues;
		const uint maskCompact = 1;
		const uint maskSubtotalTop = 2;
		const uint maskDragOff = 4;
		const uint maskDragToCol = 8;
		const uint maskDragToData = 0x10;
		const uint maskDragToPage = 0x20;
		const uint maskDragToRow = 0x40;
		const uint maskOutline = 0x80;
		const uint maskShowDropDowns = 0x100;
		const uint maskTopAutoShow = 0x200;
		const uint maskEnumItemType = 0x1FFFC00;
		const uint maskEnumSortTypeField = 0x6000000;
		const uint maskEnumAxis = 0x38000000;
		const int offsetEnumItemType = 10;
		const int offsetEnumSortTypeField = 25;
		const int offsetEnumAxis = 27;
		const uint maskAllDrilled = 1;
		const uint maskAutoShow = 2;
		const uint maskDataField = 4;
		const uint maskDefaultAttributeDrillState = 8;
		const uint maskHiddenLevel = 0x10;
		const uint maskHideNewItems = 0x20;
		const uint maskIncludeNewItemsInFilter = 0x40;
		const uint maskInsertBlankRow = 0x80;
		const uint maskInsertPageBreak = 0x100;
		const uint maskMeasureFilter = 0x200;
		const uint maskNonAutoSortDefault = 0x400;
		const uint maskServerField = 0x800;
		const uint maskShowPropAsCaption = 0x1000;
		const uint maskShowPropCell = 0x2000;
		const uint maskShowPropTip = 0x4000;
		const uint maskDataSourceSort = 0x8000;
		const uint maskHasDataSourceSort = 0x10000;
		const uint maskMultipleItemSelectionAllowed = 0x20000;
		const uint maskShowAll = 0x40000;
		const uint maskFillDownLabels = 0x80000;
		#endregion
		#region Constructors
		public PivotFieldInfo() {
			firstPackedValues = 0x4000BFF;
			secondPackedValues = 0;
			itemPageCount = 10;
		}
		#endregion
		#region Properties
		public PivotTableAxis Axis {
			get { return (PivotTableAxis)PackedValues.GetIntBitValue(this.firstPackedValues, maskEnumAxis, offsetEnumAxis); }
			set { PackedValues.SetIntBitValue(ref this.firstPackedValues, maskEnumAxis, offsetEnumAxis, (int)value); }
		}
		public PivotTableSortTypeField SortType {
			get { return (PivotTableSortTypeField)PackedValues.GetIntBitValue(this.firstPackedValues, maskEnumSortTypeField, offsetEnumSortTypeField); }
			set { PackedValues.SetIntBitValue(ref this.firstPackedValues, maskEnumSortTypeField, offsetEnumSortTypeField, (int)value); }
		}
		public PivotFieldItemType Subtotal {
			get { return (PivotFieldItemType)PackedValues.GetIntBitValue(this.firstPackedValues, maskEnumItemType, offsetEnumItemType); }
			set { PackedValues.SetIntBitValue(ref this.firstPackedValues, maskEnumItemType, offsetEnumItemType, (int)value); }
		}
		public int ItemPageCount { get { return itemPageCount; } set { itemPageCount = value; } }
		public int? RankBy { get { return rankBy; } set { rankBy = value; } }
		public int NumberFormatIndex { get { return numberFormatIndex; } set { numberFormatIndex = value; } }
		public bool Compact {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskCompact); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskCompact, value); }
		}
		public bool DragOff {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskDragOff); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskDragOff, value); }
		}
		public bool DragToCol {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskDragToCol); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskDragToCol, value); }
		}
		public bool DragToData {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskDragToData); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskDragToData, value); }
		}
		public bool DragToPage {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskDragToPage); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskDragToPage, value); }
		}
		public bool DragToRow {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskDragToRow); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskDragToRow, value); }
		}
		public bool Outline {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskOutline); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskOutline, value); }
		}
		public bool ShowDropDowns {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskShowDropDowns); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskShowDropDowns, value); }
		}
		public bool SubtotalTop {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskSubtotalTop); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskSubtotalTop, value); }
		}
		public bool TopAutoShow {
			get { return PackedValues.GetBoolBitValue(this.firstPackedValues, maskTopAutoShow); }
			set { PackedValues.SetBoolBitValue(ref this.firstPackedValues, maskTopAutoShow, value); }
		}
		public bool MultipleItemSelectionAllowed {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskMultipleItemSelectionAllowed); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskMultipleItemSelectionAllowed, value); }
		}
		public bool ShowItemsWithNoData {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskShowAll); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskShowAll, value); }
		}
		public bool AllDrilled {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskAllDrilled); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskAllDrilled, value); }
		}
		public bool AutoShow {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskAutoShow); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskAutoShow, value); }
		}
		public bool IsDataField {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskDataField); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskDataField, value); }
		}
		public bool DefaultAttributeDrillState {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskDefaultAttributeDrillState); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskDefaultAttributeDrillState, value); }
		}
		public bool HiddenLevel {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskHiddenLevel); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskHiddenLevel, value); }
		}
		public bool HideNewItems {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskHideNewItems); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskHideNewItems, value); }
		}
		public bool IncludeNewItemsInFilter {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskIncludeNewItemsInFilter); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskIncludeNewItemsInFilter, value); }
		}
		public bool InsertBlankRow {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskInsertBlankRow); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskInsertBlankRow, value); }
		}
		public bool InsertPageBreak {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskInsertPageBreak); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskInsertPageBreak, value); }
		}
		public bool MeasureFilter {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskMeasureFilter); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskMeasureFilter, value); }
		}
		public bool NonAutoSortDefault {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskNonAutoSortDefault); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskNonAutoSortDefault, value); }
		}
		public bool ServerField {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskServerField); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskServerField, value); }
		}
		public bool ShowPropAsCaption {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskShowPropAsCaption); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskShowPropAsCaption, value); }
		}
		public bool ShowPropCell {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskShowPropCell); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskShowPropCell, value); }
		}
		public bool ShowPropTip {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskShowPropTip); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskShowPropTip, value); }
		}
		public bool DataSourceSort {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskDataSourceSort); }
			set {
				PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskDataSourceSort, value);
				HasDataSourceSort = true;
			}
		}
		public bool HasDataSourceSort {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskHasDataSourceSort); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskHasDataSourceSort, value); }
		}
		public bool FillDownLabels {
			get { return PackedValues.GetBoolBitValue(this.secondPackedValues, maskFillDownLabels); }
			set { PackedValues.SetBoolBitValue(ref this.secondPackedValues, maskFillDownLabels, value); }
		}
		#endregion
		#region ICloneable<PivotFieldInfo> Members
		public PivotFieldInfo Clone() {
			PivotFieldInfo result = new PivotFieldInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PivotFieldInfo> Members
		public void CopyFrom(PivotFieldInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.itemPageCount = value.ItemPageCount;
			this.rankBy = value.RankBy;
			this.numberFormatIndex = value.numberFormatIndex;
			this.firstPackedValues = value.firstPackedValues;
			this.secondPackedValues = value.secondPackedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			PivotFieldInfo info = obj as PivotFieldInfo;
			if (info == null)
				return false;
			return this.itemPageCount == info.ItemPageCount &&
				   this.rankBy == info.RankBy &&
				   this.numberFormatIndex == info.numberFormatIndex &&
				   this.firstPackedValues == info.firstPackedValues &&
				   this.secondPackedValues == info.secondPackedValues;
		}
		public override int GetHashCode() {
			CombinedHashCode hashCode = new CombinedHashCode();
			hashCode.AddInt(this.itemPageCount);
			hashCode.AddInt(this.rankBy ?? 0);
			hashCode.AddInt(this.numberFormatIndex);
			hashCode.AddInt((int)this.firstPackedValues);
			hashCode.AddInt((int)this.secondPackedValues);
			return hashCode.CombinedHash32;
		}
	}
	#endregion
	#region PivotField
	public class PivotField : SpreadsheetUndoableIndexBasedObject<PivotFieldInfo> {
		#region Fields
		readonly PivotItemCollection items;
		readonly PivotTable pivotTable;
		readonly PivotArea pivotArea;
		string name;
		string subtotalCaption;
		string uniqueMemberProperty;
		#endregion
		public PivotField(PivotTable pivotTable)
			: base(pivotTable.DocumentModel) {
			this.pivotTable = pivotTable;
			items = new PivotItemCollection(pivotTable.DocumentModel);
			pivotArea = new PivotArea(pivotTable.DocumentModel);
		}
		#region Properties
		public PivotItemCollection Items { get { return items; } }
		public PivotArea PivotArea { get { return pivotArea; } }
		#region Name
		public PivotTable PivotTable { get { return pivotTable; } }
		public string Name {
			get { return name; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(Name, value) != 0) {
					pivotTable.CheckActiveTransaction();
					SetName(value);
				}
			}
		}
		public void SetName(string value) {
			HistoryHelper.SetValue(DocumentModel, Name, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetNameCore); 
		}
		protected internal void SetNameCore(string value) {
			this.name = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region SubtotalCaption
		public string SubtotalCaption {
			get { return subtotalCaption; }
			set {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(SubtotalCaption, value) != 0) {
					pivotTable.CheckActiveTransaction();
					SetSubtotalCaption(value);
				}
			}
		}
		public void SetSubtotalCaption(string value) {
			HistoryHelper.SetValue(DocumentModel, SubtotalCaption, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetSubtotalCaptionCore);
		}
		protected internal void SetSubtotalCaptionCore(string value) {
			this.subtotalCaption = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
		}
		#endregion
		#region UniqueMemberProperty
		public string UniqueMemberProperty {
			get { return uniqueMemberProperty; }
			set { HistoryHelper.SetValue(DocumentModel, UniqueMemberProperty, value, StringExtensions.ComparerInvariantCultureIgnoreCase, SetUniqueMemberPropertyCore); }
		}
		protected internal void SetUniqueMemberPropertyCore(string value) {
			uniqueMemberProperty = value;
		}
		#endregion
		#region Axis
		public PivotTableAxis Axis {
			get { return Info.Axis; }
			set {
				if (Axis != value)
					SetPropertyValue(SetAxisCore, value);
			}
		}
		DocumentModelChangeActions SetAxisCore(PivotFieldInfo info, PivotTableAxis value) {
			info.Axis = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SortType
		public PivotTableSortTypeField SortType {
			get { return Info.SortType; }
			set {
				if (SortType != value) {
					pivotTable.CheckActiveTransaction();
					SetSortType(value);
				}
			}
		}
		public void SetSortType(PivotTableSortTypeField value) {
			SetPropertyValue(SetSortTypeCore, value);
		}
		DocumentModelChangeActions SetSortTypeCore(PivotFieldInfo info, PivotTableSortTypeField value) {
			info.SortType = value;
			pivotTable.CalculationInfo.InvalidateCalculatedCache();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ItemPageCount
		public int ItemPageCount {
			get { return Info.ItemPageCount; }
			set {
				if (ItemPageCount != value)
					SetPropertyValue(SetItemPageCountCore, value);
			}
		}
		DocumentModelChangeActions SetItemPageCountCore(PivotFieldInfo info, int value) {
			info.ItemPageCount = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RankBy
		public int? RankBy {
			get { return Info.RankBy; }
			set {
				if (RankBy != value)
					SetPropertyValue(SetRankByCore, value);
			}
		}
		DocumentModelChangeActions SetRankByCore(PivotFieldInfo info, int? value) {
			info.RankBy = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NumberFormat
		public int NumberFormatIndex {
			get { return Info.NumberFormatIndex; }
			set {
				if (NumberFormatIndex != value) {
					pivotTable.CheckActiveTransaction();
					SetNumberFormatIndex(value);
				}
			}
		}
		public void SetNumberFormatIndex(int value) {
			SetPropertyValue(SetNumberFormatIndexCore, value);
		}
		DocumentModelChangeActions SetNumberFormatIndexCore(PivotFieldInfo info, int value) {
			info.NumberFormatIndex = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Compact
		public bool Compact {
			get { return Info.Compact; }
			set {
				if (Compact != value) {
					pivotTable.CheckActiveTransaction();
					SetCompact(value);
				}
			}
		}
		public void SetCompact(bool value) {
			SetPropertyValue(SetCompactCore, value);
		}
		DocumentModelChangeActions SetCompactCore(PivotFieldInfo info, bool value) {
			info.Compact = value;
			pivotTable.CalculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CompactForm
		internal bool CompactForm { get { return Compact && Outline; } }
		#endregion
		#region DragOff
		public bool DragOff {
			get { return Info.DragOff; }
			set {
				if (DragOff != value)
					SetPropertyValue(SetDragOffCore, value);
			}
		}
		DocumentModelChangeActions SetDragOffCore(PivotFieldInfo info, bool value) {
			info.DragOff = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DragToCol
		public bool DragToCol {
			get { return Info.DragToCol; }
			set {
				if (DragToCol != value)
					SetPropertyValue(SetDragToColCore, value);
			}
		}
		DocumentModelChangeActions SetDragToColCore(PivotFieldInfo info, bool value) {
			info.DragToCol = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DragToData
		public bool DragToData {
			get { return Info.DragToData; }
			set {
				if (DragToData != value)
					SetPropertyValue(SetDragToDataCore, value);
			}
		}
		DocumentModelChangeActions SetDragToDataCore(PivotFieldInfo info, bool value) {
			info.DragToData = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DragToPage
		public bool DragToPage {
			get { return Info.DragToPage; }
			set {
				if (DragToPage != value)
					SetPropertyValue(SetDragToPageCore, value);
			}
		}
		DocumentModelChangeActions SetDragToPageCore(PivotFieldInfo info, bool value) {
			info.DragToPage = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DragToRow
		public bool DragToRow {
			get { return Info.DragToRow; }
			set {
				if (DragToRow != value)
					SetPropertyValue(SetDragToRowCore, value);
			}
		}
		DocumentModelChangeActions SetDragToRowCore(PivotFieldInfo info, bool value) {
			info.DragToRow = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MultipleItemSelectionAllowed
		public bool MultipleItemSelectionAllowed {
			get { return Info.MultipleItemSelectionAllowed; }
			set {
				if (MultipleItemSelectionAllowed != value)
					SetPropertyValue(SetMultipleItemSelectionAllowedCore, value);
			}
		}
		DocumentModelChangeActions SetMultipleItemSelectionAllowedCore(PivotFieldInfo info, bool value) {
			info.MultipleItemSelectionAllowed = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Outline
		public bool Outline {
			get { return Info.Outline; }
			set {
				if (Outline != value) {
					pivotTable.CheckActiveTransaction();
					SetOutline(value);
				}
			}
		}
		public void SetOutline(bool value) {
			SetPropertyValue(SetOutlineCore, value);
		}
		DocumentModelChangeActions SetOutlineCore(PivotFieldInfo info, bool value) {
			info.Outline = value;
			pivotTable.CalculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowAll
		public bool ShowItemsWithNoData {
			get { return Info.ShowItemsWithNoData; }
			set {
				if (ShowItemsWithNoData != value) {
					pivotTable.CheckActiveTransaction();
					SetShowItemsWithNoData(value);
				}
			}
		}
		public void SetShowItemsWithNoData(bool value) {
			SetPropertyValue(SetShowItemsWithNoDataCore, value);
		}
		DocumentModelChangeActions SetShowItemsWithNoDataCore(PivotFieldInfo info, bool value) {
			info.ShowItemsWithNoData = value;
			pivotTable.CalculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDropDowns
		public bool ShowDropDowns {
			get { return Info.ShowDropDowns; }
			set {
				if (ShowDropDowns != value)
					SetPropertyValue(SetShowDropDownsCore, value);
			}
		}
		DocumentModelChangeActions SetShowDropDownsCore(PivotFieldInfo info, bool value) {
			info.ShowDropDowns = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SubtotalTop
		public bool SubtotalTop {
			get { return Info.SubtotalTop; }
			set {
				if (SubtotalTop != value) {
					pivotTable.CheckActiveTransaction();
					SetSubtotalTop(value);
				}
			}
		}
		public void SetSubtotalTop(bool value) {
			SetPropertyValue(SetSubtotalTopCore, value);
		}
		DocumentModelChangeActions SetSubtotalTopCore(PivotFieldInfo info, bool value) {
			info.SubtotalTop = value;
			pivotTable.CalculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TopAutoShow
		public bool TopAutoShow {
			get { return Info.TopAutoShow; }
			set {
				if (TopAutoShow != value)
					SetPropertyValue(SetTopAutoShowCore, value);
			}
		}
		DocumentModelChangeActions SetTopAutoShowCore(PivotFieldInfo info, bool value) {
			info.TopAutoShow = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AllDrilled
		public bool AllDrilled {
			get { return Info.AllDrilled; }
			set {
				if (AllDrilled != value)
					SetPropertyValue(SetAllDrilledCore, value);
			}
		}
		DocumentModelChangeActions SetAllDrilledCore(PivotFieldInfo info, bool value) {
			info.AllDrilled = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AutoShow
		public bool AutoShow {
			get { return Info.AutoShow; }
			set {
				if (AutoShow != value)
					SetPropertyValue(SetAutoShowCore, value);
			}
		}
		DocumentModelChangeActions SetAutoShowCore(PivotFieldInfo info, bool value) {
			info.AutoShow = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IsDataField
		public bool IsDataField {
			get { return Info.IsDataField; }
			set {
				if (IsDataField != value)
					SetPropertyValue(SetIsDataFieldCore, value);
			}
		}
		DocumentModelChangeActions SetIsDataFieldCore(PivotFieldInfo info, bool value) {
			info.IsDataField = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DefaultAttributeDrillState
		public bool DefaultAttributeDrillState {
			get { return Info.DefaultAttributeDrillState; }
			set {
				if (DefaultAttributeDrillState != value)
					SetPropertyValue(SetDefaultAttributeDrillStateCore, value);
			}
		}
		DocumentModelChangeActions SetDefaultAttributeDrillStateCore(PivotFieldInfo info, bool value) {
			info.DefaultAttributeDrillState = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HiddenLevel
		public bool HiddenLevel {
			get { return Info.HiddenLevel; }
			set {
				if (HiddenLevel != value)
					SetPropertyValue(SetHiddenLevelCore, value);
			}
		}
		DocumentModelChangeActions SetHiddenLevelCore(PivotFieldInfo info, bool value) {
			info.HiddenLevel = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HideNewItems
		public bool HideNewItems {
			get { return Info.HideNewItems; }
			set {
				if (HideNewItems != value)
					SetPropertyValue(SetHideNewItemsCore, value);
			}
		}
		DocumentModelChangeActions SetHideNewItemsCore(PivotFieldInfo info, bool value) {
			info.HideNewItems = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IncludeNewItemsInFilter
		public bool IncludeNewItemsInFilter {
			get { return Info.IncludeNewItemsInFilter; }
			set {
				if (IncludeNewItemsInFilter != value)
					SetPropertyValue(SetIncludeNewItemsInFilterCore, value);
			}
		}
		DocumentModelChangeActions SetIncludeNewItemsInFilterCore(PivotFieldInfo info, bool value) {
			info.IncludeNewItemsInFilter = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertBlankRow
		public bool InsertBlankRow {
			get { return Info.InsertBlankRow; }
			set {
				if (InsertBlankRow != value) {
					pivotTable.CheckActiveTransaction();
					SetInsertBlankRow(value);
				}
			}
		}
		public void SetInsertBlankRow(bool value) {
			SetPropertyValue(SetInsertBlankRowCore, value);
		}
		DocumentModelChangeActions SetInsertBlankRowCore(PivotFieldInfo info, bool value) {
			info.InsertBlankRow = value;
			pivotTable.CalculationInfo.InvalidateLayout();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region InsertPageBreak
		public bool InsertPageBreak {
			get { return Info.InsertPageBreak; }
			set {
				if (InsertPageBreak != value)
					SetPropertyValue(SetInsertPageBreakCore, value);
			}
		}
		DocumentModelChangeActions SetInsertPageBreakCore(PivotFieldInfo info, bool value) {
			info.InsertPageBreak = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MeasureFilter
		public bool MeasureFilter {
			get { return Info.MeasureFilter; }
			set {
				if (MeasureFilter != value)
					SetPropertyValue(SetMeasureFilterCore, value);
			}
		}
		DocumentModelChangeActions SetMeasureFilterCore(PivotFieldInfo info, bool value) {
			info.MeasureFilter = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NonAutoSortDefault
		public bool NonAutoSortDefault {
			get { return Info.NonAutoSortDefault; }
			set {
				if (NonAutoSortDefault != value)
					SetPropertyValue(SetNonAutoSortDefaultCore, value);
			}
		}
		DocumentModelChangeActions SetNonAutoSortDefaultCore(PivotFieldInfo info, bool value) {
			info.NonAutoSortDefault = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ServerField
		public bool ServerField {
			get { return Info.ServerField; }
			set {
				if (ServerField != value)
					SetPropertyValue(SetServerFieldCore, value);
			}
		}
		DocumentModelChangeActions SetServerFieldCore(PivotFieldInfo info, bool value) {
			info.ServerField = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowPropAsCaption
		public bool ShowPropAsCaption {
			get { return Info.ShowPropAsCaption; }
			set {
				if (ShowPropAsCaption != value)
					SetPropertyValue(SetShowPropAsCaptionCore, value);
			}
		}
		DocumentModelChangeActions SetShowPropAsCaptionCore(PivotFieldInfo info, bool value) {
			info.ShowPropAsCaption = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowPropCell
		public bool ShowPropCell {
			get { return Info.ShowPropCell; }
			set {
				if (ShowPropCell != value)
					SetPropertyValue(SetShowPropCellCore, value);
			}
		}
		DocumentModelChangeActions SetShowPropCellCore(PivotFieldInfo info, bool value) {
			info.ShowPropCell = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowPropTip
		public bool ShowPropTip {
			get { return Info.ShowPropTip; }
			set {
				if (ShowPropTip != value)
					SetPropertyValue(SetShowPropTipCore, value);
			}
		}
		DocumentModelChangeActions SetShowPropTipCore(PivotFieldInfo info, bool value) {
			info.ShowPropTip = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DataSourceSort
		public bool DataSourceSort {
			get { return Info.DataSourceSort; }
			set {
				HasDataSourceSort = true;
				if (DataSourceSort != value)
					SetPropertyValue(SetDataSourceSortCore, value);
			}
		}
		DocumentModelChangeActions SetDataSourceSortCore(PivotFieldInfo info, bool value) {
			info.DataSourceSort = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasDataSourceSort
		public bool HasDataSourceSort {
			get { return Info.HasDataSourceSort; }
			set {
				if (HasDataSourceSort != value)
					SetPropertyValue(SetHasDataSourceSortCore, value);
			}
		}
		DocumentModelChangeActions SetHasDataSourceSortCore(PivotFieldInfo info, bool value) {
			info.HasDataSourceSort = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FillDownLabels
		public bool FillDownLabels {
			get { return Info.FillDownLabels; }
			set {
				if (FillDownLabels != value) {
					pivotTable.CheckActiveTransaction();
					SetFillDownLabels(value);
				}
			}
		}
		public void SetFillDownLabels(bool value) {
			SetPropertyValue(SetFillDownLabelsCore, value);
		}
		DocumentModelChangeActions SetFillDownLabelsCore(PivotFieldInfo info, bool value) {
			info.FillDownLabels = value;
			pivotTable.CalculationInfo.InvalidateWorksheetData();
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FormatString
		public string FormatString { get { return DocumentModel.Cache.NumberFormatCache[NumberFormatIndex].FormatCode; } }
		public void SetFormatString(string newValue, IErrorHandler errorHandler) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(FormatString, newValue) == 0)
				return;
			pivotTable.BeginTransaction(errorHandler);
			try {
				NumberFormatIndex = NumberFormatHelper.GetNumberFormatIndex(newValue, DocumentModel);
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		#endregion
		#region Subtotal
		#region Subtotal
		public PivotFieldItemType Subtotal {
			get { return Info.Subtotal; }
			set {
				pivotTable.CheckActiveTransaction();
				SetSubtotal(value);
			}
		}
		public void SetSubtotal(PivotFieldItemType value) {
			PivotFieldItemType nonDefaultSubtotal = value & ~PivotFieldItemType.DefaultValue;
			if (nonDefaultSubtotal > 0)
				value = nonDefaultSubtotal;
			else
				if ((value & PivotFieldItemType.DefaultValue) > 0)
					value = PivotFieldItemType.DefaultValue;
			if (Subtotal != value)
				SetPropertyValue(SetSubtotalCore, value);
		}
		public void SetSubtotal(PivotFieldItemType type, IErrorHandler errorHandler) {
			pivotTable.BeginTransaction(errorHandler);
			try {
				Subtotal = type;
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		DocumentModelChangeActions SetSubtotalCore(PivotFieldInfo info, PivotFieldItemType value) {
			info.Subtotal = value;
			pivotTable.CalculationInfo.InvalidateCalculatedCache();
			return DocumentModelChangeActions.None;
		}
		#endregion
		public PivotFieldItemType GetSubtotalMergedWith(PivotFieldItemType itemType, bool value) {
			if (value)
				return Subtotal | itemType;
			else
				return Subtotal & ~itemType;
		}
		#region DefaultSubtotal
		public bool DefaultSubtotal {
			get { return (Subtotal & PivotFieldItemType.DefaultValue) > 0; }
			set {
				if (value)
					Subtotal |= PivotFieldItemType.DefaultValue;
				else
					Subtotal &= ~PivotFieldItemType.DefaultValue;
			}
		}
		#endregion
		#region ProductSubtotal
		public bool ProductSubtotal {
			get { return (Subtotal & PivotFieldItemType.Product) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Product, value); }
		}
		#endregion
		#region MinSubtotal
		public bool MinSubtotal {
			get { return (Subtotal & PivotFieldItemType.Min) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Min, value); }
		}
		#endregion
		#region MaxSubtotal
		public bool MaxSubtotal {
			get { return (Subtotal & PivotFieldItemType.Max) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Max, value); }
		}
		#endregion
		#region AvgSubtotal
		public bool AvgSubtotal {
			get { return (Subtotal & PivotFieldItemType.Avg) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Avg, value); }
		}
		#endregion
		#region CountASubtotal
		public bool CountASubtotal {
			get { return (Subtotal & PivotFieldItemType.CountA) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.CountA, value); }
		}
		#endregion
		#region CountSubtotal
		public bool CountSubtotal {
			get { return (Subtotal & PivotFieldItemType.Count) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Count, value); }
		}
		#endregion
		#region StdDevPSubtotal
		public bool StdDevPSubtotal {
			get { return (Subtotal & PivotFieldItemType.StdDevP) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.StdDevP, value); }
		}
		#endregion
		#region StdDevSubtotal
		public bool StdDevSubtotal {
			get { return (Subtotal & PivotFieldItemType.StdDev) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.StdDev, value); }
		}
		#endregion
		#region SumSubtotal
		public bool SumSubtotal {
			get { return (Subtotal & PivotFieldItemType.Sum) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Sum, value); }
		}
		#endregion
		#region VarPSubtotal
		public bool VarPSubtotal {
			get { return (Subtotal & PivotFieldItemType.VarP) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.VarP, value); }
		}
		#endregion
		#region VarSubtotal
		public bool VarSubtotal {
			get { return (Subtotal & PivotFieldItemType.Var) > 0; }
			set { Subtotal = GetSubtotalMergedWith(PivotFieldItemType.Var, value); }
		}
		#endregion
		#endregion
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<PivotFieldInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.PivotFieldCache;
		}
		#endregion
		#region Notification
		public IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			return PivotArea.CanRangeInsert(range, mode);
		}
		public IModelErrorInfo CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			return PivotArea.CanRangeRemove(range, mode);
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			PivotArea.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			PivotArea.OnRangeRemoving(context);
		}
		#endregion
		public void UnHideItems(IErrorHandler errorHandler) {
			ShowAllPivotItemsCommand command = new ShowAllPivotItemsCommand(this, errorHandler);
			command.Execute();
		}
		public bool IsFilterApplied(int index) {
			if (MeasureFilter)
				return true;
			for (int i = 0; i < pivotTable.Filters.Count; i++) {
				PivotFilter filter = pivotTable.Filters[i];
				if (filter.FieldIndex == index)
					return true;
			}
			if (PivotTable.Fields[index].Items.HiddenItemsCount > 0)
				return true;
			return false;
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotField source) {
			CopyFrom(source);
			items.CopyFromNoHistory(newPivot, source.items);
			pivotArea.CopyFromNoHistory(newPivot, offset, source.pivotArea);
			name = source.name;
			subtotalCaption = source.subtotalCaption;
			uniqueMemberProperty = source.uniqueMemberProperty;
		}
	}
	#endregion
	#region PivotItem
	public class PivotItem : IEquatable<PivotItem> {
		#region Fields
		readonly PivotTable pivotTable;
		uint packedValues;
		int itemIndex;
		string itemUserCaption;
		const uint offsetDrillAcross = 1;
		const uint offsetHideDetails = 2;
		const uint offsetChildItems = 4;
		const uint offsetHasExpandedView = 8;
		const uint offsetCalculatedMember = 0x10;
		const uint offsetHidden = 0x20;
		const uint offsetHasMissingValue = 0x40;
		const uint offsetHasCharacterValue = 0x80;
		const uint maskPivotFieldItemType = 0x7FFF00;
		const int offsetPivotFieldItemType = 8;
		#endregion
		public PivotItem(PivotTable pivotTable) {
			packedValues = 257;
			this.pivotTable = pivotTable;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return PivotTable.DocumentModel; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public bool DrillAcross {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDrillAcross); }
			set {
				if (DrillAcross != value)
					SetPackValue(packedValues, CreateNewPValueBool(offsetDrillAcross, value));
			}
		}
		public bool HideDetails {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetHideDetails); }
			set {
				if (HideDetails != value) {
					PivotTable.CheckActiveTransaction();
					SetHideDetails(value);
				}
			}
		}
		public bool ChildItems {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetChildItems); }
			set {
				if (ChildItems != value)
					SetPackValue(packedValues, CreateNewPValueBool(offsetChildItems, value));
			}
		}
		public bool HasExpandedView {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetHasExpandedView); }
			set {
				if (HasExpandedView != value)
					SetPackValue(packedValues, CreateNewPValueBool(offsetHasExpandedView, value));
			}
		}
		public bool CalculatedMember {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetCalculatedMember); }
			set {
				if (CalculatedMember != value)
					SetPackValue(packedValues, CreateNewPValueBool(offsetCalculatedMember, value));
			}
		}
		public bool IsHidden {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetHidden); }
			set {
				if (IsHidden != value) {
					PivotTable.CheckActiveTransaction();
					SetIsHidden(value);
				}
			}
		}
		public bool HasMissingValue {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetHasMissingValue); }
			set {
				if (HasMissingValue != value)
					SetPackValue(packedValues, CreateNewPValueBool(offsetHasMissingValue, value));
			}
		}
		public bool HasCharacterValue {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetHasCharacterValue); }
			set {
				if (HasCharacterValue != value)
					SetPackValue(packedValues, CreateNewPValueBool(offsetHasCharacterValue, value));
			}
		}
		public PivotFieldItemType ItemType {
			get { return (PivotFieldItemType)PackedValues.GetIntBitValue(this.packedValues, maskPivotFieldItemType, offsetPivotFieldItemType); }
			set {
				if (ItemType != value)
					SetPackValue(packedValues, CreateNewPValueEnum((int)value));
			}
		}
		public int ItemIndex { get { return itemIndex; } set { SetItemIndex(value); } }
		public string ItemUserCaption { get { return itemUserCaption; } set { SetItemUserCaption(value); } }
		public bool IsDataItem { get { return ItemType == PivotFieldItemType.Data; } }
		#endregion
		#region HiddenChanged
		EventHandler onHiddenChanged;
		public event EventHandler HiddenChanged { add { onHiddenChanged += value; } remove { onHiddenChanged -= value; } }
		protected internal virtual void RaiseHiddenChanged(bool value) {
			if (onHiddenChanged != null)
				onHiddenChanged(this, EventArgs.Empty);
		}
		#endregion
		#region HideDetailsChanged
		EventHandler onHideDetailsChanged;
		public event EventHandler HideDetailsChanged { add { onHideDetailsChanged += value; } remove { onHideDetailsChanged -= value; } }
		protected internal virtual void RaiseHideDetailsChanged(bool value) {
			if (onHideDetailsChanged != null)
				onHideDetailsChanged(this, EventArgs.Empty);
		}
		#endregion
		void SetPackValue(uint oldValue, uint newValue) {
			ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, (int)oldValue, (int)newValue, SetPackValueCore);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPackValueCore(int value) {
			bool previousHideDetails = HideDetails;
			bool previousHidden = IsHidden;
			packedValues = (uint)value;
			if (previousHideDetails != HideDetails)
				RaiseHideDetailsChanged(HideDetails);
			if (previousHidden != IsHidden)
				RaiseHiddenChanged(IsHidden);
		}
		uint CreateNewPValueEnum(int value) {
			uint nPack = this.packedValues;
			PackedValues.SetIntBitValue(ref nPack, maskPivotFieldItemType, offsetPivotFieldItemType, (int)value);
			return nPack;
		}
		void SetItemIndex(int value) {
			if (ItemIndex == value)
				return;
			ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, ItemIndex, value, SetItemIndexCore);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		void SetItemUserCaption(string value) {
			if (value == null && ItemUserCaption == null)
				return;
			if (value != null && ItemUserCaption != null)
				if (string.Compare(value, ItemUserCaption, false) == 0)
					return;
			ActionHistoryItem<string> historyItem = new ActionHistoryItem<string>(DocumentModel, ItemUserCaption, value, SetItemUserCaptionCore);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		uint CreateNewPValueBool(uint offset, bool value) {
			uint nPack = this.packedValues;
			PackedValues.SetBoolBitValue(ref nPack, offset, value);
			return nPack;
		}
		protected internal void SetDrillAcrossCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetDrillAcross, value);
		}
		public void SetHideDetails(bool value) {
			SetPackValue(packedValues, CreateNewPValueBool(offsetHideDetails, value));
			PivotTable.CalculationInfo.InvalidateLayout();
		}
		protected internal void SetHideDetailsCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetHideDetails, value);
		}
		protected internal void SetChildItemsCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetChildItems, value);
		}
		protected internal void SetHasExpandedViewCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetHasExpandedView, value);
		}
		protected internal void SetCalculatedMemberCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetCalculatedMember, value);
		}
		public void SetIsHidden(bool value) {
			System.Diagnostics.Debug.Assert(ItemType == PivotFieldItemType.Data);
			SetPackValue(packedValues, CreateNewPValueBool(offsetHidden, value));
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		protected internal void SetIsHiddenCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetHidden, value);
		}
		protected internal void SetHasMissingValueCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetHasMissingValue, value);
		}
		protected internal void SetHasCharacterValueCore(bool value) {
			PackedValues.SetBoolBitValue(ref this.packedValues, offsetHasCharacterValue, value);
		}
		protected internal void SetItemTypeCore(PivotFieldItemType value) {
			PackedValues.SetIntBitValue(ref packedValues, maskPivotFieldItemType, offsetPivotFieldItemType, (int)value);
		}
		protected internal void SetItemIndexCore(int value) {
			itemIndex = value;
		}
		protected internal void SetItemUserCaptionCore(string value) {
			itemUserCaption = value;
		}
		public override string ToString() {
			return "PivotItem type: " + ItemType + ", shared item index:" + ItemIndex;
		}
		public override bool Equals(object obj) {
			PivotItem other = obj as PivotItem;
			if (other == null)
				return false;
			return Equals(other);
		}
		public bool Equals(PivotItem other) {
			if (other == null)
				return false;
			if (!Object.ReferenceEquals(this.pivotTable, other.pivotTable))
				return false;
			return (packedValues == other.packedValues)
				&& (itemIndex == other.itemIndex)
				&& (itemUserCaption == other.itemUserCaption);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)packedValues, itemIndex, itemUserCaption.GetHashCode());
		}
		public void CopyFrom(PivotItem source) {
			this.packedValues = source.packedValues;
			this.itemIndex = source.itemIndex;
			this.itemUserCaption = source.itemUserCaption;
		}
		public void CopyFromNoHistory(PivotItem source) {
			packedValues = source.packedValues;
			itemIndex = source.itemIndex;
			itemUserCaption = source.itemUserCaption;
		}
	}
	#endregion
	#region PivotArea
	public class PivotArea {
		#region Fields
		readonly PivotAreaReferenceCollection references;
		readonly DocumentModel documentModel;
		int? field;
		int? fieldPosition;
		uint packedValues;
		CellRange range;
		const uint offsetDataOnly = 1;
		const uint offsetOutline = 2;
		const uint offsetCacheIndex = 4;
		const uint offsetCollapsedLevelsAreSubtotals = 8;
		const uint offsetGrandCol = 0x10;
		const uint offsetGrandRow = 0x20;
		const uint offsetLabelOnly = 0x40;
		const uint maskAxis = 0x780;
		const uint maskAreaType = 0x3800;
		const int offsetAxis = 7;
		const int offsetAreaType = 11;
		const int defaultPackedValues = 2051;
		#endregion
		#region Constructor
		public PivotArea(DocumentModel documentModel) {
			this.documentModel = documentModel;
			references = new PivotAreaReferenceCollection(documentModel);
			packedValues = defaultPackedValues;
		}
		#endregion
		#region Properties
		public PivotAreaReferenceCollection References { get { return references; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PivotTableAxis Axis {
			get { return (PivotTableAxis)PackedValues.GetIntBitValue(this.packedValues, maskAxis, offsetAxis); }
			set {
				if (Axis != value)
					SetEnum(maskAxis, offsetAxis, (int)value);
			}
		}
		public PivotAreaType Type {
			get { return (PivotAreaType)PackedValues.GetIntBitValue(this.packedValues, maskAreaType, offsetAreaType); }
			set {
				if (Type != value)
					SetEnum(maskAreaType, offsetAreaType, (int)value);
			}
		}
		public int? Field { get { return field; } set { SetField(value); } }
		public int? FieldPosition { get { return fieldPosition; } set { SetFieldPosition(value); } }
		public bool IsDataOnly {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetDataOnly); }
			set {
				if (IsDataOnly != value)
					SetHistory(offsetDataOnly, value);
			}
		}
		public bool IsOutline {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetOutline); }
			set {
				if (IsOutline != value)
					SetHistory(offsetOutline, value);
			}
		}
		public bool IsCacheIndex {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetCacheIndex); }
			set {
				if (IsCacheIndex != value)
					SetHistory(offsetCacheIndex, value);
			}
		}
		public bool IsCollapsedLevelsAreSubtotals {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetCollapsedLevelsAreSubtotals); }
			set {
				if (IsCollapsedLevelsAreSubtotals != value)
					SetHistory(offsetCollapsedLevelsAreSubtotals, value);
			}
		}
		public bool IsGrandColumn {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetGrandCol); }
			set {
				if (IsGrandColumn != value)
					SetHistory(offsetGrandCol, value);
			}
		}
		public bool IsGrandRow {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetGrandRow); }
			set {
				if (IsGrandRow != value)
					SetHistory(offsetGrandRow, value);
			}
		}
		public bool IsLabelOnly {
			get { return PackedValues.GetBoolBitValue(this.packedValues, offsetLabelOnly); }
			set {
				if (IsLabelOnly != value)
					SetHistory(offsetLabelOnly, value);
			}
		}
		public CellRange Range { get { return range; } set { HistoryHelper.SetValue(DocumentModel, range, value, SetRangeCore); } }
		public bool IsDefault {
			get {
				return range == null && references.Count == 0 && packedValues == defaultPackedValues &&
					   !field.HasValue && !fieldPosition.HasValue;
			}
		}
		#endregion
		void SetHistory(uint offset, bool value) {
			ActionHistoryItem<uint> historyItem =
				new ActionHistoryItem<uint>(DocumentModel, packedValues, CreateNewPValueBool(offset, value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		void SetField(int? value) {
			if (Field == value)
				return;
			ActionHistoryItem<int?> historyItem = new ActionHistoryItem<int?>(DocumentModel, Field, value, SetFieldCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		void SetFieldPosition(int? value) {
			if (FieldPosition == value)
				return;
			ActionHistoryItem<int?> historyItem = new ActionHistoryItem<int?>(DocumentModel, FieldPosition, value, SetFieldPositionCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		void SetEnum(uint mask, int offset, int value) {
			ActionHistoryItem<uint> historyItem =
				new ActionHistoryItem<uint>(DocumentModel, packedValues, CreateNewPValueEnum(mask, offset, (int)value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		uint CreateNewPValueBool(uint offset, bool value) {
			uint nPack = (uint)this.packedValues;
			PackedValues.SetBoolBitValue(ref nPack, offset, value);
			return nPack;
		}
		uint CreateNewPValueEnum(uint mask, int offset, int value) {
			uint nPack = this.packedValues;
			PackedValues.SetIntBitValue(ref nPack, mask, offset, value);
			return nPack;
		}
		void SetPackageNewValueCore(uint value) {
			packedValues = value;
		}
		protected internal void SetFieldCore(int? value) {
			field = value;
		}
		protected internal void SetFieldPositionCore(int? value) {
			fieldPosition = value;
		}
		protected internal void SetOutlineCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetOutline, value);
		}
		protected internal void SetCacheIndexCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetCacheIndex, value);
		}
		protected internal void SetGrandColCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetGrandCol, value);
		}
		protected internal void SetGrandRowCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetGrandRow, value);
		}
		protected internal void SetLabelOnlyCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetLabelOnly, value);
		}
		protected internal void SetCollapsedLevelsAreSubtotalsCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetCollapsedLevelsAreSubtotals, value);
		}
		protected internal void SetDataOnlyCore(bool value) {
			PackedValues.SetBoolBitValue(ref packedValues, offsetDataOnly, value);
		}
		protected internal void SetTypeCore(PivotAreaType value) {
			PackedValues.SetIntBitValue(ref this.packedValues, maskAreaType, offsetAreaType, (int)value);
		}
		protected internal void SetAxisCore(PivotTableAxis value) {
			PackedValues.SetIntBitValue(ref this.packedValues, maskAxis, offsetAxis, (int)value);
		}
		protected internal void SetRangeCore(CellRange value) {
			range = value;
		}
		#region Notification
		public IModelErrorInfo CanRangeInsert(CellRangeBase range, InsertCellMode mode) {
			return null;
		}
		public IModelErrorInfo CanRangeRemove(CellRangeBase range, RemoveCellMode mode) {
			return null;
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			if (range != null)
				Range = (CellRange)notificationContext.Visitor.ProcessCellRange(range.Clone());
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			if (range != null)
				Range = (CellRange)notificationContext.Visitor.ProcessCellRange(range.Clone());
		}
		#endregion
		public void Clear() {
			if (IsDefault)
				return;
			documentModel.BeginUpdate();
			try {
				references.Clear();
				Range = null;
				Field = null;
				FieldPosition = null;
				Axis = PivotTableAxis.None;
				Type = PivotAreaType.Normal;
				IsDataOnly = true;
				IsLabelOnly = false;
				IsGrandRow = false;
				IsGrandColumn = false;
				IsCacheIndex = false;
				IsOutline = true;
				IsCollapsedLevelsAreSubtotals = false;
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		public PivotArea Clone(DocumentModel anotherModel, Worksheet newWorksheet, CellPositionOffset offset) {
			PivotArea result = new PivotArea(anotherModel);
			result.CopyFrom(this, anotherModel, newWorksheet, offset);
			return result;
		}
		public void CopyFrom(PivotArea sourcePivotArea, DocumentModel anotherModel, Worksheet newWorksheet, CellPositionOffset offset) {
			this.references.CopyFrom(sourcePivotArea.References, anotherModel);
			this.field = sourcePivotArea.field;
			this.fieldPosition = sourcePivotArea.fieldPosition;
			this.packedValues = sourcePivotArea.packedValues;
			if (sourcePivotArea.range != null) {
				CellRange rangeInNewSheet = sourcePivotArea.range.Clone(newWorksheet) as CellRange;
				this.range = rangeInNewSheet.GetShiftedAny(offset);
			}
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotArea source) {
			this.references.CopyFromNoHistory(newPivot, source.References);
			field = source.field;
			fieldPosition = source.fieldPosition;
			packedValues = source.packedValues;
			if (source.range != null)
				this.range = source.range.GetShiftedAny(offset, newPivot.Worksheet) as CellRange;
		}
	}
	#endregion
	#region PivotAreaReference
	public class PivotAreaReference {
		#region Fields
		const uint maskSubtotal = 0x7FFF;
		const uint offsetSelected = 0x8000;
		const uint offsetByPosition = 0x10000;
		const uint offsetRelative = 0x20000;
		readonly DocumentModel documentModel;
		int packedValues;
		long? field;
		List<long> sharedItemsIndex; 
		#endregion
		#region Constructors
		public PivotAreaReference(DocumentModel documentModel) {
			this.documentModel = documentModel;
			packedValues = 0;
			sharedItemsIndex = new List<long>();
		}
		#endregion
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public List<long> SharedItemsIndex { get { return sharedItemsIndex; } }
		public PivotFieldItemType Subtotal {
			get { return (PivotFieldItemType)PackedValues.GetIntBitValue((uint)this.packedValues, maskSubtotal); }
			set { SetSubtotal((int)value); }
		}
		public bool IsSelected {
			get { return PackedValues.GetBoolBitValue((uint)this.packedValues, offsetSelected); }
			set {
				if (IsSelected != value)
					SetHistory(offsetSelected, value);
			}
		}
		public bool IsByPosition {
			get { return PackedValues.GetBoolBitValue((uint)this.packedValues, offsetByPosition); }
			set {
				if (IsByPosition != value)
					SetHistory(offsetByPosition, value);
			}
		}
		public bool IsRelative {
			get { return PackedValues.GetBoolBitValue((uint)this.packedValues, offsetRelative); }
			set {
				if (IsRelative != value)
					SetHistory(offsetRelative, value);
			}
		}
		#region DefaultSubtotal
		public bool DefaultSubtotal {
			get { return (Subtotal & PivotFieldItemType.DefaultValue) > 0; }
			set { SetEnumValue(PivotFieldItemType.DefaultValue, value); }
		}
		#endregion
		#region SumSubtotal
		public bool SumSubtotal {
			get { return (Subtotal & PivotFieldItemType.Sum) > 0; }
			set { SetEnumValue(PivotFieldItemType.Sum, value); }
		}
		#endregion
		#region CountASubtotal
		public bool CountASubtotal {
			get { return (Subtotal & PivotFieldItemType.CountA) > 0; }
			set { SetEnumValue(PivotFieldItemType.CountA, value); }
		}
		#endregion
		#region AvgSubtotal
		public bool AvgSubtotal {
			get { return IsEnumValue(PivotFieldItemType.Avg); }
			set { SetEnumValue(PivotFieldItemType.Avg, value); }
		}
		#endregion
		#region MaxSubtotal
		public bool MaxSubtotal {
			get { return IsEnumValue(PivotFieldItemType.Max); }
			set { SetEnumValue(PivotFieldItemType.Max, value); }
		}
		#endregion
		#region MinSubtotal
		public bool MinSubtotal {
			get { return IsEnumValue(PivotFieldItemType.Min); }
			set { SetEnumValue(PivotFieldItemType.Min, value); }
		}
		#endregion
		#region ProductSubtotal
		public bool ProductSubtotal {
			get { return IsEnumValue(PivotFieldItemType.Product); }
			set { SetEnumValue(PivotFieldItemType.Product, value); }
		}
		#endregion
		#region CountSubtotal
		public bool CountSubtotal {
			get { return IsEnumValue(PivotFieldItemType.Count); }
			set { SetEnumValue(PivotFieldItemType.Count, value); }
		}
		#endregion
		#region StdDevSubtotal
		public bool StdDevSubtotal {
			get { return IsEnumValue(PivotFieldItemType.StdDev); }
			set { SetEnumValue(PivotFieldItemType.StdDev, value); }
		}
		#endregion
		#region StdDevPSubtotal
		public bool StdDevPSubtotal {
			get { return IsEnumValue(PivotFieldItemType.StdDevP); }
			set { SetEnumValue(PivotFieldItemType.StdDevP, value); }
		}
		#endregion
		#region VarSubtotal
		public bool VarSubtotal {
			get { return IsEnumValue(PivotFieldItemType.Var); }
			set { SetEnumValue(PivotFieldItemType.Var, value); }
		}
		#endregion
		#region VarPSubtotal
		public bool VarPSubtotal {
			get { return IsEnumValue(PivotFieldItemType.VarP); }
			set { SetEnumValue(PivotFieldItemType.VarP, value); }
		}
		#endregion
		public long? Field { get { return field; } set { SetField(value); } }
		#endregion
		void SetHistory(uint offset, bool value) {
			ActionHistoryItem<int> historyItem =
				new ActionHistoryItem<int>(DocumentModel, packedValues, CreateNewPValueBool(offset, value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		void SetField(long? value) {
			if (Field == value)
				return;
			ActionHistoryItem<long?> historyItem = new ActionHistoryItem<long?>(DocumentModel, Field, value, SetFieldCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		void SetSubtotal(int value) {
			if ((int)Subtotal == value)
				return;
			ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, packedValues, CreateNewPValueEnum(value), SetPackageNewValueCore);
			documentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		bool IsEnumValue(PivotFieldItemType value) {
			return (Subtotal & value) > 0;
		}
		void SetEnumValue(PivotFieldItemType obj, bool value) {
			if (IsEnumValue(obj) == value)
				return;
			if (value)
				Subtotal |= obj;
			else
				Subtotal &= ~obj;
		}
		int CreateNewPValueBool(uint offset, bool value) {
			uint nPack = (uint)this.packedValues;
			PackedValues.SetBoolBitValue(ref nPack, offset, value);
			return (int)nPack;
		}
		int CreateNewPValueEnum(int value) {
			uint nPack = (uint)this.packedValues;
			PackedValues.SetIntBitValue(ref nPack, maskSubtotal, (int)value);
			return (int)nPack;
		}
		void SetPackageNewValueCore(int value) {
			packedValues = value;
		}
		protected internal void SetFieldCore(long? value) {
			field = value;
		}
		protected internal void SetSelectedCore(bool value) {
			packedValues = CreateNewPValueBool(offsetSelected, value);
		}
		protected internal void SetByPositionCore(bool value) {
			packedValues = CreateNewPValueBool(offsetByPosition, value);
		}
		protected internal void SetRelativeCore(bool value) {
			packedValues = CreateNewPValueBool(offsetRelative, value);
		}
		protected internal void SetSubtotalCore(int value) {
			packedValues = CreateNewPValueEnum(value);
		}
		public PivotAreaReference Clone(DocumentModel anotherModel) {
			PivotAreaReference result = new PivotAreaReference(anotherModel);
			result.CopyFromNoHistory(this);
			return result;
		}
		public void CopyFromNoHistory(PivotAreaReference source) {
			packedValues = source.packedValues;
			field = source.field;
			sharedItemsIndex.AddRange(source.sharedItemsIndex);
		}
	}
	#endregion
	#region Enum Section
	[Flags]
	public enum PivotTableAxis {
		None = 0x00,
		Value = 0x08,
		Row = 0x01,
		Column = 0x02,
		Page = 0x04
	}
	public enum PivotTableSortTypeField {
		Ascending,
		Descending,
		Manual
	}
	public enum PivotFieldItemType {
		Blank = 0x0,
		Data = 0x1,
		DefaultValue = 0x2,
		Sum = 0x4,
		CountA = 0x8,
		Avg = 0x10,
		Max = 0x20,
		Min = 0x40,
		Product = 0x80,
		Count = 0x100,
		StdDev = 0x200,
		StdDevP = 0x400,
		Var = 0x800,
		VarP = 0x1000,
		Grand = 0x2000,
		SubtotalMask = DefaultValue | Sum | CountA | Avg | Max | Min | Product | Count | StdDev | StdDevP | Var | VarP
	}
	public enum PivotAreaType {
		None = 0,
		Normal = 1,
		Data = 2,
		All = 3,
		Origin = 4,
		Button = 5,
		TopRight = 6
	}
	#endregion
	public class PivotFieldItemComparer : IComparer<PivotItem> {
		readonly int ascendingModifier;
		readonly PivotCacheSharedItemsCollection sharedItems;
		public PivotFieldItemComparer(bool ascending, PivotCacheSharedItemsCollection sharedItems) {
			this.ascendingModifier = ascending ? 1 : -1;
			this.sharedItems = sharedItems;
		}
		public int Compare(PivotItem x, PivotItem y) {
			return Compare(sharedItems[x.ItemIndex], sharedItems[y.ItemIndex]);
		}
		int Compare(IPivotCacheRecordValue pivotCacheRecordValue1, IPivotCacheRecordValue pivotCacheRecordValue2) {
			return pivotCacheRecordValue1.CompareTo(pivotCacheRecordValue2) * ascendingModifier;
		}
	}
}
