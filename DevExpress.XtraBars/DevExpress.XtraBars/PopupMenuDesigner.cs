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

using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Localization;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraBars.Design {
	[ToolboxItem(false)]
	public class PopupMenuEditor : IDisposable {
		BarLinksHolder editObject;
		PopupMenu menu;
		BarManager manager;
		private Form form;
		public PopupMenuEditor(BarManager manager, BarLinksHolder editObject) {
			this.manager = manager;
			this.editObject = editObject;
			if(editObject is PopupMenu) {
				menu = editObject as PopupMenu;
			} else {
				menu = new PopupMenu(true);
				menu.Manager = Manager;
				menu.itemLinks = ItemLinks;
			}
			menu.ItemLinks.CollectionChanged += new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
			form = new PopupMenuEditorForm(manager);
			form.AllowDrop = true;
			form.TopMost = true;
			form.Text = BarLocalizer.Active.GetLocalizedString(BarString.PopupMenuEditor);
			form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			form.MaximizeBox = false;
			form.MinimizeBox = false;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.ClientSize = new Size(0, 0);
			form.ShowInTaskbar = false;
			form.Size = new Size(200, form.Size.Height);
			form.LostFocus += new EventHandler(Form_LostFocus);
			form.GotFocus += new EventHandler(Form_GotFocus);
			form.Closed += new EventHandler(Form_Closed);
			form.Move += new EventHandler(Form_Move);
			form.DragEnter += new DragEventHandler(Form_DragEnter);
		}
		public virtual BarCustomizationManager CustomizationManager { get { return Manager.Helper.CustomizationManager; } }
		public virtual void Dispose() {
			EditObject = null;
		}
		void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(Visible) {
				menu.BeginUpdate();
				menu.EndUpdate();
			}
		}
		public BarLinksHolder EditObject { 
			get { return editObject; } 
			set {
				if(EditObject == value) return;
				if(EditObject != null)
					menu.ItemLinks.CollectionChanged -= new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
				if(editObject is BarCustomContainerItem) {
					BarCustomContainerItem item = editObject as BarCustomContainerItem;
					foreach(BarItemLink link in item.ItemLinks) link.linkedObject = item;
				}
				editObject = value;
			}
		}
		BarItemLinkCollection ItemLinks {
			get {
				if(editObject is PopupMenu) {
					return menu.ItemLinks;
				}
				if(editObject is BarLinkContainerItem) {
					BarLinkContainerItem ci = editObject as BarLinkContainerItem;
					return ci.ItemLinks;
				}
				return null;
			}
		}
		LinksInfo LinksPersistInfo { get { return ItemLinks == null ? null : ItemLinks.LinksPersistInfo; } }
		internal void Show() {
			Manager.Customize();
			CustomizationManager.CustomizationForm.AddOwnedForm(form);
			form.Visible = true;
		}
		public void Close() {
			form.Close();
		}
		protected BarManager Manager {
			get { return manager; }
		}
		private void Form_DragEnter(object sender, DragEventArgs e) {
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data) || Manager.Helper.DragManager.GetDraggingObject(e.Data) == editObject) {
				e.Effect = DragDropEffects.None;
				return;
			}
			ShowMenu();
		}
		private void Form_Closed(object sender, EventArgs e) {
			HideMenu();
			EditObject = null;
			CustomizationManager.MenuEditor = null;
		}
		private void Form_LostFocus(object sender, EventArgs e) {
			Form form = Form.ActiveForm;
			if(form is DevExpress.XtraEditors.Controls.ModalTextBox || Manager.SelectionInfo.ModalTextBoxActive) return;
			HideMenu();
		}
		private void Form_GotFocus(object sender, EventArgs e) {
			ShowMenu();
		}
		private Point GetMenuLocation() {
			return new Point(form.Location.X, form.Bounds.Bottom);
		}
		public void HideMenu() {
			if(menu != null) menu.HidePopup();
		}
		public bool Visible {
			get { return menu != null && menu.Visible; }
		}
		public void ShowMenu() {
			if(!Manager.IsCustomizing)
				Manager.Customize();
			if(!Visible) {
				menu.ShowPopup(GetMenuLocation());
				if(menu.SubControl != null && menu.SubControl.Form != null) this.form.AddOwnedForm(menu.SubControl.Form);
			}
		}
		private void Form_Move(object sender, EventArgs e) {
			if(menu.SubControl != null) {
				if(Manager.SelectionInfo.OpenedPopups.Count > 1)
					Manager.SelectionInfo.ClosePopup(Manager.SelectionInfo.OpenedPopups[1] as IPopup);
				if(menu.SubControl.Form != null) {
					menu.SubControl.Form.locationInfo = menu.CalcLocationInfo(GetMenuLocation());
					menu.SubControl.Form.Location = GetMenuLocation();
				}
			}
		}
	}
	[ToolboxItem(false)]
	public class PopupMenuEditorForm : Form, IBarObject, IDesignTimeTopForm {
		BarManager manager;
		public PopupMenuEditorForm(BarManager manager) {
			this.manager = manager;
		}
		bool IBarObject.IsBarObject { get { return true; } }
		BarManager IBarObject.Manager { get { return manager;} }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return BarMenuCloseType.None;
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) { return true; }
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) { return true; }
	}
}
