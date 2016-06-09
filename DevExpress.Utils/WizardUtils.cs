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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.Utils
{
	[Flags]
	public enum WizardButton
	{
		Back		   = 0x01,
		Next		   = 0x02,
		Finish		 = 0x04,
		DisabledFinish = 0x08,
		Cancel		 = 0x10
	}
	[ToolboxItem(false)]
	public class WizardForm : XtraForm
	{
		public const string NextPage = "";
		public const string NoPageChange = null;
		ArrayList pages = new ArrayList();
		int selectedPageIndex = -1;
		protected Control btnBack;
		protected Control btnNext;
		protected Control btnCancel;
		protected Control btnFinish;
		protected GroupBox separator;
		WizardButton wizardButtons;
		WizardPage activePage;
		protected WizardPage ActivePage { get { return activePage; } }
		public virtual WizardButton WizardButtons {
			get { return this.wizardButtons; }
			set {
				btnBack.Enabled = (value & WizardButton.Back) == WizardButton.Back;
				btnNext.Enabled = (value & WizardButton.Next) == WizardButton.Next;
				btnNext.Visible = (value & WizardButton.Finish) == 0 && (value & WizardButton.DisabledFinish) == 0;
				btnFinish.Enabled = (value & WizardButton.DisabledFinish) == 0;
				btnFinish.Visible = (value & WizardButton.Finish) == WizardButton.Finish || (value & WizardButton.DisabledFinish) == WizardButton.DisabledFinish;
				AcceptButton = (IButtonControl)(btnFinish.Visible ? btnFinish : btnNext);
				this.wizardButtons = value;
			}
		}
		public WizardForm() {
			CreateButtons();
			InitializeComponent();
			this.AcceptButton = (IButtonControl)this.btnNext;
			this.CancelButton = (IButtonControl)this.btnCancel;
			WizardButtons = WizardButton.Next | WizardButton.Back | WizardButton.Finish;
			btnFinish.Location = btnNext.Location;
		}
		public void PressButton(WizardButton btn) {
			switch (btn) {
				case WizardButton.Next:
					if (btnNext.Visible && btnNext.Enabled)
						OnClickNext(this, EventArgs.Empty);
					break;
				case WizardButton.Back:
					if (btnBack.Visible && btnBack.Enabled)
						OnClickBack(this, EventArgs.Empty);
					break;
				case WizardButton.Finish:
					if (btnFinish.Visible && btnFinish.Enabled)
						OnClickFinish(this, EventArgs.Empty);
					break;
				case WizardButton.Cancel:
					if (btnCancel.Visible && btnCancel.Enabled)
						OnClickCancel(this, EventArgs.Empty);
					break;
			}
		}
		protected virtual Control CreateWizardButton() {
			Button btn = new System.Windows.Forms.Button();
			btn.FlatStyle = FlatStyle.System;
			return btn;
		}
		Control CreateWizardButtonInternal() {
			Control control = CreateWizardButton();
			System.Diagnostics.Debug.Assert(control as IButtonControl != null);
			IButtonControl btn = (IButtonControl)control;
			return control;
		}
		private bool callOnWizardFinishForEachPage = false;
		public bool CallOnWizardFinishForEachPage{
			get { return callOnWizardFinishForEachPage; }
			set { callOnWizardFinishForEachPage = value; }
		}
		void CreateButtons() {
			this.btnBack = CreateWizardButtonInternal();
			this.btnNext = CreateWizardButtonInternal();
			this.btnCancel = CreateWizardButtonInternal();
			this.btnFinish = CreateWizardButtonInternal();
			this.btnBack.Location = new System.Drawing.Point(252, 327);
			this.btnBack.Name = "backButton";
			this.btnBack.TabIndex = 8;
			this.btnBack.Text = "< &Back";
			this.btnBack.Click += new System.EventHandler(this.OnClickBack);
			this.btnNext.Location = new System.Drawing.Point(327, 327);
			this.btnNext.Name = "nextButton";
			this.btnNext.TabIndex = 9;
			this.btnNext.Text = "&Next >";
			this.btnNext.Click += new System.EventHandler(this.OnClickNext);
			IButtonControl btn = (IButtonControl)this.btnCancel;
			btn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(412, 327);
			this.btnCancel.Name = "cancelButton";
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.OnClickCancel);
			this.btnFinish.Location = new System.Drawing.Point(10, 327);
			this.btnFinish.Name = "finishButton";
			this.btnFinish.TabIndex = 10;
			this.btnFinish.Text = "&Finish";
			this.btnFinish.Visible = false;
			this.btnFinish.Click += new System.EventHandler(this.OnClickFinish);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.separator = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			this.separator.Location = new System.Drawing.Point(0, 313);
			this.separator.Name = "separator";
			this.separator.Size = new System.Drawing.Size(505, 2);
			this.separator.TabIndex = 7;
			this.separator.TabStop = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(497, 360);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnBack,
																		  this.btnNext,
																		  this.btnCancel,
																		  this.btnFinish,
																		  this.separator});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WizardForm";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void OnPageChanged() { }
		private void ActivatePage(int newIndex) {
			if (newIndex < 0 || newIndex >= pages.Count)
				throw new ArgumentOutOfRangeException();
			WizardPage currentPage = null;
			if (selectedPageIndex != -1)
			{
				currentPage = (WizardPage)pages[selectedPageIndex];
				if (!currentPage.OnKillActive())
					return;
			}
			WizardPage newPage = (WizardPage)pages[newIndex];
			if (!newPage.OnSetActive())
				return;
			selectedPageIndex = newIndex;
			if (currentPage != null)
				currentPage.Visible = false;
			activePage = newPage;
			newPage.UpdateWizardButtons();
			newPage.Visible = true;
			newPage.Focus();
			OnPageChanged();
		}
		private void OnClickBack(object sender, EventArgs e) {
			if (selectedPageIndex == -1)
				return;
			string pageName = ((WizardPage)pages[selectedPageIndex]).OnWizardBack();
			switch (pageName) {
				case NoPageChange:
					break;
				case NextPage:
					if (selectedPageIndex - 1 >= 0)
						ActivatePage(selectedPageIndex - 1);
					break;
				default:
					foreach (WizardPage page in pages) {
						if (page.Name == pageName)
							ActivatePage(pages.IndexOf(page));
					}
					break;
			}
		}
		private void OnClickCancel(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		private void OnClickFinish(object sender, EventArgs e) {
			if(selectedPageIndex == -1)
				return;
			WizardPage page = (WizardPage)pages[selectedPageIndex];
			bool result = false;
			if(callOnWizardFinishForEachPage) {
				result = true;
				foreach(WizardPage wp in pages)
					result &= wp.OnWizardFinish();
			}
			else {
				if(page.OnWizardFinish()) {
					result = true;
				}
			}
			if(result) {
				if(page.OnKillActive())
					DialogResult = DialogResult.OK;
			}
		}
		private void OnClickNext(object sender, EventArgs e) {
			if (selectedPageIndex == -1)
				return;
			string pageName = ((WizardPage)pages[selectedPageIndex]).OnWizardNext();
			switch (pageName) {
				case NoPageChange:
					break;
				case NextPage:
					if (selectedPageIndex + 1 < pages.Count)
						ActivatePage(selectedPageIndex + 1);
					break;
				default:
					foreach (WizardPage page in pages) {
						if (page.Name == pageName)
							ActivatePage(pages.IndexOf(page));
					}
					break;
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			WizardPage page = e.Control as WizardPage;
			if (page != null) {
				page.Visible = false;
				page.Location = new Point(0, 0);
				page.Size = new Size(Width, separator.Location.Y);
				pages.Add(page);
				if (selectedPageIndex == -1)
					selectedPageIndex = 0;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (pages.Count > 0)
				ActivatePage(0);
		}
		public void SetFinishText(string text) {
			btnFinish.Text = text;
		}
		internal void UpdateInteriorPageWizardButtons() {
			WizardButton buttons = WizardButton.Finish | WizardButton.Finish;
			if (pages.Count > 1) {
				if (selectedPageIndex >= 0 && (selectedPageIndex + 1) < pages.Count)
					buttons |= WizardButton.Next;
				if (selectedPageIndex > 0)
					buttons |= WizardButton.Back;
			}
			WizardButtons = buttons;
		}
		internal void UpdateExteriorPageWizardButtons() {
		}
	}
	[ToolboxItem(false)]
	public class WizardPage : XtraUserControl
	{
		protected WizardForm Wizard { get { return (WizardForm)Parent; }
		}
		public WizardPage() {
			InitializeComponent();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			Name = "WizardPage";
			Size = new System.Drawing.Size(497, 313);
		}
		#endregion
		protected internal virtual bool OnKillActive() {
			return Validate();
		}
		protected internal virtual bool OnSetActive() {
			return true;
		}
		protected internal virtual void UpdateWizardButtons() {
		}
		protected internal virtual string OnWizardBack() {
			return WizardForm.NextPage;
		}
		protected internal virtual string OnWizardNext() {
			return WizardForm.NextPage;
		}
		protected internal virtual bool OnWizardFinish() {
			return true;
		}
	}
	[ToolboxItem(false)]
	public class ExteriorWizardPage : WizardPage
	{
		protected Label titleLabel;
		protected PictureBox watermarkPicture;
		public ExteriorWizardPage() {
			InitializeComponent();
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.titleLabel = new System.Windows.Forms.Label();
			this.watermarkPicture = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			this.titleLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.titleLabel.Location = new System.Drawing.Point(170, 13);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(292, 39);
			this.titleLabel.TabIndex = 0;
			this.titleLabel.Text = "Welcome to the Sample Setup Wizard";
			this.watermarkPicture.BackColor = System.Drawing.Color.White;
			this.watermarkPicture.Name = "watermarkPicture";
			this.watermarkPicture.Size = new System.Drawing.Size(164, 312);
			this.watermarkPicture.TabIndex = 1;
			this.watermarkPicture.TabStop = false;
			this.watermarkPicture.SizeMode = PictureBoxSizeMode.Zoom;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.watermarkPicture,
																		  this.titleLabel});
			this.Name = "ExteriorWizardPage";
			this.ResumeLayout(false);
		}
		#endregion
		protected internal override void UpdateWizardButtons() {
			Wizard.UpdateExteriorPageWizardButtons();
		}
	}
	[ToolboxItem(false)]
	public class InteriorWizardPage : WizardPage
	{
		protected Label titleLabel;
		protected Label subtitleLabel;
		protected Panel headerPanel;
		protected PictureBox headerPicture;
		protected GroupBox headerSeparator;
		public InteriorWizardPage() {
			InitializeComponent();
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.headerSeparator = new System.Windows.Forms.GroupBox();
			this.headerPanel = new System.Windows.Forms.Panel();
			this.titleLabel = new System.Windows.Forms.Label();
			this.subtitleLabel = new System.Windows.Forms.Label();
			this.headerPicture = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			this.headerSeparator.Location = new System.Drawing.Point(0, 58);
			this.headerSeparator.Name = "headerSeparator";
			this.headerSeparator.Size = new System.Drawing.Size(497, 2);
			this.headerSeparator.TabIndex = 3;
			this.headerSeparator.TabStop = false;
			this.headerPanel.BackColor = System.Drawing.Color.White;
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(497, 58);
			this.headerPanel.TabIndex = 0;
			this.titleLabel.BackColor = System.Drawing.Color.White;
			this.titleLabel.ForeColor = System.Drawing.Color.Black;
			this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.titleLabel.Location = new System.Drawing.Point(20, 10);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(410, 13);
			this.titleLabel.TabIndex = 1;
			this.titleLabel.Text = "Sample Header Title";
			this.subtitleLabel.BackColor = System.Drawing.Color.White;
			this.subtitleLabel.ForeColor = System.Drawing.Color.Black;
			this.subtitleLabel.Location = new System.Drawing.Point(41, 25);
			this.subtitleLabel.Name = "subtitleLabel";
			this.subtitleLabel.Size = new System.Drawing.Size(389, 26);
			this.subtitleLabel.TabIndex = 2;
			this.subtitleLabel.Text = "Sample header subtitle will help a user complete a certain task in the Sample wiz" +
				"ard by clarifying the task in some way.";
			this.headerPicture.BackColor = System.Drawing.Color.White;
			this.headerPicture.Location = new System.Drawing.Point(443, 5);
			this.headerPicture.Name = "headerPicture";
			this.headerPicture.Size = new System.Drawing.Size(49, 49);
			this.headerPicture.TabIndex = 4;
			this.headerPicture.TabStop = false;
			this.headerPicture.SizeMode = PictureBoxSizeMode.Zoom;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.headerPicture,
																		  this.subtitleLabel,
																		  this.titleLabel,
																		  this.headerSeparator,
																		  this.headerPanel});
			this.Name = "InteriorWizardPage";
			this.ResumeLayout(false);
		}
		#endregion
		protected internal override void UpdateWizardButtons() {
			Wizard.UpdateInteriorPageWizardButtons();
		}
	}
}
