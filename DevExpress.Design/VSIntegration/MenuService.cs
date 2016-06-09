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
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using DevExpress.Utils.Design;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
namespace DevExpress.Design.VSIntegration {
	internal class ResFinder { 
	}
	public class VSMenuService {
		static VSMenuService activeService;
		IServiceProvider serviceProvider;
		CommandBarPopup menu;
		string menuName;
		List<VSMenuItem> menuItems = new List<VSMenuItem>();
		DTE DTE {
			get { return serviceProvider.GetService(typeof(DTE)) as DTE; }
		}
		CommandBar MenuBar {
			get { return ((CommandBars)DTE.CommandBars)["MenuBar"]; }
		}
		public bool IsActive {
			get { return activeService == this; }
		}
		protected string MenuName { get { return menuName;} }
		public VSMenuService(string menuName, IServiceProvider serviceProvider) {
			this.menuName = menuName;
			this.serviceProvider = serviceProvider;
		}
		public void RegisterMenuItem(VSMenuItem menuItem) {
			menuItems.Add(menuItem);
			menuItem.SetService(this);
		}
		public void Activate() {
 			InitMenu();
			InitMenuItems();
			menu.Visible = true;
			activeService = this;
		}
		void InitMenu() {
			if(menu == null)
				menu = FindMenu();
			if(menu == null)
				menu = CreateMenu();
		}
		void InitMenuItems(){
			Dictionary<string, CommandBarButton> unusedButtons = new Dictionary<string, CommandBarButton>();
			foreach(CommandBarControl control in menu.Controls) {
				CommandBarButton button = control as CommandBarButton;
				if(button != null)
					unusedButtons.Add(button.Tag, button);
			}
			foreach(VSMenuItem menuItem in menuItems) {
				menuItem.Init(menu.Controls);
				unusedButtons.Remove(menuItem.Button.Tag);
				menuItem.Button.Visible = true;
			}
			foreach(string buttonTag in unusedButtons.Keys)
				unusedButtons[buttonTag].Visible = false;
		}
		public void Deactivate() {
			if(menu != null)
				menu.Visible = false;
			activeService = null;
		}
		public void Close() {
			if(menu == null) return;
			if(menuItems != null)
				foreach(VSMenuItem menuItem in menuItems)
					menuItem.Delete();
			menu.Delete(false);
		}
		public void Dispose() {
			if(menuItems != null) {
				foreach(VSMenuItem menuItem in menuItems)
					menuItem.Dispose();
				menuItems = null;
			}
		}
		[CLSCompliant(false)]
		protected virtual CommandBarPopup CreateMenu() {
			CommandBar menuBarCommandBar = MenuBar;
			CommandBarControl viewControl = null;
			try {
				viewControl = menuBarCommandBar.Controls[GetViewMenuName()];
			} catch { }
			CommandBarPopup popupMenu = (CommandBarPopup)menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, 1, null, (viewControl != null ? (object)(viewControl.Index + 1) : Missing.Value), true);
			string menuName = MenuName;
			popupMenu.Caption = menuName;
			popupMenu.Tag = menuName;
			return popupMenu;
		}
		string GetViewMenuName() {
			string viewMenuName;
			try {
				ResourceManager resourceManager = new ResourceManager(typeof(ResFinder).Namespace + ".CommandBar", Assembly.GetExecutingAssembly());
				CultureInfo cultureInfo = new CultureInfo(DTE.LocaleID);
				string resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "View");
				viewMenuName = resourceManager.GetString(resourceName);
			} catch {
				viewMenuName = "View";
			}
			return viewMenuName;
		}
		CommandBarPopup FindMenu() {
			return (CommandBarPopup)MenuBar.FindControl(MsoControlType.msoControlPopup, Missing.Value, MenuName, Missing.Value, Missing.Value);
		}
	}
	public abstract class VSMenuItem : IDisposable {
		static CommandBarButton CreateCommandBarButton(CommandBarControls parentCollection, string caption, Bitmap picture, MsoButtonStyle buttonStyle, object insertPosition) {
			CommandBarButton button = parentCollection.Add(MsoControlType.msoControlButton, 1, null, insertPosition, false) as CommandBarButton;
			button.Caption = caption;
			button.Style = buttonStyle;
			if(picture != null)
				button.Picture = MenuItemPictureHelper.ConvertImageToPicture(picture);
			return button;
		}
		string caption;
		string bitmapResourceName;
		CommandBarButton button;
		VSMenuService menuService;
		[CLSCompliant(false)]
		public CommandBarButton Button {
			get { return button; }
		}
		protected abstract Type ResFinderType { get; }
		public VSMenuItem(string caption, string bitmapResourceName) {
			this.caption = caption;
			this.bitmapResourceName = bitmapResourceName;
		}
		[CLSCompliant(false)]
		public void Init(CommandBarControls parentCollection) {
			if(button != null) return;
			button = parentCollection.Parent.FindControl(MsoControlType.msoControlButton, Missing.Value, caption, Missing.Value, Missing.Value) as CommandBarButton;
			if(button == null)
				Create(parentCollection);
			button.Click += new _CommandBarButtonEvents_ClickEventHandler(ButtonClickHandler);
		}
		[CLSCompliant(false)]
		protected virtual void Create(CommandBarControls parentCollection) {
			Bitmap bitmap = null;
			if(!string.IsNullOrEmpty(bitmapResourceName)) {
				bitmap = DevExpress.Utils.ResourceImageHelper.CreateBitmapFromResources(bitmapResourceName, ResFinderType);
				bitmap.MakeTransparent();
			}
			button = CreateCommandBarButton(parentCollection, caption, bitmap, MsoButtonStyle.msoButtonIconAndCaption, Missing.Value);
			button.Tag = caption;
		}
		public void Delete() {
			button.Delete(false);
		}
		public void SetService(VSMenuService menuService) {
			this.menuService = menuService;
		}
		void ButtonClickHandler(CommandBarButton ctrl, ref bool cancelDefault) {
			if(menuService.IsActive)
				OnButtonClick();
		}
		protected abstract void OnButtonClick();
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing && button != null) {
				try {
					button.Click -= new _CommandBarButtonEvents_ClickEventHandler(ButtonClickHandler);
				} catch {
				} finally {
					button = null;
				}
			}
		}
		~VSMenuItem() {
			Dispose(false);
		}
	}
	public abstract class VSToolWindowMenuItem : VSMenuItem {
		VSToolWindow toolWindow;
		public VSToolWindowMenuItem(string caption, string bitmapResourceName, VSToolWindow toolWindow)
			: base(caption, bitmapResourceName) {
			this.toolWindow = toolWindow;
		}
		protected override void OnButtonClick() {
			toolWindow.ShowPersistently();
		}
	}
}
