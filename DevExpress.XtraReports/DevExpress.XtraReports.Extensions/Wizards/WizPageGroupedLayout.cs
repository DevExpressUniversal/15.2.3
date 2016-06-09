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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class WizPageGroupedLayout : WizPageLayoutBase {
		private XtraEditors.CheckEdit chkAlignLeft2Layout;
		private XtraEditors.CheckEdit chkAlignLeft1Layout;
		private XtraEditors.CheckEdit chkOutline2Layout;
		private XtraEditors.CheckEdit chkOutline1Layout;
		private XtraEditors.CheckEdit chkStepedLayout;
		private System.ComponentModel.IContainer components = null;
		public WizPageGroupedLayout(XRWizardRunnerBase runner)
			: base(runner) {
			Initialize();
			layoutEdits = new XtraEditors.CheckEdit[] { chkStepedLayout, chkOutline1Layout, chkOutline2Layout, chkAlignLeft1Layout, chkAlignLeft2Layout };
			layoutEditValues[chkStepedLayout] = ReportLayout.Stepped;
			layoutEditValues[chkOutline1Layout] = ReportLayout.Outline1;
			layoutEditValues[chkOutline2Layout] = ReportLayout.Outline2;
			layoutEditValues[chkAlignLeft1Layout] = ReportLayout.AlignLeft1;
			layoutEditValues[chkAlignLeft2Layout] = ReportLayout.AlignLeft2;
		}
		WizPageGroupedLayout() {
			Initialize();
		}
		void Initialize() {
			InitializeComponent();
			layoutImageList.ImageSize = new Size(204, 228);
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizStepped.gif", typeof(LocalResFinder)));
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizOutline1.gif", typeof(LocalResFinder)));
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizOutline2.gif", typeof(LocalResFinder)));
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizAlignLeft1.gif", typeof(LocalResFinder)));
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizAlignLeft2.gif", typeof(LocalResFinder)));
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopGroupedLayout.gif", typeof(LocalResFinder));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageGroupedLayout));
			this.chkStepedLayout = new DevExpress.XtraEditors.CheckEdit();
			this.chkOutline1Layout = new DevExpress.XtraEditors.CheckEdit();
			this.chkOutline2Layout = new DevExpress.XtraEditors.CheckEdit();
			this.chkAlignLeft1Layout = new DevExpress.XtraEditors.CheckEdit();
			this.chkAlignLeft2Layout = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpLayout)).BeginInit();
			this.grpLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkStepedLayout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkOutline1Layout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkOutline2Layout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlignLeft1Layout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlignLeft2Layout.Properties)).BeginInit();
			this.SuspendLayout();
			this.grpLayout.Controls.Add(this.chkAlignLeft2Layout);
			this.grpLayout.Controls.Add(this.chkAlignLeft1Layout);
			this.grpLayout.Controls.Add(this.chkOutline2Layout);
			this.grpLayout.Controls.Add(this.chkOutline1Layout);
			this.grpLayout.Controls.Add(this.chkStepedLayout);
			resources.ApplyResources(this.grpLayout, "grpLayout");
			resources.ApplyResources(this.chkStepedLayout, "chkStepedLayout");
			this.chkStepedLayout.Name = "chkStepedLayout";
			this.chkStepedLayout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkStepedLayout.Properties.Appearance.BackColor")));
			this.chkStepedLayout.Properties.Appearance.Options.UseBackColor = true;
			this.chkStepedLayout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkStepedLayout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkStepedLayout.Properties.Caption = resources.GetString("chkStepedLayout.Properties.Caption");
			this.chkStepedLayout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkStepedLayout.Properties.RadioGroupIndex = 0;
			this.chkStepedLayout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			resources.ApplyResources(this.chkOutline1Layout, "chkOutline1Layout");
			this.chkOutline1Layout.Name = "chkOutline1Layout";
			this.chkOutline1Layout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkOutline1Layout.Properties.Appearance.BackColor")));
			this.chkOutline1Layout.Properties.Appearance.Options.UseBackColor = true;
			this.chkOutline1Layout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkOutline1Layout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkOutline1Layout.Properties.Caption = resources.GetString("chkOutline1Layout.Properties.Caption");
			this.chkOutline1Layout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkOutline1Layout.Properties.RadioGroupIndex = 0;
			this.chkOutline1Layout.TabStop = false;
			this.chkOutline1Layout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			resources.ApplyResources(this.chkOutline2Layout, "chkOutline2Layout");
			this.chkOutline2Layout.Name = "chkOutline2Layout";
			this.chkOutline2Layout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkOutline2Layout.Properties.Appearance.BackColor")));
			this.chkOutline2Layout.Properties.Appearance.Options.UseBackColor = true;
			this.chkOutline2Layout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkOutline2Layout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkOutline2Layout.Properties.Caption = resources.GetString("chkOutline2Layout.Properties.Caption");
			this.chkOutline2Layout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkOutline2Layout.Properties.RadioGroupIndex = 0;
			this.chkOutline2Layout.TabStop = false;
			this.chkOutline2Layout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			resources.ApplyResources(this.chkAlignLeft1Layout, "chkAlignLeft1Layout");
			this.chkAlignLeft1Layout.Name = "chkAlignLeft1Layout";
			this.chkAlignLeft1Layout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkAlignLeft1Layout.Properties.Appearance.BackColor")));
			this.chkAlignLeft1Layout.Properties.Appearance.Options.UseBackColor = true;
			this.chkAlignLeft1Layout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkAlignLeft1Layout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkAlignLeft1Layout.Properties.Caption = resources.GetString("chkAlignLeft1Layout.Properties.Caption");
			this.chkAlignLeft1Layout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkAlignLeft1Layout.Properties.RadioGroupIndex = 0;
			this.chkAlignLeft1Layout.TabStop = false;
			this.chkAlignLeft1Layout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			resources.ApplyResources(this.chkAlignLeft2Layout, "chkAlignLeft2Layout");
			this.chkAlignLeft2Layout.Name = "chkAlignLeft2Layout";
			this.chkAlignLeft2Layout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkAlignLeft2Layout.Properties.Appearance.BackColor")));
			this.chkAlignLeft2Layout.Properties.Appearance.Options.UseBackColor = true;
			this.chkAlignLeft2Layout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkAlignLeft2Layout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkAlignLeft2Layout.Properties.Caption = resources.GetString("chkAlignLeft2Layout.Properties.Caption");
			this.chkAlignLeft2Layout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkAlignLeft2Layout.Properties.RadioGroupIndex = 0;
			this.chkAlignLeft2Layout.TabStop = false;
			this.chkAlignLeft2Layout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			this.Name = "WizPageGroupedLayout";
			((System.ComponentModel.ISupportInitialize)(this.grpLayout)).EndInit();
			this.grpLayout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkStepedLayout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkOutline1Layout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkOutline2Layout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlignLeft1Layout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkAlignLeft2Layout.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void chkLayout_CheckedChanged(object sender, EventArgs e) {
			UpdateLayoutImage();
		}
		protected override string OnWizardBack() {
			RollbackChanges();
			return fWizard.GetFieldsForSummary().Count > 0 ? "WizPageSummary" : "WizPageGrouping";
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			return "WizPageStyle";
		}
	}
}
