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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Collections;
using DevExpress.Utils.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils;
using DevExpress.XtraReports.Design.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public abstract class XtraContextMenuBase : PopupMenu {
		#region static
		protected static ImageCollection imageCollection = new ImageCollection();
		protected static void SetBarItemVisibility(BarItem item, bool visible) {
			item.Visibility = visible ? BarItemVisibility.Always : BarItemVisibility.Never;
		}
		static int AddImageToImageList(string resource) {
			return AddImageToImageList(resource, typeof(DevExpress.Utils.UI.ResFinder));
		}
		protected static int AddImageToImageList(string resource, Type type) {
			Bitmap bitmap = ResLoaderBase.LoadBitmap("TlbrImages." + resource, type, Color.Magenta);
			return AddImageToImageList(bitmap);
		}
		protected static int AddImageToImageList(Image img) {
			return img != null ? imageCollection.Images.Add(img) : -1;
		}
		static XtraContextMenuBase() {
			imageCollection.TransparentColor = Color.Magenta;
		}
		#endregion
		#region inner classes
		[ToolboxItem(false)]
		public class PopupBarManager : BarManager {
			protected override void OnFormDisposed(object sender, EventArgs e) {
				return;
			}
			protected override void OnFormClosed(object sender, EventArgs e) {
				return;
			}
		}
		public class CommandMenuItemInvoker {
			private CommandID cmdID;
			private object[] parameters;
			XtraContextMenuBase owner;
			public CommandID CommandID {
				get { return cmdID; }
				set { cmdID = value; }
			}
			public object[] Params {
				get { return parameters; }
				set { parameters = value; }
			}
			public XtraContextMenuBase Owner {
				get { return owner; }
				set { owner = value; }
			}
			public void InvokeCommand() {
				if (owner.menuCommandServ == null) return;
				owner.menuCommandServ.GlobalInvoke(cmdID, parameters);
			}
		}
		public class CommandMenuItemBase : BarButtonItem, ISupportCommandParameters {
			protected CommandMenuItemInvoker invoker = new CommandMenuItemInvoker();
			CommandID ISupportCommand.CommandID {
				get { return invoker.CommandID; }
			}
			object[] ISupportCommandParameters.Parameters {
				get { return invoker.Params; }
				set { invoker.Params = value; }
			}
			protected override void OnClick(BarItemLink link) {
				base.OnClick(link);
				if (Enabled) invoker.InvokeCommand();
			}
			public CommandMenuItemBase(string text, XtraContextMenuBase owner, int imageIndex, CommandID cmdID, object param)
				: this(text, owner, imageIndex, cmdID) {
				invoker.Params = new object[] { param };
			}
			public CommandMenuItemBase(string text, XtraContextMenuBase owner, int imageIndex, CommandID cmdID)
				: base() {
				Caption = text;
				ImageIndex = imageIndex;
				invoker.Owner = owner;
				invoker.CommandID = cmdID;
			}
			public CommandMenuItemBase(string text, XtraContextMenuBase owner, CommandID cmdID)
				: this(text, owner, -1, cmdID) {
			}
		}
		public class CommandMenuCheckItem : BarCheckItem, ISupportCommandParameters {
			CommandMenuItemInvoker invoker = new CommandMenuItemInvoker();
			public CommandMenuCheckItem(string text, XtraContextMenuBase owner, int imageIndex, CommandID cmdID, object param)
				: this(text, owner, imageIndex, cmdID) {
					invoker.Params = new object[] { param };
			}
			public CommandMenuCheckItem(string text, XtraContextMenuBase owner, int imageIndex, CommandID cmdID) {
				Caption = text;
				ImageIndex = imageIndex;
				invoker.Owner = owner;
				invoker.CommandID = cmdID;
			}
			public CommandMenuCheckItem(string text, XtraContextMenuBase owner, CommandID cmdID)
				: this(text, owner, -1, cmdID) {
			}
			CommandID ISupportCommand.CommandID { get { return invoker.CommandID; } }
			object[] ISupportCommandParameters.Parameters {
				get { return invoker.Params; }
				set { invoker.Params = value; }
			}
			protected override void OnClick(BarItemLink link) {
				base.OnClick(link);
				if (Enabled) invoker.InvokeCommand();
			}
		}
#endregion
		protected IMenuCommandServiceEx menuCommandServ;
		protected MenuCommandHandlerBase menuCommandHandler;
		protected bool separator;
		protected PopupBarManager barManager = new PopupBarManager();
		protected BarAndDockingController controller = new BarAndDockingController();
		public XtraContextMenuBase() {
			barManager = new PopupBarManager();
			controller = new BarAndDockingController();
			barManager.Images = imageCollection;
			barManager.Controller = controller;
			Manager = barManager;
		}
		protected void ApplySeparator(BarItemLink link) {
			if (separator) {
				link.BeginGroup = true;
				separator = false;
			}
		}
		protected virtual void UpdateItemState(BarLinksHolder barLinksHolder, IServiceProvider provider) {
			if (barLinksHolder == null) return;
			bool visibleItemExists = false;
			bool enabledItemExists = false;
			foreach (BarItemLink link in barLinksHolder.ItemLinks) {
				if (!(link.Item is ISupportCommand)) { continue; }
				UpdateItemVisibility(link.Item);
				UpdateItemState(link.Item as BarLinksHolder, provider);
				visibleItemExists |= link.Item.Visibility == BarItemVisibility.Always;
				enabledItemExists |= link.Item.Enabled;
			}
			if (barLinksHolder is BarItem) {
				SetBarItemVisibility((BarItem)barLinksHolder, visibleItemExists);
				((BarItem)barLinksHolder).Enabled = enabledItemExists;
			}
		}
		protected void UpdateItemVisibility(BarItem item) {
			CommandID command = ((ISupportCommand)item).CommandID;
			if (command != null) {
				MenuCommand menuCommand = menuCommandServ.FindCommand(command);
				SetBarItemVisibility(item, menuCommand != null && menuCommand.Supported);
				item.Enabled = (menuCommand != null && menuCommand.Supported && menuCommand.Enabled);
			}
		}
		protected abstract UserLookAndFeel GetLookAndFeel(IServiceProvider provider);
		protected abstract Control GetBarManagerForm(IServiceProvider provider);
		protected abstract MenuCommandHandlerBase GetMenuCommandHandler(IServiceProvider serviceProvider);
		public virtual void Show(Control ctl, Point pos, IServiceProvider provider) {
			if (ctl == null || !ctl.Visible)
				return;
			menuCommandServ = provider.GetService(typeof(IMenuCommandService)) as IMenuCommandServiceEx;
			System.Diagnostics.Debug.Assert(menuCommandServ != null);
			menuCommandHandler = GetMenuCommandHandler(provider);
			if (menuCommandHandler != null)
				menuCommandHandler.UpdateCommandStatus();
			UpdateItemState(this, provider);
			barManager.Form = GetBarManagerForm(provider);
			controller.LookAndFeel.ParentLookAndFeel = GetLookAndFeel(provider);
			ShowPopup(pos);
		}
		public void AddMenuItems(IList<MenuItemDescription> items, BarLinksHolder parentItem, Action<BarLinksHolder> callback) {
			foreach(MenuItemDescription item in items) {
				if(item.IsContainer) {
					BarLinksHolder holder = AddMenuSubItem(item.Text, item.CommandID);
					callback(holder);
					AddMenuItems(item.SubItems, holder, callback);
				} else if(item.IsEmpty)
					AddSeparator();
				else
					AddMenuItem(CreateCommandMenuItem(item), parentItem ?? this);
			}
		}
		public BarLinksHolder AddMenuSubItem(string text, CommandID cmdID) {
			BarSubItem item = CreateMenuSubItem(text, -1, cmdID);
			ApplySeparator(AddItem(item));
			return item;
		}
		public void AddMenuItem(string text, int imageIndex, BarLinksHolder parentItem, CommandID cmdID, object param) {
			AddMenuItem(text, imageIndex, parentItem, cmdID, param, false, false);
		}
		public void AddMenuItem(string text, int imageIndex, BarLinksHolder parentItem, CommandID cmdID, object param, bool isBoolean, bool isChecked) {
			BarItem item = CreateCommandMenuItem(text, imageIndex, cmdID, param, isBoolean, isChecked);
			AddMenuItem(item, parentItem);
		}
		protected void AddMenuItem(BarItem item, BarLinksHolder parentItem) {
			ApplySeparator(parentItem.AddItem(item));
		}
		public void AddMenuItem(string text, string resource, BarLinksHolder parentItem, CommandID cmdID, object param) {
			int imageIndex = AddImageToImageList(resource);
			AddMenuItem(text, imageIndex, parentItem, cmdID, param);
		}
		public void AddMenuItem(string text, Image image, BarLinksHolder parentItem, CommandID cmdID, object param) {
			AddMenuItem(text, image, parentItem, cmdID, param, false, false);
		}
		public void AddMenuItem(string text, Image image, BarLinksHolder parentItem, CommandID cmdID, object param, bool isBoolean, bool isChecked) {
			int imageIndex = AddImageToImageList(image);
			AddMenuItem(text, imageIndex, parentItem, cmdID, param, isBoolean, isChecked);
		}
		public void AddSeparator() {
			separator = true;
		}
		protected virtual BarSubItem CreateMenuSubItem(string text, int imageIndex, CommandID cmdID) {
			BarSubItem subItem = new BarSubItem();
			subItem.Caption = text;
			subItem.ImageIndex = imageIndex;
			return subItem;
		}
		protected BarItem CreateCommandMenuItem(MenuItemDescription item) {
			int imageIndex = AddImageToImageList(item.Image);
			return CreateCommandMenuItem(item.Text, imageIndex, item.CommandID, item.Parameter, item.IsBoolean, item.Checked);
		}
		protected virtual BarItem CreateCommandMenuItem(string text, int imageIndex, CommandID cmdID, object param, bool isBoolean, bool isChecked) {
			if (isBoolean)
				return new CommandMenuCheckItem(text, this, imageIndex, cmdID, param) { Checked = isChecked };
			return CreateCommandMenuItem(text, imageIndex, cmdID, param);
		}
		protected virtual CommandMenuItemBase CreateCommandMenuItem(string text, int imageIndex, CommandID cmdID, object param) {
			return new CommandMenuItemBase(text, this, imageIndex, cmdID, param);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				menuCommandServ = null;
				menuCommandHandler = null;
				if(controller != null) {
					controller.Dispose();
					controller = null;
				}
				if(barManager != null) {
					barManager.Dispose();
					barManager = null;
				}
			} 
			base.Dispose(disposing);
		}
	}
}
namespace DevExpress.XtraReports.Design {
	public class MenuItemDescription {
		public static MenuItemDescription Separator = new MenuItemDescription();
		public CommandID CommandID { get; private set; }
		public Image Image { get; private set; }
		public string Text { get; private set; }
		public MenuItemDescription[] SubItems { get; private set; }
		public object Parameter { get; private set; }
		public bool IsBoolean { get; private set; }
		public bool Checked { get; private set; }
		public bool IsEmpty {
			get { return CommandID == null && string.IsNullOrEmpty(Text) && SubItems == null; }
		}
		public bool IsContainer {
			get { return SubItems != null; }
		}
		public MenuItemDescription(string text, Image image, CommandID commandID) :
			this(null, text, image, commandID) {
		}
		public MenuItemDescription(object parameter, string text, Image image, CommandID commandID) :
			this(parameter, text, image, commandID, false, false) {
		}
		public MenuItemDescription(object parameter, string text, Image image, CommandID commandID, bool isBoolean, bool isChecked) {
			Parameter = parameter;
			Text = text;
			Image = image;
			CommandID = commandID;
			IsBoolean = isBoolean;
			Checked = isChecked;
		}
		public MenuItemDescription(string text, Image image, CommandID commandID, MenuItemDescription[] subItems) {
			Text = text;
			Image = image;
			CommandID = commandID;
			SubItems = subItems;
		}
		public MenuItemDescription() {
		}
	}
	public class MenuItemDescriptionCollection : Collection<MenuItemDescription> {
		public MenuItemDescription this[CommandID commandID] {
			get {
				int index = IndexOf(commandID);
				return index >= 0 ? this[index] : null;
			}
		}
		public MenuItemDescriptionCollection() {
		}
		public MenuItemDescriptionCollection(IList<MenuItemDescription> items)
			: base(items) { 
		}
		public void Remove(CommandID commandID) {
			int index = IndexOf(commandID);
			if(index >= 0) RemoveAt(index);
		}
		public int IndexOf(CommandID commandID) {
			for(int i = 0; i < Count; i++)
				if(object.Equals(this[i].CommandID, commandID))
					return i;
			return -1;
		}
		public bool Contains(CommandID commandID) {
			return IndexOf(commandID) >= 0;
		}
		public MenuItemDescription[] ToArray() {
			MenuItemDescription[] array = new MenuItemDescription[Count];
			CopyTo(array, 0);
			return array;
		}
	}
}
