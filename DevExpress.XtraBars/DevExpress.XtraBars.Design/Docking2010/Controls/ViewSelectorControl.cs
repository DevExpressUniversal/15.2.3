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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Design {
	public partial class ViewSelectorControl : XtraUserControl {
		Font hover;
		DXPopupMenu popupMenu;
		public ViewSelectorControl() {
			hover = new Font(AppearanceObject.DefaultFont, FontStyle.Underline);
			InitializeComponent();
			documentManagerLink.Click += documentManagerLinkClick;
			viewListBox.SelectedIndexChanged += viewListBoxSelectionItemChanged;
			viewListBox.DoubleClick += viewListBoxSelectionItemChanged;
			viewComboBox.EditValueChanged += viewComboBoxSelectItemChanged;
			popupMenu = new DXPopupMenu();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			manager.ViewCollection.CollectionChanged += new Docking2010.Base.CollectionChangedHandler<BaseView>(ViewCollection_CollectionChanged);
			SubscribeChangeService();
			UpdatePopupMenu();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			UnsubscribeChangeService();
		}
		IComponentChangeService changeService = null;
		void SubscribeChangeService() {
			if(Manager != null && Manager.Site != null)
				this.changeService = Manager.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(this.changeService != null)
				this.changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
		}
		void UnsubscribeChangeService() {
			if(this.changeService != null)
				this.changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
			this.changeService = null;
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			if(Manager != null)
				PopulateViews();
		}
		void ViewCollection_CollectionChanged(Docking2010.Base.CollectionChangedEventArgs<BaseView> ea) {
			if(ea.ChangedType == Docking2010.Base.CollectionChangedType.ElementAdded)
				Manager.View = ea.Element;
			UpdatePopupMenu();
		}
		void UpdatePopupMenu() {
			if(manager != null && manager.ViewCollection.Count != 0) addViewButton.Text = "Convert To ...";
			else addViewButton.Text = "Add View";
			AddViews();
			InitializePopupMenu();
		}
		object selectedObjectCore;
		public object SelectedObject {
			get { return selectedObjectCore; }
			set {
				if(selectedObjectCore == value) return;
				selectedObjectCore = value;
				if(SelectionChanged != null)
					SelectionChanged(this, EventArgs.Empty);
			}
		}
		int lockListboxChange = 0;
		void documentManagerLinkClick(object sender, EventArgs e) {
			lockListboxChange++;
			viewComboBox.Focus();
			viewListBox.SelectedValue = Manager.View;
			SelectedObject = Manager;
			lockListboxChange--;
		}
		void viewListBoxSelectionItemChanged(object sender, System.EventArgs e) {
			if(lockListboxChange == 0)
				SelectedObject = viewListBox.SelectedValue;
		}
		void viewComboBoxSelectItemChanged(object sender, EventArgs e) {
			lockListboxChange++;
			Manager.View = (BaseView)viewComboBox.EditValue;
			SelectedObject = viewComboBox.EditValue;
			viewListBox.SelectedValue = viewComboBox.EditValue;
			lockListboxChange--;
		}
		void hyperLink_MouseEnter(object sender, System.EventArgs e) {
			((LabelControl)sender).Appearance.Font = hover;
		}
		void hyperLink_MouseLeave(object sender, System.EventArgs e) {
			((LabelControl)sender).Appearance.Font = AppearanceObject.DefaultFont;
		}
		DocumentManager manager;
		[DefaultValue(null)]
		public DocumentManager Manager {
			get { return manager; }
			set {
				if(Manager == value) return;
				manager = value;
				if(manager != null)
					PopulateViews();
				else
					Clear();
			}
		}
		protected void Clear() {
			lockListboxChange++;
			viewListBox.Items.Clear();
			viewComboBox.Properties.Items.Clear();
			viewComboBox.EditValue = null;
			lockListboxChange--;
		}
		protected void PopulateViews() {
			viewListBox.Items.Clear();
			UpdateCurrentViewComboBox();
			viewListBox.Items.AddRange(Manager.ViewCollection.ToArray());
		}
		void UpdateCurrentViewComboBox() {
			viewComboBox.Properties.Items.Clear();
			ImageComboBoxItem[] items = new ImageComboBoxItem[Manager.ViewCollection.Count];
			for(int i = 0; i < items.Length; i++) {
				BaseView view = Manager.ViewCollection[i];
				items[i] = new ImageComboBoxItem(view, (int)view.Type);
			}
			viewComboBox.Properties.Items.AddRange(items);
			viewComboBox.EditValue = Manager.View;
		}
		protected virtual void InitializePopupMenu() {
			if(this.popupMenu.Items.Count != 0) this.popupMenu.Items.Clear();
			foreach(KeyValuePair<string, ViewType> view in views) {
				if(manager.ViewCollection.Count == 0 || (manager.View != null && manager.View.Type != view.Value))
					this.popupMenu.Items.Add(CreateMenuItem(view, manager.ViewCollection.Count == 0));
			}
		}
		protected virtual DXMenuItem CreateMenuItem(KeyValuePair<string, ViewType> view, bool createView) {
			DXMenuItem dxItem;
			dxItem = new DXMenuItem(view.Key, createView ? new EventHandler(OnCreateView) : new EventHandler(OnChangeTypeView));
			dxItem.Image = GetElementImage(view.Value);
			dxItem.Tag = view.Value;
			return dxItem;
		}
		Image GetElementImage(ViewType viewType) {
			return ImageCollection.GetImageListImage(viewsIcon, (int)viewType);
		}
		void OnCreateView(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			CreateViewCore((ViewType)item.Tag);
			UpdateCurrentViewComboBox();
		}
		void OnChangeTypeView(object sender, EventArgs e) {
			DXMenuItem item = sender as DXMenuItem;
			if(item == null) return;
			ChangeTypeCore((ViewType)item.Tag);
		}
		void ChangeTypeCore(Docking2010.Views.ViewType type) {
			Docking2010.Views.BaseView view = Manager.View;
			CreateViewCore(type);
			viewListBox.Items.Remove(view);
			view.Dispose();
			UpdateCurrentViewComboBox();
			UpdatePopupMenu();
		}
		public event EventHandler SelectionChanged;
		void CreateViewCore(ViewType viewType) {
			lockListboxChange++;
			Manager.BeginUpdate();
			Docking2010.Views.BaseView view = Manager.CreateView(viewType);
			Manager.EndUpdate();
			((ISupportInitialize)view).BeginInit();
			Manager.ViewCollection.Add(view);
			((ISupportInitialize)view).EndInit();
			viewListBox.Items.Add(view);
			viewComboBox.Properties.Items.Add(new ImageComboBoxItem(view, (int)view.Type));
			lockListboxChange--;
		}
		void AddView(object sender, EventArgs e) {
			MenuManagerHelper.GetMenuManager(LookAndFeel.ActiveLookAndFeel, this).ShowPopupMenu(this.popupMenu, addViewButton, new Point(0, addViewButton.Size.Height));
		}
		void AddViews() {
			views = new Dictionary<string, ViewType>();
			views.Add("TabbedView", ViewType.Tabbed);
			views.Add("NativeMdiView", ViewType.NativeMdi);
			views.Add("WindowsUIView", ViewType.WindowsUI);
			views.Add("WidgetView", ViewType.Widget);
		}
		Dictionary<string, ViewType> views;
		private void RemoveView(object sender, EventArgs e) {
			BaseView view = viewListBox.SelectedItem as BaseView;
			if(view != null) {
				Manager.ViewCollection.Remove(view);
				view.Dispose();
				viewListBox.Items.Remove(viewListBox.SelectedItem);
				UpdateCurrentViewComboBox();
			}
		}
		void viewListBoxKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				RemoveView(this, new EventArgs());
				e.Handled = true;
			}
		}
	}
}
