#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Collections;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Design {
	public class PanelsCollectionEditorForm : DevExpress.XtraEditors.XtraForm, ICollectionEditorForm {
		private System.Windows.Forms.Panel panel1;
		private DevExpress.XtraEditors.SimpleButton simpleButton6;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private ListBoxControl listBoxControl2;
		private ListBoxControl listBoxControl1;
		private SimpleButton AddAll;
		private SimpleButton RemoveSelected;
		private SimpleButton RemoveAll;
		private SimpleButton AddSelected;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
		private DevExpress.XtraEditors.SimpleButton simpleButton5;
		private void InitializeComponent() {
			this.panel1 = new System.Windows.Forms.Panel();
			this.simpleButton6 = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButton5 = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.listBoxControl2 = new DevExpress.XtraEditors.ListBoxControl();
			this.RemoveAll = new DevExpress.XtraEditors.SimpleButton();
			this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
			this.AddAll = new DevExpress.XtraEditors.SimpleButton();
			this.AddSelected = new DevExpress.XtraEditors.SimpleButton();
			this.RemoveSelected = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			this.panel1.Controls.Add(this.simpleButton6);
			this.panel1.Controls.Add(this.simpleButton5);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(10, 502);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(2);
			this.panel1.Size = new System.Drawing.Size(710, 33);
			this.panel1.TabIndex = 1;
			this.simpleButton6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.simpleButton6.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.simpleButton6.Location = new System.Drawing.Point(633, 10);
			this.simpleButton6.Margin = new System.Windows.Forms.Padding(0);
			this.simpleButton6.Name = "simpleButton6";
			this.simpleButton6.Size = new System.Drawing.Size(75, 23);
			this.simpleButton6.TabIndex = 1;
			this.simpleButton6.Text = "Cancel";
			this.simpleButton5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.simpleButton5.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButton5.Location = new System.Drawing.Point(546, 10);
			this.simpleButton5.Margin = new System.Windows.Forms.Padding(12);
			this.simpleButton5.Name = "simpleButton5";
			this.simpleButton5.Size = new System.Drawing.Size(75, 23);
			this.simpleButton5.TabIndex = 0;
			this.simpleButton5.Text = "OK";
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.listBoxControl2);
			this.layoutControl1.Controls.Add(this.RemoveAll);
			this.layoutControl1.Controls.Add(this.listBoxControl1);
			this.layoutControl1.Controls.Add(this.AddAll);
			this.layoutControl1.Controls.Add(this.AddSelected);
			this.layoutControl1.Controls.Add(this.RemoveSelected);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(10, 9);
			this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(710, 493);
			this.layoutControl1.TabIndex = 2;
			this.layoutControl1.Text = "layoutControl1";
			this.listBoxControl2.Location = new System.Drawing.Point(376, 18);
			this.listBoxControl2.Name = "listBoxControl2";
			this.listBoxControl2.Size = new System.Drawing.Size(332, 473);
			this.listBoxControl2.StyleController = this.layoutControl1;
			this.listBoxControl2.TabIndex = 14;
			this.listBoxControl2.DoubleClick += new EventHandler(listBoxControl2_DoubleClick);
			this.RemoveAll.Location = new System.Drawing.Point(332, 200);
			this.RemoveAll.Name = "RemoveAll";
			this.RemoveAll.Size = new System.Drawing.Size(32, 22);
			this.RemoveAll.StyleController = this.layoutControl1;
			this.RemoveAll.TabIndex = 13;
			this.RemoveAll.Text = "<<";
			this.RemoveAll.Click += new EventHandler(RemoveAll_Click);
			this.listBoxControl1.Location = new System.Drawing.Point(2, 18);
			this.listBoxControl1.Name = "listBoxControl1";
			this.listBoxControl1.Size = new System.Drawing.Size(318, 473);
			this.listBoxControl1.StyleController = this.layoutControl1;
			this.listBoxControl1.TabIndex = 12;
			this.listBoxControl1.DoubleClick += new EventHandler(listBoxControl1_DoubleClick);
			this.AddAll.Location = new System.Drawing.Point(332, 174);
			this.AddAll.Name = "AddAll";
			this.AddAll.Size = new System.Drawing.Size(32, 22);
			this.AddAll.StyleController = this.layoutControl1;
			this.AddAll.TabIndex = 12;
			this.AddAll.Text = ">>";
			this.AddAll.Click += new EventHandler(AddAll_Click);
			this.AddSelected.Location = new System.Drawing.Point(332, 148);
			this.AddSelected.Name = "AddSelected";
			this.AddSelected.Size = new System.Drawing.Size(32, 22);
			this.AddSelected.StyleController = this.layoutControl1;
			this.AddSelected.TabIndex = 11;
			this.AddSelected.Text = ">";
			this.AddSelected.Click += new EventHandler(AddSelected_Click);
			this.RemoveSelected.Location = new System.Drawing.Point(332, 226);
			this.RemoveSelected.Name = "RemoveSelected";
			this.RemoveSelected.Size = new System.Drawing.Size(32, 22);
			this.RemoveSelected.StyleController = this.layoutControl1;
			this.RemoveSelected.TabIndex = 14;
			this.RemoveSelected.Text = "<";
			this.RemoveSelected.Click+=new EventHandler(RemoveSelected_Click);
			this.layoutControlGroup1.AllowCustomizeChildren = false;
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem1,
			this.layoutControlItem6,
			this.layoutControlItem3,
			this.emptySpaceItem1,
			this.emptySpaceItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(710, 493);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem2.Control = this.listBoxControl2;
			this.layoutControlItem2.CustomizationFormText = "Selected Items";
			this.layoutControlItem2.Location = new System.Drawing.Point(374, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(336, 493);
			this.layoutControlItem2.Text = "Selected items:";
			this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(75, 13);
			this.layoutControlItem4.Control = this.listBoxControl1;
			this.layoutControlItem4.CustomizationFormText = "Available Items";
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(322, 493);
			this.layoutControlItem4.Text = "Available items:";
			this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(75, 13);
			this.layoutControlItem5.Control = this.RemoveAll;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
			this.layoutControlItem5.Location = new System.Drawing.Point(322, 198);
			this.layoutControlItem5.MaxSize = new System.Drawing.Size(36, 26);
			this.layoutControlItem5.MinSize = new System.Drawing.Size(31, 26);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(52, 26);
			this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem5.Text = "layoutControlItem5";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem1.Control = this.AddSelected;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.FillControlToClientArea = false;
			this.layoutControlItem1.Location = new System.Drawing.Point(322, 146);
			this.layoutControlItem1.MaxSize = new System.Drawing.Size(36, 26);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(23, 22);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(52, 26);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem6.Control = this.RemoveSelected;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
			this.layoutControlItem6.Location = new System.Drawing.Point(322, 224);
			this.layoutControlItem6.MaxSize = new System.Drawing.Size(36, 26);
			this.layoutControlItem6.MinSize = new System.Drawing.Size(23, 26);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(52, 26);
			this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem6.Text = "layoutControlItem6";
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextToControlDistance = 0;
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem3.Control = this.AddAll;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.FillControlToClientArea = false;
			this.layoutControlItem3.Location = new System.Drawing.Point(322, 172);
			this.layoutControlItem3.MaxSize = new System.Drawing.Size(36, 26);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(31, 26);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(52, 26);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(322, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(52, 146);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(322, 250);
			this.emptySpaceItem2.MaxSize = new System.Drawing.Size(52, 243);
			this.emptySpaceItem2.MinSize = new System.Drawing.Size(52, 243);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(52, 243);
			this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.ClientSize = new System.Drawing.Size(730, 547);
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.panel1);
			this.MinimumSize = new System.Drawing.Size(746, 585);
			this.Name = "PanelsCollectionEditorForm";
			this.Padding = new System.Windows.Forms.Padding(10, 9, 10, 12);
			this.AcceptButton = this.simpleButton5;
			this.CancelButton = this.simpleButton6;
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);
		}
		private void listBoxControl2_DoubleClick(object sender, EventArgs e) {
			MoveItem(listBoxControl2, listBoxControl1);
		}
		private void listBoxControl1_DoubleClick(object sender, EventArgs e) {
			MoveItem(listBoxControl1, listBoxControl2);
		}
		private void AddSelected_Click(object sender, EventArgs e) {
			MoveItem(listBoxControl1, listBoxControl2);
		}
		private void MoveItem(ListBoxControl from, ListBoxControl to) {
			if(from.SelectedItem == null && from.Items.Count != 0) {
				from.SelectedItem = from.Items[0];
			}
			if(from.SelectedItem != null) {
				to.Items.Add(from.SelectedItem);
				from.Items.Remove(from.SelectedItem);
			}
		}
		private void RemoveSelected_Click(object sender, EventArgs e) {
			MoveItem(listBoxControl2, listBoxControl1);
		}
		private void MoveAll(ListBoxControl from, ListBoxControl to) {
			while(from.Items.Count != 0) {
				to.Items.Add(from.Items[0]);
				from.Items.RemoveAt(0);
			}
		}
		private void AddAll_Click(object sender, EventArgs e) {
			MoveAll(listBoxControl1, listBoxControl2);
		}
		private void RemoveAll_Click(object sender, EventArgs e) {
			MoveAll(listBoxControl2, listBoxControl1);
		}
		public PanelsCollectionEditorForm()
			: base() {
			InitializeComponent();
		}
		#region ICollectionEditorForm Members
		public IList DataSource {
			get { return (IList)listBoxControl1.Items; }
			set { SetItems(listBoxControl1, value); }
		}
		private void SetItems(ListBoxControl control, ICollection items) {
			control.Items.Clear();
			foreach(object item in items) {
				control.Items.Add(item);
			}
		}
		public ICollection EditValue {
			get { return (IList)listBoxControl2.Items; }
			set {
				SetItems(listBoxControl2, value);
				UpdateDataSource(value);
			}
		}
		private void UpdateDataSource(ICollection value) {
			foreach(object item in value) {
				if(listBoxControl1.Items.Contains(item)) {
					listBoxControl1.Items.Remove(item);
				}
			}
		}
		#endregion
	}
}
