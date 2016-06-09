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

using DevExpress.XtraRichEdit.Design;
namespace DevExpress.XtraRichEdit.Design {
	partial class ParagraphIndentationControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParagraphIndentationControl));
			this.lblLeft = new DevExpress.XtraEditors.LabelControl();
			this.lblRight = new DevExpress.XtraEditors.LabelControl();
			this.lblSpecial = new DevExpress.XtraEditors.LabelControl();
			this.lblBy = new DevExpress.XtraEditors.LabelControl();
			this.spnSpecial = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnRightIndent = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnLeftIndent = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.edtFirstLineIndent = new DevExpress.XtraRichEdit.Design.FirstLineIndentTypeEdit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpecial.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRightIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeftIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFirstLineIndent.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLeft.Name = "lblLeft";
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.lblSpecial, "lblSpecial");
			this.lblSpecial.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblSpecial.Name = "lblSpecial";
			resources.ApplyResources(this.lblBy, "lblBy");
			this.lblBy.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblBy.Name = "lblBy";
			resources.ApplyResources(this.spnSpecial, "spnSpecial");
			this.spnSpecial.Name = "spnSpecial";
			this.spnSpecial.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnSpecial.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spnSpecial.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnRightIndent, "spnRightIndent");
			this.spnRightIndent.Name = "spnRightIndent";
			this.spnRightIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnRightIndent.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnLeftIndent, "spnLeftIndent");
			this.spnLeftIndent.Name = "spnLeftIndent";
			this.spnLeftIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnLeftIndent.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.edtFirstLineIndent, "edtFirstLineIndent");
			this.edtFirstLineIndent.Name = "edtFirstLineIndent";
			this.edtFirstLineIndent.Properties.AccessibleName = resources.GetString("edtFirstLineIndent.Properties.AccessibleName");
			this.edtFirstLineIndent.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtFirstLineIndent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFirstLineIndent.Properties.Buttons"))))});
			this.edtFirstLineIndent.Value = null;
			this.Controls.Add(this.spnSpecial);
			this.Controls.Add(this.spnRightIndent);
			this.Controls.Add(this.spnLeftIndent);
			this.Controls.Add(this.edtFirstLineIndent);
			this.Controls.Add(this.lblBy);
			this.Controls.Add(this.lblSpecial);
			this.Controls.Add(this.lblRight);
			this.Controls.Add(this.lblLeft);
			this.Name = "ParagraphIndentationControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.spnSpecial.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRightIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeftIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFirstLineIndent.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblLeft;
		protected DevExpress.XtraEditors.LabelControl lblRight;
		protected DevExpress.XtraEditors.LabelControl lblSpecial;
		protected DevExpress.XtraEditors.LabelControl lblBy;
		protected RichTextIndentEdit spnLeftIndent;
		protected RichTextIndentEdit spnRightIndent;
		protected RichTextIndentEdit spnSpecial;
		protected FirstLineIndentTypeEdit edtFirstLineIndent;
	}
}
