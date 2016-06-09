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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Office;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.txtText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.txtTooltip")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.lblText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.lblTooltip")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.lblTarget")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.lblLinkTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.rgLinkTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.cbTargetFrame")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.labelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.btnEditAddress")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.lblAddress")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.pnlAddress")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.pnlBookmark")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.cbBoomarks")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.HyperlinkForm.lblBookmark")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class HyperlinkForm : DevExpress.XtraEditors.XtraForm, IFormOwner {
		static List<string> targetFrameDescriptions = CreateTargetFrameDescriptions();
		static List<string> CreateTargetFrameDescriptions() {
			List<string> result = new List<string>();
			result.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Blank));
			result.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Parent));
			result.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Self));
			result.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Top));
			return result;
		}
		static List<string> targetFrames = CreateTargetFrames();
		static List<string> CreateTargetFrames() {
			List<string> result = new List<string>();
			result.Add("_blank");
			result.Add("_parent");
			result.Add("_self");
			result.Add("_top");
			return result;
		}
		#region Fields
		readonly IRichEditControl control;
		readonly HyperlinkFormController controller;
		#endregion
		HyperlinkForm() {
			InitializeComponent();
		}
		public HyperlinkForm(HyperlinkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			PopulateBookmarkList();
			PopulateTargetFrameList();
			SubscribeToControlsEvents();
			UpdateForm();
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		protected internal HyperlinkFormController Controller { get { return controller; } }
		protected DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		protected bool IsAddressActive { get { return rgLinkTo.SelectedIndex == 0; } }
		#endregion
		protected virtual HyperlinkFormController CreateController(HyperlinkFormControllerParameters controllerParameters) {
			return new HyperlinkFormController(controllerParameters);
		}
		void PopulateBookmarkList() {
			this.cbBoomarks.Properties.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.HyperlinkForm_SelectedBookmarkNone));
			List<Bookmark> bookmarks = DocumentModel.GetBookmarks();
			bookmarks.Sort(new BookmarkNameComparer());
			foreach (Bookmark bookmark in bookmarks)
				this.cbBoomarks.Properties.Items.Add(bookmark.Name);
			this.cbBoomarks.SelectedIndex = 0;
		}
		void PopulateTargetFrameList() {
			foreach (string description in targetFrameDescriptions)
				cbTargetFrame.Properties.Items.Add(description);
		}
		void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void rgLinkTo_SelectedIndexChanged(object sender, EventArgs e) {
			Controller.UriType = IsAddressActive ? HyperlinkUriType.Url : HyperlinkUriType.Anchor;
			UpdateAddressPanelVisibility();
			UpdateOKButtonEnabling();
		}
		void UpdateAddressPanelVisibility() {
			bool visible = Controller.UriType == HyperlinkUriType.Url;
			ChangePanelsVisibility(visible);
		}
		void SwitchToAddress() {
			this.rgLinkTo.SelectedIndex = 0;
			ChangePanelsVisibility(true);
		}
		void SwitchToBookmark() {
			this.rgLinkTo.SelectedIndex = 1;
			ChangePanelsVisibility(false);
		}
		void ChangePanelsVisibility(bool isAddressActive) {
			this.pnlBookmark.Visible = !isAddressActive;
			this.pnlAddress.Visible = isAddressActive;
		}
		void btnEditAddress_ButtonClick(object sender, ButtonPressedEventArgs e) {
			using (OpenFileDialog fileDialog = new OpenFileDialog()) {
				string description = XtraRichEditLocalizer.GetString(XtraRichEditStringId.FileFilterDescription_AllFiles);
				fileDialog.Filter = String.Format("{0}|*.*", description);
				if (fileDialog.ShowDialog(this) == DialogResult.OK)
					this.btnEditAddress.Text = fileDialog.FileName;
			}
		}
		private void cbTargetFrame_EditValueChanged(object sender, EventArgs e) {
			if (!targetFrameDescriptions.Contains(this.cbTargetFrame.Text))
				Controller.Target = this.cbTargetFrame.Text;
			else {
				int index = targetFrameDescriptions.IndexOf(this.cbTargetFrame.Text);
				Controller.Target = targetFrames[index];
			}
		}
		private void txtText_EditValueChanged(object sender, EventArgs e) {
			Controller.TextToDisplay = txtText.Text;
		}
		private void txtTooltip_EditValueChanged(object sender, EventArgs e) {
			Controller.ToolTip = txtTooltip.Text;
		}
		private void cbBoomarks_EditValueChanged(object sender, EventArgs e) {
			ApplyBookmarkName();
			UpdateOKButtonEnabling();
		}
		private void ApplyBookmarkName() {
			if (cbBoomarks.SelectedIndex == 0)
				Controller.Anchor = String.Empty;
			else
				Controller.Anchor = cbBoomarks.Text;
		}
		void btnEditAddress_EditValueChanged(object sender, EventArgs e) {
			ApplyNavigationUri();
			UpdateOKButtonEnabling();
		}
		private void ApplyNavigationUri() {
			Controller.NavigateUri = btnEditAddress.Text;
		}
		void btnOk_Click(object sender, EventArgs e) {
			Controller.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
		void UpdateOKButtonEnabling() {
			this.btnOk.Enabled = ShouldEnableOKButton();
		}
		bool ShouldEnableOKButton() {
			return IsAddressActive ? !String.IsNullOrEmpty(this.btnEditAddress.Text) : this.cbBoomarks.SelectedIndex > 0;
		}
		#region subscribe/unsubscribe to controls events
		protected internal virtual void SubscribeToControlsEvents() {
			this.txtText.EditValueChanged += new EventHandler(txtText_EditValueChanged);
			this.txtTooltip.EditValueChanged += new EventHandler(txtTooltip_EditValueChanged);
			this.cbBoomarks.EditValueChanged += new EventHandler(cbBoomarks_EditValueChanged);
			this.cbTargetFrame.EditValueChanged += new EventHandler(cbTargetFrame_EditValueChanged);
			this.btnEditAddress.EditValueChanged += new EventHandler(btnEditAddress_EditValueChanged);
			this.rgLinkTo.SelectedIndexChanged += new EventHandler(rgLinkTo_SelectedIndexChanged);
		}
		protected internal virtual void UnsubscribeToControlsEvents() {
			this.txtText.EditValueChanged -= new EventHandler(txtText_EditValueChanged);
			this.txtTooltip.EditValueChanged -= new EventHandler(txtTooltip_EditValueChanged);
			this.cbBoomarks.EditValueChanged -= new EventHandler(cbBoomarks_EditValueChanged);
			this.cbTargetFrame.EditValueChanged -= new EventHandler(cbTargetFrame_EditValueChanged);
			this.btnEditAddress.EditValueChanged -= new EventHandler(btnEditAddress_EditValueChanged);
			this.rgLinkTo.SelectedIndexChanged -= new EventHandler(rgLinkTo_SelectedIndexChanged);
		}
		#endregion
		#region UpdateForm
		protected internal virtual void UpdateForm() {
			UnsubscribeToControlsEvents();
			try {
				UpdateFormCore();
				UpdateOKButtonEnabling();
			}
			finally {
				SubscribeToControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			UpdateTooltip();
			UpdateAddress();
			UpdateTextToDisplay();
			UpdateTargetFrame();
		}
		private void UpdateLinkTo() {
			if (Controller.UriType == HyperlinkUriType.Anchor)
				SwitchToBookmark();
			else
				SwitchToAddress();
		}
		void UpdateTooltip() {
			this.txtTooltip.Text = Controller.ToolTip;
		}
		void UpdateTextToDisplay() {
			if (Controller.CanChangeDisplayText())
				this.txtText.Text = Controller.TextToDisplay;
			else {
				string text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.HyperlinkForm_SelectionInDocument);
				this.txtText.Enabled = false;
				this.txtText.Text = text;
			}
		}
		void UpdateAddress() {
			if (Controller.UriType == HyperlinkUriType.Anchor) {
				int index = this.cbBoomarks.Properties.Items.IndexOf(Controller.Anchor);
					SwitchToBookmark();
					this.cbBoomarks.SelectedIndex = index;
			}
			else {
				SwitchToAddress();
				this.btnEditAddress.Text = Controller.NavigateUri;
			}
		}
		void UpdateTargetFrame() {
			if (!targetFrames.Contains(Controller.Target))
				this.cbTargetFrame.Text = Controller.Target;
			else {
				int index = targetFrames.IndexOf(Controller.Target);
				this.cbTargetFrame.Text = targetFrameDescriptions[index];
			}
		}
		#endregion
	}
}
