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

using DevExpress.XtraSpreadsheet.Forms.Design;
namespace DevExpress.XtraSpreadsheet.Forms {
	partial class HyperlinkForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			UnsubscribeControlsEvents();
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HyperlinkForm));
			this.teDisplayText = new DevExpress.XtraEditors.TextEdit();
			this.teTooltip = new DevExpress.XtraEditors.TextEdit();
			this.lblText = new DevExpress.XtraEditors.LabelControl();
			this.lblTooltip = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.nbLinkTo = new DevExpress.XtraNavBar.NavBarControl();
			this.nbgLinkTo = new DevExpress.XtraNavBar.NavBarGroup();
			this.nbiFileOrWebPage = new DevExpress.XtraNavBar.NavBarItem();
			this.nbiThisDocument = new DevExpress.XtraNavBar.NavBarItem();
			this.nbiEmail = new DevExpress.XtraNavBar.NavBarItem();
			this.externalAddress = new DevExpress.XtraSpreadsheet.Forms.Design.ExternalAddressControl();
			this.cellReferenceAddress = new DevExpress.XtraSpreadsheet.Forms.Design.CellReferenceAddressControl();
			this.emailAddress = new DevExpress.XtraSpreadsheet.Forms.Design.EmailAddressControl();
			((System.ComponentModel.ISupportInitialize)(this.teDisplayText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teTooltip.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nbLinkTo)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.teDisplayText, "teDisplayText");
			this.teDisplayText.Name = "teDisplayText";
			resources.ApplyResources(this.teTooltip, "teTooltip");
			this.teTooltip.Name = "teTooltip";
			resources.ApplyResources(this.lblText, "lblText");
			this.lblText.Name = "lblText";
			resources.ApplyResources(this.lblTooltip, "lblTooltip");
			this.lblTooltip.Name = "lblTooltip";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			this.nbLinkTo.ActiveGroup = this.nbgLinkTo;
			this.nbLinkTo.AllowDrop = false;
			this.nbLinkTo.ExplorerBarShowGroupButtons = false;
			this.nbLinkTo.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
			this.nbgLinkTo});
			this.nbLinkTo.HideGroupCaptions = true;
			this.nbLinkTo.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
			this.nbiFileOrWebPage,
			this.nbiThisDocument,
			this.nbiEmail});
			this.nbLinkTo.LinkSelectionMode = DevExpress.XtraNavBar.LinkSelectionModeType.OneInControl;
			resources.ApplyResources(this.nbLinkTo, "nbLinkTo");
			this.nbLinkTo.Name = "nbLinkTo";
			this.nbLinkTo.OptionsNavPane.ExpandedWidth = ((int)(resources.GetObject("resource.ExpandedWidth")));
			this.nbLinkTo.OptionsNavPane.ShowExpandButton = false;
			resources.ApplyResources(this.nbgLinkTo, "nbgLinkTo");
			this.nbgLinkTo.Expanded = true;
			this.nbgLinkTo.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.LargeIconsText;
			this.nbgLinkTo.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbiFileOrWebPage),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbiThisDocument),
			new DevExpress.XtraNavBar.NavBarItemLink(this.nbiEmail)});
			this.nbgLinkTo.Name = "nbgLinkTo";
			resources.ApplyResources(this.nbiFileOrWebPage, "nbiFileOrWebPage");
			this.nbiFileOrWebPage.Name = "nbiFileOrWebPage";
			resources.ApplyResources(this.nbiThisDocument, "nbiThisDocument");
			this.nbiThisDocument.Name = "nbiThisDocument";
			resources.ApplyResources(this.nbiEmail, "nbiEmail");
			this.nbiEmail.Name = "nbiEmail";
			resources.ApplyResources(this.externalAddress, "externalAddress");
			this.externalAddress.Name = "externalAddress";
			this.externalAddress.TextEditSelectionStart = 0;
			resources.ApplyResources(this.cellReferenceAddress, "cellReferenceAddress");
			this.cellReferenceAddress.Name = "cellReferenceAddress";
			this.cellReferenceAddress.TextEditEnabled = true;
			this.cellReferenceAddress.TextEditSelectionStart = 0;
			this.cellReferenceAddress.TreeListFocusedNode = null;
			resources.ApplyResources(this.emailAddress, "emailAddress");
			this.emailAddress.EmailSelectionStart = 0;
			this.emailAddress.EmailText = "";
			this.emailAddress.Name = "emailAddress";
			this.emailAddress.SubjectSelectionStart = 0;
			this.emailAddress.SubjectText = "";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.nbLinkTo);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblTooltip);
			this.Controls.Add(this.lblText);
			this.Controls.Add(this.teTooltip);
			this.Controls.Add(this.teDisplayText);
			this.Controls.Add(this.externalAddress);
			this.Controls.Add(this.cellReferenceAddress);
			this.Controls.Add(this.emailAddress);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HyperlinkForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Shown += new System.EventHandler(this.HyperlinkForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.teDisplayText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teTooltip.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nbLinkTo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.TextEdit teDisplayText;
		private DevExpress.XtraEditors.TextEdit teTooltip;
		private DevExpress.XtraEditors.LabelControl lblText;
		private DevExpress.XtraEditors.LabelControl lblTooltip;
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private XtraNavBar.NavBarControl nbLinkTo;
		private XtraNavBar.NavBarGroup nbgLinkTo;
		private XtraNavBar.NavBarItem nbiFileOrWebPage;
		private XtraNavBar.NavBarItem nbiThisDocument;
		private XtraNavBar.NavBarItem nbiEmail;
		private ExternalAddressControl externalAddress;
		private CellReferenceAddressControl cellReferenceAddress;
		private EmailAddressControl emailAddress;
		private XtraEditors.SimpleButton btnCancel;
	}
}
