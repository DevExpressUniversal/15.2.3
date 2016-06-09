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

namespace DevExpress.XtraRichEdit.Forms.Design {
	partial class TableRowHeightControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableRowHeightControl));
			this.chkSpecifyHeight = new DevExpress.XtraEditors.CheckEdit();
			this.spnHeight = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblHeightIn = new DevExpress.XtraEditors.LabelControl();
			this.edtRowHeightType = new DevExpress.XtraRichEdit.Design.HeightTypeEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkSpecifyHeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnHeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRowHeightType.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkSpecifyHeight, "chkSpecifyHeight");
			this.chkSpecifyHeight.Name = "chkSpecifyHeight";
			this.chkSpecifyHeight.Properties.AutoWidth = true;
			this.chkSpecifyHeight.Properties.Caption = resources.GetString("chkSpecifyHeight.Properties.Caption");
			resources.ApplyResources(this.spnHeight, "spnHeight");
			this.spnHeight.Name = "spnHeight";
			this.spnHeight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnHeight.Properties.IsValueInPercent = false;
			this.lblHeightIn.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblHeightIn, "lblHeightIn");
			this.lblHeightIn.Name = "lblHeightIn";
			resources.ApplyResources(this.edtRowHeightType, "edtRowHeightType");
			this.edtRowHeightType.Name = "edtRowHeightType";
			this.edtRowHeightType.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtRowHeightType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtRowHeightType.Properties.Buttons"))))});
			this.edtRowHeightType.Value = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.edtRowHeightType);
			this.Controls.Add(this.lblHeightIn);
			this.Controls.Add(this.spnHeight);
			this.Controls.Add(this.chkSpecifyHeight);
			this.Name = "TableRowHeightControl";
			((System.ComponentModel.ISupportInitialize)(this.chkSpecifyHeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnHeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRowHeightType.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chkSpecifyHeight;
		private DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnHeight;
		private DevExpress.XtraEditors.LabelControl lblHeightIn;
		private DevExpress.XtraRichEdit.Design.HeightTypeEdit edtRowHeightType;
	}
}
