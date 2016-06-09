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
using System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatBordersCommandBase
	public abstract class FormatBordersCommandBase : SpreadsheetCommand {
		protected FormatBordersCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected Color LineColor { get { return DocumentModel.UiBorderInfoRepository.CurrentItem.Color; } }
		protected XlBorderLineStyle LineStyle { get { return DocumentModel.UiBorderInfoRepository.CurrentItem.LineStyle; } }
		protected CellRangeBase SelectedRanges {
			get {
				IList<CellRange> selectedRanges = DocumentModel.ActiveSheet.Selection.SelectedRanges;
				if (selectedRanges.Count == 1)
					return selectedRanges[0];
				int selectedRangesCount = selectedRanges.Count;
				List<CellRangeBase> ranges = new List<CellRangeBase>();
				for (int i = 0; i < selectedRangesCount; i++)
					ranges.Add(selectedRanges[i]);
				return new CellUnion(DocumentModel.ActiveSheet, ranges);
			}
		}
		protected internal abstract MergedBorderInfo BorderModifier { get; }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				ChangeRangeBordersCommand modelCommand = new ChangeRangeBordersCommand(SelectedRanges, BorderModifier);
				modelCommand.Execute();
			} finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region FormatAllBordersCommand
	public class FormatAllBordersCommand : FormatBordersCommandBase {
		public FormatAllBordersCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatAllBorders; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAllBorders; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatAllBordersDescription; } }
		public override string ImageName { get { return "BordersAll"; } }
		protected override bool UseOfficeImage { get { return true; } }
		protected internal override MergedBorderInfo BorderModifier { get { return MergedBorderInfo.CreateAllBorders(LineStyle, LineColor); } }
	}
	#endregion
}
