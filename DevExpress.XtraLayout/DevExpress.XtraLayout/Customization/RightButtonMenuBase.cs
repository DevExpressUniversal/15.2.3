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
using DevExpress.Utils.Menu;
using DevExpress.XtraLayout.Localization;
namespace DevExpress.XtraLayout {
	public class ParentMenuItemInfo : MenuItemInfo{
		ArrayList menuItems;
		public ParentMenuItemInfo(string name, Image image, EventHandler handler, bool isChecked,	bool isCheckBox) : base(name, image, handler, isChecked,	isCheckBox){ 
			menuItems = new ArrayList();
		}
		public void Add(MenuItemInfo mi) {
			menuItems.Add(mi);	 
		}
		public ArrayList MenuItems{
			get { return menuItems;}
		}
	}
	public class MenuItemInfo{
		string name;
		Image image;
		EventHandler handler;
		bool isChecked;
		bool isCheckBox;
		public MenuItemInfo(string name, Image image, EventHandler handler, bool isChecked,	bool isCheckBox) { 
			this.name = name;	
			this.image = image;
			this.handler = handler;
			this.isChecked = isChecked;
			this.isCheckBox = isCheckBox;
		}
		public string Name{
			get { return name; }
		}
		public Image Image{
			get { return image; }
		}
		public EventHandler Handler{
			get { return handler; }
		}
		public bool IsChecked{
			get { return isChecked; }
		}
		public bool IsParentMenuItem{
			get { return isChecked; }
		}
		public bool IsCheckBox{
			get { return isCheckBox; }
		}
	}
	public class RightButtonMenuBase : IDisposable {
		internal const string MenuSeparator = "_menuSeparator";
		protected DXPopupMenu menuCore;
		ILayoutControl owner; 
		bool menuItemFlag;
		ArrayList menuItemInfoList;
		public RightButtonMenuBase(ILayoutControl owner, ArrayList menuItemInfoList) {
			menuCore = new DXPopupMenu();
			this.owner = owner;
			menuItemFlag = Owner.EnableCustomizationMode;
			this.menuItemInfoList = menuItemInfoList;
			CreateMenu();	
		}
		protected ILayoutControl Owner {
			get { return owner;}
		}
		public DXPopupMenu Menu{
			get { return menuCore; }
		}
		void CreateMenu() {
		  	for (int i = 0; i < menuItemInfoList.Count; i++){
				MenuItemInfo info = menuItemInfoList[i] as MenuItemInfo;
				DXMenuItem item;
				if (info.IsCheckBox) {
					item = CreateMenuCheckItem(info.Name, info.IsChecked, info.Image, info.Handler); 
				}
				else {
					if(info is ParentMenuItemInfo) {
						ParentMenuItemInfo parentMI = info as ParentMenuItemInfo;
						item = CreateParentMenuItem(info.Name);
						DXSubMenuItem sitem = item as DXSubMenuItem;
						sitem.Image = parentMI.Image;
						foreach(MenuItemInfo mi in parentMI.MenuItems) {
							if (mi.IsCheckBox) {
								sitem.Items.Add(CreateMenuCheckItem(mi.Name, mi.IsChecked, mi.Image, mi.Handler)); 
							}
							else {
								sitem.Items.Add(CreateMenuItem(mi.Name, mi.Image, mi.Handler));		 
							}
						}
					}
					else {
						 item = CreateMenuItem(info.Name, info.Image, info.Handler);
					}
				}
				menuCore.Items.Add(item);
			}
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, Image image, EventHandler handler ) {
			DXMenuItem item = new DXMenuItem(caption, handler , image);
			return item;
		}
		protected virtual DXSubMenuItem CreateParentMenuItem(string caption) { 
			return new DXSubMenuItem(caption);
		}
			protected virtual DXMenuItem CreateMenuCheckItem(string caption, bool check, Image image, EventHandler handler) {
			DXMenuCheckItem item = new DXMenuCheckItem(caption, check);
			item.Image = image;
			item.CheckedChanged += handler;
			return item;
		}
		void OnClick(object Sender, System.EventArgs e) {
			if(!menuItemFlag) { 
				Owner.ShowCustomizationForm();
			}
			else { 
				Owner.HideCustomizationForm();
			} 
		}
		public void Dispose() {
			menuCore.Dispose();
		}
		public virtual void ShowMenu(Point point) {
			MenuManagerHelper.ShowMenu(menuCore, Owner.LookAndFeel.ActiveLookAndFeel, null, owner.Control, point);
		}
	}
}
