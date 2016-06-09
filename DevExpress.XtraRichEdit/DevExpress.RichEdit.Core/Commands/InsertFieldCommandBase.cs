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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertFieldCommand
	public class InsertFieldCommand : TransactedInsertObjectCommand {
		public InsertFieldCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertFieldCommandFieldCode")]
#endif
		public string FieldCode {
			get {
				InsertFieldCoreCommand command = (InsertFieldCoreCommand)InsertObjectCommand;
				return command.FieldCode;
			}
			set {
				InsertFieldCoreCommand command = (InsertFieldCoreCommand)InsertObjectCommand;
				command.FieldCode = value;
			}
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertFieldCoreCommand(Control);
		}
	}
	#endregion
	#region ShowInsertMergeFieldFormCommand
	public class ShowInsertMergeFieldFormCommand : RichEditMenuItemSimpleCommand {
		public ShowInsertMergeFieldFormCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowInsertMergeFieldFormCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ShowInsertMergeFieldForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowInsertMergeFieldFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowInsertMergeFieldForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowInsertMergeFieldFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowInsertMergeFieldFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowInsertMergeFieldFormCommandImageName")]
#endif
		public override string ImageName { get { return "InsertDataField"; } }
		protected internal override void ExecuteCore() {
#if !SL
			Control.ShowInsertMergeFieldForm();
#endif
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable && DocumentModel.MailMergeDataController.IsReady;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
#endregion
#region InsertMergeFieldCommand
	public class InsertMergeFieldCommand : RichEditMenuItemSimpleCommand {
		string fieldName = String.Empty;
		public InsertMergeFieldCommand(IRichEditControl control)
			: base(control) {
		}
		public InsertMergeFieldCommand(IRichEditControl control, string fieldArgument)
			: base(control) {
			this.fieldName = fieldArgument;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertMergeFieldCommandFieldArgument")]
#endif
		public string FieldArgument { get { return fieldName; } set { fieldName = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertMergeFieldCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertMergeField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertMergeFieldCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertMergeFieldDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertMergeFieldCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertMailMergeField; } }
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<string> valueBasedState = state as IValueBasedCommandUIState<string>;
			if (valueBasedState != null) {
				if (!String.IsNullOrEmpty(valueBasedState.Value))
					fieldName = valueBasedState.Value;
			}
			base.ForceExecute(state);
		}
		protected internal override void ExecuteCore() {
			InsertFieldCommand command = new InsertFieldCommand(Control);
			if(String.IsNullOrEmpty(fieldName) || !fieldName.Contains(" "))
				command.FieldCode = String.Format("MERGEFIELD {0}", fieldName);
			else
				command.FieldCode = String.Format("MERGEFIELD \"{0}\"", fieldName);
			command.CommandSourceType = CommandSourceType;
			command.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<string> result = new DefaultValueBasedCommandUIState<string>();
			result.Value = fieldName;
			return result;
		}
	}
#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertMergeFieldPlaceholderCommand
	public class InsertMergeFieldPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public InsertMergeFieldPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertMergeField; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertMergeFieldDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertMailMergeFieldPlaceholder; } }
		public override string ImageName { get { return "InsertDataField"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			using (RichEditDataControllerAdapterBase adapter = DocumentModel.CreateMailMergeDataController()) {
				adapter.DataSource = Options.MailMerge.DataSource;
				state.Enabled = IsContentEditable && adapter.GetColumnNames().Length > 0;
				state.Visible = true;
				ApplyDocumentProtectionToSelectedCharacters(state);
			}
		}
	}
	#endregion
	#region InsertFieldCoreCommand
	public class InsertFieldCoreCommand : InsertTextCoreBaseCommand {
		string fieldCode = String.Empty;
		public InsertFieldCoreCommand(IRichEditControl control, string fieldCode)
			: base(control) {
			FieldCode = fieldCode;
		}
		public InsertFieldCoreCommand(IRichEditControl control)
			: base(control) {
		}
		public string FieldCode { get { return fieldCode; } set { fieldCode = value; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertField; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertFieldDescription; } }
		protected internal override bool AllowAutoCorrect { get { return false; } }
		protected internal override void ModifyModel() {
			DocumentLogPosition startPosition = DocumentModel.Selection.End;
			base.ModifyModel();
			Field field = ActivePieceTable.CreateField(startPosition, GetInsertedText().Length, GetForceVisible());
			UpdateField(field);
		}
		protected internal virtual void UpdateField(Field field) {
			UpdateFieldCommand updateCommand = new UpdateFieldCommand(Control, field);
			updateCommand.Execute();
		}
		protected internal override string GetInsertedText() {
			return String.Format(" {0} ", FieldCode);
		}
	}
	#endregion
}
