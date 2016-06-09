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

using DevExpress.Office.History;
using DevExpress.Utils.Commands;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Commands {
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
	#region MultiCommand
	public abstract class MultiCommand : SpreadsheetMenuItemSimpleCommand {
		CommandCollection commands = new CommandCollection();
		protected MultiCommand(ISpreadsheetControl control)
			: base(control) {
			CreateCommands();
		}
		#region Properties
		protected internal CommandCollection Commands { get { return commands; } }
		public override CommandSourceType CommandSourceType {
			get { return base.CommandSourceType; }
			set {
				if(base.CommandSourceType == value)
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
			for(int i = 0; i < count; i++)
				Commands[i].CommandSourceType = this.CommandSourceType;
		}
		public override void UpdateUIState(ICommandUIState state) {
			int count = Commands.Count;
			for(int i = 0; i < count; i++) {
				Command command = Commands[i];
				ICommandUIState commandState = command.CreateDefaultCommandUIState();
				command.UpdateUIState(commandState);
				if(commandState.Enabled && commandState.Visible) {
					state.Enabled = commandState.Enabled;
					state.Visible = commandState.Visible;
					state.Checked = commandState.Checked;
					if(UpdateUIStateMode == MultiCommandUpdateUIStateMode.EnableIfAnyAvailable)
						return;
				}
				else {
					if(UpdateUIStateMode == MultiCommandUpdateUIStateMode.EnableIfAllAvailable) {
						state.Enabled = commandState.Enabled;
						state.Visible = commandState.Visible;
						state.Checked = commandState.Checked;
						return;
					}
				}
			}
			state.Enabled = (count > 0) && (UpdateUIStateMode == MultiCommandUpdateUIStateMode.EnableIfAllAvailable);
			state.Visible = true;
			state.Checked = false;
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
			for(int i = 0; i < count; i++) {
				Command command = Commands[i];
				ICommandUIState commandState = command.CreateDefaultCommandUIState();
				command.UpdateUIState(commandState);
				if(commandState.Enabled && commandState.Visible) {
					bool executed = ExecuteCommand(command, state);
					if(executed && ExecutionMode == MultiCommandExecutionMode.ExecuteFirstAvailable)
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
}
