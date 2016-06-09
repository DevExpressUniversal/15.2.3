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
	partial class TableOfContentsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableOfContentsForm));
			this.chkShowPageNumbers = new DevExpress.XtraEditors.CheckEdit();
			this.chkRightAlignPageNumbers = new DevExpress.XtraEditors.CheckEdit();
			this.chkUseHyperlinks = new DevExpress.XtraEditors.CheckEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.spEditShowLevels = new DevExpress.XtraEditors.SpinEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.BtnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.previewControl = new DevExpress.XtraRichEdit.Native.PreviewRichEditControl();
			((System.ComponentModel.ISupportInitialize)(this.chkShowPageNumbers.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRightAlignPageNumbers.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUseHyperlinks.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spEditShowLevels.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkShowPageNumbers, "chkShowPageNumbers");
			this.chkShowPageNumbers.Name = "chkShowPageNumbers";
			this.chkShowPageNumbers.Properties.Caption = resources.GetString("chkShowPageNumbers.Properties.Caption");
			this.chkShowPageNumbers.CheckedChanged += new System.EventHandler(this.chkShowPageNumbers_CheckedChanged);
			resources.ApplyResources(this.chkRightAlignPageNumbers, "chkRightAlignPageNumbers");
			this.chkRightAlignPageNumbers.Name = "chkRightAlignPageNumbers";
			this.chkRightAlignPageNumbers.Properties.Caption = resources.GetString("chkRightAlignPageNumbers.Properties.Caption");
			this.chkRightAlignPageNumbers.CheckedChanged += new System.EventHandler(this.chkRightAlignPageNumbers_CheckedChanged);
			resources.ApplyResources(this.chkUseHyperlinks, "chkUseHyperlinks");
			this.chkUseHyperlinks.Name = "chkUseHyperlinks";
			this.chkUseHyperlinks.Properties.Caption = resources.GetString("chkUseHyperlinks.Properties.Caption");
			this.chkUseHyperlinks.CheckedChanged += new System.EventHandler(this.chkUseHyperlinks_CheckedChanged);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.spEditShowLevels, "spEditShowLevels");
			this.spEditShowLevels.Name = "spEditShowLevels";
			this.spEditShowLevels.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spEditShowLevels.Properties.Buttons"))))});
			this.spEditShowLevels.Properties.Mask.EditMask = resources.GetString("spEditShowLevels.Properties.Mask.EditMask");
			this.spEditShowLevels.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("spEditShowLevels.Properties.Mask.MaskType")));
			this.spEditShowLevels.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("spEditShowLevels.Properties.Mask.UseMaskAsDisplayFormat")));
			this.spEditShowLevels.Properties.MaxValue = new decimal(new int[] {
			9,
			0,
			0,
			0});
			this.spEditShowLevels.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spEditShowLevels.EditValueChanged += new System.EventHandler(this.spEditShowLevels_EditValueChanged);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.BtnCancel, "BtnCancel");
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			this.previewControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
			resources.ApplyResources(this.previewControl, "previewControl");
			this.previewControl.Controller = null;
			this.previewControl.Name = "previewControl";
			this.previewControl.Options.Fields.UseCurrentCultureDateTimeFormat = false;
			this.previewControl.Options.MailMerge.KeepLastParagraph = false;
			this.previewControl.TabStop = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.previewControl);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.spEditShowLevels);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.chkUseHyperlinks);
			this.Controls.Add(this.chkRightAlignPageNumbers);
			this.Controls.Add(this.chkShowPageNumbers);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TableOfContentsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.chkShowPageNumbers.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkRightAlignPageNumbers.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkUseHyperlinks.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spEditShowLevels.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.CheckEdit chkShowPageNumbers;
		private XtraEditors.CheckEdit chkRightAlignPageNumbers;
		private XtraEditors.CheckEdit chkUseHyperlinks;
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SpinEdit spEditShowLevels;
		private XtraEditors.LabelControl labelControl1;
		private Native.PreviewRichEditControl previewControl;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.SimpleButton BtnCancel;
	}
}
