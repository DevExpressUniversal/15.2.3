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
	partial class SimpleNumberingListFormBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.lblNumberFormat = new DevExpress.XtraEditors.LabelControl();
			this.lblNumberStyle = new DevExpress.XtraEditors.LabelControl();
			this.edtStart = new DevExpress.XtraEditors.SpinEdit();
			this.lblStartAt = new DevExpress.XtraEditors.LabelControl();
			this.lblNumberPosition = new DevExpress.XtraEditors.LabelControl();
			this.lblTextPosition = new DevExpress.XtraEditors.LabelControl();
			this.edtNumberFormat = new DevExpress.XtraRichEdit.Forms.SimpleRichEditControl();
			this.edtDisplayFormat = new DevExpress.XtraRichEdit.Forms.Design.DisplayFormatEdit();
			this.edtNumberingAlignment = new DevExpress.XtraRichEdit.Design.NumberingAlignmentEdit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDisplayFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtNumberingAlignment.Properties)).BeginInit();
			this.lblNumberFormat.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblNumberFormat.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblNumberFormat.LineVisible = true;
			this.lblNumberFormat.Name = "lblNumberFormat";
			this.lblNumberStyle.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblNumberStyle.Name = "lblNumberStyle";
			this.edtStart.Name = "edtStart";
			this.edtStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtStart.Properties.EditFormat.FormatString = "0{0}";
			this.edtStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.edtStart.Properties.IsFloatValue = false;
			this.edtStart.Properties.MaxValue = new decimal(new int[] {
			1500,
			0,
			0,
			0});
			this.lblStartAt.Name = "lblStartAt";
			this.lblNumberPosition.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblNumberPosition.LineVisible = true;
			this.lblNumberPosition.Name = "lblNumberPosition";
			this.lblTextPosition.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblTextPosition.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblTextPosition.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblTextPosition.LineVisible = true;
			this.lblTextPosition.Name = "lblTextPosition";
			this.edtNumberFormat.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
			this.edtNumberFormat.Modified = true;
			this.edtNumberFormat.Name = "edtNumberFormat";
			this.edtNumberFormat.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.edtNumberFormat.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.edtNumberFormat.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.edtNumberFormat.Options.VerticalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.edtNumberFormat.Views.SimpleView.Padding = new System.Windows.Forms.Padding(3, 1, 0, 0);
			this.edtNumberFormat.Options.Behavior.Drag = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.edtNumberFormat.Options.Behavior.Drop = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.edtNumberFormat.Options.Behavior.ShowPopupMenu = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.edtDisplayFormat.Name = "edtDisplayFormat";
			this.edtNumberingAlignment.Name = "edtNumberingAlignment";
			this.Controls.Add(this.lblTextPosition);
			this.Controls.Add(this.edtNumberingAlignment);
			this.Controls.Add(this.edtDisplayFormat);
			this.Controls.Add(this.lblNumberPosition);
			this.Controls.Add(this.lblStartAt);
			this.Controls.Add(this.edtStart);
			this.Controls.Add(this.lblNumberStyle);
			this.Controls.Add(this.lblNumberFormat);
			this.Controls.Add(this.edtNumberFormat);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(294, 274);
			this.Name = "SimpleNumberingListFormBase";
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDisplayFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtNumberingAlignment.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected  SimpleRichEditControl edtNumberFormat;
		protected DevExpress.XtraEditors.LabelControl lblNumberFormat;
		protected DevExpress.XtraEditors.LabelControl lblNumberStyle;
		protected DevExpress.XtraEditors.SpinEdit edtStart;
		protected DevExpress.XtraEditors.LabelControl lblStartAt;
		protected DevExpress.XtraEditors.LabelControl lblNumberPosition;
		protected DevExpress.XtraRichEdit.Forms.Design.DisplayFormatEdit edtDisplayFormat;
		protected DevExpress.XtraRichEdit.Design.NumberingAlignmentEdit edtNumberingAlignment;
		protected DevExpress.XtraEditors.LabelControl lblTextPosition;
	}
}
