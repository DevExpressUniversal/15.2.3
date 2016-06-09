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
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking.Controller;
namespace DevExpress.XtraBars.Docking {
	[ToolboxItem(false)]
	public class DockControllerMenu : DXPopupMenu {
		IDockController controllerCore;
		public DockControllerMenu(IDockController controller)
			: base() {
			this.controllerCore = controller;
		}
		public IDockController Controller {
			get { return controllerCore; }
		}
		Control palcementTargetCore;
		public Control PlacementTarget {
			get { return palcementTargetCore; }
			set { palcementTargetCore = value; }
		}
		public void Init(AutoHideContainer container) {
			ClearBeforeInit();
			foreach(Helpers.DockLayout layout in container.AutoHideInfo) {
				if(!layout.HasChildren)
					CreateActivationMenuItem(layout);
				else {
					foreach(Helpers.DockLayout buttonItem in layout) {
						CreateActivationMenuItem(buttonItem);
					}
				}
			}
		}
		protected void CreateActivationMenuItem(Helpers.DockLayout layout) {
			DXMenuCheckItem item = CreateMenuCheckItem(layout.Panel, DockControllerCommand.Activate);
			if(layout.Panel == layout.DockManager.ActivePanel)
				item.Checked = true;
			Items.Add(item);
		}
		public void Init(DockPanel panel) {
			ClearBeforeInit();
			foreach(DockControllerCommand command in Controller.GetCommands(panel)) {
				Items.Add(CreateMenuItem((object)panel, command));
			}
		}
		public void InitBarDocumentListItem(DockPanel panel, BarDockingMenuItem documentListItem) {
			ClearBeforeInit();
			foreach(DockControllerCommand command in Controller.GetCommands(panel)) {
				documentListItem.CommandList.Add(CreateMenuItem((object)panel, command, documentListItem));
			}
		}
		protected void ClearBeforeInit() {
			name = null;
			Items.Clear();
		}
		string name;
		protected DXMenuCheckItem CreateMenuCheckItem(DockPanel panel, DockControllerCommand command) {
			DockControllerCommand.Args args = new DockControllerCommand.Args(command, panel);
			DXMenuCheckItem item = new DXMenuCheckItem(panel.Text);
			item.Image = panel.Image; 
			DockControllerCommand.CommandGroup currentGroup = DockControllerCommand.GetCommandGroup(command);
			if(currentGroup.Name != name) {
				if(!string.IsNullOrEmpty(name))
					item.BeginGroup = true;
				name = currentGroup.Name;
			}
			item.Tag = new DockControllerMenuTag(Controller, args);
			item.Enabled = DockControllerCommand.CanExecute(Controller, args);
			item.Click += Execute;
			return item;
		}
		protected BarInMdiChildrenListItem CreateMenuItem(object parameter, DockControllerCommand command, BarDockingMenuItem listItem) {
			BarInMdiChildrenListItem item = new BarInMdiChildrenListItem(listItem, command.Caption);
			AssingItemProperties(parameter, command, item);
			return item;
		}
		protected DXMenuItem CreateMenuItem(object parameter, DockControllerCommand command) {
			Docking2010.Customization.CommandMenuItem item = new Docking2010.Customization.CommandMenuItem(command.Caption);
			AssingItemProperties(parameter, command, item);
			item.Click += Execute;
			return item;
		}
		void AssingItemProperties(object parameter, DockControllerCommand command, Docking2010.Customization.ICommandItem item) {
			DockControllerCommand.Args args = new DockControllerCommand.Args(command, parameter);
			item.Image = command.Image;
			DockControllerCommand.CommandGroup currentGroup = DockControllerCommand.GetCommandGroup(command);
			if(currentGroup.Name != name) {
				if(!string.IsNullOrEmpty(name))
					item.BeginGroup = true;
				name = currentGroup.Name;
			}
			item.Tag = new DockControllerMenuTag(Controller, args);
			item.Enabled = DockControllerCommand.CanExecute(Controller, args);
		}
		static void Execute(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			DockControllerMenuTag tag = (DockControllerMenuTag)item.Tag;
			DockControllerCommand.Execute(tag.Controller, tag.Args);
		}
		internal class DockControllerMenuTag {
			IDockController controllerCore;
			DockControllerCommand.Args argsCore;
			public DockControllerMenuTag(IDockController controller, DockControllerCommand.Args args) {
				controllerCore = controller;
				argsCore = args;
			}
			public IDockController Controller {
				get { return controllerCore; }
			}
			public DockControllerCommand.Args Args {
				get { return argsCore; }
			}
		}
		public void Add(DockControllerCommand command, object commandParameter) {
			Items.Add(CreateMenuItem(commandParameter, command));
		}
		public void Insert(int index, DockControllerCommand command, object commandParameter) {
			Items.Insert(index, CreateMenuItem(commandParameter, command));
		}
		public void Remove(DockControllerCommand command) {
			DXMenuItem item = Find(command);
			if(item != null) Items.Remove(item);
		}
		public void Remove(DockControllerCommand command, object commandParameter) {
			DXMenuItem item = Find(command, commandParameter);
			if(item != null) Items.Remove(item);
		}
		public DXMenuItem Find(DockControllerCommand command) {
			return FindItem(Items, (args) => { return args.Command == command; });
		}
		public DXMenuItem Find(DockControllerCommand command, object commandParameter) {
			return FindItem(Items, (args) =>
			{
				return args.Command == command && object.Equals(args.Parameter, commandParameter);
			});
		}
		static DXMenuItem FindItem(IEnumerable items,  Predicate<DockControllerCommand.Args> filter) {
			foreach(DXMenuItem item in items) {
				DockControllerMenuTag tag = item.Tag as DockControllerMenuTag;
				if(tag != null && filter(tag.Args))
					return item;
			}
			return null;
		}
	}
}
