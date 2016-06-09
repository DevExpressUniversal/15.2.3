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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ReplaceCommand
	public class ReplaceCommand : FindReplaceCommandBase, ICellInplaceEditorCommitControllerOwner {
		public ReplaceCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.EditingReplace; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingReplace; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_EditingReplaceDescription; } }
		protected override bool UseOfficeImage { get { return true; } }
		public override string ImageName { get { return "Replace"; } }
		protected internal override bool IsReplaceCommand { get { return true; } }
		#endregion
		protected internal override void PerformDefaultAction(FindReplaceViewModel viewModel) {
			ReplaceNext(viewModel);
		}
		protected internal void ReplaceNext(FindReplaceViewModel viewModel) {
			if (ActiveSheet.ReadOnly)
				return;
			if (ActiveSheet.Properties.Protection.SheetLocked) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CantReplaceOnProtectedSheet));
				return;
			}
			if (!PerformReplace(viewModel)) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ReplaceCantFindMatch));
				return;
			}
			PerformFindNext(viewModel);
		}
		bool PerformReplace(FindReplaceViewModel viewModel) {
			SpreadsheetSearchEngine engine = new SpreadsheetSearchEngine();
			ModelSearchOptions options = SpreadsheetSearchHelper.CreateSearchOptions(ActiveSheet, viewModel, new CellRange(ActiveSheet, Selection.ActiveCell, Selection.ActiveCell));
			IEnumerator<ICell> enumerator = engine.Search(viewModel.FindWhat, options);
			if (!enumerator.MoveNext() || enumerator.Current == null)
				return false;
			return ReplaceCellContent(enumerator.Current, viewModel);
		}
		protected internal void ReplaceAll(FindReplaceViewModel viewModel) {
			if (ActiveSheet.ReadOnly)
				return;
			if (ActiveSheet.Properties.Protection.SheetLocked) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CantReplaceOnProtectedSheet));
				return;
			}
			int replacementCount;
			DocumentModel.BeginUpdateFromUI();
			try {
				replacementCount = ReplaceAllCore(viewModel);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
			if (replacementCount <= 0)
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_SearchCantFindData));
			else
				Control.ShowWarningMessage(String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ReplaceAllSucceeded), replacementCount));
		}
		protected internal int ReplaceAllCore(FindReplaceViewModel viewModel) {
			int count = Selection.SelectedRanges.Count;
			if (count == 1 && Selection.ActiveRange.CellCount == 1)
				return ReplaceAllInRange(viewModel, new CellRange(ActiveSheet, new CellPosition(0, 0), new CellPosition(ActiveSheet.MaxColumnCount - 1, ActiveSheet.MaxRowCount - 1)));
			else {
				int result = 0;
				for (int i = 0; i < count; i++)
					result += ReplaceAllInRange(viewModel, Selection.SelectedRanges[i]);
				return result;
			}
		}
		protected internal int ReplaceAllInRange(FindReplaceViewModel viewModel, CellRange searchRange) {
			int count = 0;
			SpreadsheetSearchEngine engine = new SpreadsheetSearchEngine();
			ModelSearchOptions options = SpreadsheetSearchHelper.CreateSearchOptions(ActiveSheet, viewModel, searchRange);
			IEnumerator<ICell> enumerator = engine.Search(viewModel.FindWhat, options);
			foreach (ICell cell in new Enumerable<ICell>(enumerator)) {
				if (ReplaceCellContent(cell, viewModel))
					count++;
			}
			return count;
		}
		bool ReplaceCellContent(ICell cell, FindReplaceViewModel viewModel) {
			if (cell.HasFormula)
				return ReplaceCellFormula(cell, viewModel);
			else
				return ReplaceCellValue(cell, viewModel);
		}
		bool ReplaceCellFormula(ICell cell, FindReplaceViewModel viewModel) {
			if (string.IsNullOrEmpty(viewModel.FindWhat))
				return false;
			string content = cell.FormulaBody;
			string newContent = content.Replace(viewModel.FindWhat, viewModel.ReplaceWith);
			CellInplaceEditorCommitController controller = new CellInplaceEditorCommitController(this, cell);
			CellInplaceEditorCommitResult commitResult = controller.Commit(newContent);
			if (!commitResult.Success) {
				Control.ShowWarningMessage(commitResult.Message);
				return false;
			}
			return true;
		}
		bool ReplaceCellValue(ICell cell, FindReplaceViewModel viewModel) {
			string findWhat = string.IsNullOrEmpty(viewModel.FindWhat) ? string.Empty : viewModel.FindWhat;
			string content = cell.Value.ToText(cell.Context).InlineTextValue;
			string newContent;
			if (viewModel.MatchEntireCellContents) {
				if (findWhat.Length != content.Length)
					return false;
				newContent = viewModel.ReplaceWith;
			}
			else {
				if (!string.IsNullOrEmpty(findWhat))
					newContent = ReplaceContent(content, viewModel);
				else if (string.IsNullOrEmpty(content))
					newContent = viewModel.ReplaceWith;
				else
					return false;
			}
			if (string.IsNullOrEmpty(newContent))
				cell.ClearContent();
			else
				cell.SetTextSmart(newContent);
			return true;
		}
		string ReplaceContent(string content, FindReplaceViewModel viewModel) {
			string findWhat = string.IsNullOrEmpty(viewModel.FindWhat) ? string.Empty : viewModel.FindWhat;
			string replaceWith = string.IsNullOrEmpty(viewModel.ReplaceWith) ? string.Empty : viewModel.ReplaceWith;
			int findWhatLength = findWhat.Length;
			int replaceWithLength = replaceWith.Length;
			CompareOptions options = viewModel.MatchCase ? CompareOptions.None : CompareOptions.IgnoreCase;
			CompareInfo compareInfo = ActiveSheet.DataContext.Culture.CompareInfo;
			int startIndex = 0;
			int index = compareInfo.IndexOf(content, findWhat, startIndex, options);
			while (index >= 0) {
				content = content.Remove(index, findWhatLength).Insert(index, replaceWith);
				startIndex = index + replaceWithLength;
				if (startIndex >= content.Length)
					break;
				index = compareInfo.IndexOf(content, findWhat, startIndex, options);
			}
			return content;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = state.Enabled && !ActiveSheet.ReadOnly;
		}
		void ICellInplaceEditorCommitControllerOwner.RaiseCellValueChanged(SpreadsheetCellEventArgs args) {
		}
		public DialogResult ShowDataValidationDialog(string text, string message, string title, DataValidationErrorStyle errorStyle) {
			return DialogResult.OK;
		}
	}
	#endregion
}
