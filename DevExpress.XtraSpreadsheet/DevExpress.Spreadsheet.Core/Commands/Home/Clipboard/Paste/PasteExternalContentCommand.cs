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
using System.Collections.Generic;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.Office.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class PasteExternalContentCommand : MultiCommand {
		readonly PasteSource pasteSource;
		ModelPasteSpecialOptions pasteOptions;
		Exception pasteException;
		readonly CellPosition cellPosition;
		public PasteExternalContentCommand(ISpreadsheetControl control, PasteSource pasteSource, CellPosition cell)
			: base(control) {
			Guard.ArgumentNotNull(pasteSource, "pasteSource");
			this.pasteSource = pasteSource;
			this.pasteOptions = new ModelPasteSpecialOptions();
			this.cellPosition = cell;
			InitializeCommands();
		}
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_Paste; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_PasteDescription; } }
		public ModelPasteSpecialOptions PasteOptions {
			get { return pasteOptions; }
			set {
				if (pasteOptions == value)
					return;
				pasteOptions = value;
				Commands.Clear();
				CreateCommands();
				InitializeCommands();
			}
		}
		protected internal virtual void InitializeCommands() {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				PasteCommandBase command = Commands[i] as PasteCommandBase;
				if (command != null) {
					command.PasteSource = pasteSource;
					command.PasteOptions = pasteOptions;
				}
#if !DXPORTABLE
				command = Commands[i] as PasteFieldListInfoCommand;
				if (command != null) {
					((PasteFieldListInfoCommand)command).Range = new CellRange(ActiveSheet, this.cellPosition, this.cellPosition);
				}
				command = Commands[i] as PasteMailMergeParameterCommand;
				if (command != null) {
					((PasteMailMergeParameterCommand)command).Range = new CellRange(ActiveSheet, this.cellPosition, this.cellPosition);
				}
#endif
			}
		}
		protected internal override void CreateCommands() {
			Commands.Add(new PasteLoadDocumentFromFileCommand(Control));
			Commands.Add(new PasteFieldListInfoCommand(Control));
			Commands.Add(new PasteMailMergeParameterCommand(Control));
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			pasteException = null;
			try {
				base.ForceExecuteCore(state);
				if (pasteException != null) {
					Control.ShowWarningMessage(pasteException.Message);
				}
			}
			finally {
				pasteException = null;
			}
		}
		protected internal override bool ExecuteCommand(Command command, ICommandUIState state) {
			try {
				pasteException = null;
				base.ExecuteCommand(command, state);
				return true;
			}
			catch (Exception e) {
				if (pasteException == null)
					pasteException = e;
				return true;
			}
		}
	}
}
