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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region PivotTableLabelFiltersViewModel
	public class PivotTableShowValuesAsViewModel : ViewModelBase {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly Dictionary<int, string> baseFieldNames = new Dictionary<int,string>();
		readonly Dictionary<int, string> baseItemNames = new Dictionary<int,string>();
		string currentBaseFieldName = String.Empty;
		string currentBaseItemName = String.Empty;
		string dataFieldName;
		string calculationType;
		#endregion
		public PivotTableShowValuesAsViewModel(ISpreadsheetControl control) {
			this.control = control;
			PopulateBaseFieldNames();
		}
		#region Properties
		public IEnumerable<string> BaseFieldNames { get { return baseFieldNames.Values; } }
		public IEnumerable<string> BaseItemNames { get { return baseItemNames.Values; } }
		public string DataFieldName { get { return dataFieldName; } set { dataFieldName = value; } }
		public string CalculationType { get { return calculationType; } set { calculationType = value; } }
		public bool BaseItemEnabled { get; set; }
		public PivotTableShowValuesAsWithBaseFieldCommandBase Command { get; set; }
		#region BaseFieldIndex
		public int BaseFieldIndex {
			get { return GetBaseFieldIndex(); }
			set {
				if (BaseFieldIndex == value)
					return;
				currentBaseFieldName = baseFieldNames[value];
			}
		}
		public string CurrentBaseFieldName {
			get { return currentBaseFieldName; }
			set {
				if (CurrentBaseFieldName == value)
					return;
				currentBaseFieldName = value;
				if (BaseItemEnabled) {
					baseItemNames.Clear();
					PopulateBaseItemNames(BaseFieldIndex);
				}
				OnPropertyChanged("CurrentBaseFieldName");
				OnPropertyChanged("CurrentBaseItemName");
			}
		}
		#endregion
		#region BaseItemIndex
		public int BaseItemIndex {
			get { return GetBaseItemIndex(); }
			set {
				if (BaseItemIndex == value)
					return;
				currentBaseItemName = baseItemNames[value];
			}
		}
		public string CurrentBaseItemName {
			get { return currentBaseItemName; }
			set {
				if (CurrentBaseItemName == value)
					return;
				currentBaseItemName = value;
				OnPropertyChanged("CurrentBaseItemName");
			}
		}
		#endregion
		#endregion
		int GetBaseFieldIndex() {
			foreach (int key in baseFieldNames.Keys) {
				if (baseFieldNames[key] == currentBaseFieldName)
					return key;
			}
			return -1;
		}
		int GetBaseItemIndex() {
			foreach (int key in baseItemNames.Keys) {
				if (baseItemNames[key] == currentBaseItemName)
					return key;
			}
			return -1;
		}
		#region Initialize
		void PopulateBaseFieldNames() {
			PivotTable pivotTable = GetPivot();
			for (int i = 0; i < pivotTable.Fields.Count; i++) {
				PivotTableAxis axis = pivotTable.Fields[i].Axis;
				if (axis == PivotTableAxis.Row || axis == PivotTableAxis.Column) {
					string fieldName = pivotTable.GetFieldCaption(i);
					baseFieldNames.Add(i, pivotTable.GetFieldCaption(i));
					if (String.IsNullOrEmpty(currentBaseFieldName))
						currentBaseFieldName = fieldName;
				}
			}
		}
		public void PopulateBaseItemNames() {
			PopulateBaseItemNames(BaseFieldIndex);
		}
		void PopulateBaseItemNames(int baseFieldIndex) {
			PivotTable pivotTable = GetPivot();
			baseItemNames.Add(PivotTableLayoutCalculator.PreviousItem, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemPrevious));
			baseItemNames.Add(PivotTableLayoutCalculator.NextItem, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemNext));
			PivotItemCollection items = pivotTable.Fields[baseFieldIndex].Items;
			for (int i = 0; i < items.DataItemsCount; i++)
				baseItemNames.Add(i, PivotTableFieldsFilterItemsCommandBase.GetItemName(pivotTable, baseFieldIndex, i));
			CurrentBaseItemName = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_BaseItemPrevious);
		}
		PivotTable GetPivot() {
			Worksheet activeSheet = control.InnerControl.DocumentModel.ActiveSheet;
			PivotTableStaticInfo info = activeSheet.PivotTableStaticInfo;
			return activeSheet.PivotTables[info.TableIndex];
		}
		#endregion
		public void ApplyChanges() {
			if (Command != null)
				Command.ApplyChanges(this);
		}
	}
	#endregion
}
