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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertObjectCommandBase (abstract class)
	public abstract class InsertObjectCommandBase : RichEditSelectionCommand {
		protected InsertObjectCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return IsContentEditable && CanEditSelection();
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override void PerformModifyModel() {
			DocumentModel.BeginUpdate();
			try {
				ModifyModel();
			}
			finally {
				DocumentModel.EndUpdate();
			}
			ActiveView.EnforceFormattingCompleteForVisibleArea();
		}
		protected internal abstract void ModifyModel();
	}
	#endregion
	#region TransactedInsertObjectCommand (abstract class)
	public abstract class TransactedInsertObjectCommand : TransactedMultiCommand {
		protected TransactedInsertObjectCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return InsertObjectCommand.MenuCaptionStringId; } }
		public override XtraRichEditStringId DescriptionStringId { get { return InsertObjectCommand.DescriptionStringId; } }
		public override string MenuCaption { get { return InsertObjectCommand.MenuCaption; } }
		public override string Description { get { return InsertObjectCommand.Description; } }
		public override string ImageName { get { return InsertObjectCommand.ImageName; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteAllAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		protected internal virtual RichEditCommand InsertObjectCommand { get { return (RichEditCommand)Commands[1]; } }
		protected internal virtual bool KeepLastParagraphMarkInSelection { get { return true; } }
		#endregion
		protected internal override void CreateCommands() {
			Command deleteCommand = CreateDeleteCommand();			
			Commands.Add(deleteCommand);
			CreateInsertObjectCommands();
		}
		protected internal virtual void CreateInsertObjectCommands() {
			Commands.Add(CreateInsertObjectCommand());
		}
		protected internal virtual Command CreateDeleteCommand() {
			DeleteNonEmptySelectionCommandKeepSingleImage command;
			if (KeepLastParagraphMarkInSelection)
				command = new DeleteSelectionKeepLastParagraphCommand(Control);
			else
				command = new DeleteNonEmptySelectionCommandKeepSingleImage(Control);
			command.RestoreInputPositionFormatting = true;
			return command;
		}
		public override void UpdateUIState(ICommandUIState state) {
			if (InsertObjectCommand != null) {
				ICommandUIState commandState = InsertObjectCommand.CreateDefaultCommandUIState();
				InsertObjectCommand.UpdateUIState(commandState);
				state.Enabled = commandState.Enabled;
				state.Visible = commandState.Visible;
				UpdateUIStateViaService(state);
			}
			else
				base.UpdateUIState(state);			
		}
		protected internal abstract RichEditCommand CreateInsertObjectCommand();
	}
	#endregion
}
