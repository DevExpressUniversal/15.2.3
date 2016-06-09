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
	partial class TableSizeControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableSizeControl));
			this.chkPreferredWidth = new DevExpress.XtraEditors.CheckEdit();
			this.lblMeasureIn = new DevExpress.XtraEditors.LabelControl();
			this.cbWidthType = new DevExpress.XtraRichEdit.Design.WidthTypeEdit();
			this.spnPreferredWidth = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkPreferredWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbWidthType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnPreferredWidth.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chkPreferredWidth, "chkPreferredWidth");
			this.chkPreferredWidth.Name = "chkPreferredWidth";
			this.chkPreferredWidth.Properties.AutoWidth = true;
			this.chkPreferredWidth.Properties.Caption = resources.GetString("chkPreferredWidth.Properties.Caption");
			this.lblMeasureIn.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblMeasureIn, "lblMeasureIn");
			this.lblMeasureIn.Name = "lblMeasureIn";
			resources.ApplyResources(this.cbWidthType, "cbWidthType");
			this.cbWidthType.Name = "cbWidthType";
			this.cbWidthType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbWidthType.Properties.Buttons"))))});
			this.cbWidthType.Properties.UnitType = DevExpress.Office.DocumentUnit.Inch;
			this.cbWidthType.Value = null;
			resources.ApplyResources(this.spnPreferredWidth, "spnPreferredWidth");
			this.spnPreferredWidth.Name = "spnPreferredWidth";
			this.spnPreferredWidth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnPreferredWidth.Properties.IsValueInPercent = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbWidthType);
			this.Controls.Add(this.lblMeasureIn);
			this.Controls.Add(this.spnPreferredWidth);
			this.Controls.Add(this.chkPreferredWidth);
			this.Name = "TableSizeControl";
			((System.ComponentModel.ISupportInitialize)(this.chkPreferredWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbWidthType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnPreferredWidth.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chkPreferredWidth;
		private DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnPreferredWidth;
		private DevExpress.XtraEditors.LabelControl lblMeasureIn;
		private DevExpress.XtraRichEdit.Design.WidthTypeEdit cbWidthType;
	}
}
