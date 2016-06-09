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

namespace DevExpress.XtraRichEdit.Design {
	partial class ParagraphSpacingControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParagraphSpacingControl));
			this.lblBefore = new DevExpress.XtraEditors.LabelControl();
			this.lblAfter = new DevExpress.XtraEditors.LabelControl();
			this.lblLineSpacing = new DevExpress.XtraEditors.LabelControl();
			this.lblAt = new DevExpress.XtraEditors.LabelControl();
			this.spnAtFloatSpacing = new DevExpress.XtraEditors.SpinEdit();
			this.spnSpacingBefore = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnAtSpacing = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnSpacingAfter = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.edtLineSpacing = new DevExpress.XtraRichEdit.Design.LineSpacingEdit();
			((System.ComponentModel.ISupportInitialize)(this.spnAtFloatSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpacingBefore.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnAtSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpacingAfter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLineSpacing.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblBefore, "lblBefore");
			this.lblBefore.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblBefore.Name = "lblBefore";
			resources.ApplyResources(this.lblAfter, "lblAfter");
			this.lblAfter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblAfter.Name = "lblAfter";
			resources.ApplyResources(this.lblLineSpacing, "lblLineSpacing");
			this.lblLineSpacing.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLineSpacing.Name = "lblLineSpacing";
			resources.ApplyResources(this.lblAt, "lblAt");
			this.lblAt.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblAt.Name = "lblAt";
			resources.ApplyResources(this.spnAtFloatSpacing, "spnAtFloatSpacing");
			this.spnAtFloatSpacing.Name = "spnAtFloatSpacing";
			this.spnAtFloatSpacing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnAtFloatSpacing.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.spnAtFloatSpacing.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spnAtFloatSpacing.Properties.Increment = new decimal(new int[] {
			5,
			0,
			0,
			65536});
			this.spnAtFloatSpacing.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("spnAtFloatSpacing.Properties.Mask.BeepOnError")));
			this.spnAtFloatSpacing.Properties.Mask.EditMask = resources.GetString("spnAtFloatSpacing.Properties.Mask.EditMask");
			this.spnAtFloatSpacing.Properties.MaxValue = new decimal(new int[] {
			133,
			0,
			0,
			0});
			this.spnAtFloatSpacing.Properties.MinValue = new decimal(new int[] {
			5,
			0,
			0,
			65536});
			this.spnAtFloatSpacing.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(this.OnSpnAtFloatSpacingSpin);
			resources.ApplyResources(this.spnSpacingBefore, "spnSpacingBefore");
			this.spnSpacingBefore.Name = "spnSpacingBefore";
			this.spnSpacingBefore.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnSpacingBefore.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnAtSpacing, "spnAtSpacing");
			this.spnAtSpacing.Name = "spnAtSpacing";
			this.spnAtSpacing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnAtSpacing.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnSpacingAfter, "spnSpacingAfter");
			this.spnSpacingAfter.Name = "spnSpacingAfter";
			this.spnSpacingAfter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnSpacingAfter.Properties.IsValueInPercent = false;
			this.edtLineSpacing.LineSpacing = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.AtLeast;
			resources.ApplyResources(this.edtLineSpacing, "edtLineSpacing");
			this.edtLineSpacing.Name = "edtLineSpacing";
			this.edtLineSpacing.Properties.AccessibleName = resources.GetString("edtLineSpacing.Properties.AccessibleName");
			this.edtLineSpacing.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtLineSpacing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLineSpacing.Properties.Buttons"))))});
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.spnAtFloatSpacing);
			this.Controls.Add(this.spnSpacingBefore);
			this.Controls.Add(this.spnAtSpacing);
			this.Controls.Add(this.spnSpacingAfter);
			this.Controls.Add(this.edtLineSpacing);
			this.Controls.Add(this.lblAt);
			this.Controls.Add(this.lblLineSpacing);
			this.Controls.Add(this.lblAfter);
			this.Controls.Add(this.lblBefore);
			this.Name = "ParagraphSpacingControl";
			((System.ComponentModel.ISupportInitialize)(this.spnAtFloatSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpacingBefore.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnAtSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpacingAfter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLineSpacing.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblBefore;
		protected DevExpress.XtraEditors.LabelControl lblAfter;
		protected DevExpress.XtraEditors.LabelControl lblLineSpacing;
		protected DevExpress.XtraEditors.LabelControl lblAt;
		protected LineSpacingEdit edtLineSpacing;
		protected RichTextIndentEdit spnSpacingAfter;
		protected RichTextIndentEdit spnAtSpacing;
		protected RichTextIndentEdit spnSpacingBefore;
		protected DevExpress.XtraEditors.SpinEdit spnAtFloatSpacing;
	}
}
