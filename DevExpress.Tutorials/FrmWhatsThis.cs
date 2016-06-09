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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils.Frames;
namespace DevExpress.Tutorials {
	public class FrmWhatsThis : FrmWhatsThisBase {
		private DevExpress.XtraEditors.SimpleButton btnCopyClipboard;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Panel pnlButtonContainer;
		private System.Windows.Forms.Panel pnlDescContainer;
		private DevExpress.XtraEditors.SimpleButton btnClose;
		private System.Windows.Forms.Label lblMemberListHeader;
		private System.Windows.Forms.Label lblMemberList;
		private System.Windows.Forms.Panel pnlTextContainer;
		private System.Windows.Forms.Panel pnlMemoLeft;
		private System.Windows.Forms.Panel pnlMemoRight;
		private System.Windows.Forms.Panel pnlDescriptionSeparator2;
		private System.Windows.Forms.Panel pnlDescriptionSeparator3;
		private System.Windows.Forms.Panel pnlDescriptionSeparator1;
		private System.Windows.Forms.Panel pnlDescriptionSeparator4;
		private System.Windows.Forms.Label lblDescriptionHeader;
		private DevExpress.XtraEditors.PictureEdit pictureDTScreenshot;
		private System.Windows.Forms.Panel pnlDescriptionSeparator5;
		private System.Windows.Forms.ImageList imageListTabs;
		private DevExpress.XtraTab.XtraTabControl tabctrlInfo;
		private DevExpress.XtraTab.XtraTabPage tabpgImage;
		private ColoredTextControl coloredTextControl;
		private DevExpress.XtraTab.XtraTabPage tabpgColoredCode;
		private System.ComponentModel.IContainer components;
		public FrmWhatsThis() {
			InitializeComponent();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmWhatsThis));
			this.btnCopyClipboard = new DevExpress.XtraEditors.SimpleButton();
			this.lblDescription = new System.Windows.Forms.Label();
			this.pnlButtonContainer = new System.Windows.Forms.Panel();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.pnlDescContainer = new System.Windows.Forms.Panel();
			this.pnlTextContainer = new System.Windows.Forms.Panel();
			this.pnlDescriptionSeparator4 = new System.Windows.Forms.Panel();
			this.lblMemberList = new System.Windows.Forms.Label();
			this.pnlDescriptionSeparator3 = new System.Windows.Forms.Panel();
			this.lblMemberListHeader = new System.Windows.Forms.Label();
			this.pnlDescriptionSeparator2 = new System.Windows.Forms.Panel();
			this.pnlDescriptionSeparator1 = new System.Windows.Forms.Panel();
			this.lblDescriptionHeader = new System.Windows.Forms.Label();
			this.pnlDescriptionSeparator5 = new System.Windows.Forms.Panel();
			this.pnlMemoLeft = new System.Windows.Forms.Panel();
			this.pnlMemoRight = new System.Windows.Forms.Panel();
			this.pictureDTScreenshot = new DevExpress.XtraEditors.PictureEdit();
			this.imageListTabs = new System.Windows.Forms.ImageList(this.components);
			this.tabctrlInfo = new DevExpress.XtraTab.XtraTabControl();
			this.tabpgColoredCode = new DevExpress.XtraTab.XtraTabPage();
			this.coloredTextControl = new DevExpress.Tutorials.ColoredTextControl();
			this.tabpgImage = new DevExpress.XtraTab.XtraTabPage();
			this.pnlButtonContainer.SuspendLayout();
			this.pnlDescContainer.SuspendLayout();
			this.pnlTextContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureDTScreenshot.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabctrlInfo)).BeginInit();
			this.tabctrlInfo.SuspendLayout();
			this.tabpgColoredCode.SuspendLayout();
			this.tabpgImage.SuspendLayout();
			this.SuspendLayout();
			this.btnCopyClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCopyClipboard.Location = new System.Drawing.Point(473, 8);
			this.btnCopyClipboard.Name = "btnCopyClipboard";
			this.btnCopyClipboard.Size = new System.Drawing.Size(80, 24);
			this.btnCopyClipboard.TabIndex = 1;
			this.btnCopyClipboard.Text = "Copy";
			this.btnCopyClipboard.Click += new System.EventHandler(this.btnCopyClipboard_Click);
			this.lblDescription.BackColor = System.Drawing.SystemColors.Info;
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
			this.lblDescription.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblDescription.Location = new System.Drawing.Point(0, 22);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(635, 32);
			this.lblDescription.TabIndex = 3;
			this.lblDescription.Text = "Description content";
			this.pnlButtonContainer.BackColor = System.Drawing.SystemColors.Info;
			this.pnlButtonContainer.Controls.Add(this.btnClose);
			this.pnlButtonContainer.Controls.Add(this.btnCopyClipboard);
			this.pnlButtonContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtonContainer.Location = new System.Drawing.Point(0, 176);
			this.pnlButtonContainer.Name = "pnlButtonContainer";
			this.pnlButtonContainer.Size = new System.Drawing.Size(651, 40);
			this.pnlButtonContainer.TabIndex = 4;
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(561, 8);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(80, 24);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			this.pnlDescContainer.BackColor = System.Drawing.SystemColors.Info;
			this.pnlDescContainer.Controls.Add(this.pnlTextContainer);
			this.pnlDescContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescContainer.Location = new System.Drawing.Point(8, 0);
			this.pnlDescContainer.Name = "pnlDescContainer";
			this.pnlDescContainer.Size = new System.Drawing.Size(635, 110);
			this.pnlDescContainer.TabIndex = 5;
			this.pnlTextContainer.Controls.Add(this.pnlDescriptionSeparator4);
			this.pnlTextContainer.Controls.Add(this.lblMemberList);
			this.pnlTextContainer.Controls.Add(this.pnlDescriptionSeparator3);
			this.pnlTextContainer.Controls.Add(this.lblMemberListHeader);
			this.pnlTextContainer.Controls.Add(this.pnlDescriptionSeparator2);
			this.pnlTextContainer.Controls.Add(this.lblDescription);
			this.pnlTextContainer.Controls.Add(this.pnlDescriptionSeparator1);
			this.pnlTextContainer.Controls.Add(this.lblDescriptionHeader);
			this.pnlTextContainer.Controls.Add(this.pnlDescriptionSeparator5);
			this.pnlTextContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlTextContainer.Location = new System.Drawing.Point(0, 0);
			this.pnlTextContainer.Name = "pnlTextContainer";
			this.pnlTextContainer.Size = new System.Drawing.Size(635, 110);
			this.pnlTextContainer.TabIndex = 10;
			this.pnlDescriptionSeparator4.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescriptionSeparator4.Location = new System.Drawing.Point(0, 96);
			this.pnlDescriptionSeparator4.Name = "pnlDescriptionSeparator4";
			this.pnlDescriptionSeparator4.Size = new System.Drawing.Size(635, 8);
			this.pnlDescriptionSeparator4.TabIndex = 10;
			this.lblMemberList.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblMemberList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblMemberList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
			this.lblMemberList.Location = new System.Drawing.Point(0, 80);
			this.lblMemberList.Name = "lblMemberList";
			this.lblMemberList.Size = new System.Drawing.Size(635, 16);
			this.lblMemberList.TabIndex = 6;
			this.lblMemberList.Text = "Member List";
			this.pnlDescriptionSeparator3.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescriptionSeparator3.Location = new System.Drawing.Point(0, 78);
			this.pnlDescriptionSeparator3.Name = "pnlDescriptionSeparator3";
			this.pnlDescriptionSeparator3.Size = new System.Drawing.Size(635, 2);
			this.pnlDescriptionSeparator3.TabIndex = 8;
			this.lblMemberListHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblMemberListHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.lblMemberListHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
			this.lblMemberListHeader.Location = new System.Drawing.Point(0, 62);
			this.lblMemberListHeader.Name = "lblMemberListHeader";
			this.lblMemberListHeader.Size = new System.Drawing.Size(635, 16);
			this.lblMemberListHeader.TabIndex = 5;
			this.lblMemberListHeader.Text = "Related Member List:";
			this.pnlDescriptionSeparator2.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescriptionSeparator2.Location = new System.Drawing.Point(0, 54);
			this.pnlDescriptionSeparator2.Name = "pnlDescriptionSeparator2";
			this.pnlDescriptionSeparator2.Size = new System.Drawing.Size(635, 8);
			this.pnlDescriptionSeparator2.TabIndex = 7;
			this.pnlDescriptionSeparator1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescriptionSeparator1.Location = new System.Drawing.Point(0, 20);
			this.pnlDescriptionSeparator1.Name = "pnlDescriptionSeparator1";
			this.pnlDescriptionSeparator1.Size = new System.Drawing.Size(635, 2);
			this.pnlDescriptionSeparator1.TabIndex = 9;
			this.lblDescriptionHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDescriptionHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.lblDescriptionHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
			this.lblDescriptionHeader.Location = new System.Drawing.Point(0, 4);
			this.lblDescriptionHeader.Name = "lblDescriptionHeader";
			this.lblDescriptionHeader.Size = new System.Drawing.Size(635, 16);
			this.lblDescriptionHeader.TabIndex = 11;
			this.lblDescriptionHeader.Text = "Description:";
			this.pnlDescriptionSeparator5.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlDescriptionSeparator5.Location = new System.Drawing.Point(0, 0);
			this.pnlDescriptionSeparator5.Name = "pnlDescriptionSeparator5";
			this.pnlDescriptionSeparator5.Size = new System.Drawing.Size(635, 4);
			this.pnlDescriptionSeparator5.TabIndex = 12;
			this.pnlMemoLeft.BackColor = System.Drawing.SystemColors.Info;
			this.pnlMemoLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlMemoLeft.Location = new System.Drawing.Point(0, 0);
			this.pnlMemoLeft.Name = "pnlMemoLeft";
			this.pnlMemoLeft.Size = new System.Drawing.Size(8, 176);
			this.pnlMemoLeft.TabIndex = 6;
			this.pnlMemoRight.BackColor = System.Drawing.SystemColors.Info;
			this.pnlMemoRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlMemoRight.Location = new System.Drawing.Point(643, 0);
			this.pnlMemoRight.Name = "pnlMemoRight";
			this.pnlMemoRight.Size = new System.Drawing.Size(8, 176);
			this.pnlMemoRight.TabIndex = 7;
			this.pictureDTScreenshot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureDTScreenshot.Location = new System.Drawing.Point(0, 0);
			this.pictureDTScreenshot.Name = "pictureDTScreenshot";
			this.pictureDTScreenshot.Size = new System.Drawing.Size(629, 164);
			this.pictureDTScreenshot.TabIndex = 0;
			this.imageListTabs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTabs.ImageStream")));
			this.imageListTabs.TransparentColor = System.Drawing.Color.Magenta;
			this.imageListTabs.Images.SetKeyName(0, "");
			this.imageListTabs.Images.SetKeyName(1, "");
			this.tabctrlInfo.Appearance.BackColor = System.Drawing.SystemColors.Info;
			this.tabctrlInfo.Appearance.Options.UseBackColor = true;
			this.tabctrlInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabctrlInfo.Images = this.imageListTabs;
			this.tabctrlInfo.Location = new System.Drawing.Point(8, 110);
			this.tabctrlInfo.Name = "tabctrlInfo";
			this.tabctrlInfo.SelectedTabPage = this.tabpgColoredCode;
			this.tabctrlInfo.Size = new System.Drawing.Size(635, 66);
			this.tabctrlInfo.TabIndex = 9;
			this.tabctrlInfo.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabpgColoredCode,
			this.tabpgImage});
			this.tabpgColoredCode.Controls.Add(this.coloredTextControl);
			this.tabpgColoredCode.Name = "tabpgColoredCode";
			this.tabpgColoredCode.Size = new System.Drawing.Size(629, 35);
			this.tabpgColoredCode.Text = "Code";
			this.coloredTextControl.BackColor = System.Drawing.SystemColors.Window;
			this.coloredTextControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.coloredTextControl.ForeColor = System.Drawing.Color.Black;
			this.coloredTextControl.HintBorderVisible = false;
			this.coloredTextControl.LexemProcessorKind = "CSharp";
			this.coloredTextControl.LexerKind = "CSharp";
			this.coloredTextControl.Location = new System.Drawing.Point(0, 0);
			this.coloredTextControl.Name = "coloredTextControl";
			this.coloredTextControl.Size = new System.Drawing.Size(629, 35);
			this.coloredTextControl.TabIndex = 0;
			this.coloredTextControl.TextPadding = 8;
			this.tabpgImage.Controls.Add(this.pictureDTScreenshot);
			this.tabpgImage.ImageIndex = 1;
			this.tabpgImage.Name = "tabpgImage";
			this.tabpgImage.Size = new System.Drawing.Size(629, 164);
			this.tabpgImage.Text = "Screenshot";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(651, 216);
			this.Controls.Add(this.tabctrlInfo);
			this.Controls.Add(this.pnlDescContainer);
			this.Controls.Add(this.pnlMemoRight);
			this.Controls.Add(this.pnlMemoLeft);
			this.Controls.Add(this.pnlButtonContainer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(400, 250);
			this.Name = "FrmWhatsThis";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.pnlButtonContainer.ResumeLayout(false);
			this.pnlDescContainer.ResumeLayout(false);
			this.pnlTextContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureDTScreenshot.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabctrlInfo)).EndInit();
			this.tabctrlInfo.ResumeLayout(false);
			this.tabpgColoredCode.ResumeLayout(false);
			this.tabpgImage.ResumeLayout(false);
			this.ResumeLayout(false);
	  }
		#endregion
		bool isExistMembers = false;
		public override void Show(string controlName, WhatsThisParams whatsThisParams, SourceFileType sourceFileType) {
			lblMemberList.Visible = false;
			isExistMembers = !string.IsNullOrEmpty(whatsThisParams.MemberList);
			this.SuspendLayout();
			UpdateCodeControl(sourceFileType);
			base.Show(controlName, whatsThisParams, sourceFileType);
			UpdateMemberList();
			btnClose.Focus();
			BeginInvoke(new MethodInvoker(UpdateMemberList));
			bool allowScrolls = BestFitForm();
			this.Width -= 1;
			this.ResumeLayout();
			this.Width += 1; 
			if(!allowScrolls && coloredTextControl.ViewInfo.ScrollInfo.ScrollBarVisible)
				BestFitForm(); 
		}
		bool IsExistMembers { get { return isExistMembers; } }
		bool IsExistDescription { get { return !string.IsNullOrEmpty(lblDescription.Text); } }
		void UpdateMemberList() {
			memberInfo.Refresh();
			UpdateDescriptionPanel();
			lblMemberListHeader.Visible = pnlDescriptionSeparator4.Visible = IsExistMembers;
			lblDescriptionHeader.Visible = pnlDescriptionSeparator2.Visible = IsExistDescription;
			int height = 0;
			foreach(Control ctrl in pnlTextContainer.Controls)
				if(ctrl.Visible)
					height += ctrl.Height;
			pnlDescContainer.Height = height;
		}
		bool BestFitForm() {
			bool ret = false;
			Screen screen = Screen.FromControl(this);
			int indent = DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(0, 35)).Height;
			this.ClientSize = new Size(coloredTextControl.ViewInfo.Populator.TotalWidth + indent * 4 / 3,
				pnlDescContainer.Height + pnlButtonContainer.Height + Math.Max(coloredTextControl.ViewInfo.Populator.TotalHeight, indent * 2) + indent);
			if(this.Width > screen.Bounds.Width / 3 * 2) {
				this.Width = screen.Bounds.Width / 3 * 2;
				ret = true;
			}
			if(this.Height > screen.Bounds.Height / 3 * 2) {
				this.Height = screen.Bounds.Height / 3 * 2;
				this.Width += indent;
				ret = true;
			}
			ControlUtils.UpdateFrmToFitScreen(this);
			return ret;
		}
		protected override void UpdateControls(WhatsThisParams whatsThisParams) {
			base.UpdateControls(whatsThisParams);
			coloredTextControl.Text = whatsThisParams.Code;
			lblDescription.Text = whatsThisParams.Description;
			SetMemberList(whatsThisParams.MemberList);
			if(whatsThisParams.DTImage == string.Empty)
				tabpgImage.PageVisible = false;
			else {
				tabpgImage.PageVisible = true;
				pictureDTScreenshot.Image = Image.FromFile(FilePathUtils.FindFilePath(whatsThisParams.DTImage, true));
			}
		}
		LabelInfo memberInfo = new LabelInfo();
		void SetMemberList(string list) {
			memberInfo.Parent = lblMemberList.Parent;
			memberInfo.Size = new Size(0, string.IsNullOrEmpty(list) ? 0 : 23);
			memberInfo.Dock = DockStyle.Top;
			memberInfo.BringToFront();
			pnlDescriptionSeparator4.BringToFront();
			memberInfo.Font = lblMemberList.Font;
			string[] members = list.Split(',');
			for(int i = 0; i < members.Length; i++ ) {
				memberInfo.Texts.Add(members[i].Trim(), Color.Blue, true);
				if(i != members.Length - 1) memberInfo.Texts.Add(", ");
			}
			memberInfo.AutoHeight = true;
			memberInfo.ItemClick += new LabelInfoItemClickEvent(OnLabelClick);
		}
		void OnLabelClick(object sender, LabelInfoItemClickEventArgs e) {
			string name = e.InfoText.Text;
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = string.Format("http://search.devexpress.com/?q={0}&p=T4|P1|0&d=128", name);
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		private void UpdateCodeControl(SourceFileType type) {
			switch(type) {
				case SourceFileType.CS:
					coloredTextControl.LexerKind = "CSharp";
					coloredTextControl.LexemProcessorKind = "CSharp";
					break;
				case SourceFileType.VB:
					coloredTextControl.LexerKind = "VB";
					coloredTextControl.LexemProcessorKind = "VB";
					break;
			}
		}
		protected override void UpdateDescriptionPanel() {
			ControlUtils.UpdateLabelHeight(lblDescription);
			ControlUtils.UpdateLabelHeight(lblMemberList);
			int totalHeight = 0;
			foreach(Control control in pnlTextContainer.Controls)
				if(control.Visible)
					totalHeight += control.Height;
			pnlDescContainer.Height = totalHeight;
		}
		private void btnCopyClipboard_Click(object sender, System.EventArgs e) {
			Clipboard.SetDataObject(coloredTextControl.Text);
		}
		private void btnClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
	}
	public class FrmWhatsThisBase : XtraForm {
		string controlName;
		public virtual void Show(string controlName, WhatsThisParams whatsThisParams, SourceFileType sourceFileType) {
			this.controlName = controlName;
			UpdateControls(whatsThisParams);
			UpdateDescriptionPanel();
			ControlUtils.UpdateFrmToFitScreen(this);
			Show();
		}
		protected virtual void UpdateControls(WhatsThisParams whatsThisParams) {
			this.Text = whatsThisParams.Caption + " (" + controlName + ")";
		}
		protected virtual void UpdateDescriptionPanel() {
		}
		public string ControlName { get { return controlName; } }
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			this.Close();
		}
		bool resizing = false;
		protected override void OnResize(EventArgs e) {
			if(resizing) return;
			resizing = true;
			base.OnResize(e);
			UpdateDescriptionPanel();
			resizing = false;
		}
	}
}
