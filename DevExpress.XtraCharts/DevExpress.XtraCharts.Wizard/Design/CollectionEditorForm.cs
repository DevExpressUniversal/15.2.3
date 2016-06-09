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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
namespace DevExpress.XtraCharts.Design {
	public class CollectionEditorForm : XtraForm {
		Chart chart;
		SolidBrush foreBrush = null;
		protected ListBoxControl fListBox;
		protected SimpleButton btnUp;
		protected SimpleButton btnDown;
		protected SimpleButton btnAdd;
		protected SimpleButton btnRemove;
		protected SimpleButton btnClose;
		protected LabelControl labelControl1;
		protected PropertyGridControl propertyGrid;
		protected virtual object[] CollectionToArray { get { return new object[0]; } }
		protected virtual bool SelectableItems { get { return false; } }
		protected Chart Chart { get { return chart; } }
		public CollectionEditorForm() {
			InitializeComponent();
			btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		}
		void UpdateButtons() {
			btnRemove.Enabled =
				fListBox.Items.Count > 0 &&
				fListBox.SelectedIndex >= 0 &&
				fListBox.SelectedIndex < fListBox.Items.Count;
			btnUp.Enabled =
				fListBox.Items.Count > 1 &&
				fListBox.SelectedIndex > 0;
			btnDown.Enabled =
				fListBox.Items.Count > 1 &&
				fListBox.SelectedIndex < fListBox.Items.Count - 1;
		}
		void SetSelectedIndex(int index) {
			if (index < 0)
				index = 0;
			if (index >= fListBox.Items.Count)
				index = fListBox.Items.Count - 1;
			if (index >= 0)
				fListBox.SelectedIndex = index;
		}
		void FillListBox() {
			fListBox.Items.Clear();
			fListBox.Items.AddRange(CollectionToArray);
		}
		void UpdatePropertyGrid() {
			propertyGrid.SelectedObject = fListBox.SelectedItem;
		}
		void SelectedItemChanged() {
			UpdateButtons();
			UpdatePropertyGrid();
			if (SelectableItems && chart != null)
				chart.SelectHitElement((IHitTest)fListBox.SelectedItem);
			if (chart != null)
				chart.Container.RenderProvider.Invalidate();
		}
		void btnAdd_Click(object sender, System.EventArgs e) {
			object[] items = AddItems();
			if (items != null && items.Length > 0) {
				FillListBox();
				int index = fListBox.Items.IndexOf(items[items.Length - 1]);
				SetSelectedIndex(index);
			}
		}
		void btnRemove_Click(object sender, System.EventArgs e) {
			object item = fListBox.SelectedItem;
			if (item != null) {
				int index = fListBox.SelectedIndex;
				RemoveItem(item);
				FillListBox();
				SetSelectedIndex(index);
				if (fListBox.Items.Count == 0)
					SelectedItemChanged();
			}
		}
		void fListBox_SelectedIndexChanged(object sender, System.EventArgs e) {
			SelectedItemChanged();
		}
		void btnUp_Click(object sender, EventArgs e) {
			int index = fListBox.SelectedIndex;
			Swap(index - 1, index);
			FillListBox();
			SetSelectedIndex(index - 1);
		}
		void btnDown_Click(object sender, EventArgs e) {
			int index = fListBox.SelectedIndex;
			Swap(index, index + 1);
			FillListBox();
			SetSelectedIndex(index + 1);
		}
		void fListBox_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			int index = e.Index;
			if (index >= 0 || index < CollectionToArray.Length) {
				Rectangle bounds = e.Bounds;
				Graphics graphics = e.Graphics;
				AppearanceObject appearance = e.Appearance;
				Brush brush;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
					UserLookAndFeel lookAndFeel = fListBox.LookAndFeel;
					using (Brush backBrush = new SolidBrush(LookAndFeelHelper.GetSystemColor(lookAndFeel, SystemColors.Highlight)))
						graphics.FillRectangle(backBrush, bounds);
					brush = new SolidBrush(LookAndFeelHelper.GetSystemColor(lookAndFeel, SystemColors.HighlightText));
				}
				else
					brush = appearance.GetForeBrush(new GraphicsCache(graphics));
				graphics.DrawString(GetItemDisplayText(index), appearance.Font, brush, bounds);
				e.Handled = true;
			}
		}
		void propertyGridControl1_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			int index = fListBox.SelectedIndex;
			FillListBox();
			SetSelectedIndex(index);
		}
		void btnClose_Click(object sender, EventArgs e) {
			Controls.Clear();	   
			Close();
		}
		void DisposeForeBrush() {
			if (foreBrush != null) {
				foreBrush.Dispose();
				foreBrush = null;
			}
		}
		void InitializeForeBrush(Color foreColor) {
			if (foreBrush == null)
				foreBrush = new SolidBrush(foreColor);
			else if (foreBrush.Color != foreColor) {
				DisposeForeBrush();
				foreBrush = new SolidBrush(foreColor);
			}
		}
		protected virtual object[] AddItems() {
			return null;
		}
		protected virtual void RemoveItem(object item) {
		}
		protected virtual string GetItemDisplayText(int index) {
			return "";
		}
		protected virtual void Swap(int index1, int index2) {
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				DisposeForeBrush();
			}
			base.Dispose(disposing);
		}
		public void Initialize(Chart chart) {
			this.chart = chart;
			if (chart != null && chart.Container != null && chart.Container.ServiceProvider != null)
				propertyGrid.ServiceProvider = chart.Container;
			FillListBox();
			SetSelectedIndex(0);
			UpdateButtons();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionEditorForm));
			this.fListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.fListBox, "fListBox");
			this.fListBox.Name = "fListBox";
			this.fListBox.SelectedIndexChanged += new System.EventHandler(this.fListBox_SelectedIndexChanged);
			this.fListBox.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.fListBox_DrawItem);
			this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
			this.btnUp.ImageIndex = 0;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
			this.btnDown.ImageIndex = 1;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.propertyGridControl1_CellValueChanged);
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.fListBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CollectionEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.fListBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
}
