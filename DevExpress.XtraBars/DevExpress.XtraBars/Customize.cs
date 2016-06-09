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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Text;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Customization {
	public class CustomizationForm : XtraForm {
		private System.ComponentModel.IContainer components;
		protected CustomizationControl localizationManager;
		private System.Windows.Forms.ImageList imageList1;
		BarManager manager;
		public BarManager Manager { get { return manager; } }
		public CustomizationForm()
			: this(new CustomizationControl(), UserLookAndFeel.Default) {
		}
		public CustomizationForm(CustomizationControl lmanager, UserLookAndFeel lookAndFeel) {
			this.localizationManager = lmanager;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			InitializeComponent();
			this.LookAndFeel.Assign(lookAndFeel); 
			SuspendLayout();
			UpdateLookAndFeel(LookAndFeel, this);
			ResumeLayout(false);
			if(lookAndFeel.GetTouchUI()) {
				Scale(new SizeF(1.4f + lookAndFeel.GetTouchScaleFactor() / 10.0f, 1.4f + lookAndFeel.GetTouchScaleFactor() / 10.0f));
			}
			HandleEvents();
			this.CancelButton = localizationManager.btClose;
		}
		protected virtual void UpdateLookAndFeel(UserLookAndFeel lookAndFeel, Control ctrl) {
			BaseControl bc = ctrl as BaseControl;
			if(bc != null) bc.LookAndFeel.Assign(lookAndFeel);
			XtraTab.XtraTabControl tab = ctrl as XtraTab.XtraTabControl;
			if(tab != null) {
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Flat)
					tab.PaintStyleName = "Style3D";
				else
					tab.LookAndFeel.Assign(lookAndFeel);
			}
			foreach(Control c in ctrl.Controls) UpdateLookAndFeel(lookAndFeel, c);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
#if !XtraV3
				if(LookAndFeel != null) LookAndFeel.Dispose();
#endif
			}
			if(components != null)
				components.Dispose();
			components = null;
			base.Dispose(disposing);
		}
		protected virtual void HandleEvents() {
			LManager.btNewBar.Click += new System.EventHandler(this.btNewBar_Click);
			LManager.btRenameBar.Click += new System.EventHandler(this.btRenameBar_Click);
			LManager.btClose.Click += new System.EventHandler(this.btClose_Click);
			LManager.btResetBar.Click += new System.EventHandler(this.btReset_Click);
			LManager.btDeleteBar.Click += new System.EventHandler(this.btDeleteBar_Click);
			LManager.toolBarsList.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);
			LManager.toolBarsList.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.toolBarsList_ItemCheck);
			LManager.toolBarsList.ItemChecking += new DevExpress.XtraEditors.Controls.ItemCheckingEventHandler(this.toolBarsList_ItemChecking);
			LManager.cbOptions_ShowShortcutInTips.CheckedChanged += new System.EventHandler(this.cbOptions_ShowShortcutInTips_CheckedChanged);
			LManager.cbOptions_largeIcons.CheckedChanged += new System.EventHandler(this.cbOptions_LargeIcons_CheckedChanged);
			LManager.cbOptions_showTips.CheckedChanged += new System.EventHandler(this.cbOptions_showTips_CheckedChanged);
			LManager.btOptions_Reset.Click += new System.EventHandler(this.btOptions_Reset_Click);
			LManager.cbOptions_showFullMenusAfterDelay.CheckedChanged += new System.EventHandler(this.cbOptions_showFullMenusAfterDelay_CheckedChanged);
			LManager.cbOptionsShowFullMenus.CheckedChanged += new System.EventHandler(this.cbOptionsShowFullMenus_CheckedChanged);
			LManager.cbOptions_MenuAnimation.SelectedIndexChanged += new System.EventHandler(this.cbOptions_MenuAnimation_SelectedIndexChanged);
			LManager.tabControl.SelectedPageChanged += new TabPageChangedEventHandler(this.tabControl1_SelectedIndexChanged);
			LManager.tpCommands.Click += new System.EventHandler(this.tpCommands_Click);
			LManager.lbCommands.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lbCommands_MeasureItem);
			LManager.lbCommands.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbCommands_KeyDown);
			LManager.lbCommands.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbCommands_MouseDown);
			LManager.lbCommands.SelectedIndexChanged += new System.EventHandler(this.lbCommands_SelectedIndexChanged);
			LManager.lbCommands.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.lbCommands_QueryContinueDrag);
			LManager.lbCommands.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbCommands_MouseUp);
			LManager.lbCommands.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbCommands_MouseMove);
			LManager.lbCommands.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.lbCommands_GiveFeedback);
			LManager.lbCommands.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lbCommands_DrawItem);
			LManager.lbCommands.DoubleClick += new EventHandler(this.lbCommand_DoubleClick);
			LManager.lbCategories.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbCategories_KeyDown);
			LManager.lbCategories.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbCategories_MouseDown);
			LManager.lbCategories.DragOver += new System.Windows.Forms.DragEventHandler(this.lbCategories_DragOver);
			LManager.lbCategories.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbCategories_DragDrop);
			LManager.lbCategories.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbCategories_MouseUp);
			LManager.lbCategories.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbCategories_DragEnter);
			LManager.lbCategories.DragLeave += new System.EventHandler(this.lbCategories_DragLeave);
			LManager.lbCategories.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.lbCategories_GiveFeedback);
			LManager.lbCategories.SelectedIndexChanged += new System.EventHandler(this.lbCategories_SelectedIndexChanged);
			LManager.timer1.Tick += new System.EventHandler(this.timer1_Tick);
		}
		DevExpress.XtraBars.Customization.Helpers.Menus menus;
		protected virtual void InitHelpers() {
			if(!IsDesignMode) return;
			this.LManager.btResetBar.Visible = true;
			menus = new DevExpress.XtraBars.Customization.Helpers.Menus(this, LManager.lCategories, LManager.lCommands);
			menus.commandsMenu.BeforePopup += new CancelEventHandler(OnCommandsMenu_Popup);
			menus.barManager1.Controller = Manager.Controller;
			menus.barManager1.Form = this;
			menus.barManager1.Images = this.imageList1;
		}
		public virtual CustomizationControl LManager { get { return localizationManager; } }
		public void UpdateBarVisibility(Bar bar) {
			if(!IsDesignMode) return;
			InitToolbarsBox(bar);
		}
		public virtual void Init(BarManager AManager) {
			this.manager = AManager;
			InitHelpers();
			if(Manager.IsRightToLeft) {
				this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			}
			if(Manager.IsOnTopMostForm) TopMost = true;
			LManager.lbCommands.BackColor = Manager.PaintStyle.DrawParameters.Colors.MenuAppearance.Menu.BackColor;
			LManager.btResetBar.Enabled = !IsDesignMode;
			InitToolbarsBox(null);
			InitCategories(0);
			LManager.cbOptions_largeIcons.Checked = Manager.LargeIcons;
			LManager.cbOptions_showTips.Checked = Manager.ShowScreenTipsInToolbars;
			LManager.cbOptions_ShowShortcutInTips.Checked = Manager.ShowShortcutInScreenTips;
			LManager.cbOptions_showFullMenusAfterDelay.Checked = Manager.ShowFullMenusAfterDelay;
			LManager.cbOptionsShowFullMenus.Checked = Manager.ShowFullMenus;
			InitOptionsMenuAnimation();
			cbOptionsShowFullMenus_CheckedChanged(this, EventArgs.Empty);
			cbOptions_showTips_CheckedChanged(this, EventArgs.Empty);
			if(IsDesignMode) {
				this.LManager.tabControl.TabPages.Remove(LManager.tpOptions);
			}
			this.LManager.tabControl.SelectedTabPageIndex = 0;
			SelectObject(null);
		}
		int lockMenuAnimation = 0;
		BarString[] locAnimation = new BarString[] { BarString.MenuAnimationSystem, BarString.MenuAnimationNone, 
													   BarString.MenuAnimationSlide, BarString.MenuAnimationFade, BarString.MenuAnimationUnfold, BarString.MenuAnimationRandom };
		protected void InitOptionsMenuAnimation() {
			try {
				this.lockMenuAnimation++;
				LManager.cbOptions_MenuAnimation.Properties.Items.Clear();
				Array vals = Enum.GetValues(typeof(AnimationType));
				foreach(AnimationType anType in vals) {
					int index = (int)anType;
					string locValue = anType.ToString();
					if(index < locAnimation.Length) locValue = BarLocalizer.Active.GetLocalizedString(locAnimation[index]);
					LManager.cbOptions_MenuAnimation.Properties.Items.Add(locValue);
				}
				LManager.cbOptions_MenuAnimation.SelectedIndex = (int)Manager.MenuAnimationType;
			}
			finally {
				this.lockMenuAnimation--;
			}
		}
		protected virtual void SelectObject(object obj) {
			if(Manager != null)
				Manager.Helper.CustomizationManager.SelectObject(obj);
		}
		bool IsDesignMode {
			get {
				if(Manager == null) return false;
				return Manager.IsDesignMode;
			}
		}
		internal void OnItemChanged(BarItem item) {
			if(item.Category == LManager.lbCategories.SelectedItem) {
				LManager.lbCommands.Invalidate();
			}
		}
		bool isCustomization = false;
		public virtual void ShowCustomization(bool show) {
			if(Visible == show) return;
			if(show) {
				isCustomization = true;
				if(Manager.Form is Form) {
					(Manager.Form as Form).AddOwnedForm(this);
				}
				Show();
				Focus();
			}
			else {
				if(Manager.Form is Form) {
					(Manager.Form as Form).RemoveOwnedForm(this);
				}
				EndCustomization();
			}
		}
		private void AddItem() {
			AddItem addItem = new AddItem();
			addItem.Init(this);
			DialogResult res = addItem.ShowDialog(this);
			int catIndex = addItem.cbCategories.SelectedIndex;
			addItem.Dispose();
			if(res == DialogResult.OK) {
				LManager.lbCategories.SelectedIndex = catIndex;
				InitCommands(LManager.lbCommands.Items.Count);
			}
		}
		protected virtual void EndCustomization() {
			if(isCustomization) {
				isCustomization = false;
			}
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if(Manager != null)
				Manager.HideCustomization();
		}
		protected virtual void InitToolbarsBox(Bar selItem) {
			LManager.toolBarsList.Items.BeginUpdate();
			try {
				this.LManager.toolBarsList.ItemCheck -= new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.toolBarsList_ItemCheck);
				this.LManager.toolBarsList.ItemChecking -= new DevExpress.XtraEditors.Controls.ItemCheckingEventHandler(this.toolBarsList_ItemChecking);
				LManager.toolBarsList.Items.Clear();
				foreach(Bar bar in manager.Bars) {
					CheckState ch = bar.Visible ? CheckState.Checked : CheckState.Unchecked;
					if(bar.IsMainMenu || bar.OptionsBar.DisableClose) ch = CheckState.Indeterminate;
					if(bar.OptionsBar.Hidden && !IsDesignMode) continue;
					LManager.toolBarsList.Items.Add(new CheckedListBoxItem(bar, ch));
				}
				if(selItem != null) {
					LManager.toolBarsList.SelectedItem = FindListBoxItem(LManager.toolBarsList, selItem);
				}
				if(selItem == null && LManager.toolBarsList.Items.Count > 0)
					LManager.toolBarsList.SelectedIndex = 0;
			}
			finally {
				LManager.toolBarsList.Items.EndUpdate();
				this.LManager.toolBarsList.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.toolBarsList_ItemCheck);
				this.LManager.toolBarsList.ItemChecking += new DevExpress.XtraEditors.Controls.ItemCheckingEventHandler(this.toolBarsList_ItemChecking);
			}
		}
		protected virtual void InitCategories(int selIndex) {
			LManager.lbCategories.Items.BeginUpdate();
			try {
				LManager.lbCategories.Items.Clear();
				foreach(BarManagerCategory category in Manager.Categories) {
					if(!category.Visible && !IsDesignMode) continue;
					LManager.lbCategories.Items.Add(category);
				}
				if(IsDesignMode)
					LManager.lbCategories.Items.Add(BarManagerCategory.TotalCategory);
				if(ShouldShowDefaultCategory)
					LManager.lbCategories.Items.Insert(0, BarManagerCategory.DefaultCategory);
				if(selIndex > -1 && selIndex < LManager.lbCategories.Items.Count)
					LManager.lbCategories.SelectedIndex = selIndex;
				InitCommands(-1);
			}
			finally {
				LManager.lbCategories.Items.EndUpdate();
			}
			EnableMenus();
		}
		protected bool ShouldShowDefaultCategory {
			get { return IsDesignMode || GetItemsForCategory(BarManagerCategory.DefaultCategory).Count != 0; }
		}
		void OnCommandsMenu_Popup(object sender, CancelEventArgs e) {
			bool enable = IsValidCommand(LManager.lbCommands.SelectedIndex);
			BarItem item = (enable ? LManager.lbCommands.Items[LManager.lbCommands.SelectedIndex] as BarItem : null);
			menus.deleteCommand.Enabled = enable;
			menus.clearCommand.Enabled = enable;
			menus.moveUpCommand.Enabled = enable && LManager.lbCommands.SelectedIndex > 0;
			menus.moveDownCommand.Enabled = enable && LManager.lbCommands.SelectedIndex < LManager.lbCommands.Items.Count - 1;
			menus.resetGlyphCommand.Enabled = enable && item != null && item.Glyph != null;
			if(enable) {
				enable = item is BarLinkContainerItem;
				if(item is BarToolbarsListItem) enable = false;
				if(item is BarListItem) enable = false;
			}
			menus.subMenuEditorCommand.Enabled = enable;
		}
		void EnableMenus() {
			if(!IsDesignMode) return;
			BarManagerCategory selectedCategory = LManager.lbCategories.SelectedItem as BarManagerCategory;
			bool enable = selectedCategory != null;
			if(IsSystemCategory(selectedCategory)) enable = false;
			menus.visibleCategory.Enabled = enable;
			enable = enable && Manager.Categories.Count > 0;
			menus.deleteCategory.Enabled = enable;
			menus.renameCategory.Enabled = enable;
			if(LManager.lbCategories.SelectedItem != null)
				menus.visibleCategory.Down = (LManager.lbCategories.SelectedItem as BarManagerCategory).Visible;
			enable = IsValidCommand(LManager.lbCommands.SelectedIndex);
			BarItem item = (enable ? LManager.lbCommands.Items[LManager.lbCommands.SelectedIndex] as BarItem : null);
			menus.deleteCommand.Enabled = enable;
			menus.moveUpCommand.Enabled = enable && LManager.lbCommands.SelectedIndex > 0;
			menus.moveDownCommand.Enabled = enable && LManager.lbCommands.SelectedIndex < LManager.lbCommands.Items.Count - 1;
			menus.resetGlyphCommand.Enabled = enable && item != null && item.Glyph != null;
			if(enable) {
				enable = item is BarLinkContainerItem;
				if(item is BarToolbarsListItem) enable = false;
				if(item is BarListItem) enable = false;
			}
			menus.subMenuEditorCommand.Enabled = enable;
		}
		protected virtual void InitCommands(int index) {
			LManager.lbCommands.Items.BeginUpdate();
			try {
				BarManagerCategory category = LManager.lbCategories.SelectedItem as BarManagerCategory;
				LManager.lbCommands.Items.Clear();
				List<BarItem> items = GetItemsForCategory(category);
				LManager.lbCommands.Items.AddRange(items.ToArray());
			}
			finally {
				LManager.lbCommands.Items.EndUpdate();
			}
			if(index > LManager.lbCommands.Items.Count)
				index = LManager.lbCommands.Items.Count - 1;
			if(index > -1 && index < LManager.lbCommands.Items.Count)
				LManager.lbCommands.SelectedIndex = index;
			this.LManager.lbDescription.Text = (SelectedItem == null ? "" : SelectedItem.Description);
			EnableMenus();
		}
		protected List<BarItem> GetItemsForCategory(BarManagerCategory category) {
			List<BarItem> list = new List<BarItem>();
			foreach(BarItem barItem in Manager.Items) {
				if(barItem.Category == category || category == BarManagerCategory.TotalCategory) {
					if(!barItem.ShowInCustomizationForm)
						continue;
					if(IsDesignMode || (barItem.Visibility != BarItemVisibility.Never && barItem.Visibility != BarItemVisibility.OnlyInRuntime)) {
						list.Add(barItem);
					}
				}
			}
			return list;
		}
		bool IsValidBar(int index) {
			return (index > -1 && index < LManager.toolBarsList.Items.Count);
		}
		bool IsValidCategory(int index) {
			return (index > -1 && index < LManager.lbCategories.Items.Count);
		}
		bool IsValidCommand(int index) {
			return (index > -1 && index < LManager.lbCommands.Items.Count);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CustomizationForm));
			this.localizationManager.SuspendLayout();
			this.SuspendLayout();
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Silver;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = localizationManager.Size;
			this.localizationManager.Dock = System.Windows.Forms.DockStyle.Left;
			this.localizationManager.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.localizationManager.TabIndex = 1;
			this.localizationManager.TabStop = true;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.localizationManager});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CustomizationForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = localizationManager.WindowCaption; 
			this.Closing += new System.ComponentModel.CancelEventHandler(this.CustomizationForm_Closing);
			this.localizationManager.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private void btClose_Click(object sender, System.EventArgs e) {
			Close();
		}
		string CreateBarEditForm(bool newToolbar, string caption) {
			BarForm editForm = new BarForm(Manager);
			editForm.LookAndFeel.Assign(Manager.PaintStyle.CustomizationLookAndFeel);
			editForm.ShowInTaskbar = false;
			editForm.ControlBox = false;
			editForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			editForm.MaximizeBox = editForm.MinimizeBox = false;
			editForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			editForm.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			CustomizationControl xLM = BarLocalizer.Active.Customization.Clone();
			xLM.pnlNBDlg.Location = Point.Empty;
			xLM.tbNBDlgName.Text = caption;
			editForm.Text = BarLocalizer.Active.GetLocalizedString(newToolbar ? BarString.NewToolbarCaption : BarString.RenameToolbarCaption);
			editForm.ClientSize = xLM.pnlNBDlg.Size;
			editForm.Controls.Add(xLM.pnlNBDlg);
			editForm.AcceptButton = xLM.btNBDlgOk;
			editForm.CancelButton = xLM.btNBDlgCancel;
			DialogResult res = editForm.ShowDialog(this);
			string ret = res == DialogResult.OK ? xLM.tbNBDlgName.Text : null;
			return ret;
		}
		private void btNewBar_Click(object sender, System.EventArgs e) {
			string caption = CreateBarEditForm(true, Manager.GetNewBarName());
			if(caption != null) {
				Bar bar = new Bar(Manager, caption);
				if(!IsDesignMode) {
					bar.OptionsBar.AllowRename = true;
					bar.OptionsBar.AllowDelete = true;
				}
				bar.Text = caption;
				bar.DockStyle = BarDockStyle.Top;
				bar.Visible = true;
				FireManagerComponentChanged();
				InitToolbarsBox(bar);
			}
		}
		private void btRenameBar_Click(object sender, System.EventArgs e) {
			Bar bar = GetSelectedToolBar();
			if(bar == null) return;
			string caption = CreateBarEditForm(false, bar.Text);
			if(caption != null) {
				bar.Text = caption;
				InitToolbarsBox(bar);
			}
		}
		internal void FireManagerComponentChanged() {
			Manager.FireManagerChanged();
		}
		private void tabControl1_SelectedIndexChanged(object sender, TabPageChangedEventArgs e) {
			if(LManager.tabControl.SelectedTabPageIndex == 0)
				toolBarsList_SelectedIndexChanged(LManager.toolBarsList, EventArgs.Empty);
			if(LManager.tabControl.SelectedTabPageIndex == 2)
				lbCommands_SelectedIndexChanged(LManager.lbCommands, EventArgs.Empty);
		}
		private void toolBarsList_SelectedIndexChanged(object sender, System.EventArgs e) {
			SelectObject(GetSelectedToolBar());
		}
		bool IsValidIndex(ListBox box, int index) {
			return (index > -1 && index < box.Items.Count);
		}
		private void ListBox_SelectedIndexChanged(object sender, System.EventArgs e) {
			object obj = GetListBoxItem(sender as CheckedListBoxControl, (sender as CheckedListBoxControl).SelectedIndex);
			SelectObject(obj);
			if(sender == LManager.toolBarsList) {
				Bar bar = obj as Bar;
				LManager.btDeleteBar.Enabled = (bar != null && (IsDesignMode || bar.OptionsBar.AllowDelete));
				LManager.btRenameBar.Enabled = (bar != null && (!IsDesignMode && bar.OptionsBar.AllowRename));
				LManager.btResetBar.Enabled = (bar != null && !bar.OptionsBar.DisableCustomization);
			}
		}
		BarItem SelectedItem {
			get {
				if(IsValidCommand(LManager.lbCommands.SelectedIndex))
					return LManager.lbCommands.Items[LManager.lbCommands.SelectedIndex] as BarItem;
				return null;
			}
		}
		bool lbCommandsLockUpdate = false;
		private void lbCommands_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(this.lbCommandsLockUpdate) return;
			BarItem barItem = SelectedItem;
			SelectObject(barItem);
			EnableMenus();
			this.LManager.lbDescription.AllowHtmlString = (barItem == null ? false : barItem.IsAllowHtmlText);
			this.LManager.lbDescription.Text = (barItem == null ? "" : barItem.Description);
		}
		private void toolBarsList_ItemChecking(object sender, DevExpress.XtraEditors.Controls.ItemCheckingEventArgs e) {
			Bar bar = GetToolBar(e.Index);
			if(bar != null && (bar.IsMainMenu || bar.OptionsBar.DisableClose)) {
				e.Cancel = true;
			}
		}
		private void toolBarsList_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			Bar bar = GetToolBar(e.Index);
			if(bar != null)
				bar.Visible = e.State == CheckState.Checked;
		}
		bool CheckIndex(int index) {
			if(index >= LManager.lbCommands.Items.Count || index < 0 || LManager.lbCommands.Items.Count == 0) return false;
			return true;
		}
		private void lbCommands_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e) {
			if(!CheckIndex(e.Index)) return;
			Size size = CalcItemSize(LManager.lbCommands.Items[e.Index] as BarItem);
			e.ItemHeight = size.Height;
			e.ItemWidth = size.Width;
		}
		private void lbCommands_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e) {
			e.Handled = true;
			if(!CheckIndex(e.Index)) return;
			DrawBarItem(e.Graphics, e.Bounds, LManager.lbCommands.Items[e.Index] as BarItem,
				(e.State & DrawItemState.Selected) != 0);
		}
		private Point downPoint = Point.Empty;
		private BarItem downItem = null, draggingItem = null;
		private void lbCommands_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) != 0) {
				downPoint = Control.MousePosition;
				downItem = null;
				int index = LManager.lbCommands.IndexFromPoint(new Point(e.X, e.Y));
				if(index > -1 && index < LManager.lbCommands.Items.Count)
					downItem = (BarItem)LManager.lbCommands.Items[index];
			}
		}
		private void lbCategories_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && IsDesignMode) {
				int index = LManager.lbCategories.IndexFromPoint(LManager.lbCategories.PointToClient(Control.MousePosition));
				if(IsValidCategory(index)) {
					LManager.lbCategories.SelectedIndex = index;
				}
				menus.categoriesMenu.ShowPopup(Control.MousePosition);
			}
		}
		private void lbCommands_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			Capture = false;
			downItem = null;
			if(e.Button == MouseButtons.Right && IsDesignMode) {
				int index = LManager.lbCommands.IndexFromPoint(LManager.lbCommands.PointToClient(Control.MousePosition));
				if(IsValidCommand(index)) {
					LManager.lbCommands.SelectedIndex = index;
				}
				menus.commandsMenu.ShowPopup(Control.MousePosition);
			}
		}
		private void lbCommands_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) != 0) {
				if(downItem != null) {
					Point p = Control.MousePosition;
					p.Offset(-downPoint.X, -downPoint.Y);
					if(Math.Abs(p.X) > 4 || Math.Abs(p.Y) > 4) {
						Manager.SelectionInfo.CustomizeSelectedLink = null;
						Manager.SelectionInfo.SelectedItem = downItem;
						try {
							draggingItem = downItem;
							LManager.lbCategories.AllowDrop = true;
							Manager.Helper.DragManager.UseDefaultCursors = true;
							Manager.Helper.DragManager.StartDragging(null, e, downItem, LManager.lbCommands);
						}
						finally {
							Manager.Helper.DragManager.UseDefaultCursors = false;
							LManager.lbCategories.AllowDrop = false;
							Manager.SelectionInfo.SelectedItem = null;
							draggingItem = null;
						}
					}
				}
			}
		}
		private void lbCommands_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
			Manager.Helper.DragManager.ItemOnQueryContinueDrag(e, null);
		}
		private void lbCommands_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			Manager.Helper.DragManager.ItemOnGiveFeedback(e, null);
		}
		protected virtual BarItemLink CreateCustomLink(BarItemLink link) {
			BarQMenuCustomizationItem qi = new BarQMenuCustomizationItem(null, Manager, false);
			qi.ButtonStyle = BarButtonStyle.Default;
			qi.Tag = link;
			return qi.CreateLink(null, this);
		}
		void SubscribeEvents(BarLinkViewInfo linkInfo) {
			BarQMenuCustomizationLinkViewInfo vi = linkInfo as BarQMenuCustomizationLinkViewInfo;
			if(linkInfo != null) linkInfo.GlyphSizeEvent += new BarLinkGetValueEventHandler(OnComboBoxList_CalcGlyphSize);
			if(vi != null && vi.InnerViewInfo != null) vi.InnerViewInfo.GlyphSizeEvent += new BarLinkGetValueEventHandler(OnComboBoxList_CalcGlyphSize);
		}
		void UnSubscribeEvents(BarLinkViewInfo linkInfo) {
			BarQMenuCustomizationLinkViewInfo vi = linkInfo as BarQMenuCustomizationLinkViewInfo;
			if(linkInfo != null) linkInfo.GlyphSizeEvent -= new BarLinkGetValueEventHandler(OnComboBoxList_CalcGlyphSize);
			if(vi != null && vi.InnerViewInfo != null) vi.InnerViewInfo.GlyphSizeEvent -= new BarLinkGetValueEventHandler(OnComboBoxList_CalcGlyphSize);
		}
		protected Size CalcItemSize(BarItem barItem) {
			BarLinkViewInfo vi = barItem.CreateLink(null, this).CreateViewInfo();
			vi.SkipRightIndentInMenu = true;
			SubscribeEvents(vi);
			Size res = vi.CalcLinkSize(null, null);
			UnSubscribeEvents(vi);
			return res;
		}
		private void lbCategories_SelectedIndexChanged(object sender, EventArgs e) {
			if(draggingItem == null)
				InitCommands(-1);
		}
		void OnComboBoxList_CalcGlyphSize(object sender, BarLinkGetValueEventArgs e) {
			Size size = (Size)e.Value;
			if(!size.IsEmpty) {
				BarLinkViewInfo vi = sender as BarLinkViewInfo;
				size = vi.UpdateGlyphSize(new Size(16, 16), true);
				e.Value = size;
			}
		}
		internal class MyCustomViewInfo : CustomControlViewInfo {
			public MyCustomViewInfo(BarManager manager, BarDrawParameters drawParameters)
				: base(manager, drawParameters, new CustomControl(manager)) {
				UpdateAppearance();
			}
			protected override void UpdateAppearance() {
				base.UpdateAppearance();
				Appearance.Normal.ForeColor = Manager.GetController().PaintStyle.DrawParameters.Colors.MenuAppearance.Menu.ForeColor;
			}
		}
		MyCustomViewInfo linkDrawViewInfo;
		MyCustomViewInfo LinkDrawViewInfo {
			get {
				if(linkDrawViewInfo == null)
					linkDrawViewInfo = new MyCustomViewInfo(Manager, Manager.DrawParameters);
				return linkDrawViewInfo;
			}
		}
		protected void DrawBarItem(Graphics g, Rectangle bounds, BarItem barItem, bool focused) {
			if(bounds.Width < 1 || bounds.Height < 1) return;
			using(SolidBrush b = new SolidBrush(BarSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[BarSkins.SkinPopupMenu].Color.GetBackColor())) {
				g.FillRectangle(b, bounds);
			}
			BarItemLink link = CreateCustomLink(barItem.CreateLink(null, this));
			if(link == null) return;
			BarLinkViewInfo vi = link.CreateViewInfo();
			SubscribeEvents(vi);
			((BarQMenuCustomizationLinkViewInfo)vi).InnerViewInfo.DrawNormalGlyphInCheckedState = true;
			vi.ParentViewInfo = LinkDrawViewInfo;
			vi.Bounds = bounds;
			vi.CalcViewInfo(g, this, bounds);
			vi.UpdateLinkWidthInSubMenu(bounds.Width);
			if(focused)
				vi.LinkState |= BarLinkState.Highlighted;
			GraphicsInfoArgs dra = new GraphicsInfoArgs(new GraphicsCache(new PaintEventArgs(g, Rectangle.Empty)), Rectangle.Empty);
			vi.Painter.Draw(dra, vi, null);
			UnSubscribeEvents(vi);
			dra.Cache.Dispose();
		}
		private void lbCommands_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Insert)
				InsertItem();
			if(e.KeyCode == Keys.Delete)
				DeleteItem(LManager.lbCommands.SelectedIndex);
			if((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && e.Alt) {
				MoveCommand(LManager.lbCommands.SelectedItem, LManager.lbCommands.SelectedIndex, e.KeyCode == Keys.Up ? -1 : 1);
				e.Handled = true;
			}
		}
		private void InsertItem() {
			if(!IsDesignMode) return;
			AddItem();
		}
		private void DeleteItem(int index) {
			if(!IsDesignMode) return;
			if(LManager.lbCommands.Items.Count == 0 || index < 0 || index >= LManager.lbCommands.Items.Count) return;
			BarItem item = LManager.lbCommands.Items[index] as BarItem;
			item.Dispose();
			InitCommands(index);
		}
		private string GetCategoryName(string name) {
			string resString = null;
			EditForm editForm = new EditForm();
			editForm.LookAndFeel.ParentLookAndFeel = Manager.PaintStyle.CustomizationLookAndFeel;
			editForm.ShowInTaskbar = false;
			editForm.Text = name == "" ? "New Category" : "Edit Category";
			editForm.lbCaption.Text = "&Category Name:";
			editForm.tbName.Text = name;
			editForm.ClientSize = new Size(317, 104);
			editForm.tbName.TextChanged += new EventHandler(tbName_TextChanged);
			tbName_TextChanged(editForm.tbName, EventArgs.Empty);
			DialogResult res = editForm.ShowDialog(this);
			if(res == DialogResult.OK) {
				resString = editForm.tbName.Text;
			}
			editForm.tbName.TextChanged -= new EventHandler(tbName_TextChanged);
			editForm.Dispose();
			return resString;
		}
		void tbName_TextChanged(object sender, System.EventArgs e) {
			TextEdit te = sender as TextEdit;
			if(te == null) return;
			SimpleButton sb = te.Parent.Controls[te.Parent.Controls.Count - 1] as SimpleButton;
			sb.Enabled = !IsCatogoryExist(te.Text);
			if(te.Text == string.Empty) sb.Enabled = false;
		}
		bool IsCatogoryExist(string name) {
			foreach(BarManagerCategory cat in Manager.Categories)
				if(cat.Name == name) return true;
			return false;
		}
		void InsertCategory(BarManagerCategory category) {
			if(!IsDesignMode) return;
			string catName = GetCategoryName("");
			if(catName == null) return;
			BarManagerCategory cat = new BarManagerCategory(catName);
			int index = Manager.Categories.IndexOf(category);
			if(category == BarManagerCategory.DefaultCategory && Manager.Categories.Count > 0) {
				Manager.Categories.Insert(0, cat);
				InitCategories(1);
				return;
			}
			if(Manager.Categories.Count == 0 || index == -1) {
				Manager.Categories.Add(cat);
				InitCategories(Manager.Categories.Count);
				return;
			}
			Manager.Categories.Insert(index, cat);
			InitCategories(index + 1);
		}
		private void DeleteCommands(BarManagerCategory category) {
			for(int n = Manager.Items.Count - 1; n >= 0; n--) {
				if(Manager.Items[n].Category == category || category == BarManagerCategory.TotalCategory)
					Manager.Items.RemoveAt(n);
			}
		}
		private void DeleteCategory(int index) {
			if(!IsDesignMode) return;
			if(!IsValidCategory(index)) return;
			BarManagerCategory category = LManager.lbCategories.Items[index] as BarManagerCategory;
			if(IsSystemCategory(category)) return;
			DialogResult res = XtraMessageBox.Show(Manager.PaintStyle.CustomizationLookAndFeel, LManager.lbCategories, "Are you sure you want to delete the '" +
				category.ToString() + "' category?",
				"XtraBars", MessageBoxButtons.OKCancel);
			LManager.lbCategories.Focus();
			if(res == DialogResult.Cancel) return;
			Manager.Categories.Remove(category);
			InitCategories(index);
		}
		private void lbCategories_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Insert)
				InsertCategory(LManager.lbCategories.SelectedItem as BarManagerCategory);
			if(e.KeyCode == Keys.Delete)
				DeleteCategory(LManager.lbCategories.SelectedIndex);
		}
		private void lbCategories_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
		}
		private void timer1_Tick(object sender, System.EventArgs e) {
			Point p = LManager.lbCategories.PointToClient(Control.MousePosition);
			int index = LManager.lbCategories.SelectedIndex;
			if(p.Y < 4) index--;
			if(p.Y > LManager.lbCategories.ClientSize.Height - 5) index++;
			if(draggingItem != null && index != LManager.lbCategories.SelectedIndex && index >= 0 && index < LManager.lbCategories.Items.Count) {
				LManager.lbCategories.SelectedIndex = index;
				LManager.timer1.Start();
			}
			else LManager.timer1.Stop();
		}
		int enterCatIndex = -1;
		private void lbCategories_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			e.Effect = DragDropEffects.None;
			if(!IsDesignMode) return;
			enterCatIndex = LManager.lbCategories.SelectedIndex;
			BarManagerCategory cat = LManager.lbCategories.SelectedItem as BarManagerCategory;
			if(IsSystemCategory(cat))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy;
		}
		private void lbCategories_DragLeave(object sender, System.EventArgs e) {
			LManager.lbCategories.SelectedIndex = enterCatIndex;
		}
		private void lbCategories_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			e.UseDefaultCursors = false;
			Cursor.Current = (Cursor)Manager.GetController().DragCursors[BarManager.DragCursor];
		}
		private void lbCategories_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			if(draggingItem != null && draggingItem.Category != LManager.lbCategories.SelectedItem) {
				BarManagerCategory cat = LManager.lbCategories.SelectedItem as BarManagerCategory;
				if(IsSystemCategory(cat)) return;
				draggingItem.Category = cat;
				InitCommands(LManager.lbCommands.Items.IndexOf(draggingItem));
				InitCommands(LManager.lbCommands.Items.IndexOf(draggingItem));
			}
		}
		private void lbCategories_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			if(draggingItem != null) {
				if(!IsDesignMode) {
					e.Effect = DragDropEffects.None;
					return;
				}
				e.Effect = DragDropEffects.Copy;
				Point p = LManager.lbCategories.PointToClient(new Point(e.X, e.Y));
				int i = LManager.lbCategories.IndexFromPoint(p);
				if(p.Y < 4 || p.Y > LManager.lbCategories.ClientSize.Height - 5) {
					LManager.timer1.Interval = 200;
					LManager.timer1.Start();
				}
				else {
					if(i > -1 || i < LManager.lbCategories.Items.Count)
						LManager.lbCategories.SelectedIndex = i;
				}
				if(IsSystemCategory(LManager.lbCategories.SelectedItem as BarManagerCategory))
					e.Effect = DragDropEffects.None;
			}
		}
		object FindListBoxItem(CheckedListBoxControl listBox, object val) {
			foreach(CheckedListBoxItem item in listBox.Items) {
				if(object.Equals(item.Value, val)) return item;
			}
			return null;
		}
		object GetListBoxItem(CheckedListBoxControl listBox, int index) {
			if(index < listBox.Items.Count && index > -1) {
				return listBox.Items[index].Value;
			}
			return null;
		}
		Bar GetToolBar(int index) { return GetListBoxItem(LManager.toolBarsList, index) as Bar; }
		Bar GetSelectedToolBar() { return GetToolBar(LManager.toolBarsList.SelectedIndex); }
		private void btDeleteBar_Click(object sender, System.EventArgs e) {
			Bar bar = GetSelectedToolBar();
			if(bar == null) return;
			bar.Dispose();
			InitToolbarsBox(Manager.Bars.Count > 0 ? Manager.Bars[0] : null);
		}
		private void btReset_Click(object sender, System.EventArgs e) {
			Bar bar = GetSelectedToolBar();
			if(bar == null) return;
			bar.AskReset();
		}
		internal void deleteCategory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			DeleteCategory(LManager.lbCategories.SelectedIndex);
		}
		internal void insertCategory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			InsertCategory(LManager.lbCategories.SelectedItem as BarManagerCategory);
		}
		internal void renameCategory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			BarManagerCategory category = LManager.lbCategories.SelectedItem as BarManagerCategory;
			int index = LManager.lbCategories.SelectedIndex;
			if(category == null || IsSystemCategory(category)) return;
			string name = GetCategoryName(category.Name);
			if(name != null) {
				category.Name = name;
				category = Manager.Categories.Replace(category) as BarManagerCategory;
				if(index >= 0) LManager.lbCategories.Items[index] = category;
				InitCategories(index);
			}
		}
		bool IsSystemCategory(BarManagerCategory category) {
			if(category == null || category == BarManagerCategory.DefaultCategory || category == BarManagerCategory.TotalCategory) return true;
			return false;
		}
		internal void visibleCategory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			BarManagerCategory category = LManager.lbCategories.SelectedItem as BarManagerCategory;
			if(category == null || IsSystemCategory(category)) return;
			category.Visible = menus.visibleCategory.Down;
		}
		void lbCommand_DoubleClick(object sender, EventArgs e) {
			MenuEdit();
		}
		void MenuEdit() {
			int index = LManager.lbCommands.SelectedIndex;
			if(!IsValidCommand(index)) return;
			BarLinkContainerItem ci = LManager.lbCommands.SelectedItem as BarLinkContainerItem;
			if(ci == null) return;
			Manager.Helper.CustomizationManager.CustomizeMenu(ci);
		}
		internal void subMenuEditorCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			MenuEdit();
		}
		internal void resetGlyphCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			int index = LManager.lbCommands.SelectedIndex;
			if(!IsValidCommand(index)) return;
			BarItem item = LManager.lbCommands.SelectedItem as BarItem;
			if(item != null) item.Glyph = null;
		}
		internal void moveUpCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			int index = LManager.lbCommands.SelectedIndex;
			if(!IsValidCommand(index)) return;
			MoveCommand(LManager.lbCommands.SelectedItem, index, -1);
		}
		internal void moveDownCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			int index = LManager.lbCommands.SelectedIndex;
			if(!IsValidCommand(index)) return;
			MoveCommand(LManager.lbCommands.SelectedItem, index, 1);
		}
		void MoveCommand(object selItem, int cmdIndex, int delta) {
			if(cmdIndex == 0 && delta == -1) return;
			if(cmdIndex >= LManager.lbCommands.Items.Count - 1 && delta == 1) return;
			BarItem item = LManager.lbCommands.Items[cmdIndex + delta] as BarItem;
			if(item == null) return;
			int index = Manager.Items.IndexOf(item);
			if(index == -1) return;
			LManager.lbCommands.Items.BeginUpdate();
			try {
				this.lbCommandsLockUpdate = true;
				Manager.Items.InternalRemove(selItem);
				Manager.Items.InternalInsert(index, selItem);
				LManager.lbCommands.Items.Remove(selItem);
				LManager.lbCommands.Items.Insert(cmdIndex + delta, selItem);
				LManager.lbCommands.SelectedIndex = cmdIndex + delta;
				EnableMenus();
			}
			finally {
				this.lbCommandsLockUpdate = false;
				LManager.lbCommands.Items.EndUpdate();
			}
			Manager.FireManagerChanged();
		}
		internal void clearCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			int index = LManager.lbCategories.SelectedIndex;
			BarManagerCategory category = LManager.lbCategories.Items[index] as BarManagerCategory;
			string text = "Are you sure you want to delete all commands in the '" +
				category.Name + "' category?";
			if(IsSystemCategory(category))
				text = "Are you sure you want to delete all commands in the " + category.Name + "?";
