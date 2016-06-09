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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
namespace DevExpress.Utils.Design {
	public class MenuHandlerService {
		public event AllowMenuCommandEventHandler AllowMenuCommand;
		IDesignerHost host = null;
		ISelectionService selectionService;
		int refCount = 0;
		bool disableCommands = false;
		public MenuHandlerService(IDesignerHost host) {
			if(host == null) return;
			this.host = host;
			InstallService();
		}
		public static void InstallService(IDesignerHost host, AllowMenuCommandEventHandler handler) {
			if(host == null) return;
			MenuHandlerService service = host.GetService(typeof(MenuHandlerService)) as MenuHandlerService;
			if(service == null) {
				service = new MenuHandlerService(host);
				host.AddService(typeof(MenuHandlerService), service);
			}
			else {
				service.AddReference();
			}
			service.AllowMenuCommand += handler;
		}
		void AddReference() {
			refCount++;
		}
		public static void UnInstallService(IDesignerHost host, AllowMenuCommandEventHandler handler) {
			if(host == null) return;
			MenuHandlerService service = host.GetService(typeof(MenuHandlerService)) as MenuHandlerService;
			if(service != null) {
				service.Release(handler);
			}
		}
		protected internal void Release(AllowMenuCommandEventHandler handler) {
			if(handler != null)
				AllowMenuCommand -= handler;
			this.refCount--;
			if(this.refCount < 1) {
				UnInstallService();
				host.RemoveService(typeof(MenuHandlerService));
			}
		}
		public static void InstallService(IDesignerHost host) {
			InstallService(host, null);
		}
		public bool DisableCommands { 
			get { return disableCommands; }
			set { 
				disableCommands = value; 
			}
		}
		protected ISelectionService SelectionService { get { return selectionService; } }
		public IDesignerHost Host { get { return host; } }
		public IMenuCommandService MenuService { 
			get { 
				IMenuCommandService res = host.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
				if(res == null) return null;
				if(res.GetType().FullName == "System.Windows.Forms.Design.Behavior.BehaviorService+MenuCommandHandler") {
					PropertyDescriptor pd = TypeDescriptor.GetProperties(res)["MenuService"];
					if(pd != null && pd.GetValue(res) is IMenuCommandService) return pd.GetValue(res) as IMenuCommandService;
				}
				return res;
			} 
		}
		public IComponent RootComponent { get { return Host.RootComponent; } }
		protected void InstallService() {
			AddReference();
			this.selectionService = Host.GetService(typeof(ISelectionService)) as ISelectionService;
			foreach(CommandID cmd in commandsToDisable) ReplaceCommand(cmd);
		}
		protected void UnInstallService() {
			foreach(CommandID cmd in commandsToDisable) UnhookCommand(cmd);
		}
		void UnhookCommand(CommandID id) {
			WrappedMenuCommand command = GetCommand(id) as WrappedMenuCommand;
			if(command == null) return;
			MenuService.RemoveCommand(command);
			MenuService.AddCommand(command.OldCommand);;
		}
		CommandID[] commandsToDisable = new CommandID[] { 
			MenuCommands.KeyCancel, 
			MenuCommands.KeyDefaultAction,
			MenuCommands.KeyMoveDown,
			MenuCommands.KeyMoveLeft,
			MenuCommands.KeyMoveRight,
			MenuCommands.KeyMoveUp,
			MenuCommands.Delete,
			MenuCommands.Copy,
			MenuCommands.Cut,
			MenuCommands.Paste };
		MenuCommand GetCommand(CommandID command) {
			return MenuService.FindCommand(command);
		}
		void ReplaceCommand(CommandID id) {
			MenuCommand command = GetCommand(id);
			if(command == null) return;
			MenuService.RemoveCommand(command);
			if(GetCommand(id) == null)  
				MenuService.AddCommand(new WrappedMenuCommand(this, command));
		}
		public virtual bool CanInvokeCommand(WrappedMenuCommand command) {
			if(DisableCommands) return false;
			AllowMenuCommandEventArgs e = new AllowMenuCommandEventArgs(command, SelectionService);
			if(SelectionService != null) {
				IDXMenuCommandHandler handler = SelectionService.PrimarySelection as IDXMenuCommandHandler;
				if(handler == null && SelectionService.PrimarySelection != null) {
					handler = Host.GetDesigner(SelectionService.PrimarySelection as IComponent) as IDXMenuCommandHandler;
				}
				if(handler != null) handler.OnAllowInvokeCommand(e);
			}
			RaiseAllowMenuCommand(e);
			return e.Allow;
		}
		Control FindFocusedControl() {
			Form form = Form.ActiveForm;
			if(form == null || form.ActiveControl == null) {
				Control root = RootComponent as Control;
				if(root != null) {
					form = root.FindForm();
					if(form != null) root = form.ActiveControl;
					return FindFocusedChild(root) as Control;
				}
				return null;
			}
			return form.ActiveControl;
		}
		Control FindFocusedChild(Control control) {
			if(control == null) return null;
			if(control.HasChildren) {
				for(int n = 0; n < control.Controls.Count; n++) {
					Control fc = FindFocusedChild(control.Controls[n]);
					if(fc != null) return fc;
				}
				return null;
			}
			return control.Focused ? control : null;
		}
		protected virtual void RaiseAllowMenuCommand(AllowMenuCommandEventArgs e) {
			if(AllowMenuCommand != null) AllowMenuCommand(this, e);
		}
	}
	public interface IDXMenuCommandHandler {
		void OnAllowInvokeCommand(AllowMenuCommandEventArgs e);
	}
	public delegate void AllowMenuCommandEventHandler(object sender, AllowMenuCommandEventArgs e);
	public class AllowMenuCommandEventArgs : EventArgs {
		bool allow = true;
		WrappedMenuCommand wrappedCommand;
		ISelectionService selectionService;
		public AllowMenuCommandEventArgs(WrappedMenuCommand wrappedCommand, ISelectionService selectionService) {
			this.selectionService = selectionService;
			this.wrappedCommand = wrappedCommand;
		}
		public ISelectionService SelectionService { get { return selectionService; } }
		public CommandID Command { get { return WrappedCommand.CommandID; } }
		public bool Allow { get { return allow; } set { allow = value; } }
		public WrappedMenuCommand WrappedCommand { get { return wrappedCommand; } }
	}
	public class WrappedMenuCommand : MenuCommand {
		MenuCommand oldCommand;
		MenuHandlerService service;
		public WrappedMenuCommand(MenuHandlerService service, MenuCommand oldCommand) : base(null, oldCommand.CommandID) {
			this.service = service;
			this.oldCommand = oldCommand;
		}
		public override void Invoke() {
			if(!CanInvoke) return;
			OldCommand.Invoke();
		}
		public override int OleStatus {
			get {
				if(CanInvoke) return base.OleStatus;
				return 0;
			}
		}
#if DXWhidbey
		public override void Invoke(object args) {
				if(!CanInvoke) return;
				OldCommand.Invoke(args);
		}
#endif
		public bool CanInvoke { get { return Service.CanInvokeCommand(this); } }
		public MenuCommand OldCommand { get { return oldCommand; } }
		public MenuHandlerService Service { get { return service; } }
	}
}
