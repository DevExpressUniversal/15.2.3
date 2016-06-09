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
	public class WizPageUngroupedLayout : WizPageLayoutBase {
		private XtraEditors.CheckEdit chkJustifiedLayout;
		private XtraEditors.CheckEdit chkTabularLayout;
		private XtraEditors.CheckEdit chkColumnarLayout;
		private System.ComponentModel.IContainer components = null;
		public WizPageUngroupedLayout(XRWizardRunnerBase runner)
			: base(runner) {
			Initialize();
			layoutEdits = new XtraEditors.CheckEdit[] { chkColumnarLayout, chkTabularLayout, chkJustifiedLayout };
			layoutEditValues[chkColumnarLayout] = ReportLayout.Columnar;
			layoutEditValues[chkTabularLayout] = ReportLayout.Tabular;
			layoutEditValues[chkJustifiedLayout] = ReportLayout.Justified;
		}
		WizPageUngroupedLayout()
			: base() {
			Initialize();
		}
		void Initialize() {
			InitializeComponent();
			layoutImageList.ImageSize = new Size(204, 228);
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizColumnar.gif", typeof(LocalResFinder)));
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizTabular.gif", typeof(LocalResFinder)));
			layoutImageList.Images.Add(ResourceImageHelper.CreateBitmapFromResources("Images.WizJustified.gif", typeof(LocalResFinder)));
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopUngroupedLayout.gif", typeof(LocalResFinder));
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageUngroupedLayout));
			this.chkColumnarLayout = new DevExpress.XtraEditors.CheckEdit();
			this.chkTabularLayout = new DevExpress.XtraEditors.CheckEdit();
			this.chkJustifiedLayout = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.grpLayout)).BeginInit();
			this.grpLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkColumnarLayout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkTabularLayout.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkJustifiedLayout.Properties)).BeginInit();
			this.SuspendLayout();
			this.grpLayout.Controls.Add(this.chkJustifiedLayout);
			this.grpLayout.Controls.Add(this.chkTabularLayout);
			this.grpLayout.Controls.Add(this.chkColumnarLayout);
			resources.ApplyResources(this.chkColumnarLayout, "chkColumnarLayout");
			this.chkColumnarLayout.Name = "chkColumnarLayout";
			this.chkColumnarLayout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkColumnarLayout.Properties.Appearance.BackColor")));
			this.chkColumnarLayout.Properties.Appearance.Options.UseBackColor = true;
			this.chkColumnarLayout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkColumnarLayout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkColumnarLayout.Properties.Caption = resources.GetString("chkColumnarLayout.Properties.Caption");
			this.chkColumnarLayout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkColumnarLayout.Properties.RadioGroupIndex = 0;
			this.chkColumnarLayout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			resources.ApplyResources(this.chkTabularLayout, "chkTabularLayout");
			this.chkTabularLayout.Name = "chkTabularLayout";
			this.chkTabularLayout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkTabularLayout.Properties.Appearance.BackColor")));
			this.chkTabularLayout.Properties.Appearance.Options.UseBackColor = true;
			this.chkTabularLayout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkTabularLayout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkTabularLayout.Properties.Caption = resources.GetString("chkTabularLayout.Properties.Caption");
			this.chkTabularLayout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkTabularLayout.Properties.RadioGroupIndex = 0;
			this.chkTabularLayout.TabStop = false;
			this.chkTabularLayout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			resources.ApplyResources(this.chkJustifiedLayout, "chkJustifiedLayout");
			this.chkJustifiedLayout.Name = "chkJustifiedLayout";
			this.chkJustifiedLayout.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("chkJustifiedLayout.Properties.Appearance.BackColor")));
			this.chkJustifiedLayout.Properties.Appearance.Options.UseBackColor = true;
			this.chkJustifiedLayout.Properties.Appearance.Options.UseTextOptions = true;
			this.chkJustifiedLayout.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chkJustifiedLayout.Properties.Caption = resources.GetString("chkJustifiedLayout.Properties.Caption");
			this.chkJustifiedLayout.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.chkJustifiedLayout.Properties.RadioGroupIndex = 0;
			this.chkJustifiedLayout.TabStop = false;
			this.chkJustifiedLayout.CheckedChanged += new System.EventHandler(this.chkLayout_CheckedChanged);
			this.Name = "WizPageUngroupedLayout";
			((System.ComponentModel.ISupportInitialize)(this.grpLayout)).EndInit();
			this.grpLayout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkColumnarLayout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkTabularLayout.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkJustifiedLayout.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void chkLayout_CheckedChanged(object sender, EventArgs e) {
			UpdateLayoutImage();
		}
		protected override string OnWizardBack() {
			RollbackChanges();
			return "WizPageGrouping";
		}
		protected override string OnWizardNext() {
			ApplyChanges();
			return "WizPageStyle";
		}
	}
}
