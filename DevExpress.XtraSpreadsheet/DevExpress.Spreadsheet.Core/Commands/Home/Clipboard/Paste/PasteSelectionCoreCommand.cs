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
using DevExpress.Spreadsheet;
using DevExpress.Office.Commands.Internal;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	public class PasteSelectionCommand : MultiCommand {
		DocumentFormat format = DocumentFormat.Undefined;
		PasteSource pasteSource;
		ModelPasteSpecialOptions pasteOptions;
		Exception pasteException;
		public PasteSelectionCommand(ISpreadsheetControl control)
			: base(control) {
			var clipboardProvider = this.DocumentModel.GetService<DevExpress.XtraSpreadsheet.IClipboardProvider>();
			this.pasteSource = new SpreadsheetPasteSource(clipboardProvider);
			this.pasteOptions = new ModelPasteSpecialOptions();
			InitializeCommands();
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PasteSelection; } }
		public DocumentFormat Format { get { return format; } set { SetFormat(value); } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_Paste; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_PasteDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Paste"; } }
		public ModelPasteSpecialOptions PasteOptions { get { return pasteOptions; } set { SetPasteSpecial(value); } }
		protected internal override void CreateCommands() {
			AddCommand(new PasteContentCopiedRange(Control));
			AddCommand(new PasteContentCommandBiff8(Control));
			AddCommand(new PasteContentCsv(Control));
			AddCommand(new PasteContentTabDelimited(Control));
#if !SL && !DXPORTABLE
			AddCommand(new PasteImageCommand(Control));
			AddCommand(new PasteMetafileCommand(Control));
			AddCommand(new PasteImagesFromFilesCommand(Control));
#endif
		}
		protected internal virtual void AddCommand(PasteCommandBase command) {
			if (CanAddCommandFormat(command))
				Commands.Add(command);
		}
		protected internal virtual bool CanAddCommandFormat(PasteCommandBase command) {
			return this.Format == DocumentFormat.Undefined || this.Format == command.Format;
		}
		void SetFormat(DocumentFormat value) {
			if (format == value)
				return;
			format = value;
			Commands.Clear();
			CreateCommands();
			InitializeCommands();
		}
		void SetPasteSpecial(ModelPasteSpecialOptions value) {
			if (pasteOptions == value)
				return;
			pasteOptions = value;
			Commands.Clear();
			CreateCommands();
			InitializeCommands();
		}
		protected internal virtual void InitializeCommands() {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				PasteCommandBase command = Commands[i] as PasteCommandBase;
				if (command != null) {
					command.PasteSource = pasteSource;
					command.PasteOptions = pasteOptions;
				}
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			pasteException = null;
			this.DocumentModel.BeginUpdate();
			StoreCopiedRange();
			try {
				DocumentModel.RaiseClipboardDataPasting();
				base.ForceExecute(state);
				if (pasteException != null) {
					Control.ShowWarningMessage(pasteException.Message);
				}
			}
			finally {
				RestoreCopiedRange();
				this.DocumentModel.EndUpdate();
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
		bool suppressResetingCopiedRangeOld = false;
		public void RestoreCopiedRange() {
			this.DocumentModel.SuppressResetingCopiedRange = suppressResetingCopiedRangeOld;
		}
		public void StoreCopiedRange() {
			if (DocumentModel.IsCopyCutMode && DocumentModel.CopiedRangeProvider.IsCut)
				return;
			suppressResetingCopiedRangeOld = DocumentModel.SuppressResetingCopiedRange;
			this.DocumentModel.SuppressResetingCopiedRange = true;
		}
	}
}
