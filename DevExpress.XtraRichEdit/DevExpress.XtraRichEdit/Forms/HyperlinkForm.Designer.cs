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
	partial class HyperlinkForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HyperlinkForm));
			this.txtText = new DevExpress.XtraEditors.TextEdit();
			this.txtTooltip = new DevExpress.XtraEditors.TextEdit();
			this.lblText = new DevExpress.XtraEditors.LabelControl();
			this.lblTooltip = new DevExpress.XtraEditors.LabelControl();
			this.lblTarget = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblLinkTo = new DevExpress.XtraEditors.LabelControl();
			this.rgLinkTo = new DevExpress.XtraEditors.RadioGroup();
			this.cbTargetFrame = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnEditAddress = new DevExpress.XtraEditors.ButtonEdit();
			this.lblAddress = new DevExpress.XtraEditors.LabelControl();
			this.pnlAddress = new RichEditPanel();
			this.pnlBookmark = new RichEditPanel();
			this.cbBoomarks = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblBookmark = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTooltip.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgLinkTo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTargetFrame.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnEditAddress.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAddress)).BeginInit();
			this.pnlAddress.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlBookmark)).BeginInit();
			this.pnlBookmark.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbBoomarks.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.txtText, "txtText");
			this.txtText.Name = "txtText";
			resources.ApplyResources(this.txtTooltip, "txtTooltip");
			this.txtTooltip.Name = "txtTooltip";
			resources.ApplyResources(this.lblText, "lblText");
			this.lblText.Name = "lblText";
			resources.ApplyResources(this.lblTooltip, "lblTooltip");
			this.lblTooltip.Name = "lblTooltip";
			resources.ApplyResources(this.lblTarget, "lblTarget");
			this.lblTarget.Name = "lblTarget";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.lblLinkTo, "lblLinkTo");
			this.lblLinkTo.Name = "lblLinkTo";
			resources.ApplyResources(this.rgLinkTo, "rgLinkTo");
			this.rgLinkTo.Name = "rgLinkTo";
			this.rgLinkTo.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgLinkTo.Properties.Appearance.BackColor")));
			this.rgLinkTo.Properties.Appearance.Options.UseBackColor = true;
			this.rgLinkTo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgLinkTo.Properties.Columns = 1;
			this.rgLinkTo.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLinkTo.Properties.Items"))), resources.GetString("rgLinkTo.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgLinkTo.Properties.Items2"))), resources.GetString("rgLinkTo.Properties.Items3"))});
			resources.ApplyResources(this.cbTargetFrame, "cbTargetFrame");
			this.cbTargetFrame.Name = "cbTargetFrame";
			this.cbTargetFrame.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTargetFrame.Properties.Buttons"))))});
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.btnEditAddress, "btnEditAddress");
			this.btnEditAddress.Name = "btnEditAddress";
			this.btnEditAddress.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnEditAddress.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnEditAddress_ButtonClick);
			resources.ApplyResources(this.lblAddress, "lblAddress");
			this.lblAddress.Name = "lblAddress";
			this.pnlAddress.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAddress.Controls.Add(this.lblAddress);
			this.pnlAddress.Controls.Add(this.btnEditAddress);
			resources.ApplyResources(this.pnlAddress, "pnlAddress");
			this.pnlAddress.Name = "pnlAddress";
			this.pnlBookmark.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBookmark.Controls.Add(this.cbBoomarks);
			this.pnlBookmark.Controls.Add(this.lblBookmark);
			resources.ApplyResources(this.pnlBookmark, "pnlBookmark");
			this.pnlBookmark.Name = "pnlBookmark";
			resources.ApplyResources(this.cbBoomarks, "cbBoomarks");
			this.cbBoomarks.Name = "cbBoomarks";
			this.cbBoomarks.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbBoomarks.Properties.Buttons"))))});
			this.cbBoomarks.Properties.DropDownRows = 15;
			this.cbBoomarks.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblBookmark, "lblBookmark");
			this.lblBookmark.Name = "lblBookmark";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.pnlBookmark);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.cbTargetFrame);
			this.Controls.Add(this.pnlAddress);
			this.Controls.Add(this.rgLinkTo);
			this.Controls.Add(this.lblLinkTo);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblTarget);
			this.Controls.Add(this.lblTooltip);
			this.Controls.Add(this.lblText);
			this.Controls.Add(this.txtTooltip);
			this.Controls.Add(this.txtText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HyperlinkForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.txtText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtTooltip.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgLinkTo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTargetFrame.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnEditAddress.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAddress)).EndInit();
			this.pnlAddress.ResumeLayout(false);
			this.pnlAddress.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlBookmark)).EndInit();
			this.pnlBookmark.ResumeLayout(false);
			this.pnlBookmark.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbBoomarks.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.TextEdit txtText;
		protected DevExpress.XtraEditors.TextEdit txtTooltip;
		protected DevExpress.XtraEditors.LabelControl lblText;
		protected DevExpress.XtraEditors.LabelControl lblTooltip;
		protected DevExpress.XtraEditors.LabelControl lblTarget;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblLinkTo;
		protected DevExpress.XtraEditors.RadioGroup rgLinkTo;
		protected DevExpress.XtraEditors.ComboBoxEdit cbTargetFrame;
		protected DevExpress.XtraEditors.LabelControl labelControl1;
		protected DevExpress.XtraEditors.ButtonEdit btnEditAddress;
		protected DevExpress.XtraEditors.LabelControl lblAddress;
		protected DevExpress.XtraEditors.PanelControl pnlAddress;
		protected DevExpress.XtraEditors.PanelControl pnlBookmark;
		protected DevExpress.XtraEditors.ComboBoxEdit cbBoomarks;
		protected DevExpress.XtraEditors.LabelControl lblBookmark;
	}
}
