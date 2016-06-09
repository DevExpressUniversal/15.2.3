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

using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.ComponentModel;
#if !SL
using System.Collections.Generic;
#else
using System.Windows;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraSpreadsheet.Commands {
	public class CutSelectionCommand : SpreadsheetMenuItemSimpleCommand {
		public CutSelectionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_CutSelection; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_CutSelectionDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Cut"; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.CutSelection; } }
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				if (InnerControl.IsInplaceEditorActive)
					InnerControl.InplaceEditor.Cut();
				else {
					IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
					if (selectedRanges.Count == 0)
						return;
					CellRange range = selectedRanges[0];
					if (ActiveSheet.Properties.Protection.SheetLocked) {
						if (range.ContainsLockedCells()) {
							ModelErrorInfo error = new ModelErrorInfo(ModelErrorType.CellOrChartIsReadonly);
							this.ErrorHandler.HandleError(error);
							return;
						}
					}
					DocumentModel.CopyCutRangeToClipboard(range, true);
				}
				DocumentModel.ApplyChanges(DocumentModelChangeActions.ResetInvalidDataCircles | DocumentModelChangeActions.Redraw);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			if (InnerControl.IsInplaceEditorActive) {
				state.Visible = true;
				state.Enabled = InnerControl.InplaceEditor.CanCopy();
				state.Checked = false;
			}
			else {
				bool isSingleSelection = DocumentModel.ActiveSheet.Selection.SelectedRanges.Count == 1;
				state.Checked = false;
				ApplyCommandRestrictionOnEditableControl(state, Options.InnerBehavior.Cut, isSingleSelection);
			}
			if (ActiveSheet.ReadOnly)
				state.Enabled = false;
		}
	}
}
