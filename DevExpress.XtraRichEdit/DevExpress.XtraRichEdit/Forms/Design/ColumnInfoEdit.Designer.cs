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
	partial class ColumnInfoEdit {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.edtIndex = new DevExpress.XtraEditors.TextEdit();
			this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
			this.edtSpacing = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.edtWidth = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtIndex.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtWidth.Properties)).BeginInit();
			this.SuspendLayout();
			this.edtIndex.Location = new System.Drawing.Point(0, 0);
			this.edtIndex.Name = "edtIndex";
			this.edtIndex.Properties.ReadOnly = true;
			this.edtIndex.Size = new System.Drawing.Size(34, 20);
			this.edtIndex.TabIndex = 0;
			this.edtSpacing.Location = new System.Drawing.Point(126, 0);
			this.edtSpacing.Name = "edtSpacing";
			this.edtSpacing.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtSpacing.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.edtSpacing.Properties.IsValueInPercent = false;
			this.edtSpacing.Size = new System.Drawing.Size(80, 20);
			this.edtSpacing.TabIndex = 2;
			this.edtWidth.Location = new System.Drawing.Point(40, 0);
			this.edtWidth.Name = "edtWidth";
			this.edtWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtWidth.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.edtWidth.Properties.IsValueInPercent = false;
			this.edtWidth.Size = new System.Drawing.Size(80, 20);
			this.edtWidth.TabIndex = 2;
			this.Appearance.Options.UseBackColor = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.edtSpacing);
			this.Controls.Add(this.edtWidth);
			this.Controls.Add(this.edtIndex);
			this.Name = "ColumnInfoEdit";
			this.Size = new System.Drawing.Size(217, 23);
			((System.ComponentModel.ISupportInitialize)(this.edtIndex.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtWidth.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.TextEdit edtIndex;
		private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
		private DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtWidth;
		private DevExpress.XtraRichEdit.Design.RichTextIndentEdit edtSpacing;
	}
}
