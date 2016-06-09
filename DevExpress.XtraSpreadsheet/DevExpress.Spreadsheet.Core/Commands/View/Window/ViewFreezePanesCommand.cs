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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ViewFreezePanesCommand
	public class ViewFreezePanesCommand : ViewFreezePanesSpecificCommand {
		public ViewFreezePanesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ViewFreezePanes; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ViewFreezePanes; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ViewFreezePanesDescription; } }
		public override string ImageName { get { return "FreezePanes"; } }
		#endregion
		protected internal override void Modify(CellRange visibleRange) {
			CellPosition activeCell = ActiveSheet.Selection.ActiveCell;
			CellPosition visibleTopLeftCell = visibleRange.TopLeft;
			int activeColumn = activeCell.Column;
			int activeRow = activeCell.Row;
			if (activeCell.EqualsPosition(visibleTopLeftCell) || !visibleRange.ContainsCell(activeColumn, activeRow)) {
				SplitVisibleRangeByFourParts(visibleRange);
				return;
			}
			int xSplit = activeColumn - visibleTopLeftCell.Column;
			int ySplit = activeRow - visibleTopLeftCell.Row;
			if (visibleTopLeftCell.Column == activeColumn)
				ActiveSheet.FreezeRows(ySplit, visibleTopLeftCell);
			else if (visibleTopLeftCell.Row == activeRow)
				ActiveSheet.FreezeColumns(xSplit, visibleTopLeftCell);
			else
				ActiveSheet.FreezePanes(xSplit, ySplit, visibleTopLeftCell);
		}
		void SplitVisibleRangeByFourParts(CellRange visibleRange) {
			CellPosition visibleTopLeftCell = visibleRange.TopLeft;
			int xSplit = visibleRange.Width / 2;
			int ySplit = visibleRange.Height / 2;
			ActiveSheet.FreezePanes(xSplit, ySplit, visibleTopLeftCell);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible &= !ActiveSheet.IsFrozen();
		}
	}
	#endregion
}
