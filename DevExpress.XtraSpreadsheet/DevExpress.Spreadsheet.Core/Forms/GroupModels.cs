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

using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region GroupViewModel
	public class GroupViewModel : ViewModelBase {
		#region fields
		ISpreadsheetControl control;
		bool rows;
		CellRange range;
		#endregion
		public GroupViewModel(ISpreadsheetControl control, CellRange range) {
			this.range = range;
			this.control = control;
			this.rows = true;
		}
		#region Properties
		public bool Rows { get { return rows; } set { rows = value; } }
		protected ISpreadsheetControl Control { get { return control; } }
		protected CellRange Range { get { return range; } }
		public virtual string FormCaption { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_GroupFormCaption); } }
		public virtual string LabelText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_GroupCommand); } }
		#endregion
		public virtual void ApplyChanges() {
			GroupCommand command = new GroupCommand(control);
			command.ApplyChanges(rows, range);
		}
	}
	#endregion
	#region UngroupViewModel
	public class UngroupViewModel : GroupViewModel {
		public UngroupViewModel(ISpreadsheetControl control, CellRange range)
			: base(control, range) {
		}
		#region Properties
		public override string FormCaption { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_UngroupFormCaption); } }
		public override string LabelText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_UngroupCommand); } }
		#endregion
		public override void ApplyChanges() {
			UngroupCommand command = new UngroupCommand(Control);
			command.ApplyChanges(Rows, Range);
		}
	}
	#endregion
	#region OutlineSettingsViewModel
	public class OutlineSettingsViewModel : ViewModelBase {
		#region fields
		ISpreadsheetControl control;
		bool applyStyles;
		bool showColumnSumsRight;
		bool showRowSumsBelow;
		#endregion
		public OutlineSettingsViewModel(ISpreadsheetControl control, Worksheet sheet) {
			this.control = control;
			this.applyStyles = sheet.Properties.GroupAndOutlineProperties.ApplyStyles;
			this.showColumnSumsRight = sheet.Properties.GroupAndOutlineProperties.ShowColumnSumsRight;
			this.showRowSumsBelow = sheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow;
		}
		#region Properties
		public bool ApplyStyles { get { return applyStyles; } set { applyStyles = value; } }
		public bool ShowColumnSumsRight { get { return showColumnSumsRight; } set { showColumnSumsRight = value; } }
		public bool ShowRowSumsBelow { get { return showRowSumsBelow; } set { showRowSumsBelow = value; } }
		#endregion
		public void ApplyChanges() {
			OutlineSettingsCommand command = new OutlineSettingsCommand(control);
			command.ApplyChanges(applyStyles, showColumnSumsRight, showRowSumsBelow);
		}
		public void CreateOutline() {
			AutoOutlineCommand command = new AutoOutlineCommand(control);
			command.Execute();
		}
	}
	#endregion
	#region OutlineSubtotalViewModel
	public class OutlineSubtotalViewModel : ViewModelBase {
		#region fields
		ISpreadsheetControl control;
		int functionIndex;
		string functionText;
		int changedColumnIndex;
		bool replaceCurrentSubtotals;
		bool pageBreakBeetwenGroups;
		bool showRowSumsBelow;
		List<string> columnNames;
		List<int> subTotalColumnIndices;
		CellRange range;
		#endregion
		public OutlineSubtotalViewModel(ISpreadsheetControl control, CellRange range, Worksheet sheet, List<string> columnNames) {
			this.control = control;
			this.functionIndex = 0;
			this.replaceCurrentSubtotals = true;
			this.pageBreakBeetwenGroups = false;
			this.range = range;
			this.showRowSumsBelow = sheet.Properties.GroupAndOutlineProperties.ShowRowSumsBelow;
			this.columnNames = columnNames;
			this.subTotalColumnIndices = new List<int>();
			this.changedColumnIndex = columnNames.Count - 1;
		}
		#region Properties
		public int FunctionIndex { get { return functionIndex; } set { functionIndex = value; } }
		public string FunctionText { get { return functionText; } set { functionText = value; } }
		public int ChangedColumnIndex { get { return changedColumnIndex; } set { changedColumnIndex = value; } }
		public bool ReplaceCurrentSubtotals { get { return replaceCurrentSubtotals; } set { replaceCurrentSubtotals = value; } }
		public bool PageBreakBeetwenGroups { get { return pageBreakBeetwenGroups; } set { pageBreakBeetwenGroups = value; } }
		public bool ShowRowSumsBelow { get { return showRowSumsBelow; } set { showRowSumsBelow = value; } }
		public List<string> ColumnNames { get { return columnNames; } }
		public List<int> SubtotalColumnIndices { get { return subTotalColumnIndices; } }
		public List<SubtotalFunctionItem> FunctionItems { get { return SubtotalFunctionItem.GetSubtotalFormItems(); } }
		public CellRange Range { get { return range; } }
		#endregion
		public void ApplyChanges() {
			SubtotalCommand command = new SubtotalCommand(control);
			command.ApplyChanges(this);
		}
		public void RemoveAll() {
			SubtotalCommand command = new SubtotalCommand(control);
			command.RemoveAllSubtotals(range);
		}
	}
	public class SubtotalFunctionItem {
		#region static
		public static List<SubtotalFunctionItem> GetSubtotalFormItems() {
			List<SubtotalFunctionItem> result = new List<SubtotalFunctionItem>();
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertSum), XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Subtotal_SumName), 9));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCount), 3));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertAverage), 1));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertMax), 4));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertMin), 5));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertProduct), 6));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCountNumbers), XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertCount), 2));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertStdDev), 7));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertStdDevp), 8));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertVar), 10));
			result.Add(new SubtotalFunctionItem(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FunctionsInsertVarp), 11));
			return result;
		}
		#endregion
		#region fields
		string name;
		string displayName;
		int index;
		#endregion
		public SubtotalFunctionItem(string name, string displayName, int index) {
			this.name = name;
			this.displayName = displayName;
			this.index = index;
		}
		public SubtotalFunctionItem(string name, int index)
			: this(name, name, index) {
		}
		#region Properties
		public string Name { get { return name; } }
		public string DisplayName { get { return displayName; } }
		public int Index { get { return index; } }
		#endregion
		public override string ToString() {
			return name;
		}
	}
	#endregion
}
