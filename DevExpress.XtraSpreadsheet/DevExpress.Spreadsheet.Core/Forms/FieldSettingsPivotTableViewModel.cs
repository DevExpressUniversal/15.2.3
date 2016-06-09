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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class FieldSettingsPivotTableViewModel : ViewModelBase {
		#region StaticMembers
		private static Dictionary<PivotFieldItemType, string> PopulateFunctionTable() {
			Dictionary<PivotFieldItemType, string> result = new Dictionary<PivotFieldItemType, string>();
			result.Add(PivotFieldItemType.Sum, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionSum));
			result.Add(PivotFieldItemType.CountA, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCount));
			result.Add(PivotFieldItemType.Avg, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionAverage));
			result.Add(PivotFieldItemType.Max, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMax));
			result.Add(PivotFieldItemType.Min, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMin));
			result.Add(PivotFieldItemType.Product, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionProduct));
			result.Add(PivotFieldItemType.Count, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCountNumbers));
			result.Add(PivotFieldItemType.StdDev, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDev));
			result.Add(PivotFieldItemType.StdDevP, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDevp));
			result.Add(PivotFieldItemType.Var, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVar));
			result.Add(PivotFieldItemType.VarP, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVarp));
			return result;
		}
		#endregion
		#region Fields
		ISpreadsheetControl control;
		PivotTable pivotTable;
		PivotField pivotField;
		string sourceName;
		string customName;
		PivotFieldItemType subtotal;
		bool isAutomaticSubtotal;
		bool isNoneSubtotal;
		bool isCustomSubtotal;
		Dictionary<PivotFieldItemType, string> functionTable = PopulateFunctionTable();
		bool includeNewItemsInFilter;
		bool outline;
		bool compact;
		bool subtotalTop;
		bool tabular;
		bool fillDownLabels;
		bool insertBlankRow;
		bool showItemsWithNoData;
		bool insertPageBreak;
		#endregion
		public FieldSettingsPivotTableViewModel(ISpreadsheetControl control) {
			this.control = control;
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public PivotTable PivotTable {
			get { return pivotTable; }
			set {
				if (PivotTable == value)
					return;
				pivotTable = value;
			}
		}
		public PivotField PivotField {
			get { return pivotField; }
			set {
				if (PivotField == value)
					return;
				pivotField = value;
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
		public bool IsAutomaticSubtotal {
			get { return isAutomaticSubtotal; }
			set {
				if (IsAutomaticSubtotal == value)
					return;
				isAutomaticSubtotal = value;
				if (value) {
					IsNoneSubtotal = !value;
					IsCustomSubtotal = !value;
				}
				OnPropertyChanged("IsAutomaticSubtotal");
				OnPropertyChanged("IsNoneSubtotal");
				OnPropertyChanged("IsCustomSubtotal");
			}
		}
		public bool IsNoneSubtotal {
			get { return isNoneSubtotal; }
			set {
				if (IsNoneSubtotal == value)
					return;
				isNoneSubtotal = value;
				if (value) {
					IsAutomaticSubtotal = !value;
					IsCustomSubtotal = !value;
				}
				OnPropertyChanged("IsNoneSubtotal");
				OnPropertyChanged("IsAutomaticSubtotal");
				OnPropertyChanged("IsCustomSubtotal");
			}
		}
		public bool IsCustomSubtotal {
			get { return isCustomSubtotal; }
			set {
				if (IsCustomSubtotal == value)
					return;
				isCustomSubtotal = value;
				if (value) {
					IsAutomaticSubtotal = !value;
					IsNoneSubtotal = !value;
				}
				OnPropertyChanged("IsCustomSubtotal");
				OnPropertyChanged("IsAutomaticSubtotal");
				OnPropertyChanged("IsNoneSubtotal");
				OnPropertyChanged("IsSubtotalFunctionListEnabled");
			}
		}
		public bool IsSubtotalFunctionListEnabled { get { return IsCustomSubtotal; } }
		public PivotFieldItemType Subtotal {
			get { return subtotal; }
			set {
				if (Subtotal == value)
					return;
				subtotal = value;
				OnPropertyChanged("Subtotal");
			}
		}
		#region FunctionsList
		public Dictionary<PivotFieldItemType, string> FunctionTable {
			get { return functionTable; }
		}
		public List<string> FunctionList {
			get { return new List<string>(functionTable.Values); }
		}
		PivotFieldItemType GetSubtotalFunctionByString(string functionName) {
			foreach (PivotFieldItemType key in functionTable.Keys)
				if (functionTable[key] == functionName)
					return key;
			Exceptions.ThrowInternalException();
			return PivotFieldItemType.Blank;
		}
		#endregion
		public bool IncludeNewItemsInFilter {
			get { return includeNewItemsInFilter; }
			set {
				if (IncludeNewItemsInFilter == value)
					return;
				includeNewItemsInFilter = value;
				OnPropertyChanged("IncludeNewItemsInFilter");
			}
		}
		public bool Outline {
			get { return outline; }
			set {
				if (Outline == value)
					return;
				outline = value;
				Tabular = !value;
				OnPropertyChanged("Outline");
				OnPropertyChanged("Tabular");
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
		public bool SubtotalTop {
			get { return subtotalTop; }
			set {
				if (SubtotalTop == value)
					return;
				subtotalTop = value;
				OnPropertyChanged("SubtotalTop");
			}
		}
		public bool Tabular {
			get { return tabular; }
			set {
				if (Tabular == value)
					return;
				tabular = value;
				Outline = !value;
				OnPropertyChanged("Tabular");
				OnPropertyChanged("Outline");
			}
		}
		public bool FillDownLabels {
			get { return fillDownLabels; }
			set {
				if (FillDownLabels == value)
					return;
				fillDownLabels = value;
				OnPropertyChanged("FillDownLabels");
			}
		}
		public bool InsertBlankRow {
			get { return insertBlankRow; }
			set {
				if (InsertBlankRow == value)
					return;
				insertBlankRow = value;
				OnPropertyChanged("InsertBlankRow");
			}
		}
		public bool ShowItemsWithNoData {
			get { return showItemsWithNoData; }
			set {
				if (ShowItemsWithNoData == value)
					return;
				showItemsWithNoData = value;
				OnPropertyChanged("ShowItemsWithNoData");
			}
		}
		public bool InsertPageBreak {
			get { return insertPageBreak; }
			set {
				if (InsertPageBreak == value)
					return;
				insertPageBreak = value;
				OnPropertyChanged("InsertPageBreak");
			}
		}
		#endregion
		public void SetSubtotal(List<string> selectedItems) {
			Subtotal = 0x0;
			if (IsAutomaticSubtotal)
				Subtotal = PivotFieldItemType.DefaultValue;
			else if (IsCustomSubtotal)
				foreach (string function in selectedItems)
					Subtotal |= GetSubtotalFunctionByString(function);
		}
		public bool Validate() {
			return CreateCommand().Validate(this);
		}
		public void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		FieldSettingsPivotTableCommand CreateCommand() {
			return new FieldSettingsPivotTableCommand(Control);
		}
	}
}
