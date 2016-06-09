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
using System.Drawing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public abstract class BaseViewControllerCommand {
		protected BaseViewControllerCommand() { }
		public string Caption {
			get { return DocumentManagerLocalizer.GetString(ID); }
		}
		public Image Image {
			get { return Resources.CommandResourceLoader.GetCommandGlyph(ID); }
		}
		internal object Parameter { get; set; }
		protected abstract DocumentManagerStringId ID { get; }
		protected abstract void ExecuteCore(IBaseViewController controller, object parameter);
		protected abstract bool CanExecuteCore(object parameter);
		#region static
		protected static bool Check<T>(object parameter, out T bObj) where T : class, DevExpress.Utils.Base.IBaseObject {
			bObj = parameter as T;
			return bObj != null && !bObj.IsDisposing;
		}
		IBaseViewController controller;
		protected BaseView GetView() {
			return controller.View;
		}
		internal static bool CanExecute(IBaseViewController controller, Args e) {
			e.Command.controller = controller;
			try {
				return (controller != null) && e.Command.CanExecuteCore(e.Parameter);
			}
			finally { e.Command.controller = null; }
		}
		internal static void Execute(IBaseViewController controller, Args e) {
			e.Command.controller = controller;
			try {
				e.Command.ExecuteCore(controller, e.Parameter);
			}
			finally { e.Command.controller = null; }
		}
		public static readonly BaseViewControllerCommand Activate = new ActivateCommand();
		public static readonly BaseViewControllerCommand Close = new CloseCommand();
		public static readonly BaseViewControllerCommand CloseAll = new CloseAllCommand();
		public static readonly BaseViewControllerCommand CloseAllButThis = new CloseAllButThisCommand();
		public static readonly BaseViewControllerCommand Float = new FloatCommand();
		public static readonly BaseViewControllerCommand FloatAll = new FloatAllCommand();
		protected internal static readonly BaseViewControllerCommand ForceFloat = new ForceFloatCommand();
		protected internal static readonly BaseViewControllerCommand CloseAllDocumentsAndHosts = new CloseAllDocumentsAndHostsCommand();
		public static readonly BaseViewControllerCommand Dock = new DockCommand();
		public static readonly BaseViewControllerCommand ShowWindowsDialog = new ShowWindowsDialogCommand();
		public static readonly BaseViewControllerCommand ResetLayout = new ResetLayoutCommand();
		#endregion static
		internal class Args {
			object parameterCore;
			BaseViewControllerCommand commandCore;
			public Args(BaseViewControllerCommand command, object parameter) {
				commandCore = command;
				parameterCore = parameter;
			}
			public BaseViewControllerCommand Command { get { return commandCore; } }
			public object Parameter { get { return parameterCore; } }
		}
		#region Commands
		[CommandGroup("Common")]
		public class ActivateCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandActivate; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseDocument document;
				return Check(parameter, out document) && document.CanActivate();
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				BaseDocument document = parameter as BaseDocument;
				controller.Activate(document);
			}
		}
		[CommandGroup("Common", Order = 0, Index = 0)]
		public class CloseCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandClose; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseDocument document;
				return Check(parameter, out document) && document.Properties.CanClose;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				BaseDocument document = parameter as BaseDocument;
				controller.Close(document);
			}
		}
		[CommandGroup("Common", Order = 0, Index = 1)]
		public class CloseAllButThisCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandCloseAllButThis; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseDocument document;
				if(Check(parameter, out document)) {
					BaseView view = document.Manager.View;
					return (view != null) && view.CanCloseAllButThis(document);
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				BaseDocument document = parameter as BaseDocument;
				controller.CloseAllButThis(document);
			}
		}
		[CommandGroup("Additional", Order = 10, Visibility = CommandGroupVisibility.Always)]
		public class CloseAllCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandCloseAll; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseView view;
				if(!Check(parameter, out view)) {
					BaseDocument document;
					if(Check(parameter, out document))
						view = document.Manager.View;
				}
				return (view != null) && view.CanCloseAll();
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				controller.CloseAll();
			}
		}
		[CommandGroup("Additional", Order = 11, Visibility = CommandGroupVisibility.Always)]
		public class ResetLayoutCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandResetLayout; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseView view;
				if(!Check(parameter, out view)) {
					return false;
				}
				return (view != null) && view.AllowResetLayout != DevExpress.Utils.DefaultBoolean.False;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				controller.ResetLayout();
			}
		}
		[CommandGroup("Additional", Order = 12, Visibility = CommandGroupVisibility.Always)]
		public class ShowWindowsDialogCommand : BaseViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandOpenedWindowsDialog; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseView view;
				if(!Check(parameter, out view)) {
					BaseDocument document;
					if(Check(parameter, out document))
						view = document.Manager.View;
				}
				return (view != null) && (view.Documents.Count + view.FloatDocuments.Count > 0);
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				controller.ShowWindowsDialog();
			}
		}
		[CommandGroup("Additional", Order = 1, Index = 0, Visibility = CommandGroupVisibility.Always)]
		public class FloatCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandFloat; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseDocument document;
				return Check(parameter, out document) && document.CanFloat(GetView());
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				BaseDocument document = parameter as BaseDocument;
				BaseView view = GetView();
				if(view.OnBeginFloating(document, FloatingReason.ContextMenuAction)) {
					if(controller.Float(document))
						view.OnEndFloating(document, EndFloatingReason.ContextMenuAction);
				}
			}
		}
		[CommandGroup("Additional", Order = 1, Index = 1, Visibility = CommandGroupVisibility.Always)]
		public class FloatAllCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandFloatAll; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseView view;
				if(!Check(parameter, out view)) {
					BaseDocument document;
					if(Check(parameter, out document))
						view = document.Manager.View;
				}
				return (view != null) && view.CanFloatAll();
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				controller.FloatAll();
			}
		}
		protected class ForceFloatCommand : FloatCommand {
			protected override bool CanExecuteCore(object parameter) {
				BaseDocument document;
				return Check(parameter, out document) && document.CanFloat();
			}
		}
		protected class CloseAllDocumentsAndHostsCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandCloseAll; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseView view;
				if(!Check(parameter, out view)) {
					BaseDocument document;
					if(Check(parameter, out document))
						view = document.Manager.View;
				}
				return (view != null) && view.CanCloseAll();
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				controller.CloseAllDocumentsAndHosts();
			}
		}
		[CommandGroup("Additional", Order = 3, Visibility = CommandGroupVisibility.Always)]
		public class DockCommand : BaseViewControllerCommand {
			protected sealed override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandDock; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseDocument document;
				return Check(parameter, out document) && document.IsFloating;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				controller.Dock((BaseDocument)parameter);
			}
		}
		#endregion Commands
		#region CommandGroup
		public enum CommandGroupVisibility { ContextMenuOnly, Always }
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
		public sealed class CommandGroup : Attribute {
			public CommandGroup(string name) {
				nameCore = name;
				Order = -1;
				Index = -1;
			}
			string nameCore;
			public string Name {
				get { return nameCore; }
			}
			public int Order { get; set; }
			public int Index { get; set; }
			public CommandGroupVisibility Visibility { get; set; }
			public static readonly CommandGroup Unknown = new CommandGroup("Unknown");
		}
		public static CommandGroup GetCommandGroup(BaseViewControllerCommand command) {
			object[] attributes = command.GetType().GetCustomAttributes(
				typeof(BaseViewControllerCommand.CommandGroup), true);
			return (attributes.Length == 1) ? (CommandGroup)attributes[0] : CommandGroup.Unknown;
		}
		public static int Compare(BaseViewControllerCommand x, BaseViewControllerCommand y) {
			if(x == y) return 0;
			CommandGroup group1 = GetCommandGroup(x);
			CommandGroup group2 = GetCommandGroup(y);
			if(group1.Order == group2.Order)
				return group1.Index.CompareTo(group2.Index);
			else return group1.Order.CompareTo(group2.Order);
		}
		#endregion CommandGroup
	}
}
