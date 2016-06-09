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
	partial class BorderShadingUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BorderShadingUserControl));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.CBoxApplyTo = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.btnVerticalBorderRight = new DevExpress.XtraEditors.CheckButton();
			this.btnVerticalBorderIn = new DevExpress.XtraEditors.CheckButton();
			this.btnVerticalBorderLeft = new DevExpress.XtraEditors.CheckButton();
			this.btnHorizontBorderDown = new DevExpress.XtraEditors.CheckButton();
			this.btnHorizontBorderIn = new DevExpress.XtraEditors.CheckButton();
			this.btnHorizontBorderUp = new DevExpress.XtraEditors.CheckButton();
			this.btnOptions = new DevExpress.XtraEditors.SimpleButton();
			this.previewBSUserControl = new DevExpress.XtraRichEdit.Forms.Design.PreviewBSUserControl();
			((System.ComponentModel.ISupportInitialize)(this.CBoxApplyTo.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.labelControl1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.CBoxApplyTo, "CBoxApplyTo");
			this.CBoxApplyTo.Name = "CBoxApplyTo";
			this.CBoxApplyTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("CBoxApplyTo.Properties.Buttons"))))});
			this.CBoxApplyTo.Properties.Items.AddRange(new object[] {
			resources.GetString("CBoxApplyTo.Properties.Items"),
			resources.GetString("CBoxApplyTo.Properties.Items1")});
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.Name = "labelControl4";
			resources.ApplyResources(this.btnVerticalBorderRight, "btnVerticalBorderRight");
			this.btnVerticalBorderRight.Name = "btnVerticalBorderRight";
			this.btnVerticalBorderRight.CheckedChanged += new System.EventHandler(this.btnVerticalBorderRight_CheckedChanged);
			resources.ApplyResources(this.btnVerticalBorderIn, "btnVerticalBorderIn");
			this.btnVerticalBorderIn.Name = "btnVerticalBorderIn";
			this.btnVerticalBorderIn.CheckedChanged += new System.EventHandler(this.btnVerticalBorderIn_CheckedChanged);
			resources.ApplyResources(this.btnVerticalBorderLeft, "btnVerticalBorderLeft");
			this.btnVerticalBorderLeft.Name = "btnVerticalBorderLeft";
			this.btnVerticalBorderLeft.CheckedChanged += new System.EventHandler(this.btnVerticalBorderLeft_CheckedChanged);
			resources.ApplyResources(this.btnHorizontBorderDown, "btnHorizontBorderDown");
			this.btnHorizontBorderDown.Name = "btnHorizontBorderDown";
			this.btnHorizontBorderDown.CheckedChanged += new System.EventHandler(this.btnHorizontBorderDown_CheckedChanged);
			resources.ApplyResources(this.btnHorizontBorderIn, "btnHorizontBorderIn");
			this.btnHorizontBorderIn.Name = "btnHorizontBorderIn";
			this.btnHorizontBorderIn.CheckedChanged += new System.EventHandler(this.btnHorizontBorderIn_CheckedChanged);
			resources.ApplyResources(this.btnHorizontBorderUp, "btnHorizontBorderUp");
			this.btnHorizontBorderUp.Name = "btnHorizontBorderUp";
			this.btnHorizontBorderUp.CheckedChanged += new System.EventHandler(this.btnHorizontBorderUp_CheckedChanged);
			resources.ApplyResources(this.btnOptions, "btnOptions");
			this.btnOptions.Name = "btnOptions";
			this.previewBSUserControl.BorderInfoSource = null;
			this.previewBSUserControl.DocumentModel = null;
			this.previewBSUserControl.DrawColumns = false;
			this.previewBSUserControl.DrawPageBorderHorizontalDown = true;
			this.previewBSUserControl.DrawPageBorderHorizontalUp = true;
			this.previewBSUserControl.DrawPageBorderVerticalLeft = true;
			this.previewBSUserControl.DrawPageBorderVerticalRight = true;
			this.previewBSUserControl.DrawParagraph = false;
			this.previewBSUserControl.FillColor = System.Drawing.Color.Empty;
			this.previewBSUserControl.HorizontalLineDown = DevExpress.XtraRichEdit.Forms.Design.BorderLineState.Known;
			this.previewBSUserControl.HorizontalLineIn = DevExpress.XtraRichEdit.Forms.Design.BorderLineState.Known;
			this.previewBSUserControl.HorizontalLineUp = DevExpress.XtraRichEdit.Forms.Design.BorderLineState.Known;
			resources.ApplyResources(this.previewBSUserControl, "previewBSUserControl");
			this.previewBSUserControl.Name = "previewBSUserControl";
			this.previewBSUserControl.VerticalLineIn = DevExpress.XtraRichEdit.Forms.Design.BorderLineState.Known;
			this.previewBSUserControl.VerticalLineLeft = DevExpress.XtraRichEdit.Forms.Design.BorderLineState.Known;
			this.previewBSUserControl.VerticalLineRight = DevExpress.XtraRichEdit.Forms.Design.BorderLineState.Known;
			this.previewBSUserControl.Load += new System.EventHandler(this.previewBSUserControl1_Load);
			this.previewBSUserControl.Click += new System.EventHandler(this.previewBSUserControl_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnOptions);
			this.Controls.Add(this.previewBSUserControl);
			this.Controls.Add(this.labelControl4);
			this.Controls.Add(this.CBoxApplyTo);
			this.Controls.Add(this.labelControl3);
			this.Controls.Add(this.btnVerticalBorderRight);
			this.Controls.Add(this.btnVerticalBorderIn);
			this.Controls.Add(this.btnVerticalBorderLeft);
			this.Controls.Add(this.btnHorizontBorderDown);
			this.Controls.Add(this.btnHorizontBorderIn);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.btnHorizontBorderUp);
			this.Name = "BorderShadingUserControl";
			((System.ComponentModel.ISupportInitialize)(this.CBoxApplyTo.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.ComboBoxEdit CBoxApplyTo;
		private XtraEditors.LabelControl labelControl4;
		private PreviewBSUserControl previewBSUserControl;
		private XtraEditors.CheckButton btnVerticalBorderRight;
		private XtraEditors.CheckButton btnVerticalBorderIn;
		private XtraEditors.CheckButton btnVerticalBorderLeft;
		private XtraEditors.CheckButton btnHorizontBorderDown;
		private XtraEditors.CheckButton btnHorizontBorderIn;
		private XtraEditors.CheckButton btnHorizontBorderUp;
		private XtraEditors.SimpleButton btnOptions;
	}
}
