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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Customization {
	[ToolboxItem(false)]
	public class BaseViewControllerMenu : DXPopupMenu {
		IBaseViewController controllerCore;
		public BaseViewControllerMenu(IBaseViewController controller)
			: base() {
			this.controllerCore = controller;
		}
		public IBaseViewController Controller {
			get { return controllerCore; }
		}
		Control palcementTargetCore;
		public Control PlacementTarget {
			get { return palcementTargetCore; }
			set { palcementTargetCore = value; }
		}
		public void Init(BaseDocument document) {
			ClearBeforeInit();
			foreach(BaseViewControllerCommand command in Controller.GetCommands(document)) {
				Items.Add(CreateMenuItem((object)document, command));
			}
		}
		public virtual void Init(BaseView view) {
			ClearBeforeInit();
			foreach(BaseViewControllerCommand command in Controller.GetCommands(view)) {
				Items.Add(CreateMenuItem((object)view, command));
			}
		}
		public virtual void Init(BarDockingMenuItem listItem, IEnumerable<BaseViewControllerCommand> commands) {
			ClearBeforeInit();
			foreach(BaseViewControllerCommand command in commands) {
				listItem.CommandList.Add(CreateMenuItem(command, listItem));
			}
		}
		protected void ClearBeforeInit() {
			name = null;
			Items.Clear();
		}
		string name;
		protected DXMenuCheckItem CreateMenuCheckItem(BaseDocument document, BaseViewControllerCommand command) {
			BaseViewControllerCommand.Args args = new BaseViewControllerCommand.Args(command, document);
			DXMenuCheckItem item = new DXMenuCheckItem(document.Caption);
			item.Image = document.Image ?? DevExpress.Utils.ImageCollection.GetImageListImage(Controller.Manager.Images, document.ImageIndex);
			BaseViewControllerCommand.CommandGroup currentGroup = BaseViewControllerCommand.GetCommandGroup(command);
			if(currentGroup.Name != name) {
				if(!string.IsNullOrEmpty(name))
					item.BeginGroup = true;
				name = currentGroup.Name;
			}
			item.AllowGlyphSkinning = document.Properties.CanUseGlyphSkinning;
			item.Tag = new BaseViewControllerMenuTag(Controller, args);
			item.Enabled = BaseViewControllerCommand.CanExecute(Controller, args);
			item.Click += Execute;
			return item;
		}
		protected DXMenuItem CreateMenuItem(object parameter, BaseViewControllerCommand command) {
			CommandMenuItem item = new CommandMenuItem(command.Caption);
			AssingItemProperties(item, command, parameter);
			item.Click += Execute;
			return item;
		}
		protected BarInMdiChildrenListItem CreateMenuItem(BaseViewControllerCommand command, BarDockingMenuItem listItem) {
			BarInMdiChildrenListItem item = new BarInMdiChildrenListItem(listItem, command.Caption);
			AssingItemProperties(item, command, null);
			return item;
		}
		protected void AssingItemProperties(ICommandItem item, BaseViewControllerCommand command, object parameter) {
			BaseViewControllerCommand.Args args = new BaseViewControllerCommand.Args(command, command.Parameter ?? parameter);
			item.Image = command.Image;
			BaseViewControllerCommand.CommandGroup commandGroup = BaseViewControllerCommand.GetCommandGroup(command);
			if(commandGroup.Name != name) {
				if(!string.IsNullOrEmpty(name))
					item.BeginGroup = true;
				name = commandGroup.Name;
			}
			if(item.UseCommandGroupVisibility)
				item.Visible = (commandGroup.Visibility == BaseViewControllerCommand.CommandGroupVisibility.Always);
			IBaseViewController actualController = CheckActualController(args);
			item.Tag = new BaseViewControllerMenuTag(actualController, args);
			item.Enabled = BaseViewControllerCommand.CanExecute(actualController, args);
			command.Parameter = null;
		}
		IBaseViewController CheckActualController(BaseViewControllerCommand.Args args) {
			IBaseViewController actualController = Controller;
			BaseDocument document = args.Parameter as BaseDocument;
			if(document != null) {
				BaseView actualView = Controller.GetView(document);
				if(actualView != null)
					actualController = actualView.Controller;
			}
			return actualController;
		}
		static void Execute(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			BaseViewControllerMenuTag tag = (BaseViewControllerMenuTag)item.Tag;
			BaseViewControllerCommand.Execute(tag.Controller, tag.Args);
		}
		internal class BaseViewControllerMenuTag {
			IBaseViewController controllerCore;
			BaseViewControllerCommand.Args argsCore;
			public BaseViewControllerMenuTag(IBaseViewController controller, BaseViewControllerCommand.Args args) {
				controllerCore = controller;
				argsCore = args;
			}
			public IBaseViewController Controller {
				get { return controllerCore; }
			}
			public BaseViewControllerCommand.Args Args {
				get { return argsCore; }
			}
		}
		public void Add(BaseViewControllerCommand command, object commandParameter) {
			Items.Add(CreateMenuItem(commandParameter, command));
		}
		public void Insert(int index, BaseViewControllerCommand command, object commandParameter) {
			Items.Insert(index, CreateMenuItem(commandParameter, command));
		}
		public void Remove(BaseViewControllerCommand command) {
			DXMenuItem item = Find(command);
			if(item != null) Items.Remove(item);
		}
		public void Remove(BaseViewControllerCommand command, object commandParameter) {
			DXMenuItem item = Find(command, commandParameter);
			if(item != null) Items.Remove(item);
		}
		public DXMenuItem Find(BaseViewControllerCommand command) {
			return FindItem(Items, (args) => { return args.Command == command; });
		}
		public DXMenuItem Find(BaseViewControllerCommand command, object commandParameter) {
			return FindItem(Items, (args) =>
			{
				return args.Command == command && object.Equals(args.Parameter, commandParameter);
			});
		}
		static DXMenuItem FindItem(IEnumerable items, Predicate<BaseViewControllerCommand.Args> filter) {
			foreach(DXMenuItem item in items) {
				BaseViewControllerMenuTag tag = item.Tag as BaseViewControllerMenuTag;
				if(tag != null && filter(tag.Args))
					return item;
			}
			return null;
		}
	}
	[ToolboxItem(false)]
	public class TabbedViewControllerMenu : BaseViewControllerMenu {
		public TabbedViewControllerMenu(Views.Tabbed.ITabbedViewController controller)
			: base(controller) {
		}
		public void Init(Views.Tabbed.DocumentGroup group) {
			ClearBeforeInit();
			BaseDocument[] documents = group.Items.ToArray();
			Array.Sort(documents, SortAlphabetically);
			foreach(Views.Tabbed.Document document in documents) {
				DXMenuCheckItem item = CreateMenuCheckItem(document, BaseViewControllerCommand.Activate);
				item.Enabled = document.IsEnabled;
				item.Visible = document.IsVisible;
				item.Checked = document.IsSelected;
				Items.Add(item);
			}
		}
		static int SortAlphabetically(BaseDocument document1, BaseDocument document2) {
			if(document1 == document2) return 0;
			string caption1 = document1.Caption;
			string caption2 = document2.Caption;
			return string.Compare(caption1, caption2, true);
		}
	}
	[ToolboxItem(false)]
	public class NativeMdiViewControllerMenu : BaseViewControllerMenu {
		public NativeMdiViewControllerMenu(Views.NativeMdi.INativeMdiViewController controller)
			: base(controller) {
		}
	}
	public interface ICommandItem {
		string Caption { get; set; }
		Image Image { get; set; }
		bool BeginGroup { get; set; }
		object Tag { get; set; }
		bool Enabled { get; set; }
		bool UseCommandGroupVisibility { get; }
		bool Visible { get; set; }
	}
	public class CommandMenuItem : DXMenuItem, ICommandItem {
		public CommandMenuItem(string caption)
			: base(caption) {
		}
		public bool UseCommandGroupVisibility {
			get { return false; }
		}
	}
}
