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

namespace DevExpress.XtraRichEdit.Forms {
	partial class ColumnsSetupForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnsSetupForm));
			DevExpress.Office.DocumentModelUnitTwipsConverter documentModelUnitTwipsConverter1 = new DevExpress.Office.DocumentModelUnitTwipsConverter();
			this.grpPresets = new DevExpress.XtraEditors.GroupControl();
			this.columnsPresetControlRight = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlLeft = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlThree = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlTwo = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlOne = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.grpColumns = new DevExpress.XtraEditors.GroupControl();
			this.columnsEdit = new DevExpress.XtraRichEdit.Design.ColumnsEdit();
			this.chkEqualColumnWidth = new DevExpress.XtraEditors.CheckEdit();
			this.lblColumnCount = new DevExpress.XtraEditors.LabelControl();
			this.edtColumnCount = new DevExpress.XtraEditors.SpinEdit();
			this.chkLineBetween = new DevExpress.XtraEditors.CheckEdit();
			this.cbApplyTo = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblApplyTo = new DevExpress.XtraEditors.LabelControl();
			this.chkStartNewColumn = new DevExpress.XtraEditors.CheckEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.grpPresets)).BeginInit();
			this.grpPresets.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpColumns)).BeginInit();
			this.grpColumns.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkEqualColumnWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtColumnCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLineBetween.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbApplyTo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkStartNewColumn.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.grpPresets, "grpPresets");
			this.grpPresets.Controls.Add(this.columnsPresetControlRight);
			this.grpPresets.Controls.Add(this.columnsPresetControlLeft);
			this.grpPresets.Controls.Add(this.columnsPresetControlThree);
			this.grpPresets.Controls.Add(this.columnsPresetControlTwo);
			this.grpPresets.Controls.Add(this.columnsPresetControlOne);
			this.grpPresets.Name = "grpPresets";
			resources.ApplyResources(this.columnsPresetControlRight, "columnsPresetControlRight");
			this.columnsPresetControlRight.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlRight.Appearance.BackColor")));
			this.columnsPresetControlRight.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlRight.Image = new RightNarrowColumnsInfoPreset().Image;
			this.columnsPresetControlRight.Name = "columnsPresetControlRight";
			resources.ApplyResources(this.columnsPresetControlLeft, "columnsPresetControlLeft");
			this.columnsPresetControlLeft.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlLeft.Appearance.BackColor")));
			this.columnsPresetControlLeft.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlLeft.Image = new LeftNarrowColumnsInfoPreset().Image;
			this.columnsPresetControlLeft.Name = "columnsPresetControlLeft";
			resources.ApplyResources(this.columnsPresetControlThree, "columnsPresetControlThree");
			this.columnsPresetControlThree.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlThree.Appearance.BackColor")));
			this.columnsPresetControlThree.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlThree.Image = new ThreeUniformColumnsInfoPreset().Image;
			this.columnsPresetControlThree.Name = "columnsPresetControlThree";
			resources.ApplyResources(this.columnsPresetControlTwo, "columnsPresetControlTwo");
			this.columnsPresetControlTwo.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlTwo.Appearance.BackColor")));
			this.columnsPresetControlTwo.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlTwo.Image = new TwoUniformColumnsInfoPreset().Image;
			this.columnsPresetControlTwo.Name = "columnsPresetControlTwo";
			resources.ApplyResources(this.columnsPresetControlOne, "columnsPresetControlOne");
			this.columnsPresetControlOne.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlOne.Appearance.BackColor")));
			this.columnsPresetControlOne.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlOne.Image = new SingleColumnsInfoPreset().Image;
			this.columnsPresetControlOne.Name = "columnsPresetControlOne";
			resources.ApplyResources(this.grpColumns, "grpColumns");
			this.grpColumns.Controls.Add(this.columnsEdit);
			this.grpColumns.Controls.Add(this.chkEqualColumnWidth);
			this.grpColumns.Name = "grpColumns";
			this.columnsEdit.DefaultUnitType = DevExpress.Office.DocumentUnit.Inch;
			resources.ApplyResources(this.columnsEdit, "columnsEdit");
			this.columnsEdit.Name = "columnsEdit";
			this.columnsEdit.ValueUnitConverter = documentModelUnitTwipsConverter1;
			resources.ApplyResources(this.chkEqualColumnWidth, "chkEqualColumnWidth");
			this.chkEqualColumnWidth.Name = "chkEqualColumnWidth";
			this.chkEqualColumnWidth.Properties.AutoWidth = true;
			this.chkEqualColumnWidth.Properties.Caption = resources.GetString("chkEqualColumnWidth.Properties.Caption");
			this.chkEqualColumnWidth.CheckedChanged += new System.EventHandler(this.OnEqualColumnWidthChanged);
			resources.ApplyResources(this.lblColumnCount, "lblColumnCount");
			this.lblColumnCount.Name = "lblColumnCount";
			resources.ApplyResources(this.edtColumnCount, "edtColumnCount");
			this.edtColumnCount.MaximumSize = new System.Drawing.Size(100, 0);
			this.edtColumnCount.Name = "edtColumnCount";
			this.edtColumnCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtColumnCount.Properties.Mask.EditMask = resources.GetString("edtColumnCount.Properties.Mask.EditMask");
			this.edtColumnCount.Properties.MaxValue = new decimal(new int[] {
			64,
			0,
			0,
			0});
			resources.ApplyResources(this.chkLineBetween, "chkLineBetween");
			this.chkLineBetween.Name = "chkLineBetween";
			this.chkLineBetween.Properties.AutoWidth = true;
			this.chkLineBetween.Properties.Caption = resources.GetString("chkLineBetween.Properties.Caption");
			resources.ApplyResources(this.cbApplyTo, "cbApplyTo");
			this.cbApplyTo.Name = "cbApplyTo";
			this.cbApplyTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbApplyTo.Properties.Buttons"))))});
			this.cbApplyTo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblApplyTo, "lblApplyTo");
			this.lblApplyTo.Name = "lblApplyTo";
			resources.ApplyResources(this.chkStartNewColumn, "chkStartNewColumn");
			this.chkStartNewColumn.Name = "chkStartNewColumn";
			this.chkStartNewColumn.Properties.AutoWidth = true;
			this.chkStartNewColumn.Properties.Caption = resources.GetString("chkStartNewColumn.Properties.Caption");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.chkStartNewColumn);
			this.Controls.Add(this.lblApplyTo);
			this.Controls.Add(this.cbApplyTo);
			this.Controls.Add(this.chkLineBetween);
			this.Controls.Add(this.edtColumnCount);
			this.Controls.Add(this.lblColumnCount);
			this.Controls.Add(this.grpColumns);
			this.Controls.Add(this.grpPresets);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColumnsSetupForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.grpPresets)).EndInit();
			this.grpPresets.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grpColumns)).EndInit();
			this.grpColumns.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkEqualColumnWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtColumnCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLineBetween.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbApplyTo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkStartNewColumn.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.GroupControl grpPresets;
		protected DevExpress.XtraEditors.GroupControl grpColumns;
		protected DevExpress.XtraEditors.LabelControl lblColumnCount;
		protected DevExpress.XtraEditors.SpinEdit edtColumnCount;
		protected DevExpress.XtraEditors.CheckEdit chkLineBetween;
		protected DevExpress.XtraEditors.CheckEdit chkEqualColumnWidth;
		protected DevExpress.XtraEditors.ComboBoxEdit cbApplyTo;
		protected DevExpress.XtraEditors.LabelControl lblApplyTo;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.CheckEdit chkStartNewColumn;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraRichEdit.Design.ColumnsEdit columnsEdit;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlOne;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlTwo;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlRight;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlLeft;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlThree;
	}
}
