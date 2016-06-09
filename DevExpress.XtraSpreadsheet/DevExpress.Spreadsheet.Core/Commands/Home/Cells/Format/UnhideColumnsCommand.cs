﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
namespace DevExpress.XtraSpreadsheet.Commands {
	#region UnhideColumnsCommand
	public class UnhideColumnsCommand : SpreadsheetSelectedRangesCommand {
		public UnhideColumnsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.UnhideColumns; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideColumns; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideColumnsDescription; } }
		#endregion
		protected internal override void Modify(CellRange range) {
			Column column = ActiveSheet.Columns.GetColumnRangeForReading(range.LeftColumnIndex);
			bool doScroll = range.LeftColumnIndex == 0 && column.IsHidden;
			ActiveSheet.UnhideColumns(range.LeftColumnIndex, range.RightColumnIndex);
			if (doScroll)
				ActiveSheet.ScrollToColumn(range.LeftColumnIndex);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Column.Unhide, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatColumnsLocked);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region UnhideColumnsContextMenuItemCommand
	public class UnhideColumnsContextMenuItemCommand : UnhideColumnsCommand {
		public UnhideColumnsContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.UnhideColumnsContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideColumnsContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_UnhideColumnsContextMenuItemDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled &= ActiveSheet.Selection.AsRange().IsColumnRangeInterval();
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
