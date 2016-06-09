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

using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region FieldsReportFilterAreaMode
	public enum FieldsReportFilterArea {
		DownThenOver = 0,
		OverThenDown = 1
	}
	#endregion
	#region MissingItemsLimitMode
	public enum MissingItemsLimit {
		Automatic = 0,
		None = 1,
		Max = 2
	}
	#endregion
	#region OptionsPivotTableViewModel
	public class OptionsPivotTableViewModel : ViewModelBase {
		#region StaticMembers
		static Dictionary<FieldsReportFilterArea, string> PopulateFieldsReportFilterAreaDictionary() {
			Dictionary<FieldsReportFilterArea, string> result = new Dictionary<FieldsReportFilterArea, string>();
			result.Add(FieldsReportFilterArea.DownThenOver, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.OptionsPivotTableForm_PageDownThenOver));
			result.Add(FieldsReportFilterArea.OverThenDown, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.OptionsPivotTableForm_PageOverThenDown));
			return result;
		}
		static Dictionary<MissingItemsLimit, string> PopulateMissingItemsLimitDictionary() {
			Dictionary<MissingItemsLimit, string> result = new Dictionary<MissingItemsLimit, string>();
			result.Add(MissingItemsLimit.Automatic, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.OptionsPivotTableForm_MissingItemsLimitAutomatic));
			result.Add(MissingItemsLimit.None, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.OptionsPivotTableForm_MissingItemsLimitNone));
			result.Add(MissingItemsLimit.Max, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.OptionsPivotTableForm_MissingItemsLimitMax));
			return result;
		}
		#endregion
		#region Fields
		Dictionary<FieldsReportFilterArea, string> fieldsReportFilterAreaDictionary = PopulateFieldsReportFilterAreaDictionary();
		Dictionary<MissingItemsLimit, string> missingItemsLimitDictionary = PopulateMissingItemsLimitDictionary();
		ISpreadsheetControl control;
		PivotTable pivotTable;
		string name;
		bool mergeItem;
		int indent;
		FieldsReportFilterArea fieldsReportFilterAreaMode;
		bool pageOverThenDown;
		int pageWrap;
		bool showError;
		string errorCaption;
		bool showMissing;
		string missingCaption;
		bool useAutoFormatting;
		bool preserveFormatting;
		bool rowGrandTotals;
		bool columnGrandTotals;
		bool subtotalHiddenItems;
		bool multipleFieldFilters;
		bool customListSort;
		bool showDrill;
		bool showDataTips;
		bool showMemberPropertyTips;
		bool showHeaders;
		bool gridDropZones;
		bool outline;
		bool outlineData;
		bool compact;
		bool compactData;
		bool showValuesRow;
		bool showEmptyRow;
		bool showEmptyColumn;
		bool showItems;
		bool fieldListSortAscending;
		bool printDrill;
		bool itemPrintTitles;
		bool fieldPrintTitles;
		bool saveData;
		bool enableDrill;
		bool refreshOnLoad;
		MissingItemsLimit missingItemsLimit;
		int missingItemsLimitValue;
		bool editData;
		string altText;
		string altTextSummary;
		#endregion
		public OptionsPivotTableViewModel(ISpreadsheetControl control) {
			this.control = control;
		}
		#region Properties
		public PivotTable PivotTable {
			get { return pivotTable; }
			set {
				if (PivotTable == value)
					return;
				pivotTable = value;
			}
		}
		public string Name {
			get { return name; }
			set {
				if (Name == value)
					return;
				name = value;
				OnPropertyChanged("Name");
			}
		}
		#region Layout & Format
		public bool MergeItem {
			get { return mergeItem; }
			set {
				if (MergeItem == value)
					return;
				mergeItem = value;
				OnPropertyChanged("MergeItem");
			}
		}
		public int Indent {
			get { return indent; }
			set {
				if (Indent == value)
					return;
				indent = value;
				OnPropertyChanged("Indent");
			}
		}
		#region FieldsReportFilterAreaMode
		public List<string> FieldsReportFilterList {
			get { return new List<string>(fieldsReportFilterAreaDictionary.Values); }
		}
		public string FieldsReportFilterAreaMode {
			get { return GetFilterAreaModeStringByEnum(fieldsReportFilterAreaMode); }
			set {
				if (FieldsReportFilterAreaMode == value)
					return;
				fieldsReportFilterAreaMode = GetFilterAreaByString(value);
				PageOverThenDown = (fieldsReportFilterAreaMode == FieldsReportFilterArea.DownThenOver) ? false : true;
				OnPropertyChanged("FieldsReportFilterAreaMode");
			}
		}
		public string GetFilterAreaModeStringByEnum(FieldsReportFilterArea fieldsReportFilterAreaMode) {
			return fieldsReportFilterAreaDictionary[fieldsReportFilterAreaMode];
		}
		public FieldsReportFilterArea GetFilterAreaByString(string commentsMode) {
			foreach (FieldsReportFilterArea key in fieldsReportFilterAreaDictionary.Keys)
				if (fieldsReportFilterAreaDictionary[key] == commentsMode)
					return key;
			Exceptions.ThrowInternalException();
			return FieldsReportFilterArea.DownThenOver;
		}
		#endregion
		public bool PageOverThenDown {
			get { return pageOverThenDown; }
			set {
				if (PageOverThenDown == value)
					return;
				pageOverThenDown = value;
				OnPropertyChanged("PageOverThenDown");
				OnPropertyChanged("ColumnTextVisible");
				OnPropertyChanged("RowTextVisible");
			}
		}
		public bool RowTextVisible { get { return pageOverThenDown; } }
		public bool ColumnTextVisible { get { return !pageOverThenDown; } }
		public int PageWrap {
			get { return pageWrap; }
			set {
				if (PageWrap == value)
					return;
				pageWrap = value;
				OnPropertyChanged("PageWrap");
			}
		}		
		public bool ShowError {
			get { return showError; }
			set {
				if (ShowError == value)
					return;
				showError = value;
				OnPropertyChanged("ShowError");
				OnPropertyChanged("ErrorCaptionEnabled");
			}
		}
		public bool ErrorCaptionEnabled { get { return ShowError; } }
		public string ErrorCaption {
			get { return errorCaption; }
			set {
				if (ErrorCaption == value)
					return;
				errorCaption = value;
				OnPropertyChanged("ErrorCaption");
			}
		}
		public bool ShowMissing {
			get { return showMissing; }
			set {
				if (ShowMissing == value)
					return;
				showMissing = value;
				OnPropertyChanged("ShowMissing");
				OnPropertyChanged("MissingCaptionEnabled");
			}
		}
		public bool MissingCaptionEnabled { get { return ShowMissing; } }
		public string MissingCaption {
			get { return missingCaption; }
			set {
				if (MissingCaption == value)
					return;
				missingCaption = value;
				OnPropertyChanged("MissingCaption");
			}
		}
		public bool UseAutoFormatting {
			get { return useAutoFormatting; }
			set {
				if (UseAutoFormatting == value)
					return;
				useAutoFormatting = value;
				OnPropertyChanged("UseAutoFormatting");
			}
		}
		public bool PreserveFormatting {
			get { return preserveFormatting; }
			set {
				if (PreserveFormatting == value)
					return;
				preserveFormatting = value;
				OnPropertyChanged("PreserveFormatting");
			}
		}
		#endregion
		#region Totals & Filters
		public bool RowGrandTotals {
			get { return rowGrandTotals; }
			set {
				if (RowGrandTotals == value)
					return;
				rowGrandTotals = value;
				OnPropertyChanged("RowGrandTotals");
			}
		}
		public bool ColumnGrandTotals {
			get { return columnGrandTotals; }
			set {
				if (ColumnGrandTotals == value)
					return;
				columnGrandTotals = value;
				OnPropertyChanged("ColumnGrandTotals");
			}
		}
		public bool SubtotalHiddenItemsEnabled { get { return false; } }
		public bool SubtotalHiddenItems {
			get { return subtotalHiddenItems; }
			set {
				if (SubtotalHiddenItems == value)
					return;
				subtotalHiddenItems = value;
				OnPropertyChanged("SubtotalHiddenItems");
			}
		}
		public bool MultipleFieldFilters {
			get { return multipleFieldFilters; }
			set {
				if (MultipleFieldFilters == value)
					return;
				multipleFieldFilters = value;
				OnPropertyChanged("MultipleFieldFilters");
			}
		}
		public bool CustomListSort {
			get { return customListSort; }
			set {
				if (CustomListSort == value)
					return;
				customListSort = value;
				OnPropertyChanged("CustomListSort");
			}
		}
		#endregion
		#region Display
		public bool ShowDrill {
			get { return showDrill; }
			set {
				if (ShowDrill == value)
					return;
				showDrill = value;
				OnPropertyChanged("ShowDrill");
			}
		}
		public bool ShowDataTips {
			get { return showDataTips; }
			set {
				if (ShowDataTips == value)
					return;
				showDataTips = value;
				OnPropertyChanged("ShowDataTips");
			}
		}
		public bool ShowMemberPropertyTipsEnabled { get { return false; } }
		public bool ShowMemberPropertyTips {
			get { return showMemberPropertyTips; }
			set {
				if (ShowMemberPropertyTips == value)
					return;
				showMemberPropertyTips = value;
				OnPropertyChanged("ShowMemberPropertyTips");
			}
		}
		public bool ShowHeaders {
			get { return showHeaders; }
			set {
				if (ShowHeaders == value)
					return;
				showHeaders = value;
				OnPropertyChanged("ShowHeaders");
			}
		}
		public bool GridDropZones {
			get { return gridDropZones; }
			set {
				if (GridDropZones == value)
					return;
				gridDropZones = value;
				Outline = !value;
				OutlineData = !value;
				Compact = !value;
				CompactData = !value;
				OnPropertyChanged("Outline");
				OnPropertyChanged("OutlineData");
				OnPropertyChanged("GridDropZones");
				OnPropertyChanged("ShowValuesRowEnabled");
			}
		}
		public bool Outline {
			get { return outline; }
			set {
				if (Outline == value)
					return;
				outline = value;
				OnPropertyChanged("Outline");
			}
		}
		public bool OutlineData {
			get { return outlineData; }
			set {
				if (OutlineData == value)
					return;
				outlineData = value;
				OnPropertyChanged("OutlineData");
			}
		}
		public bool Compact {
			get { return compact; }
			set {
				if (Compact == value)
					return;
				compact = value;
				OnPropertyChanged("Compact");
			}
		}
		public bool CompactData {
			get { return compactData; }
			set {
				if (CompactData == value)
					return;
				compactData = value;
				OnPropertyChanged("CompactData");
			}
		}
		public bool ShowValuesRowEnabled { get { return !GridDropZones; } }
		public bool ShowValuesRow {
			get { return showValuesRow; }
			set {
				if (ShowValuesRow == value)
					return;
				showValuesRow = value;
				OnPropertyChanged("ShowValuesRow");
			}
		}
		public bool ShowEmptyRowEnabled { get { return false; } }
		public bool ShowEmptyRow {
			get { return showEmptyRow; }
			set {
				if (ShowEmptyRow == value)
					return;
				showEmptyRow = value;
				OnPropertyChanged("ShowEmptyRow");
			}
		}
		public bool ShowEmptyColumnEnabled { get { return false; } }
		public bool ShowEmptyColumn {
			get { return showEmptyColumn; }
			set {
				if (ShowEmptyColumn == value)
					return;
				showEmptyColumn = value;
				OnPropertyChanged("ShowEmptyColumn");
			}
		}
		public bool ShowItemsEnabled { get { return false; } }
		public bool ShowItems {
			get { return showItems; }
			set {
				if (ShowItems == value)
					return;
				showItems = value;
				OnPropertyChanged("ShowItems");
			}
		}
		public bool FieldListSortAscending {
			get { return fieldListSortAscending; }
			set {
				if (FieldListSortAscending == value)
					return;
				fieldListSortAscending = value;
				OnPropertyChanged("FieldListSortAscending");
			}
		}
		#endregion
		#region Printing
		public bool PrintDrill {
			get { return printDrill; }
			set {
				if (PrintDrill == value)
					return;
				printDrill = value;
				OnPropertyChanged("PrintDrill");
			}
		}
		public bool ItemPrintTitles {
			get { return itemPrintTitles; }
			set {
				if (ItemPrintTitles == value)
					return;
				itemPrintTitles = value;
				OnPropertyChanged("ItemPrintTitles");
			}
		}
		public bool FieldPrintTitles {
			get { return fieldPrintTitles; }
			set {
				if (FieldPrintTitles == value)
					return;
				fieldPrintTitles = value;
				OnPropertyChanged("FieldPrintTitles");
			}
		}
		#endregion
		#region Data
		public bool SaveData {
			get { return saveData; }
			set {
				if (SaveData == value)
					return;
				saveData = value;
				OnPropertyChanged("SaveData");
			}
		}
		public bool EnableDrill {
			get { return enableDrill; }
			set {
				if (EnableDrill == value)
					return;
				enableDrill = value;
				OnPropertyChanged("EnableDrill");
			}
		}
		public bool RefreshOnLoad {
			get { return refreshOnLoad; }
			set {
				if (RefreshOnLoad == value)
					return;
				refreshOnLoad = value;
				OnPropertyChanged("RefreshOnLoad");
			}
		}
		#region MissingItemsLimitMode
		public List<string> MissingItemsLimitList {
			get { return new List<string>(missingItemsLimitDictionary.Values); }
		}
		public string MissingItemsLimitMode {
			get { return GetMissingItemsLimitModeStringByEnum(missingItemsLimit); }
			set {
				if (MissingItemsLimitMode == value)
					return;
				missingItemsLimit = GetMissingItemsLimitByString(value);
				if (missingItemsLimit == MissingItemsLimit.Automatic)
					MissingItemsLimitValue = PivotCache.DefaultMissingItemsLimit;
				else if (missingItemsLimit == MissingItemsLimit.None)
					MissingItemsLimitValue = PivotCache.NoneMissingItemsLimit;
				else
					MissingItemsLimitValue = PivotCache.MaxMissingItemsLimit;
				OnPropertyChanged("MissingItemsLimitMode");
			}
		}
		public string GetMissingItemsLimitModeStringByEnum(MissingItemsLimit missingItemsLimitMode) {
			return missingItemsLimitDictionary[missingItemsLimitMode];
		}
		public MissingItemsLimit GetMissingItemsLimitByString(string commentsMode) {
			foreach (MissingItemsLimit key in missingItemsLimitDictionary.Keys)
				if (missingItemsLimitDictionary[key] == commentsMode)
					return key;
			Exceptions.ThrowInternalException();
			return MissingItemsLimit.Automatic;
		}
		#endregion
		public int MissingItemsLimitValue {
			get { return missingItemsLimitValue; }
			set {
				if (MissingItemsLimitValue == value)
					return;
				missingItemsLimitValue = value;
				OnPropertyChanged("missingItemsLimitValue");
			}
		}
		public bool EditDataEnabled { get { return false; } }
		public bool EditData {
			get { return editData; }
			set {
				if (EditData == value)
					return;
				editData = value;
				OnPropertyChanged("EditData");
			}
		}
		#endregion
		#region AltText
		public string AltText {
			get { return altText; }
			set {
				if (AltText == value)
					return;
				altText = value;
				OnPropertyChanged("AltText");
			}
		}
		public string AltTextSummary {
			get { return altTextSummary; }
			set {
				if (AltTextSummary == value)
					return;
				altTextSummary = value;
				OnPropertyChanged("AltTextSummary");
			}
		}
		#endregion
		#endregion
		public bool Validate() {
			return CreateCommand().Validate(this);
		}
		public void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		OptionsPivotTableCommand CreateCommand() {
			return new OptionsPivotTableCommand(control);
		}
	}
	#endregion
}
