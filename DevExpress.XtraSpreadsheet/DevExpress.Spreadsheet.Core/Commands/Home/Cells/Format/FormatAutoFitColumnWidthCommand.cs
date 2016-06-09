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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatAutoFitColumnWidthCommand
	public class FormatAutoFitColumnWidthCommand : SpreadsheetSelectedRangesCommand {
		public FormatAutoFitColumnWidthCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAutoFitColumnWidth; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAutoFitColumnWidth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAutoFitColumnWidthDescription; } }
		#endregion
		protected internal override void Modify(CellRange range) {
			ActiveSheet.TryBestFitColumn(range, ColumnBestFitMode.None);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Column.Resize, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatColumnsLocked);
			if (state.Enabled)
				ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Column.AutoFit);
		}
	}
	#endregion
	#region FormatAutoFitWidthUsingMouseCommandBase (abstract class)
	public abstract class FormatAutoFitWidthUsingMouseCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected FormatAutoFitWidthUsingMouseCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				if (CanAutoFitAllSelectedItems())
					AutoFitAllSelectedItems();
				else
					AutoFitOnlyGeneralItem();
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		bool CanAutoFitAllSelectedItems() {
			IList<CellRange> ranges = ActiveSheet.Selection.SelectedRanges;
			bool isContainsItem = false;
			foreach (CellRange range in ranges) {
				if (!IsRangeInterval(range))
					return false;
				isContainsItem |= IsContainsItem(range);
			}
			return isContainsItem;
		}
		protected internal virtual void AutoFitAllSelectedItems() {
			IList<CellRange> ranges = ActiveSheet.Selection.SelectedRanges;
			foreach (CellRange range in ranges)
				AutoFitSelectedItem(range);
		}
		protected internal abstract bool IsRangeInterval(CellRange range);
		protected internal abstract bool IsContainsItem(CellRange range);
		protected internal abstract void AutoFitSelectedItem(CellRange range);
		protected internal abstract void AutoFitOnlyGeneralItem();
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Column.Resize, !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatColumnsLocked);
			if (state.Enabled)
				ApplyCommandsRestriction(state, Options.InnerBehavior.Column.AutoFit);
		}
	}
	#endregion
	#region FormatAutoFitColumnWidthUsingMouseCommand
	public class FormatAutoFitColumnWidthUsingMouseCommand : FormatAutoFitWidthUsingMouseCommandBase {
		#region Fields
		int columnIndex = -1;
		#endregion
		public FormatAutoFitColumnWidthUsingMouseCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAutoFitColumnWidthUsingMouse; } }
		protected internal int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		#endregion
		protected internal override bool IsRangeInterval(CellRange range) {
			return range.IsColumnRangeInterval();
		}
		protected internal override bool IsContainsItem(CellRange range) {
			return range.TopLeft.Column <= ColumnIndex && range.BottomRight.Column >= ColumnIndex;
		}
		protected internal override void AutoFitSelectedItem(CellRange range) {
			ActiveSheet.TryBestFitColumn(range, ColumnBestFitMode.None);
		}
		protected internal override void AutoFitOnlyGeneralItem() {
			ActiveSheet.TryBestFitColumn(ColumnIndex, ColumnBestFitMode.None);
		}
	}
	#endregion
}
