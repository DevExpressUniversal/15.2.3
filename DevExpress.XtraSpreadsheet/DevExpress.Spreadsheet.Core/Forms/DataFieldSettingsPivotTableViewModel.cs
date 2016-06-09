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

using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region DataFieldSettingsInitialTabPage
	public enum DataFieldSettingsInitialTabPage {
		SummarizeValuesBy = 0,
		ShowValuesAs = 1
	}
	#endregion
	#region DataFieldSettingsPivotTableViewModel
	public class DataFieldSettingsPivotTableViewModel : ViewModelBase {
		#region StaticMembers
		static Dictionary<PivotDataConsolidateFunction, string> PopulateSubtotalFunctionTable() {
			Dictionary<PivotDataConsolidateFunction, string> result = new Dictionary<PivotDataConsolidateFunction, string>();
			result.Add(PivotDataConsolidateFunction.Sum, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionSum));
			result.Add(PivotDataConsolidateFunction.Count, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCount));
			result.Add(PivotDataConsolidateFunction.Average, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionAverage));
			result.Add(PivotDataConsolidateFunction.Max, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMax));
			result.Add(PivotDataConsolidateFunction.Min, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMin));
			result.Add(PivotDataConsolidateFunction.Product, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionProduct));
			result.Add(PivotDataConsolidateFunction.CountNums, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCountNumbers));
			result.Add(PivotDataConsolidateFunction.StdDev, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDev));
			result.Add(PivotDataConsolidateFunction.StdDevp, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDevp));
			result.Add(PivotDataConsolidateFunction.Var, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVar));
			result.Add(PivotDataConsolidateFunction.Varp, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVarp));
			return result;
		}
		static Dictionary<PivotShowDataAs, string> PopulateShowDataAsItemTable() {
			Dictionary<PivotShowDataAs, string> result = new Dictionary<PivotShowDataAs, string>();
			result.Add(PivotShowDataAs.Normal, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueAsNoCalculation));
			result.Add(PivotShowDataAs.PercentOfTotal, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfTotal));
			result.Add(PivotShowDataAs.PercentOfColumn, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfColumn));
			result.Add(PivotShowDataAs.PercentOfRow, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRow));
			result.Add(PivotShowDataAs.Percent, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercent));
			result.Add(PivotShowDataAs.PercentOfParentRow, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentRow));
			result.Add(PivotShowDataAs.PercentOfParentColumn, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentColumn));
			result.Add(PivotShowDataAs.PercentOfParent, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParent));
			result.Add(PivotShowDataAs.Difference, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueDifference));
			result.Add(PivotShowDataAs.PercentDifference, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentDifference));
			result.Add(PivotShowDataAs.RunningTotal, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRunningTotal));
			result.Add(PivotShowDataAs.PercentOfRunningTotal, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRunningTotal));
			result.Add(PivotShowDataAs.RankAscending, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankAscending));
			result.Add(PivotShowDataAs.RankDescending, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankDescending));
			result.Add(PivotShowDataAs.Index, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueIndex));
			return result;
		}
		#endregion
		#region Fields
		Dictionary<PivotDataConsolidateFunction, string> subtotalFunctionTable = PopulateSubtotalFunctionTable();
		Dictionary<PivotShowDataAs, string> showDataAsTable = PopulateShowDataAsItemTable();
		ISpreadsheetControl control;
		DataFieldSettingsInitialTabPage initialTabPage;
		PivotTable pivotTable;
		PivotDataField pivotDataField;
		string sourceName;
		string customName;
		PivotDataConsolidateFunction subtotal = PivotDataConsolidateFunction.Sum;
		PivotShowDataAs showDataAs;
		const int numFmtIdNormal = 0;
		const int numFmtIdPercent = 10;
		int numberFormatIndex;
		Dictionary<int, string> baseFieldTable = new Dictionary<int, string>();
		string baseFieldName;
		int baseField;
		Dictionary<int, string> baseItemTable = new Dictionary<int, string>();
		string baseItemName;
		int baseItem;
		IErrorHandler errorHandler;
		bool isPivotItemCollectionEmpty;
		#endregion
		public DataFieldSettingsPivotTableViewModel(ISpreadsheetControl control, DataFieldSettingsInitialTabPage initialTabPage) {
			this.control = control;
			this.initialTabPage = initialTabPage;
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public DataFieldSettingsInitialTabPage InitialTabPage { get { return initialTabPage; } }
		public PivotTable PivotTable {
			get { return pivotTable; }
			set {
				if (PivotTable == value)
					return;
				pivotTable = value;
			}
		}
		public PivotDataField PivotDataField {
			get { return pivotDataField; }
			set {
				if (PivotDataField == value)
					return;
				pivotDataField = value;
			}
		}
		public string SourceName {
			get { return sourceName; }
			set {
				if (SourceName == value)
					return;
				sourceName = value;
				OnPropertyChanged("SourceName");
			}
		}
		public string CustomName {
			get { return customName; }
			set {
				if (CustomName == value)
					return;
				customName = value;
				OnPropertyChanged("CustomName");
			}
		}
		#region Subtotal
		public List<string> SubtotalFunctions { get { return new List<string>(subtotalFunctionTable.Values); } }
		public string Subtotal {
			get { return GetSubtotalStringByEnum(subtotal); }
			set {
				if (Subtotal == value)
					return;
				subtotal = GetSubtotalByString(value);
				CustomName = value + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_PartOfCustomName) + sourceName;
				OnPropertyChanged("Subtotal");
				OnPropertyChanged("CustomName");
			}
		}
		public string GetSubtotalStringByEnum(PivotDataConsolidateFunction subtotal) {
			return subtotalFunctionTable[subtotal];
		}
		public PivotDataConsolidateFunction GetSubtotalByString(string subtotal) {
			foreach (PivotDataConsolidateFunction key in subtotalFunctionTable.Keys)
				if (subtotalFunctionTable[key] == subtotal)
					return key;
			Exceptions.ThrowInternalException();
			return PivotDataConsolidateFunction.Sum;
		}
		#endregion
		#region ShowDataAs
		public List<string> ShowDataAsList { get { return new List<string>(showDataAsTable.Values); } }
		public string ShowDataAs {
			get { return GetShowDataAsStringByEnum(showDataAs); }
			set {
				if (ShowDataAs == value)
					return;
				showDataAs = GetShowDataAsByString(value);
				if (showDataAs == PivotShowDataAs.Normal || showDataAs == PivotShowDataAs.Difference ||
					showDataAs == PivotShowDataAs.RunningTotal || showDataAs == PivotShowDataAs.RankAscending ||
					showDataAs == PivotShowDataAs.RankDescending || showDataAs == PivotShowDataAs.Index)
					NumberFormatIndex = numFmtIdNormal;
				else
					NumberFormatIndex = numFmtIdPercent;
				OnPropertyChanged("ShowDataAs");
				OnPropertyChanged("BaseFieldEnabled");
				OnPropertyChanged("BaseItemEnabled");
			}
		}
		public string GetShowDataAsStringByEnum(PivotShowDataAs showDataAs) {
			return showDataAsTable[showDataAs];
		}
		public PivotShowDataAs GetShowDataAsByString(string showDataAs) {
			foreach (PivotShowDataAs key in showDataAsTable.Keys)
				if (showDataAsTable[key] == showDataAs)
					return key;
			Exceptions.ThrowInternalException();
			return PivotShowDataAs.Normal;
		}
		#endregion
		public int NumberFormatIndex {
			get { return numberFormatIndex; }
			set {
				if (NumberFormatIndex == value)
					return;
				numberFormatIndex = value;
				OnPropertyChanged("NumberFormatIndex");
			}
		}
		#region BaseField
		public bool BaseFieldEnabled {
			get {
				return showDataAs != PivotShowDataAs.Normal && showDataAs != PivotShowDataAs.PercentOfTotal &&
					   showDataAs != PivotShowDataAs.PercentOfColumn && showDataAs != PivotShowDataAs.PercentOfRow &&
					   showDataAs != PivotShowDataAs.PercentOfParentRow && showDataAs != PivotShowDataAs.PercentOfParentColumn &&
					   showDataAs != PivotShowDataAs.Index;
			}
		}
		public Dictionary<int, string> BaseFieldTable {
			get { return baseFieldTable; }
			set {
				if (BaseFieldTable == value)
					return;
				baseFieldTable = value;
				OnPropertyChanged("BaseFieldTable");
			}
		}
		public string BaseFieldName {
			get { return baseFieldName; }
			set {
				if (BaseFieldName == value)
					return;
				baseFieldName = value;
				BaseField = GetBaseFieldByString(baseFieldName);
				OnPropertyChanged("BaseFieldName");
			}
		}
		public int BaseField {
			get { return baseField; }
			set {
				if (BaseField == value)
					return;
				baseField = value;
				OnPropertyChanged("BaseField");
			}
		}
		public string GetBaseFieldNameStringByInt(int field) {
			return baseFieldTable[field];
		}
		public int GetBaseFieldByString(string field) {
			foreach (int key in baseFieldTable.Keys)
				if (baseFieldTable[key] == field)
					return key;
			Exceptions.ThrowInternalException();
			return -1;
		}
		public Dictionary<int, string> PopulateBaseFieldTable() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			for (int i = 0; i < pivotTable.Fields.Count; i++)
				if (!String.IsNullOrEmpty(pivotTable.Fields[i].Name))
					result.Add(i, pivotTable.Fields[i].Name);
				else
					result.Add(i, pivotTable.GetFieldCaption(i));
			return result;
		}
		#endregion
		#region BaseItem
		public bool BaseItemEnabled {
			get {
				return showDataAs == PivotShowDataAs.Percent ||
					   showDataAs == PivotShowDataAs.Difference ||
					   showDataAs == PivotShowDataAs.PercentDifference;
			}
		}
		public Dictionary<int, string> BaseItemTable {
			get { return baseItemTable; }
			set {
				if (BaseItemTable == value)
					return;
				baseItemTable = value;
				OnPropertyChanged("BaseItemTable");
			}
		}
		public string BaseItemName {
			get { return baseItemName; }
			set {
				if (BaseItemName == value)
					return;
				baseItemName = value;
				BaseItem = GetBaseItemByString(baseItemName);
			}
		}
		public int BaseItem {
			get { return baseItem; }
			set {
				if (BaseItem == value)
					return;
				baseItem = value;
				OnPropertyChanged("BaseItem");
			}
		}
		public string GetBaseItemNameStringByInt(int item) {
			if (!isPivotItemCollectionEmpty)
				return baseItemTable.ContainsKey(item) ? baseItemTable[item] : baseItemTable[0];
			else
				return String.Empty;
		}
		public int GetBaseItemByString(string item) {
			if (BaseItemTable.Count == 0 || String.IsNullOrEmpty(item))
				return 0;
			foreach (int key in baseItemTable.Keys)
				if (baseItemTable[key] == item)
					return key;
			Exceptions.ThrowInternalException();
			return -1;
		}
		public Dictionary<int, string> PopulateBaseItemTable(int baseField, bool isShowWarning) {
			WorkbookDataContext dataContext = Control.Document.Model.DocumentModel.DataContext;
			Dictionary<int, string> result = new Dictionary<int, string>();
			isPivotItemCollectionEmpty = false;
			PivotItemCollection itemCollection = PivotTable.Fields[BaseField].Items;
			if (itemCollection.Count == 0) {
				if (isShowWarning)
					HandleError(new ModelErrorInfo(ModelErrorType.PivotTableSavedWithoutUnderlyingData));
				isPivotItemCollectionEmpty = true;
				return result;
			}
			result.Add(PivotTableLayoutCalculator.PreviousItem, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemPrevious));
			result.Add(PivotTableLayoutCalculator.NextItem, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemNext));
			for (int i = 0; i < itemCollection.DataItemsCount; i++)
				result.Add(i, PivotTableFieldsFilterItemsCommandBase.GetItemName(PivotTable, baseField, i));
			return result;
		}
		#endregion
		public IErrorHandler ErrorHandler {
			get { return errorHandler; }
			set {
				if (ErrorHandler == value)
					return;
				errorHandler = value;
			}
		}
		#endregion
		void HandleError(IModelErrorInfo error) {
			if (error != null)
				ErrorHandler.HandleError(error);
		}
		public bool Validate() {
			return CreateCommand().Validate(this);
		}
		public void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		DataFieldSettingsPivotTableCommand CreateCommand() {
			return new DataFieldSettingsPivotTableCommand(Control);
		}
	}
	#endregion
}
