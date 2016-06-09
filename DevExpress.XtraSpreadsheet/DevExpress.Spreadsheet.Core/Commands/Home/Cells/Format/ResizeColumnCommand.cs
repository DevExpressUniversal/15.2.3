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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ResizeColumnCommand
	public class ResizeColumnCommand : SpreadsheetCommand {
		public ResizeColumnCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<ResizeColumnCommandArgument>();
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				ExecuteCore(state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Column.Resize, !InnerControl.IsInplaceEditorActive);
			if (state.Enabled)
				state.Enabled = (state as IValueBasedCommandUIState<ResizeColumnCommandArgument>) != null;
		}
		protected internal virtual void ExecuteCore(ICommandUIState state) {
			IValueBasedCommandUIState<ResizeColumnCommandArgument> valueBasedState = state as IValueBasedCommandUIState<ResizeColumnCommandArgument>;
			if (valueBasedState == null)
				return;
			ResizeColumnCommandArgument argument = valueBasedState.Value;
			if (argument == null)
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				float widthInCharacters = argument.WidthInCharacters;
				if (DocumentModel.ActiveSheet.Selection.IsAllSelected)
					AllSelectResize(widthInCharacters);
				else {
					CellRangeBase selectionRange = DocumentModel.ActiveSheet.Selection.AsRange();
					int columnIndex = argument.ColumnIndex;
					if (IsSelectedColumnRangeResize(selectionRange, columnIndex))
						ResizeColumnRange(selectionRange, widthInCharacters);
					else {
						Column column = DocumentModel.ActiveSheet.Columns.GetIsolatedColumn(columnIndex);
						column.SetCustomWidth(widthInCharacters);
						CorrectActiveView(columnIndex, columnIndex, argument.IsFirstColumn);
					}
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void AllSelectResize(float widthInCharacters) {
			ActiveSheet.UnhideColumns(0, ActiveSheet.MaxColumnCount - 1);
			foreach (Column column in DocumentModel.ActiveSheet.Columns.GetExistingColumns()) {
				column.BeginUpdate();
				try {
					column.BestFit = false;
					column.IsCustomWidth = false;
					column.Width = 0;
				}
				finally {
					column.EndUpdate();
				}
			}
			DocumentModel.ActiveSheet.Properties.FormatProperties.DefaultColumnWidth = widthInCharacters;
		}
		bool IsSelectedColumnRangeResize(CellRangeBase range, int columnIndex) {
			if (!range.IsColumnRangeInterval())
				return false;
			CellRange cellRange = range as CellRange;
			if (cellRange != null)
				return IsSelectedColumnRangeResizeCore(cellRange, columnIndex);
			foreach (CellRange singleRange in (range as CellUnion).InnerCellRanges)
				if (IsSelectedColumnRangeResizeCore(singleRange, columnIndex))
					return true;
			return false;
		}
		bool IsSelectedColumnRangeResizeCore(CellRangeBase range, int columnIndex) {
			return range.TopLeft.Column <= columnIndex && columnIndex <= range.BottomRight.Column;
		}
		void ResizeColumnRange(CellRangeBase range, float widthInCharacters) {
			if (range.RangeType == CellRangeType.UnionRange)
				foreach (CellRange singleRange in (range as CellUnion).InnerCellRanges)
					ResizeColumnSingleRange(singleRange, widthInCharacters);
			else
				ResizeColumnSingleRange(range, widthInCharacters);
		}
		void ResizeColumnSingleRange(CellRangeBase range, float widthInCharacters) {
			int left = range.TopLeft.Column;
			int right = range.BottomRight.Column;
			if (widthInCharacters == 0)
				ActiveSheet.HideColumns(left, right);
			else
				ActiveSheet.UnhideColumns(left, right);
			IList<Column> list = ActiveSheet.Columns.GetColumnRangesEnsureExist(left, right);
			for (int i = 0; i < list.Count; ++i)
				list[i].SetCustomWidth(widthInCharacters);
			bool needScroll = right == ActiveSheet.ActiveView.TopLeftCell.Column - 1;
			CorrectActiveView(left, right, needScroll); 
		}
		void CorrectActiveView(int firstColumnIndex, int lastColumnIndex, bool needCorrection) {
			ModelWorksheetView activeView = ActiveSheet.ActiveView;
			if (needCorrection) {
				activeView.TopLeftCell = new CellPosition(firstColumnIndex, activeView.TopLeftCell.Row);
				if (activeView.IsFrozen()
					&& activeView.HorizontalSplitPosition + activeView.TopLeftCell.Column != activeView.SplitTopLeftCell.Column)
					activeView.HorizontalSplitPosition = activeView.SplitTopLeftCell.Column - activeView.TopLeftCell.Column;
			}
		}
	}
	#endregion
	public class ResizeColumnCommandArgument {
		public int ColumnIndex { get; set; }
		public float WidthInCharacters { get; set; }
		public bool IsFirstColumn { get; set; }
	}
}
