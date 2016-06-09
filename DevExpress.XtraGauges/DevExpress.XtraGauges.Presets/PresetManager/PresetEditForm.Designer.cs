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

namespace DevExpress.XtraGauges.Presets.PresetManager {
	partial class PresetEditControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.saveButton = new DevExpress.XtraEditors.SimpleButton();
			this.categoryEdit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.nameEdit = new DevExpress.XtraEditors.TextEdit();
			this.descriptionEdit = new DevExpress.XtraEditors.MemoEdit();
			this.previewEdit = new DevExpress.XtraEditors.PictureEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.categoryEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nameEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.descriptionEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.previewEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.saveButton);
			this.layoutControl1.Controls.Add(this.categoryEdit);
			this.layoutControl1.Controls.Add(this.nameEdit);
			this.layoutControl1.Controls.Add(this.descriptionEdit);
			this.layoutControl1.Controls.Add(this.previewEdit);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(530, 377);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.saveButton.Location = new System.Drawing.Point(393, 350);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(132, 22);
			this.saveButton.StyleController = this.layoutControl1;
			this.saveButton.TabIndex = 9;
			this.saveButton.Text = "Save Preset";
			this.saveButton.Click += new System.EventHandler(this.okButton_Click);
			this.categoryEdit.Location = new System.Drawing.Point(53, 1);
			this.categoryEdit.Name = "categoryEdit";
			this.categoryEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.categoryEdit.Size = new System.Drawing.Size(207, 20);
			this.categoryEdit.StyleController = this.layoutControl1;
			this.categoryEdit.TabIndex = 7;
			this.nameEdit.Location = new System.Drawing.Point(300, 1);
			this.nameEdit.Name = "nameEdit";
			this.nameEdit.Size = new System.Drawing.Size(225, 20);
			this.nameEdit.StyleController = this.layoutControl1;
			this.nameEdit.TabIndex = 6;
			this.descriptionEdit.Location = new System.Drawing.Point(6, 42);
			this.descriptionEdit.Name = "descriptionEdit";
			this.descriptionEdit.Size = new System.Drawing.Size(519, 43);
			this.descriptionEdit.StyleController = this.layoutControl1;
			this.descriptionEdit.TabIndex = 5;
			this.previewEdit.Location = new System.Drawing.Point(6, 106);
			this.previewEdit.Name = "previewEdit";
			this.previewEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.previewEdit.Properties.Appearance.Options.UseBackColor = true;
			this.previewEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
			this.previewEdit.Size = new System.Drawing.Size(519, 238);
			this.previewEdit.StyleController = this.layoutControl1;
			this.previewEdit.TabIndex = 4;
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem4,
			this.layoutControlItem3,
			this.layoutControlItem1,
			this.layoutControlItem6,
			this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.OptionsItemText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeGroup.AutoSize;
			this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 2;
			this.layoutControlGroup1.Size = new System.Drawing.Size(530, 377);
			this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem2.Control = this.descriptionEdit;
			this.layoutControlItem2.CustomizationFormText = "Description";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 21);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(61, 50);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
			this.layoutControlItem2.Size = new System.Drawing.Size(530, 64);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.Text = "Description";
			this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(50, 20);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem4.Control = this.categoryEdit;
			this.layoutControlItem4.CustomizationFormText = "Category";
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
			this.layoutControlItem4.Size = new System.Drawing.Size(265, 21);
			this.layoutControlItem4.Text = "Category";
			this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(45, 20);
			this.layoutControlItem4.TextToControlDistance = 2;
			this.layoutControlItem3.Control = this.nameEdit;
			this.layoutControlItem3.CustomizationFormText = "Name";
			this.layoutControlItem3.Location = new System.Drawing.Point(265, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
			this.layoutControlItem3.Size = new System.Drawing.Size(265, 21);
			this.layoutControlItem3.Text = "Name";
			this.layoutControlItem3.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutControlItem3.TextSize = new System.Drawing.Size(27, 20);
			this.layoutControlItem3.TextToControlDistance = 2;
			this.layoutControlItem1.Control = this.previewEdit;
			this.layoutControlItem1.CustomizationFormText = "Preview";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 85);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(530, 259);
			this.layoutControlItem1.Text = "Preview";
			this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(50, 20);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem6.Control = this.saveButton;
			this.layoutControlItem6.CustomizationFormText = "layoutControlItem6";
			this.layoutControlItem6.Location = new System.Drawing.Point(387, 344);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(143, 33);
			this.layoutControlItem6.Text = "layoutControlItem6";
			this.layoutControlItem6.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextToControlDistance = 0;
			this.layoutControlItem6.TextVisible = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 344);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(387, 33);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "PresetEditControl";
			this.Size = new System.Drawing.Size(530, 377);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.categoryEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nameEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.descriptionEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.previewEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraEditors.ComboBoxEdit categoryEdit;
		private DevExpress.XtraEditors.TextEdit nameEdit;
		private DevExpress.XtraEditors.MemoEdit descriptionEdit;
		private DevExpress.XtraEditors.PictureEdit previewEdit;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraEditors.SimpleButton saveButton;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
	}
}
