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

namespace DevExpress.XtraSpreadsheet.Forms.Design {
	partial class PageMarginsPreviewControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageMarginsPreviewControl));
			this.edtLeft = new DevExpress.XtraEditors.SpinEdit();
			this.lblLeft = new DevExpress.XtraEditors.LabelControl();
			this.edtBottom = new DevExpress.XtraEditors.SpinEdit();
			this.lblBottom = new DevExpress.XtraEditors.LabelControl();
			this.edtFooter = new DevExpress.XtraEditors.SpinEdit();
			this.lblFooterMargins = new DevExpress.XtraEditors.LabelControl();
			this.edtRight = new DevExpress.XtraEditors.SpinEdit();
			this.lblRight = new DevExpress.XtraEditors.LabelControl();
			this.edtHeader = new DevExpress.XtraEditors.SpinEdit();
			this.lblHeaderMargins = new DevExpress.XtraEditors.LabelControl();
			this.edtTop = new DevExpress.XtraEditors.SpinEdit();
			this.lblTop = new DevExpress.XtraEditors.LabelControl();
			this.drawPanelPortraitOrientation = new DevExpress.XtraEditors.PanelControl();
			this.chkVertically = new DevExpress.XtraEditors.CheckEdit();
			this.chkHorizontally = new DevExpress.XtraEditors.CheckEdit();
			this.lblCenterOnPage = new DevExpress.XtraEditors.LabelControl();
			this.drawPanelLandscapeOrientation = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtLeft.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtHeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.drawPanelPortraitOrientation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVertically.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkHorizontally.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.drawPanelLandscapeOrientation)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.edtLeft, "edtLeft");
			this.edtLeft.Name = "edtLeft";
			this.edtLeft.Properties.AccessibleName = resources.GetString("edtLeft.Properties.AccessibleName");
			this.edtLeft.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtLeft.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLeft.Properties.Buttons"))))});
			this.edtLeft.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.edtLeft.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLeft.Name = "lblLeft";
			resources.ApplyResources(this.edtBottom, "edtBottom");
			this.edtBottom.Name = "edtBottom";
			this.edtBottom.Properties.AccessibleName = resources.GetString("edtBottom.Properties.AccessibleName");
			this.edtBottom.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtBottom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtBottom.Properties.Buttons"))))});
			this.edtBottom.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.edtBottom.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			resources.ApplyResources(this.lblBottom, "lblBottom");
			this.lblBottom.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblBottom.Name = "lblBottom";
			resources.ApplyResources(this.edtFooter, "edtFooter");
			this.edtFooter.Name = "edtFooter";
			this.edtFooter.Properties.AccessibleName = resources.GetString("edtFooter.Properties.AccessibleName");
			this.edtFooter.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtFooter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFooter.Properties.Buttons"))))});
			this.edtFooter.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.edtFooter.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			resources.ApplyResources(this.lblFooterMargins, "lblFooterMargins");
			this.lblFooterMargins.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblFooterMargins.Name = "lblFooterMargins";
			resources.ApplyResources(this.edtRight, "edtRight");
			this.edtRight.Name = "edtRight";
			this.edtRight.Properties.AccessibleName = resources.GetString("edtRight.Properties.AccessibleName");
			this.edtRight.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtRight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtRight.Properties.Buttons"))))});
			this.edtRight.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.edtRight.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.edtHeader, "edtHeader");
			this.edtHeader.Name = "edtHeader";
			this.edtHeader.Properties.AccessibleName = resources.GetString("edtHeader.Properties.AccessibleName");
			this.edtHeader.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtHeader.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtHeader.Properties.Buttons"))))});
			this.edtHeader.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.edtHeader.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			resources.ApplyResources(this.lblHeaderMargins, "lblHeaderMargins");
			this.lblHeaderMargins.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblHeaderMargins.Name = "lblHeaderMargins";
			resources.ApplyResources(this.edtTop, "edtTop");
			this.edtTop.Name = "edtTop";
			this.edtTop.Properties.AccessibleName = resources.GetString("edtTop.Properties.AccessibleName");
			this.edtTop.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.SpinButton;
			this.edtTop.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtTop.Properties.Buttons"))))});
			this.edtTop.Properties.Increment = new decimal(new int[] {
			25,
			0,
			0,
			131072});
			this.edtTop.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			resources.ApplyResources(this.lblTop, "lblTop");
			this.lblTop.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblTop.Name = "lblTop";
			this.drawPanelPortraitOrientation.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("drawPanelPortraitOrientation.Appearance.BackColor")));
			this.drawPanelPortraitOrientation.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.drawPanelPortraitOrientation, "drawPanelPortraitOrientation");
			this.drawPanelPortraitOrientation.Name = "drawPanelPortraitOrientation";
			resources.ApplyResources(this.chkVertically, "chkVertically");
			this.chkVertically.Name = "chkVertically";
			this.chkVertically.Properties.AccessibleName = resources.GetString("chkVertically.Properties.AccessibleName");
			this.chkVertically.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkVertically.Properties.AutoWidth = true;
			this.chkVertically.Properties.Caption = resources.GetString("chkVertically.Properties.Caption");
			resources.ApplyResources(this.chkHorizontally, "chkHorizontally");
			this.chkHorizontally.Name = "chkHorizontally";
			this.chkHorizontally.Properties.AccessibleName = resources.GetString("chkHorizontally.Properties.AccessibleName");
			this.chkHorizontally.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkHorizontally.Properties.AutoWidth = true;
			this.chkHorizontally.Properties.Caption = resources.GetString("chkHorizontally.Properties.Caption");
			resources.ApplyResources(this.lblCenterOnPage, "lblCenterOnPage");
			this.lblCenterOnPage.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCenterOnPage.LineVisible = true;
			this.lblCenterOnPage.Name = "lblCenterOnPage";
			this.drawPanelLandscapeOrientation.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("drawPanelLandscapeOrientation.Appearance.BackColor")));
			this.drawPanelLandscapeOrientation.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.drawPanelLandscapeOrientation, "drawPanelLandscapeOrientation");
			this.drawPanelLandscapeOrientation.Name = "drawPanelLandscapeOrientation";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.drawPanelLandscapeOrientation);
			this.Controls.Add(this.chkVertically);
			this.Controls.Add(this.chkHorizontally);
			this.Controls.Add(this.lblCenterOnPage);
			this.Controls.Add(this.edtLeft);
			this.Controls.Add(this.lblLeft);
			this.Controls.Add(this.edtBottom);
			this.Controls.Add(this.lblBottom);
			this.Controls.Add(this.edtFooter);
			this.Controls.Add(this.lblFooterMargins);
			this.Controls.Add(this.edtRight);
			this.Controls.Add(this.lblRight);
			this.Controls.Add(this.edtHeader);
			this.Controls.Add(this.lblHeaderMargins);
			this.Controls.Add(this.edtTop);
			this.Controls.Add(this.lblTop);
			this.Controls.Add(this.drawPanelPortraitOrientation);
			this.Name = "PageMarginsPreviewControl";
			((System.ComponentModel.ISupportInitialize)(this.edtLeft.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtHeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.drawPanelPortraitOrientation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVertically.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkHorizontally.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.drawPanelLandscapeOrientation)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SpinEdit edtLeft;
		private XtraEditors.LabelControl lblLeft;
		private XtraEditors.SpinEdit edtBottom;
		private XtraEditors.LabelControl lblBottom;
		private XtraEditors.SpinEdit edtFooter;
		private XtraEditors.LabelControl lblFooterMargins;
		private XtraEditors.SpinEdit edtRight;
		private XtraEditors.LabelControl lblRight;
		private XtraEditors.SpinEdit edtHeader;
		private XtraEditors.LabelControl lblHeaderMargins;
		private XtraEditors.SpinEdit edtTop;
		private XtraEditors.LabelControl lblTop;
		private XtraEditors.PanelControl drawPanelPortraitOrientation;
		private XtraEditors.CheckEdit chkVertically;
		private XtraEditors.CheckEdit chkHorizontally;
		private XtraEditors.LabelControl lblCenterOnPage;
		private XtraEditors.PanelControl drawPanelLandscapeOrientation;
	}
}
