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
using DevExpress.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Localization;
using System.Drawing;
using DevExpress.Utils.Frames;
using DevExpress.Data.Summary;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.XtraGrid.GroupSummaryEditor {
	public class GroupSummaryEditorForm : XtraForm {
		GridSummaryItemsEditorController controller;
		ListBoxControl items;
		ListBoxControl itemsOrder;
		XtraTabPage orderPage;
		Font boldFont;
		SimpleButton upButton;
		SimpleButton downButton;
		IEnumerable<CheckEdit> checkEdits;
		public GroupSummaryEditorForm(ISummaryItemsOwner itemsOwner) : this(itemsOwner, null) { }
		public GroupSummaryEditorForm(ISummaryItemsOwner itemsOwner, GridColumn owner) {
			this.controller = new GridSummaryItemsEditorController(itemsOwner);
			Text = GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorFormCaption);
			Width = 350;
			Height = 400;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			StartPosition = FormStartPosition.CenterParent;
			MaximizeBox = false;
			MinimizeBox = false;
			this.Padding = new Padding(12);
			CreateControls();
			SelectOwnerItem(items, owner);
		}
		void SelectOwnerItem(ListBoxControl items, GridColumn owner) {
			if(owner == null || string.IsNullOrEmpty(owner.FieldName)) return;
			foreach(SummaryEditorUIItem item in items.Items)
				if(item.FieldName == owner.FieldName) {
					items.SelectedItem = item;
					return;
				}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && this.boldFont != null) {
				this.boldFont.Dispose();
				this.boldFont = null;
			}
			base.Dispose(disposing);
		}
		protected GridSummaryItemsEditorController Controller { get { return controller; } }
		void CreateControls() {
			XtraPanel panel = new XtraPanel();
			panel.Parent = this;
			panel.Dock = DockStyle.Bottom;
			panel.Height = 35;
			panel.Padding = new Padding(0, 10, 0, 0);
			this.AcceptButton = CreateButton(panel, GridStringId.GroupSummaryEditorFormOkButton, DialogResult.OK);
			AddSplitter(panel);
			this.CancelButton = CreateButton(panel, GridStringId.GroupSummaryEditorFormCancelButton, DialogResult.Cancel);
			XtraTabControl tabControl = new XtraTabControl();
			tabControl.Parent = this;
			tabControl.SelectedPageChanged += new TabPageChangedEventHandler(tabControl_SelectedPageChanged);
			tabControl.Dock = DockStyle.Fill;
			tabControl.BringToFront();
			CreateItemPageControl(tabControl);
			CreateOrderPageControl(tabControl);
			ActiveSummaryItemChanged();
			this.ActiveControl = this.items;
		}
		void tabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			if(e.Page == this.orderPage) {
				FillOrderListItems();
			}
		}
		void FillOrderListItems() {
			this.itemsOrder.BeginUpdate();
			try {
				this.itemsOrder.Items.Clear();
				foreach(SummaryEditorOrderUIItem item in Controller.CreateOrderItems()) {
					this.itemsOrder.Items.Add(item);
				}
			} finally {
				this.itemsOrder.EndUpdate();
			}
			UpdateUpDownButtonsEnabled();
		}
		void CreateItemPageControl(XtraTabControl parent) {
			XtraTabPage itemsPage = parent.TabPages.Add(GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorFormItemsTabCaption));
			this.items = new ListBoxControl();
			this.items.Parent = itemsPage;
			this.items.SetBounds(10, 45, 185, 220);
			for(int i = 0; i < Controller.Count; i++) {
				this.items.Items.Add(Controller[i]);
			}
			this.items.SelectedIndexChanged += new EventHandler(items_SelectedIndexChanged);
			this.items.DrawItem += new ListBoxDrawItemEventHandler(items_DrawItem);
			this.checkEdits = CreateCheckEditsForSummaries(itemsPage);
			CreateCheckEditForCount(itemsPage);
			this.items.BringToFront();
		}
		void CreateOrderPageControl(XtraTabControl parent) {
			this.orderPage = parent.TabPages.Add(GridLocalizer.Active.GetLocalizedString(GridStringId.GroupSummaryEditorFormOrderTabCaption));
			this.orderPage.Padding = new Padding(10);
			this.itemsOrder = new ListBoxControl();
			this.itemsOrder.Parent = this.orderPage;
			this.itemsOrder.Dock = DockStyle.Left;
			this.itemsOrder.Width = 255;
			this.itemsOrder.SelectedIndexChanged += new EventHandler(itemsOrder_SelectedIndexChanged);
			this.upButton = CreateUpDownButton(this.orderPage, true, DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16UpIndex, 10);
			this.downButton = CreateUpDownButton(this.orderPage, false, DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16DownIndex, 48);
		}
		SimpleButton CreateUpDownButton(Control parent, bool isUp, int imageIndex, int top) {
			SimpleButton button = new SimpleButton();
			button.Parent = parent;
			button.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[imageIndex];
			button.ImageLocation = ImageLocation.MiddleCenter;
			button.Width = 22;
			button.Height = 32;
			button.Left = 276;
			button.Top = top;
			button.Tag = isUp;
			button.Click += new EventHandler(upDownButton_Click);
			return button;
		}
		void itemsOrder_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateUpDownButtonsEnabled();
		}
		void UpdateUpDownButtonsEnabled() {
			if(itemsOrder.SelectedItem == null) {
				upButton.Enabled = false;
				downButton.Enabled = false;
				return;
			}
			upButton.Enabled = ((SummaryEditorOrderUIItem)itemsOrder.SelectedItem).CanUp;
			downButton.Enabled = ((SummaryEditorOrderUIItem)itemsOrder.SelectedItem).CanDown;
		}
		void upDownButton_Click(object sender, EventArgs e) {
			bool isMoveUp = (bool)(sender as SimpleButton).Tag;
			SummaryEditorOrderUIItem item = (SummaryEditorOrderUIItem)itemsOrder.SelectedItem;
			if(isMoveUp) {
				item.MoveUp();
			} else {
				item.MoveDown();
			}
			FillOrderListItems();
			itemsOrder.SelectedIndex = item.Index;
		}
		void items_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			if(this.boldFont == null) {
				this.boldFont = new Font(e.Appearance.Font, FontStyle.Bold);
			}
			if(((SummaryEditorUIItem)e.Item).HasSummary) {
				e.Appearance.Font = this.boldFont;
			}
		}
		protected SummaryEditorUIItem ActiveUIItem { get { return this.items.SelectedItem as SummaryEditorUIItem; } }
		protected override void OnClosed(EventArgs e) {
			if(DialogResult == DialogResult.OK) {
				Controller.Apply();
			}
		}
		void items_SelectedIndexChanged(object sender, EventArgs e) {
			ActiveSummaryItemChanged();
		}
		void ActiveSummaryItemChanged() {
			if(ActiveUIItem == null) return;
			foreach(CheckEdit checkEdit in this.checkEdits) {
				SummaryItemType summaryType = (SummaryItemType)checkEdit.Tag;
				checkEdit.Checked = ActiveUIItem[summaryType];
				checkEdit.Enabled = ActiveUIItem.CanDoSummary(summaryType);
			}
		}
		void AddSplitter(Control parent) {
			PanelControl panel = new PanelControl();
			panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			panel.Parent = parent;
			panel.Width = 6;
			panel.Dock = DockStyle.Right;
		}
		SimpleButton CreateButton(Control parent, GridStringId localizedId, DialogResult dialogResult) {
			SimpleButton button = new SimpleButton();
			button.Parent = parent;
			button.DialogResult = dialogResult;
			button.Text = GridLocalizer.Active.GetLocalizedString(localizedId);
			button.Height = 24;
			button.Width = 80;
			button.Dock = DockStyle.Right;
			return button;
		}
		CheckEdit CreateCheckEdit(Control parent, SummaryItemType summaryType, EventHandler checkedChanged, int left, int top) {
			CheckEdit checkEdit = new CheckEdit();
			checkEdit.Parent = parent;
			checkEdit.Text = GridSummaryItemsEditorController.GetNameBySummaryType(summaryType);
			checkEdit.Tag = summaryType;
			checkEdit.Width = 80;
			checkEdit.Height = 22;
			checkEdit.Left = left;
			checkEdit.Top = top;
			checkEdit.Enabled = false;
			checkEdit.Properties.AutoWidth = true;
			checkEdit.CheckedChanged += checkedChanged;
			return checkEdit;
		}
		IEnumerable<CheckEdit> CreateCheckEditsForSummaries(Control parent) {
			List<CheckEdit> checkEdits = new List<CheckEdit>();
			EventHandler checkedChanged = new EventHandler(checkEdit_CheckedChanged);
			int top = this.items.Top;
			int left = this.items.Width + 20;
			checkEdits.Add(CreateCheckEdit(parent, SummaryItemType.Max, checkedChanged, left, top));
			checkEdits.Add(CreateCheckEdit(parent, SummaryItemType.Min, checkedChanged, left, top + 25));
			checkEdits.Add(CreateCheckEdit(parent, SummaryItemType.Average, checkedChanged, left, top + 50));
			checkEdits.Add(CreateCheckEdit(parent, SummaryItemType.Sum, checkedChanged, left, top + 75));
			return checkEdits;
		}
		CheckEdit CreateCheckEditForCount(Control parent) {
			CheckEdit checkEdit = CreateCheckEdit(parent, SummaryItemType.Count, null, 10, 15);
			checkEdit.Enabled = true;
			checkEdit.CheckState = (Controller.HasCountByEmptyField ? CheckState.Checked : CheckState.Unchecked);
			checkEdit.CheckedChanged += new EventHandler(checkEditForCount_CheckedChanged);
			return checkEdit;
		}
		void checkEdit_CheckedChanged(object sender, EventArgs e) {
			CheckEdit checkEdit = (CheckEdit)sender;
			SummaryItemType summaryType = (SummaryItemType)checkEdit.Tag;
			Controller[ActiveUIItem.FieldName][summaryType] = checkEdit.Checked;
			this.items.Refresh();
		}
		void checkEditForCount_CheckedChanged(object sender, EventArgs e) {
			Controller.HasCountByEmptyField = !Controller.HasCountByEmptyField;
		}
	}
}
