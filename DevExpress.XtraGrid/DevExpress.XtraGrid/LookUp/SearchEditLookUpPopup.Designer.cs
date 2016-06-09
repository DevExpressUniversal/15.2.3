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

namespace DevExpress.XtraGrid.Editors {
	partial class SearchEditLookUpPopup {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lc = new DevExpress.XtraLayout.LayoutControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btClear = new DevExpress.XtraEditors.SimpleButton();
			this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
			this.teFind = new DevExpress.XtraEditors.TextEdit();
			this.btFind = new DevExpress.XtraEditors.SimpleButton();
			this.btAddNew = new DevExpress.XtraEditors.SimpleButton();
			this.lcMain = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgFind = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciLabelFind = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciButtonFind = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgAction = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciClear = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciAddNew = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.lc)).BeginInit();
			this.lc.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teFind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciLabelFind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonFind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAction)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciClear)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAddNew)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			this.lc.AllowCustomization = false;
			this.lc.Controls.Add(this.labelControl2);
			this.lc.Controls.Add(this.labelControl1);
			this.lc.Controls.Add(this.btClear);
			this.lc.Controls.Add(this.listBoxControl1);
			this.lc.Controls.Add(this.teFind);
			this.lc.Controls.Add(this.btFind);
			this.lc.Controls.Add(this.btAddNew);
			this.lc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lc.Location = new System.Drawing.Point(0, 0);
			this.lc.Name = "lc";
			this.lc.OptionsView.EnableTransparentBackColor = false;
			this.lc.Root = this.lcMain;
			this.lc.Size = new System.Drawing.Size(573, 400);
			this.lc.TabIndex = 0;
			this.lc.Text = "lc";
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl2.LineVisible = true;
			this.labelControl2.Location = new System.Drawing.Point(5, 356);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(563, 13);
			this.labelControl2.StyleController = this.lc;
			this.labelControl2.TabIndex = 10;
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Location = new System.Drawing.Point(5, 38);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(563, 13);
			this.labelControl1.StyleController = this.lc;
			this.labelControl1.TabIndex = 9;
			this.btClear.AutoWidthInLayoutControl = true;
			this.btClear.Location = new System.Drawing.Point(484, 373);
			this.btClear.MinimumSize = new System.Drawing.Size(78, 0);
			this.btClear.Name = "btClear";
			this.btClear.Size = new System.Drawing.Size(78, 22);
			this.btClear.StyleController = this.lc;
			this.btClear.TabIndex = 8;
			this.btClear.Text = "Clear";
			this.btClear.Click += new System.EventHandler(this.btClear_Click);
			this.listBoxControl1.Location = new System.Drawing.Point(5, 55);
			this.listBoxControl1.Name = "listBoxControl1";
			this.listBoxControl1.Size = new System.Drawing.Size(563, 297);
			this.listBoxControl1.StyleController = this.lc;
			this.listBoxControl1.TabIndex = 6;
			this.teFind.Location = new System.Drawing.Point(9, 13);
			this.teFind.MaximumSize = new System.Drawing.Size(300, 0);
			this.teFind.Name = "teFind";
			this.teFind.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
			this.teFind.Size = new System.Drawing.Size(300, 20);
			this.teFind.StyleController = this.lc;
			this.teFind.TabIndex = 4;
			this.teFind.EditValueChanged += new System.EventHandler(this.teFind_EditValueChanged);
			this.teFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teFind_KeyDown);
			this.btFind.AutoWidthInLayoutControl = true;
			this.btFind.Location = new System.Drawing.Point(315, 12);
			this.btFind.MinimumSize = new System.Drawing.Size(80, 0);
			this.btFind.Name = "btFind";
			this.btFind.Size = new System.Drawing.Size(80, 22);
			this.btFind.StyleController = this.lc;
			this.btFind.TabIndex = 5;
			this.btFind.Text = "Find";
			this.btFind.Click += new System.EventHandler(this.btFind_Click);
			this.btAddNew.AutoWidthInLayoutControl = true;
			this.btAddNew.Location = new System.Drawing.Point(390, 373);
			this.btAddNew.MinimumSize = new System.Drawing.Size(78, 0);
			this.btAddNew.Name = "btAddNew";
			this.btAddNew.Size = new System.Drawing.Size(78, 22);
			this.btAddNew.StyleController = this.lc;
			this.btAddNew.TabIndex = 7;
			this.btAddNew.Text = "Add New";
			this.btAddNew.Click += new System.EventHandler(this.btAddNew_Click);
			this.lcMain.CustomizationFormText = "lcMain";
			this.lcMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcMain.GroupBordersVisible = false;
			this.lcMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciGrid,
			this.lcgFind,
			this.lcgAction});
			this.lcMain.Location = new System.Drawing.Point(0, 0);
			this.lcMain.Name = "lcMain";
			this.lcMain.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 10, 3);
			this.lcMain.Size = new System.Drawing.Size(573, 400);
			this.lcMain.Text = "lcMain";
			this.lcMain.TextVisible = false;
			this.lciGrid.Control = this.listBoxControl1;
			this.lciGrid.CustomizationFormText = "lciGrid";
			this.lciGrid.Location = new System.Drawing.Point(0, 43);
			this.lciGrid.Name = "lciGrid";
			this.lciGrid.Size = new System.Drawing.Size(567, 301);
			this.lciGrid.Text = "lciGrid";
			this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
			this.lciGrid.TextToControlDistance = 0;
			this.lciGrid.TextVisible = false;
			this.lcgFind.CustomizationFormText = "lcgFind";
			this.lcgFind.GroupBordersVisible = false;
			this.lcgFind.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciLabelFind,
			this.lciButtonFind,
			this.layoutControlItem1});
			this.lcgFind.Location = new System.Drawing.Point(0, 0);
			this.lcgFind.Name = "lcgFind";
			this.lcgFind.Size = new System.Drawing.Size(567, 43);
			this.lcgFind.Text = "lcgFind";
			this.lcgFind.TextVisible = false;
			this.lciLabelFind.Control = this.teFind;
			this.lciLabelFind.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.lciLabelFind.CustomizationFormText = "Find:";
			this.lciLabelFind.Location = new System.Drawing.Point(0, 0);
			this.lciLabelFind.Name = "lciLabelFind";
			this.lciLabelFind.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 2, 2, 2);
			this.lciLabelFind.Size = new System.Drawing.Size(308, 26);
			this.lciLabelFind.Text = "Find:";
			this.lciLabelFind.TextSize = new System.Drawing.Size(0, 0);
			this.lciLabelFind.TextToControlDistance = 0;
			this.lciLabelFind.TextVisible = false;
			this.lciLabelFind.TrimClientAreaToControl = false;
			this.lciButtonFind.Control = this.btFind;
			this.lciButtonFind.CustomizationFormText = "layoutControlItem1";
			this.lciButtonFind.Location = new System.Drawing.Point(308, 0);
			this.lciButtonFind.Name = "lciButtonFind";
			this.lciButtonFind.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 2, 2);
			this.lciButtonFind.Size = new System.Drawing.Size(259, 26);
			this.lciButtonFind.Text = "lciButtonFind";
			this.lciButtonFind.TextSize = new System.Drawing.Size(0, 0);
			this.lciButtonFind.TextToControlDistance = 0;
			this.lciButtonFind.TextVisible = false;
			this.layoutControlItem1.Control = this.labelControl1;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(567, 17);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.lcgAction.CustomizationFormText = "lcgAction";
			this.lcgAction.GroupBordersVisible = false;
			this.lcgAction.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciClear,
			this.lciAddNew,
			this.emptySpaceItem1,
			this.layoutControlItem2});
			this.lcgAction.Location = new System.Drawing.Point(0, 344);
			this.lcgAction.Name = "lcgAction";
			this.lcgAction.Size = new System.Drawing.Size(567, 43);
			this.lcgAction.Text = "lcgAction";
			this.lcgAction.TextVisible = false;
			this.lciClear.Control = this.btClear;
			this.lciClear.CustomizationFormText = "lciClear";
			this.lciClear.Location = new System.Drawing.Point(473, 17);
			this.lciClear.Name = "lciClear";
			this.lciClear.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 8, 2, 2);
			this.lciClear.Size = new System.Drawing.Size(94, 26);
			this.lciClear.Text = "lciClear";
			this.lciClear.TextSize = new System.Drawing.Size(0, 0);
			this.lciClear.TextToControlDistance = 0;
			this.lciClear.TextVisible = false;
			this.lciAddNew.Control = this.btAddNew;
			this.lciAddNew.CustomizationFormText = "lciAddNew";
			this.lciAddNew.Location = new System.Drawing.Point(379, 17);
			this.lciAddNew.Name = "lciAddNew";
			this.lciAddNew.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 8, 2, 2);
			this.lciAddNew.Size = new System.Drawing.Size(94, 26);
			this.lciAddNew.Text = "lciAddNew";
			this.lciAddNew.TextSize = new System.Drawing.Size(0, 0);
			this.lciAddNew.TextToControlDistance = 0;
			this.lciAddNew.TextVisible = false;
			this.lciAddNew.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 17);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(379, 26);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.Control = this.labelControl2;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(567, 17);
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lc);
			this.Name = "SearchEditLookUpPopup";
			this.Size = new System.Drawing.Size(573, 400);
			((System.ComponentModel.ISupportInitialize)(this.lc)).EndInit();
			this.lc.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teFind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciLabelFind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciButtonFind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAction)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciClear)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAddNew)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public DevExpress.XtraLayout.LayoutControl lc;
		internal DevExpress.XtraEditors.TextEdit teFind;
		private DevExpress.XtraLayout.LayoutControlGroup lcMain;
		public DevExpress.XtraLayout.LayoutControlItem lciLabelFind;
		private DevExpress.XtraEditors.SimpleButton btClear;
		private DevExpress.XtraEditors.SimpleButton btFind;
		private DevExpress.XtraEditors.SimpleButton btAddNew;
		private DevExpress.XtraLayout.LayoutControlGroup lcgFind;
		private DevExpress.XtraLayout.LayoutControlItem lciButtonFind;
		private DevExpress.XtraLayout.LayoutControlItem lciClear;
		private DevExpress.XtraLayout.LayoutControlItem lciAddNew;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		public DevExpress.XtraLayout.LayoutControlGroup lcgAction;
		private DevExpress.XtraEditors.ListBoxControl listBoxControl1;
		public DevExpress.XtraLayout.LayoutControlItem lciGrid;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
	}
}
