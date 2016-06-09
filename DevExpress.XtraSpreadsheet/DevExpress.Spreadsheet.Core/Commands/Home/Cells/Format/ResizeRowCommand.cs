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
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ResizeRowCommand
	public class ResizeRowCommand : SpreadsheetCommand {
		public ResizeRowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<ResizeRowCommandArgument>();
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
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Row.Resize, !InnerControl.IsInplaceEditorActive);
			if (state.Enabled)
				state.Enabled = (state as IValueBasedCommandUIState<ResizeRowCommandArgument>) != null;
		}
		protected internal virtual void ExecuteCore(ICommandUIState state) {
			IValueBasedCommandUIState<ResizeRowCommandArgument> valueBasedState = state as IValueBasedCommandUIState<ResizeRowCommandArgument>;
			if (valueBasedState == null)
				return;
			ResizeRowCommandArgument argument = valueBasedState.Value;
			if (argument == null)
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				float height = argument.Height;
				if (DocumentModel.ActiveSheet.Selection.IsAllSelected)
					AllSelectResize(height);
				else {
					CellRangeBase selectionRange = DocumentModel.ActiveSheet.Selection.AsRange();
					int rowIndex = argument.RowIndex;
					if (IsSelectedRowRangeResize(selectionRange, rowIndex))
						ResizeRowRange(selectionRange, height);
					else {
						ResizeSingleRow(rowIndex, height);
						CorrectActiveView(rowIndex, argument.IsFirstRow);
					}
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		void AllSelectResize(float modelHeight) {
			ActiveSheet.UnhideRows(0, ActiveSheet.MaxRowCount - 1);
			foreach (Row row in DocumentModel.ActiveSheet.Rows) {
				row.BeginUpdate();
				try {
					row.IsCustomHeight = false;
					row.Height = 0;
				}
				finally {
					row.EndUpdate();
				}
			}
			DocumentModel.ActiveSheet.Properties.FormatProperties.IsCustomHeight = true;
			DocumentModel.ActiveSheet.Properties.FormatProperties.DefaultRowHeight = modelHeight;
		}
		bool IsSelectedRowRangeResize(CellRangeBase range, int rowIndex) {
			if (!range.IsRowRangeInterval())
				return false;
			CellRange cellRange = range as CellRange;
			if (cellRange != null)
				return IsSelectedRowRangeResizeCore(cellRange, rowIndex);
			foreach (CellRange singleRange in (range as CellUnion).InnerCellRanges)
				if (IsSelectedRowRangeResizeCore(singleRange, rowIndex))
					return true;
			return false;
		}
		bool IsSelectedRowRangeResizeCore(CellRangeBase range, int rowIndex) {
			return range.TopLeft.Row <= rowIndex && rowIndex <= range.BottomRight.Row;
		}
		void ResizeRowRange(CellRangeBase range, float modelHeight) {
			if (range.RangeType == CellRangeType.UnionRange)
				foreach (CellRange singleRange in (range as CellUnion).InnerCellRanges)
					ResizeSingleRowRange(singleRange, modelHeight);
			else
				ResizeSingleRowRange(range, modelHeight);
		}
		void ResizeSingleRowRange(CellRangeBase range, float modelHeight) {
			int top = range.TopLeft.Row;
			int bottom = range.BottomRight.Row;
			if (modelHeight == 0)
				ActiveSheet.HideRows(top, bottom);
			else
				ActiveSheet.UnhideRows(top, bottom);
			for (int i = top; i <= bottom; i++)
				ResizeSingleRow(i, modelHeight);
			bool needScroll = bottom == ActiveSheet.ActiveView.TopLeftCell.Row - 1;
			CorrectActiveView(top, needScroll); 
		}
		void ResizeSingleRow(int rowIndex, float modelHeight) {
			Row row = DocumentModel.ActiveSheet.Rows.GetRow(rowIndex);
			row.SetCustomHeight(modelHeight);
		}
		void CorrectActiveView(int firstRowIndex, bool needCorrection) {
			if (needCorrection) {
				ModelWorksheetView activeView = DocumentModel.ActiveSheet.ActiveView;
				activeView.TopLeftCell = new CellPosition(activeView.TopLeftCell.Column, firstRowIndex);
				if (activeView.IsFrozen()
					&& activeView.VerticalSplitPosition + activeView.TopLeftCell.Row != activeView.SplitTopLeftCell.Row)
					activeView.VerticalSplitPosition = activeView.SplitTopLeftCell.Row - activeView.TopLeftCell.Row;
			}
		}
	}
	#endregion
	public class ResizeRowCommandArgument {
		public int RowIndex { get; set; }
		public float Height { get; set; }
		public bool IsFirstRow { get; set; }
	}
}
