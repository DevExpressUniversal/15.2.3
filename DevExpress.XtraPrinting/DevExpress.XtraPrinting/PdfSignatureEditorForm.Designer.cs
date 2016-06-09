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

namespace DevExpress.XtraPrinting.Native.WinControls {
	partial class PdfSignatureEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfSignatureEditorForm));
			this.lbCertificate = new DevExpress.XtraEditors.LabelControl();
			this.lbReason = new DevExpress.XtraEditors.LabelControl();
			this.lbLocation = new DevExpress.XtraEditors.LabelControl();
			this.lbContactInfo = new DevExpress.XtraEditors.LabelControl();
			this.tbReason = new DevExpress.XtraEditors.TextEdit();
			this.tbLocation = new DevExpress.XtraEditors.TextEdit();
			this.tbContactInfo = new DevExpress.XtraEditors.MemoEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.certificateSelector = new DevExpress.XtraPrinting.Native.WinControls.CertificateSelector();
			((System.ComponentModel.ISupportInitialize)(this.tbReason.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbContactInfo.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbCertificate, "lbCertificate");
			this.lbCertificate.Name = "lbCertificate";
			resources.ApplyResources(this.lbReason, "lbReason");
			this.lbReason.Name = "lbReason";
			resources.ApplyResources(this.lbLocation, "lbLocation");
			this.lbLocation.Name = "lbLocation";
			resources.ApplyResources(this.lbContactInfo, "lbContactInfo");
			this.lbContactInfo.Name = "lbContactInfo";
			resources.ApplyResources(this.tbReason, "tbReason");
			this.tbReason.Name = "tbReason";
			resources.ApplyResources(this.tbLocation, "tbLocation");
			this.tbLocation.Name = "tbLocation";
			resources.ApplyResources(this.tbContactInfo, "tbContactInfo");
			this.tbContactInfo.Name = "tbContactInfo";
			this.tbContactInfo.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.certificateSelector, "certificateSelector");
			this.certificateSelector.Name = "certificateSelector";
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.certificateSelector);
			this.Controls.Add(this.tbContactInfo);
			this.Controls.Add(this.tbLocation);
			this.Controls.Add(this.tbReason);
			this.Controls.Add(this.lbContactInfo);
			this.Controls.Add(this.lbLocation);
			this.Controls.Add(this.lbReason);
			this.Controls.Add(this.lbCertificate);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PdfSignatureEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.tbReason.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbLocation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbContactInfo.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl lbCertificate;
		private DevExpress.XtraEditors.LabelControl lbReason;
		private DevExpress.XtraEditors.LabelControl lbLocation;
		private DevExpress.XtraEditors.LabelControl lbContactInfo;
		private DevExpress.XtraEditors.TextEdit tbReason;
		private DevExpress.XtraEditors.TextEdit tbLocation;
		private DevExpress.XtraEditors.MemoEdit tbContactInfo;
		private CertificateSelector certificateSelector;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
	}
}
