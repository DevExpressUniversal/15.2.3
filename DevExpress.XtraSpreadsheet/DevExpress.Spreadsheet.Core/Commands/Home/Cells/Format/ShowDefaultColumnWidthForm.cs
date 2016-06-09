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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ShowColumnWidthFormCommandBase (abstract class)
	public abstract class ShowColumnWidthFormCommandBase : SpreadsheetCommand {
		protected ShowColumnWidthFormCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (InnerControl.AllowShowingForms) {
					IColumnWidthCalculationService service = GetColumnWidthService();
					if (service != null)
						ShowForm(service);
				}
				else {
					IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
					if (valueBasedState != null)
						SetWidth(valueBasedState.Value);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal IColumnWidthCalculationService GetColumnWidthService() {
			return DocumentModel.GetService<IColumnWidthCalculationService>();
		}
		public bool Validate(float widthInCharacters) {
			if (widthInCharacters < 0 || widthInCharacters > 255) {
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnWidth));
				return false;
			}
			return true;
		}
		void SetWidth(int widthInPixels) {
			IColumnWidthCalculationService service = DocumentModel.GetService<IColumnWidthCalculationService>();
			if (service == null)
				return;
			float widthInCharacters = service.ConvertPixelsToCharacters(ActiveSheet, widthInPixels);
			widthInCharacters = service.RemoveGaps(ActiveSheet, widthInCharacters);
			ApplyChanges(widthInCharacters);
		}
		public void ApplyChanges(float widthInCharacters) {
			DocumentModel.BeginUpdateFromUI();
			try {
				ApplyChangesCore(widthInCharacters);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatColumnsLocked);
		}
		protected internal abstract void ShowForm(IColumnWidthCalculationService service);
		protected internal abstract void ApplyChangesCore(float widthInCharacters);
	}
	#endregion
	#region ShowDefaultColumnWidthFormCommand
	public class ShowDefaultColumnWidthFormCommand : ShowColumnWidthFormCommandBase {
		public ShowDefaultColumnWidthFormCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatDefaultColumnWidth; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatDefaultColumnWidth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatDefaultColumnWidthDescription; } }
		#endregion
		protected internal override void ShowForm(IColumnWidthCalculationService service) {
			DefaultColumnWidthViewModel viewModel = new DefaultColumnWidthViewModel(Control);
			viewModel.Value = service.CalculateDefaultColumnWidthInChars(ActiveSheet, DocumentModel.MaxDigitWidthInPixels);
			Control.ShowDefaultColumnWidthForm(viewModel);
		}
		protected internal override void ApplyChangesCore(float widthInCharacters) {
			ActiveSheet.Properties.FormatProperties.DefaultColumnWidth = widthInCharacters;
		}
	}
	#endregion
}
