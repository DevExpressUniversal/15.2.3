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
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class PageSetupAddPrintAreaCommand : SpreadsheetSelectedRangesCommand {
		public PageSetupAddPrintAreaCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupAddPrintArea; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupAddPrintArea; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupAddPrintAreaDescription; } }
		#endregion
		protected internal override void BeforeModifying() {
			if (!PrintAreaCalculator.PrintRangeIsValid(ActiveSheet, false))
				ActiveSheet.ClearPrintRange();
		}
		protected internal override void Modify(CellRange range) {
			ActiveSheet.AddPrintRangeCore(range);
		}
		bool SelectionIntersectsPrintAreaFromDefinedName() {
			DefinedNameBase definedName;
			if (!ActiveSheet.DefinedNames.TryGetItemByName(PrintAreaCalculator.PrintAreaDefinedName, out definedName))
				return true;
			CellRangeBase printRange = ((DefinedName)definedName).GetReferencedRange();
			if (printRange == null)
				return true;
			int count = ActiveSheet.Selection.SelectedRanges.Count;
			for (int i = 0; i < count; i++)
				if (ActiveSheet.Selection.SelectedRanges[i].Intersects(printRange))
					return true;
			return false;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Print, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive);
			if (state.Enabled && PrintAreaCalculator.PrintRangeIsValid(ActiveSheet, false))
				state.Visible = !SelectionIntersectsPrintAreaFromDefinedName();
			else
				state.Visible = false;
		}
	}
}
