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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.ReportServer.Printing;
using DevExpress.Skins.Info;
using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	public partial class SetReportServerCredentialsView : UserControl, ISetReportServerCredentialsView {
		Dictionary<AuthenticationType, string> authenticationTypes = new Dictionary<AuthenticationType, string>();
		WaitForm form;
		string IPageView.HeaderText {
			get { return "Authentication credentials"; }
		}
		string IPageView.DescriptionText {
			get { return "Set credentials for authentication on Report Server"; }
		}
		public event EventHandler CredentialsChanged;
		public event EventHandler NavigateLinkClicked;
		public string ServerUri {
			get {
				return uriEdit.Text;
			}
			set {
				if(uriEdit.Properties.Items.Contains(value))
					uriEdit.SelectedItem = value;
				else uriEdit.Text = value;
			}
		}
		public AuthenticationType AuthenticationType {
			get {
				return (AuthenticationType)authenticationEdit.EditValue;
			}
			set {
				authenticationEdit.EditValue = value;
			}
		}
		public string UserName {
			get {
				return AuthenticationType == AuthenticationType.Forms ? userNameEdit.Text : null;
			}
			set {
				userNameEdit.Text = value;
			}
		}
		public string Password {
			get {
				return AuthenticationType == AuthenticationType.Forms ? passwordEdit.Text : null;
			}
			set {
				passwordEdit.Text = value;
			}
		}
		public SetReportServerCredentialsView() {
			InitializeComponent();
			authenticationTypes[AuthenticationType.Windows] = "Windows";
			authenticationTypes[AuthenticationType.Guest] = "Guest";
			authenticationTypes[AuthenticationType.Forms] = "Report Server";
			authenticationEdit.Properties.DataSource = authenticationTypes;
			authenticationEdit.EditValue = ReportServer.Printing.AuthenticationType.Windows;
			SplashScreenManager.RegisterUserSkin(new SkinBlobXmlCreator("DevExpress Design",
				"DevExpress.Utils.Design.", typeof(XtraDesignForm).Assembly, null));
		}
		public void FillServers(IEnumerable<string> servers) {
			uriEdit.FillItems(servers);
		}
		void uriEdit_TextChanged(object sender, EventArgs e) {
			CredentialsChanged.SafeRaise(this);
		}
		void authenticationEdit_EditValueChanged(object sender, EventArgs e) {
			userNameEdit.Enabled = passwordEdit.Enabled = AuthenticationType == AuthenticationType.Forms;
			CredentialsChanged.SafeRaise(this);
		}
		void userNameEdit_TextChanged(object sender, EventArgs e) {
			CredentialsChanged.SafeRaise(this);
		}
		void passwordEdit_TextChanged(object sender, EventArgs e) {
			CredentialsChanged.SafeRaise(this);
		}
		public void ShowWaitPanel(bool enable) {
			if(enable) {
				form = new WaitForm(this.FindForm());
				form.ShowWaitForm("Please wait", "Connecting");
			} else {
				form.CloseWaitForm();
			}
		}
		public void EnableLink(bool enable) {
			linkLabel1.Enabled = enable;
		}
		void uriEdit_Validated(object sender, EventArgs e) {
			if(linkLabel1.Enabled)
				this.ServerUri = UrlHelper.AppendProtocolIfNeeded(this.ServerUri);
		}
		void linkLabel1_OpenLink(object sender, XtraEditors.Controls.OpenLinkEventArgs e) {
			NavigateLinkClicked.SafeRaise(this);
			e.Handled = true;
		}
	}
	class WaitForm : XtraForm {
		readonly Form parentForm;
		class SplashWaitForm : DemoWaitForm {
			public SplashWaitForm() {
				SetDescription("Connecting...");
			}
		}
		SplashScreenManager manager;
		public WaitForm(Form parentForm) {
			this.parentForm = parentForm;
			InitializeComponent();
			Owner = parentForm;
			LookAndFeel.SetSkinStyle(parentForm is ISupportLookAndFeel ? ((ISupportLookAndFeel)parentForm).LookAndFeel.SkinName : "DevExpress Style");
		}
		private System.ComponentModel.IContainer components = null;
		private void InitializeComponent() {
			this.SuspendLayout();
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Size = parentForm.Size;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WaitForm";
			this.ShowInTaskbar = false;
			this.Text = "WaitForm";
			this.ResumeLayout(false);
		}
		public void ShowWaitForm(string caption, string description) {
			this.Size = parentForm.Size;
			Application.UseWaitCursor = true;
			this.Location = parentForm.Location;
			this.LocationChanged += WaitForm_LocationChanged;
			this.Show();
			manager = new SplashScreenManager(this, typeof(SplashWaitForm), false, false);
			manager.ShowWaitForm();
		}
		void WaitForm_LocationChanged(object sender, EventArgs e) {
			this.LocationChanged -= WaitForm_LocationChanged;
			this.Location = parentForm.Location;
			this.LocationChanged += WaitForm_LocationChanged;
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Transparent), this.Bounds);
		}
		public void CloseWaitForm() {
			Application.UseWaitCursor = false;
			if(manager.IsSplashFormVisible)
				manager.CloseWaitForm();
			else
				throw new InvalidOperationException();
			this.LocationChanged -= WaitForm_LocationChanged;
			Close();
		}
		protected override void Dispose(bool disposing) {
			if(manager != null)
				manager.Dispose();
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
