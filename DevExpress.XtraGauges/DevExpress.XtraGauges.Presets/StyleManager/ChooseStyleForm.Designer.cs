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

using DevExpress.XtraGauges.Presets.PresetManager;
namespace DevExpress.XtraGauges.Presets.Styles {
	partial class ChooseStyleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseStyleForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.layout = new DevExpress.XtraLayout.LayoutControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.checkUseFilter = new DevExpress.XtraEditors.CheckEdit();
			this.gallery = new DevExpress.XtraGauges.Presets.Styles.StylesGallery();
			this.cbFilter = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			this.groupFilter = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layout)).BeginInit();
			this.layout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.checkUseFilter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gallery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.MaximumSize = new System.Drawing.Size(80, 0);
			this.btnCancel.MinimumSize = new System.Drawing.Size(80, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layout;
			this.layout.Controls.Add(this.btnApply);
			this.layout.Controls.Add(this.btnOk);
			this.layout.Controls.Add(this.btnCancel);
			this.layout.Controls.Add(this.checkUseFilter);
			this.layout.Controls.Add(this.gallery);
			this.layout.Controls.Add(this.cbFilter);
			resources.ApplyResources(this.layout, "layout");
			this.layout.Name = "layout";
			this.layout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1641, 189, 450, 350);
			this.layout.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.MaximumSize = new System.Drawing.Size(80, 0);
			this.btnApply.MinimumSize = new System.Drawing.Size(80, 0);
			this.btnApply.Name = "btnApply";
			this.btnApply.StyleController = this.layout;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.MaximumSize = new System.Drawing.Size(80, 0);
			this.btnOk.MinimumSize = new System.Drawing.Size(80, 0);
			this.btnOk.Name = "btnOk";
			this.btnOk.StyleController = this.layout;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.checkUseFilter.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.checkUseFilter, "checkUseFilter");
			this.checkUseFilter.Name = "checkUseFilter";
			this.checkUseFilter.Properties.AutoWidth = true;
			this.checkUseFilter.Properties.Caption = resources.GetString("checkUseFilter.Properties.Caption");
			this.checkUseFilter.StyleController = this.layout;
			this.gallery.BackColor = System.Drawing.SystemColors.Window;
			this.gallery.ItemImageScaleFactor = 0.95F;
			this.gallery.ItemSize = new System.Drawing.Size(0, 0);
			this.gallery.ItemTextVerticalOffset = 0.85F;
			resources.ApplyResources(this.gallery, "gallery");
			this.gallery.Name = "gallery";
			this.gallery.SelectedIndex = -1;
			resources.ApplyResources(this.cbFilter, "cbFilter");
			this.cbFilter.MaximumSize = new System.Drawing.Size(100, 0);
			this.cbFilter.MinimumSize = new System.Drawing.Size(75, 0);
			this.cbFilter.Name = "cbFilter";
			this.cbFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbFilter.Properties.Buttons"))))});
			this.cbFilter.Properties.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Skinned;
			this.cbFilter.StyleController = this.layout;
			resources.ApplyResources(this.layoutControlGroup1, "layoutControlGroup1");
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6,
			this.emptySpaceItem1,
			this.simpleSeparator1,
			this.groupFilter});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(594, 442);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem1.Control = this.gallery;
			resources.ApplyResources(this.layoutControlItem1, "layoutControlItem1");
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(594, 393);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem4.Control = this.btnApply;
			resources.ApplyResources(this.layoutControlItem4, "layoutControlItem4");
			this.layoutControlItem4.Location = new System.Drawing.Point(327, 395);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 12, 12);
			this.layoutControlItem4.Size = new System.Drawing.Size(86, 47);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.btnOk;
			resources.ApplyResources(this.layoutControlItem5, "layoutControlItem5");
			this.layoutControlItem5.Location = new System.Drawing.Point(413, 395);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 12, 12);
			this.layoutControlItem5.Size = new System.Drawing.Size(86, 47);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem6.Control = this.btnCancel;
			resources.ApplyResources(this.layoutControlItem6, "layoutControlItem6");
			this.layoutControlItem6.Location = new System.Drawing.Point(499, 395);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 12, 12, 12);
			this.layoutControlItem6.Size = new System.Drawing.Size(95, 47);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextToControlDistance = 0;
			this.layoutControlItem6.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			resources.ApplyResources(this.emptySpaceItem1, "emptySpaceItem1");
			this.emptySpaceItem1.Location = new System.Drawing.Point(317, 395);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(10, 47);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.simpleSeparator1.AllowHotTrack = false;
			resources.ApplyResources(this.simpleSeparator1, "simpleSeparator1");
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 393);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.simpleSeparator1.Size = new System.Drawing.Size(594, 2);
			resources.ApplyResources(this.groupFilter, "groupFilter");
			this.groupFilter.GroupBordersVisible = false;
			this.groupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.layoutControlItem2});
			this.groupFilter.Location = new System.Drawing.Point(0, 395);
			this.groupFilter.Name = "groupFilter";
			this.groupFilter.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.groupFilter.Size = new System.Drawing.Size(317, 47);
			this.layoutControlItem3.Control = this.checkUseFilter;
			resources.ApplyResources(this.layoutControlItem3, "layoutControlItem3");
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 12, 12);
			this.layoutControlItem3.Size = new System.Drawing.Size(221, 47);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem2.Control = this.cbFilter;
			resources.ApplyResources(this.layoutControlItem2, "layoutControlItem2");
			this.layoutControlItem2.Location = new System.Drawing.Point(221, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 12, 12);
			this.layoutControlItem2.Size = new System.Drawing.Size(96, 47);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.AcceptButton = this.btnOk;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.layout);
			this.MinimizeBox = false;
			this.Name = "ChooseStyleForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.layout)).EndInit();
			this.layout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.checkUseFilter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gallery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbFilter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected StylesGallery gallery;
		private DevExpress.XtraEditors.ImageComboBoxEdit cbFilter;
		private DevExpress.XtraEditors.CheckEdit checkUseFilter;
		protected DevExpress.XtraEditors.SimpleButton btnApply;
		private DevExpress.XtraLayout.LayoutControl layout;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.SimpleSeparator simpleSeparator1;
		private DevExpress.XtraLayout.LayoutControlGroup groupFilter;
	}
}
