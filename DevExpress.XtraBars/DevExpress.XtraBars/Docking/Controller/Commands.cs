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
namespace DevExpress.XtraBars.Docking.Controller {
	public abstract class DockControllerCommand {
		protected DockControllerCommand() { }
		public string Caption {
			get { return DockManagerLocalizer.GetString(ID); }
		}
		public Image Image {
			get {
				return null;
			}
		}
		protected abstract DockManagerStringId ID { get; }
		protected abstract void ExecuteCore(IDockController controller, object parameter);
		protected abstract bool CanExecuteCore(object parameter);
		#region static
		protected static bool Check(object parameter, out DockPanel panel) {
			panel = parameter as DockPanel;
			return (panel != null) && !panel.IsDisposing && !panel.IsDisposed && (panel.Visibility != DockVisibility.Hidden);
		}
		internal static bool CanExecute(IDockController controller, Args e) {
			return (controller != null) && e.Command.CanExecuteCore(e.Parameter);
		}
		internal static void Execute(IDockController controller, Args e) {
			e.Command.ExecuteCore(controller, e.Parameter);
		}
		public static readonly DockControllerCommand Activate = new ActivateCommand();
		public static readonly DockControllerCommand Float = new FloatCommand();
		public static readonly DockControllerCommand Dock = new DockCommand();
		public static readonly DockControllerCommand DockAsTabbedDocument = new DockAsTabbedDocumentCommand();
		public static readonly DockControllerCommand AutoHide = new AutoHideCommand();
		public static readonly DockControllerCommand Close = new CloseCommand();
		#endregion static
		internal class Args {
			object parameterCore;
			DockControllerCommand commandCore;
			public Args(DockControllerCommand command, object parameter) {
				commandCore = command;
				parameterCore = parameter;
			}
			public DockControllerCommand Command { get { return commandCore; } }
			public object Parameter { get { return parameterCore; } }
		}
		#region Commands
		[CommandGroup("Additional")]
		public class ActivateCommand : DockControllerCommand {
			protected override DockManagerStringId ID {
				get { return DockManagerStringId.CommandActivate; }
			}
			protected override bool CanExecuteCore(object parameter) {
				DockPanel panel;
				return Check(parameter, out panel) && panel.CanActivate;
			}
			protected override void ExecuteCore(IDockController controller, object parameter) {
				DockPanel panel = parameter as DockPanel;
				controller.Activate(panel);
			}
		}
		[CommandGroup("Common", Order = 0)]
		public class FloatCommand : DockControllerCommand {
			protected override DockManagerStringId ID {
				get { return DockManagerStringId.CommandFloat; }
			}
			protected override bool CanExecuteCore(object parameter) {
				DockPanel panel;
				return Check(parameter, out panel) && 
					(panel.Dock != DockingStyle.Float) && panel.Options.AllowFloating;
			}
			protected override void ExecuteCore(IDockController controller, object parameter) {
				DockPanel panel = parameter as DockPanel;
				controller.Float(panel);
			}
		}
		[CommandGroup("Common", Order = 1)]
		public class DockCommand : DockControllerCommand {
			protected override DockManagerStringId ID {
				get { return DockManagerStringId.CommandDock; }
			}
			protected override bool CanExecuteCore(object parameter) {
				DockPanel panel;
				return Check(parameter, out panel) && !panel.IsMdiDocument &&
					(panel.Dock == DockingStyle.Float || panel.Visibility == DockVisibility.AutoHide);
			}
			protected override void ExecuteCore(IDockController controller, object parameter) {
				DockPanel panel = parameter as DockPanel;
				controller.Dock(panel);
			}
		}
		[CommandGroup("Common", Order = 2)]
		public class DockAsTabbedDocumentCommand : DockControllerCommand {
			protected override DockManagerStringId ID {
				get { return DockManagerStringId.CommandDockAsTabbedDocument; }
			}
			protected override bool CanExecuteCore(object parameter) {
				DockPanel panel;
				return Check(parameter, out panel) && !panel.IsMdiDocument && panel.Options.AllowDockAsTabbedDocument;
			}
			protected override void ExecuteCore(IDockController controller, object parameter) {
				DockPanel panel = parameter as DockPanel;
				controller.DockAsTabbedDocument(panel);
			}
		}
		[CommandGroup("Common", Order = 3)]
		public class AutoHideCommand : DockControllerCommand {
			protected override DockManagerStringId ID {
				get { return DockManagerStringId.CommandAutoHide; }
			}
			protected override bool CanExecuteCore(object parameter) {
				DockPanel panel;
				parameter = GetParentPanel(parameter as DockPanel) ?? parameter;
				return Check(parameter, out panel) && (panel.Visibility != DockVisibility.AutoHide) &&
					(panel.Dock != DockingStyle.Float);
			}
			protected override void ExecuteCore(IDockController controller, object parameter) {
				DockPanel panel = GetParentPanel(parameter as DockPanel) ?? parameter as DockPanel;
				controller.AutoHide(panel);
			}
			DockPanel GetParentPanel(DockPanel panel) {
				if(panel == null) return null;
				if(panel.Tabbed) return panel;
				if(panel.ParentPanel == null) return null;
				return GetParentPanel(panel.ParentPanel);
			}
		}
		[CommandGroup("Common", Order = 4)]
		public class CloseCommand : DockControllerCommand {
			protected override DockManagerStringId ID {
				get { return DockManagerStringId.CommandClose; }
			}
			protected override bool CanExecuteCore(object parameter) {
				DockPanel panel;
				return Check(parameter, out panel);
			}
			protected override void ExecuteCore(IDockController controller, object parameter) {
				DockPanel panel = parameter as DockPanel;
				controller.Close(panel);
			}
		}
		#endregion Commands
		#region CommandGroup
		[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
		public sealed class CommandGroup : Attribute {
			public CommandGroup(string name) {
				nameCore = name;
			}
			string nameCore;
			public string Name {
				get { return nameCore; }
			}
			int orderCore = -1;
			public int Order {
				get { return orderCore; }
				set { orderCore = value; }
			}
			int indexCore = -1;
			public int Index {
				get { return indexCore; }
				set { indexCore = value; }
			}
			public static readonly CommandGroup Unknown = new CommandGroup("Unknown");
		}
		public static CommandGroup GetCommandGroup(DockControllerCommand command) {
			object[] attributes = command.GetType().GetCustomAttributes(
				typeof(DockControllerCommand.CommandGroup), true);
			return (attributes.Length == 1) ? (CommandGroup)attributes[0] : CommandGroup.Unknown;
		}
		public static int Compare(DockControllerCommand x, DockControllerCommand y) {
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
