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
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Commands {
	#region MultiCommandExecutionMode
	public enum MultiCommandExecutionMode {
		ExecuteFirstAvailable,
		ExecuteAllAvailable
	}
	#endregion
	#region MultiCommandUpdateUIStateMode
	public enum MultiCommandUpdateUIStateMode {
		EnableIfAnyAvailable,
		EnableIfAllAvailable
	}
	#endregion
	#region MultiCommand (abstract class)
	public abstract class MultiCommand : RichEditMenuItemSimpleCommand {
		CommandCollection commands = new CommandCollection();
		protected MultiCommand(IRichEditControl control)
			: this(control, true) {
		}
		protected MultiCommand(IRichEditControl control, bool shouldCreateCommands)
			: base(control) {
			if (shouldCreateCommands)
				CreateCommands();
		}
		#region Properties
		protected internal CommandCollection Commands { get { return commands; } }
		public override CommandSourceType CommandSourceType {
			get { return base.CommandSourceType; }
			set {
				if (base.CommandSourceType == value)
					return;
				base.CommandSourceType = value;
				UpdateNestedCommandsSourceType();
			}
		}
		protected internal abstract MultiCommandExecutionMode ExecutionMode { get; }
		protected internal abstract MultiCommandUpdateUIStateMode UpdateUIStateMode { get; }
		#endregion
		protected internal virtual void UpdateNestedCommandsSourceType() {
			int count = Commands.Count;
			for (int i = 0; i < count; i++)
				Commands[i].CommandSourceType = this.CommandSourceType;
		}
		public override void UpdateUIState(ICommandUIState state) {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				Command command = Commands[i];
				ICommandUIState commandState = command.CreateDefaultCommandUIState();
				command.UpdateUIState(commandState);
				if (commandState.Enabled && commandState.Visible) {
					state.Enabled = commandState.Enabled;
					state.Visible = commandState.Visible;
					state.Checked = commandState.Checked;
					if (UpdateUIStateMode == MultiCommandUpdateUIStateMode.EnableIfAnyAvailable) {
						UpdateUIStateViaService(state);
						return;
					}
				}
				else {
					if (UpdateUIStateMode == MultiCommandUpdateUIStateMode.EnableIfAllAvailable) {
						state.Enabled = commandState.Enabled;
						state.Visible = commandState.Visible;
						state.Checked = commandState.Checked;
						UpdateUIStateViaService(state);
						return;
					}
				}
			}
			state.Enabled = (count > 0) && (UpdateUIStateMode == MultiCommandUpdateUIStateMode.EnableIfAllAvailable);
			state.Visible = true;
			state.Checked = false;
			UpdateUIStateViaService(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			try {
				ForceExecuteCore(state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ForceExecuteCore(ICommandUIState state) {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				Command command = Commands[i];
				ICommandUIState commandState = command.CreateDefaultCommandUIState();
				command.UpdateUIState(commandState);
				if (commandState.Enabled && commandState.Visible) {
					bool executed = ExecuteCommand(command, state);					
					if (executed && ExecutionMode == MultiCommandExecutionMode.ExecuteFirstAvailable)
						return;
				}
			}
		}
		protected internal virtual bool ExecuteCommand(Command command, ICommandUIState state) {
			command.ForceExecute(state);
			return true;
		}
		protected internal override void ExecuteCore() {
		}
		protected internal abstract void CreateCommands();
	}
	#endregion
	#region TransactedMultiCommand (abstract class)
	public abstract class TransactedMultiCommand : MultiCommand {
		protected TransactedMultiCommand(IRichEditControl edit)
			: base(edit) {
		}
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			try {
				Control.BeginUpdate();
				try {
					using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
						DocumentModel.SuspendSyntaxHighlight();
						try {
							base.ForceExecute(state);
						}
						finally {
							DocumentModel.ResumeSyntaxHighlight();
						}
						DocumentModel.ForceSyntaxHighlight();
						transaction.SuppressRaiseOperationComplete = true;
					}
				}
				finally {
					Control.EndUpdate();
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
	}
	#endregion
	#region CustomTransactedMultiCommand (abstract class)
	public class CustomTransactedMultiCommand : TransactedMultiCommand {
		readonly MultiCommandExecutionMode executionMode;
		readonly MultiCommandUpdateUIStateMode updateUIStateMode;
		public CustomTransactedMultiCommand(IRichEditControl control, CommandCollection commands, MultiCommandExecutionMode executionMode, MultiCommandUpdateUIStateMode updateUIStateMode)
			: base(control) {
			Guard.ArgumentNotNull(commands, "commands");
			this.Commands.AddRange(commands);
			this.executionMode = executionMode;
			this.updateUIStateMode = updateUIStateMode;
		}
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return executionMode; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return updateUIStateMode; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override void CreateCommands() {
		}
	}
	#endregion
}