#if XtraV3
			DialogResult res = XtraMessageBox.Show(Manager.PaintStyle.CustomizationLookAndFeel, LManager.lbCategories, text,
				"XtraBars", MessageBoxButtons.OKCancel);
#else
			DialogResult res = MessageBox.Show(LManager.lbCategories, text,
				"XtraBars", MessageBoxButtons.OKCancel);
#endif
			if(res == DialogResult.Cancel) return;
			DeleteCommands(category);
			InitCategories(index);
		}
		internal void deleteCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			int index = LManager.lbCommands.SelectedIndex;
			if(!IsValidCommand(index)) return;
			DeleteItem(index);
		}
		internal void addCategory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			InsertCategory(null);
		}
		internal void addCommand_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			InsertItem();
		}
		private void cbOptions_MenuAnimation_SelectedIndexChanged(object sender, EventArgs e) {
			if(this.lockMenuAnimation != 0) return;
			Manager.MenuAnimationType = (AnimationType)LManager.cbOptions_MenuAnimation.SelectedIndex;
		}
		private void cbOptionsShowFullMenus_CheckedChanged(object sender, System.EventArgs e) {
			Manager.ShowFullMenus = LManager.cbOptionsShowFullMenus.Checked;
			LManager.cbOptions_showFullMenusAfterDelay.Enabled = !Manager.ShowFullMenus;
		}
		private void cbOptions_LargeIcons_CheckedChanged(object sender, System.EventArgs e) {
			Manager.LargeIcons = LManager.cbOptions_largeIcons.Checked;
		}
		private void cbOptions_showFullMenusAfterDelay_CheckedChanged(object sender, System.EventArgs e) {
			Manager.ShowFullMenusAfterDelay = LManager.cbOptions_showFullMenusAfterDelay.Checked;
		}
		private void btOptions_Reset_Click(object sender, System.EventArgs e) {
			Manager.ResetUsageData();
		}
		private void cbOptions_showTips_CheckedChanged(object sender, System.EventArgs e) {
			Manager.ShowScreenTipsInToolbars = LManager.cbOptions_showTips.Checked;
			LManager.cbOptions_ShowShortcutInTips.Enabled = Manager.ShowScreenTipsInToolbars;
		}
		private void cbOptions_ShowShortcutInTips_CheckedChanged(object sender, System.EventArgs e) {
			Manager.ShowShortcutInScreenTips = LManager.cbOptions_ShowShortcutInTips.Checked;
		}
		private void tpCommands_Click(object sender, System.EventArgs e) {
		}
		string GenerateControlName(Control ctrl) {
			string res = ctrl.GetType().Name + ", " + ctrl.TabIndex.ToString() + ctrl.Text;
			return res;
		}
		Control GetControlByName(string name) {
			foreach(Control ctrl in Controls) {
				if(GenerateControlName(ctrl) == name) return ctrl;
			}
			return null;
		}
		private void CustomizationForm_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(Manager == null) return;
			Hashtable hash = new Hashtable();
			hash.Add(typeof(Form), Bounds);
			foreach(Control ctrl in Controls) {
				if(ctrl is IBarObject) continue;
				hash.Add(GenerateControlName(ctrl), ctrl.Bounds);
			}
			Manager.CustomizationProperties = hash;
		}
	}
	[ToolboxItem(false)]
	public class ItemsListBox : ListBoxControl {
		public ItemsListBox() {
			BackColor = SystemColors.Window;
			SetStyle(ControlConstants.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		protected override BaseControlPainter CreatePainter() { return new ItemsListBoxPainter(); }
		internal class ItemsListBoxPainter : BaseListBoxPainter {
			protected override void DrawFocusRect(ControlGraphicsInfoArgs info) { }
			protected override void DrawContent(ControlGraphicsInfoArgs info) {
				DrawVisibleItems(info);
			}
		}
	}
	internal class BarForm : XtraForm, IBarObject {
		BarManager manager;
		public BarForm(BarManager manager) {
			this.manager = manager;
		}
		public virtual BarManager Manager { get { return manager; } }
		bool IBarObject.IsBarObject { get { return true; } }
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) { return false; }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) { return BarMenuCloseType.None; }
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) { return false; }
	}
}
