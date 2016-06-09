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
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design.Commands
{
	public interface ICommandExecutor : IDisposable {
		void ExecCommand(CommandID cmdID, object[] parameters);
	}
	public delegate void MenuCommandEventHandler(object sender, MenuCommandEventArgs e);
	public class CommandExecuteEventArgs : EventArgs {
		object[] args;
		public object[] Args { get { return args; }
		}
		public CommandExecuteEventArgs(object[] args) {
			this.args = args;
		}
	}
	public class CommandSetItem : MenuCommand {
		private MenuCommand menuCommand;
		private EventHandler statusHandler;
		private EventHandler<CommandExecuteEventArgs> execHandler;
		IMenuCommandService menuService;
		bool locked;
		public bool Locked {
			get { return locked; }
			set { locked = value; }
		}
		public CommandSetItem(IMenuCommandService menuService, EventHandler<CommandExecuteEventArgs> invokeHandler, EventHandler statusHandler, CommandID commandID)
			: base(null, commandID) {
			this.menuService = menuService;
			this.statusHandler = statusHandler;
			this.execHandler = invokeHandler;
			menuCommand = menuService.FindCommand(commandID);
			if(menuCommand != null) menuService.RemoveCommand(menuCommand);
		}
		public virtual void Invoke(object[] args) {
			if(menuCommand != null)
				MenuCommandHandlerBase.InvokeCommandEx(menuCommand, args);
			if(this.execHandler == null) return;
			try {
				this.execHandler(this, new CommandExecuteEventArgs(args));
			} catch(CheckoutException exception) {
				if(exception != CheckoutException.Canceled)
					throw;
				return;
			}
		}
		public override void Invoke(object arg) {
			Invoke(new object[] { arg });
		}
		public override void Invoke() {
			Invoke(null);
		}
		public void UpdateStatus() {
			if(statusHandler != null && !Locked) {
				try {
					statusHandler(GetActualCommand(), EventArgs.Empty);
				} catch { }
			}
		}
		public void Disable() {
			MenuCommand actualCommand = GetActualCommand();
			if(actualCommand == null)
				return;
			actualCommand.Enabled = actualCommand.Supported = false;
		}
		MenuCommand GetActualCommand() {
			MenuCommand actualCommand = menuService.FindCommand(CommandID);
			return actualCommand ?? this;
		}
	}
	public class MenuCommandHandlerBase : IDisposable {
		#region static
		public static void InvokeCommandEx(MenuCommand command, object[] args) {
			if (command is CommandSetItem) {
				((CommandSetItem)command).Invoke(args);
			}
			else {
				command.Invoke();
			}
		}
		#endregion
		protected class CommandSetItemCollection : CollectionBase {
			IMenuCommandService menuService;
			public CommandSetItem this[int index] {
				get { return (CommandSetItem)InnerList[index]; }
			}
			public CommandSetItemCollection(IMenuCommandService menuService) {
				System.Diagnostics.Debug.Assert(menuService != null);
				this.menuService = menuService;
			}
			public void Add(CommandSetItem cmd) {
				menuService.AddCommand(cmd);
				InnerList.Add(cmd);
			}
			public void UpdateStatus() {
				foreach (CommandSetItem item in this) {
					item.UpdateStatus();
				}
			}
			public void Clear(EventHandler commandChangedHandler) {
				for (int i = 0; i < Count; i++)
					RemoveCommand(this[i], commandChangedHandler);
				InnerList.Clear();
			}
			private void RemoveCommand(CommandSetItem cmd, EventHandler commandChangedHandler) {
				if (cmd != null) {
					cmd.CommandChanged -= commandChangedHandler;
					menuService.RemoveCommand(cmd);
				}
			}
		}
		protected CommandSetItemCollection commands;
		protected MenuCommandEventHandler onCommandStatusChanged;
		protected System.Collections.Generic.Dictionary<CommandID, ICommandExecutor> execHT = new System.Collections.Generic.Dictionary<CommandID, ICommandExecutor>();
		protected EventHandler<CommandExecuteEventArgs> commandHandler;
		protected IMenuCommandService menuService;
		public event MenuCommandEventHandler CommandStatusChanged {
			add { onCommandStatusChanged = System.Delegate.Combine(onCommandStatusChanged, value) as MenuCommandEventHandler; }
			remove { onCommandStatusChanged = System.Delegate.Remove(onCommandStatusChanged, value) as MenuCommandEventHandler; }
		}
		public MenuCommandHandlerBase(IServiceProvider serviceProvider) {
			menuService = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			commandHandler = OnMenuCommand;
			commands = new CommandSetItemCollection(menuService);
		}
		private void OnMenuCommand(object sender, CommandExecuteEventArgs e) {
			CommandID cmdID = ((MenuCommand)sender).CommandID;
			ExecuteCommand(cmdID, e.Args);
		}
		void ExecuteCommand(CommandID cmdID, object[] parameters) {
			ICommandExecutor Executor = execHT[cmdID];
			if (Executor != null) {
				Executor.ExecCommand(cmdID, parameters);
				UpdateCommandStatus();
			}
		}
		private void OnCommandStatusChanged(object sender, MenuCommandEventArgs e) {
			if (onCommandStatusChanged != null) onCommandStatusChanged(sender, e);
		}
		protected void AddCommandExecutor(ICommandExecutor executor, EventHandler statusHandler, bool supported, params CommandID[] ids) {
			for (int i = 0; i < ids.Length; i++) {
				if (ids[i] == null) continue;
				execHT.Add(ids[i], executor);
				CommandSetItem cmd = new CommandSetItem(menuService, commandHandler, statusHandler, ids[i]);
				cmd.CommandChanged += new EventHandler(OnCommandChanged);
				cmd.Supported = supported;
				AddCommand(cmd);
			}
		}
		protected void OnCommandChanged(object sender, EventArgs e) {
			OnCommandStatusChanged(this, new MenuCommandEventArgs(sender as MenuCommand));
		}
		public void AddCommand(CommandSetItem cmd) {
			commands.Add(cmd);
		}
		public virtual void UpdateCommandStatus() {
			commands.UpdateStatus();
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				commands.Clear(new EventHandler(OnCommandChanged));
				foreach (ICommandExecutor executor in execHT.Values) {
					executor.Dispose();
				}
				execHT.Clear();
			}
		}
		~MenuCommandHandlerBase() {
			Dispose(false);
		}
	}
}
