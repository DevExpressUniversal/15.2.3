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
	partial class HeaderFooterEditControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderFooterEditControl));
			this.btnSheetName = new DevExpress.XtraEditors.SimpleButton();
			this.edtLeftFooter = new DevExpress.XtraEditors.MemoEdit();
			this.edtCenterFooter = new DevExpress.XtraEditors.MemoEdit();
			this.edtRightFooter = new DevExpress.XtraEditors.MemoEdit();
			this.edtRightHeader = new DevExpress.XtraEditors.MemoEdit();
			this.edtCenterHeader = new DevExpress.XtraEditors.MemoEdit();
			this.edtLeftHeader = new DevExpress.XtraEditors.MemoEdit();
			this.lblRightFooter = new DevExpress.XtraEditors.LabelControl();
			this.lblCenterFooter = new DevExpress.XtraEditors.LabelControl();
			this.lblLeftFooter = new DevExpress.XtraEditors.LabelControl();
			this.lblRightHeader = new DevExpress.XtraEditors.LabelControl();
			this.lblCenterHeader = new DevExpress.XtraEditors.LabelControl();
			this.lblLeftHeader = new DevExpress.XtraEditors.LabelControl();
			this.btnFileName = new DevExpress.XtraEditors.SimpleButton();
			this.btnFilePath = new DevExpress.XtraEditors.SimpleButton();
			this.btnTime = new DevExpress.XtraEditors.SimpleButton();
			this.btnDate = new DevExpress.XtraEditors.SimpleButton();
			this.lblHeaderFooter = new DevExpress.XtraEditors.LabelControl();
			this.btnPages = new DevExpress.XtraEditors.SimpleButton();
			this.btnPage = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.edtLeftFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCenterFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRightFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRightHeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCenterHeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLeftHeader.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnSheetName.Image = ((System.Drawing.Image)(resources.GetObject("btnSheetName.Image")));
			this.btnSheetName.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnSheetName, "btnSheetName");
			this.btnSheetName.Name = "btnSheetName";
			resources.ApplyResources(this.edtLeftFooter, "edtLeftFooter");
			this.edtLeftFooter.Name = "edtLeftFooter";
			this.edtLeftFooter.Properties.AccessibleName = resources.GetString("edtLeftFooter.Properties.AccessibleName");
			this.edtLeftFooter.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.edtLeftFooter.Properties.Appearance.Options.UseTextOptions = true;
			this.edtLeftFooter.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			resources.ApplyResources(this.edtCenterFooter, "edtCenterFooter");
			this.edtCenterFooter.Name = "edtCenterFooter";
			this.edtCenterFooter.Properties.AccessibleName = resources.GetString("edtCenterFooter.Properties.AccessibleName");
			this.edtCenterFooter.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.edtCenterFooter.Properties.Appearance.Options.UseTextOptions = true;
			this.edtCenterFooter.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.edtRightFooter, "edtRightFooter");
			this.edtRightFooter.Name = "edtRightFooter";
			this.edtRightFooter.Properties.AccessibleName = resources.GetString("edtRightFooter.Properties.AccessibleName");
			this.edtRightFooter.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.edtRightFooter.Properties.Appearance.Options.UseTextOptions = true;
			this.edtRightFooter.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			resources.ApplyResources(this.edtRightHeader, "edtRightHeader");
			this.edtRightHeader.Name = "edtRightHeader";
			this.edtRightHeader.Properties.AccessibleName = resources.GetString("edtRightHeader.Properties.AccessibleName");
			this.edtRightHeader.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.edtRightHeader.Properties.Appearance.Options.UseTextOptions = true;
			this.edtRightHeader.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			resources.ApplyResources(this.edtCenterHeader, "edtCenterHeader");
			this.edtCenterHeader.Name = "edtCenterHeader";
			this.edtCenterHeader.Properties.AccessibleName = resources.GetString("edtCenterHeader.Properties.AccessibleName");
			this.edtCenterHeader.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.edtCenterHeader.Properties.Appearance.Options.UseTextOptions = true;
			this.edtCenterHeader.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.edtLeftHeader, "edtLeftHeader");
			this.edtLeftHeader.Name = "edtLeftHeader";
			this.edtLeftHeader.Properties.AccessibleName = resources.GetString("edtLeftHeader.Properties.AccessibleName");
			this.edtLeftHeader.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
			this.edtLeftHeader.Properties.Appearance.Options.UseTextOptions = true;
			this.edtLeftHeader.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			resources.ApplyResources(this.lblRightFooter, "lblRightFooter");
			this.lblRightFooter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblRightFooter.Name = "lblRightFooter";
			resources.ApplyResources(this.lblCenterFooter, "lblCenterFooter");
			this.lblCenterFooter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCenterFooter.Name = "lblCenterFooter";
			resources.ApplyResources(this.lblLeftFooter, "lblLeftFooter");
			this.lblLeftFooter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLeftFooter.Name = "lblLeftFooter";
			resources.ApplyResources(this.lblRightHeader, "lblRightHeader");
			this.lblRightHeader.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblRightHeader.Name = "lblRightHeader";
			resources.ApplyResources(this.lblCenterHeader, "lblCenterHeader");
			this.lblCenterHeader.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblCenterHeader.Name = "lblCenterHeader";
			resources.ApplyResources(this.lblLeftHeader, "lblLeftHeader");
			this.lblLeftHeader.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLeftHeader.Name = "lblLeftHeader";
			this.btnFileName.Image = ((System.Drawing.Image)(resources.GetObject("btnFileName.Image")));
			this.btnFileName.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnFileName, "btnFileName");
			this.btnFileName.Name = "btnFileName";
			this.btnFilePath.Image = ((System.Drawing.Image)(resources.GetObject("btnFilePath.Image")));
			this.btnFilePath.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnFilePath, "btnFilePath");
			this.btnFilePath.Name = "btnFilePath";
			this.btnTime.Image = ((System.Drawing.Image)(resources.GetObject("btnTime.Image")));
			this.btnTime.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnTime, "btnTime");
			this.btnTime.Name = "btnTime";
			this.btnDate.Image = ((System.Drawing.Image)(resources.GetObject("btnDate.Image")));
			this.btnDate.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnDate, "btnDate");
			this.btnDate.Name = "btnDate";
			resources.ApplyResources(this.lblHeaderFooter, "lblHeaderFooter");
			this.lblHeaderFooter.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblHeaderFooter.Name = "lblHeaderFooter";
			this.btnPages.Image = ((System.Drawing.Image)(resources.GetObject("btnPages.Image")));
			this.btnPages.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnPages, "btnPages");
			this.btnPages.Name = "btnPages";
			this.btnPages.Click += new System.EventHandler(this.OnPagesClick);
			this.btnPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPage.Image")));
			this.btnPage.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnPage, "btnPage");
			this.btnPage.Name = "btnPage";
			this.btnPage.Click += new System.EventHandler(this.OnPageClick);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnPage);
			this.Controls.Add(this.btnPages);
			this.Controls.Add(this.btnSheetName);
			this.Controls.Add(this.edtLeftFooter);
			this.Controls.Add(this.edtCenterFooter);
			this.Controls.Add(this.edtRightFooter);
			this.Controls.Add(this.edtRightHeader);
			this.Controls.Add(this.edtCenterHeader);
			this.Controls.Add(this.edtLeftHeader);
			this.Controls.Add(this.lblRightFooter);
			this.Controls.Add(this.lblCenterFooter);
			this.Controls.Add(this.lblLeftFooter);
			this.Controls.Add(this.lblRightHeader);
			this.Controls.Add(this.lblCenterHeader);
			this.Controls.Add(this.lblLeftHeader);
			this.Controls.Add(this.btnFileName);
			this.Controls.Add(this.btnFilePath);
			this.Controls.Add(this.btnTime);
			this.Controls.Add(this.btnDate);
			this.Controls.Add(this.lblHeaderFooter);
			this.Name = "HeaderFooterEditControl";
			((System.ComponentModel.ISupportInitialize)(this.edtLeftFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCenterFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRightFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRightHeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtCenterHeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLeftHeader.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnSheetName;
		private XtraEditors.MemoEdit edtLeftFooter;
		private XtraEditors.MemoEdit edtCenterFooter;
		private XtraEditors.MemoEdit edtRightFooter;
		private XtraEditors.MemoEdit edtRightHeader;
		private XtraEditors.MemoEdit edtCenterHeader;
		private XtraEditors.MemoEdit edtLeftHeader;
		private XtraEditors.LabelControl lblRightFooter;
		private XtraEditors.LabelControl lblCenterFooter;
		private XtraEditors.LabelControl lblLeftFooter;
		private XtraEditors.LabelControl lblRightHeader;
		private XtraEditors.LabelControl lblCenterHeader;
		private XtraEditors.LabelControl lblLeftHeader;
		private XtraEditors.SimpleButton btnFileName;
		private XtraEditors.SimpleButton btnFilePath;
		private XtraEditors.SimpleButton btnTime;
		private XtraEditors.SimpleButton btnDate;
		private XtraEditors.LabelControl lblHeaderFooter;
		private XtraEditors.SimpleButton btnPages;
		private XtraEditors.SimpleButton btnPage;
	}
}
