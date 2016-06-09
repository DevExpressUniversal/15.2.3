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

namespace DevExpress.XtraScheduler.Forms {
	partial class PageSetupForm {
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupForm));
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.format = new DevExpress.XtraTab.XtraTabPage();
			this.tabFormatControl = new DevExpress.XtraScheduler.Design.PageSetupFormatTabControl();
			this.paper = new DevExpress.XtraTab.XtraTabPage();
			this.tabPaperControl = new DevExpress.XtraScheduler.Design.PageSetupPaperTabControl();
			this.tabPageResources = new DevExpress.XtraTab.XtraTabPage();
			this.tabResources = new DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnPrintPreview = new DevExpress.XtraEditors.SimpleButton();
			this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.format.SuspendLayout();
			this.paper.SuspendLayout();
			this.tabPageResources.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.SuspendLayout();
			this.xtraTabControl.Appearance.BackColor = System.Drawing.SystemColors.Control;
			this.xtraTabControl.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.format;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.format,
			this.paper,
			this.tabPageResources});
			this.format.Controls.Add(this.tabFormatControl);
			this.format.Name = "format";
			resources.ApplyResources(this.format, "format");
			this.tabFormatControl.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.tabFormatControl.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.tabFormatControl, "tabFormatControl");
			this.tabFormatControl.Name = "tabFormatControl";
			this.paper.Controls.Add(this.tabPaperControl);
			this.paper.Name = "paper";
			resources.ApplyResources(this.paper, "paper");
			this.tabPaperControl.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.tabPaperControl.Appearance.Options.UseBackColor = true;
			this.tabPaperControl.ApplySettingsToAllStyles = false;
			resources.ApplyResources(this.tabPaperControl, "tabPaperControl");
			this.tabPaperControl.Name = "tabPaperControl";
			this.tabPageResources.Controls.Add(this.tabResources);
			this.tabPageResources.Name = "tabPageResources";
			resources.ApplyResources(this.tabPageResources, "tabPageResources");
			resources.ApplyResources(this.tabResources, "tabResources");
			this.tabResources.Name = "tabResources";
			this.tabResources.ResourceOptions = null;
			this.tabResources.Storage = null;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnPrintPreview, "btnPrintPreview");
			this.btnPrintPreview.Name = "btnPrintPreview";
			resources.ApplyResources(this.btnPrint, "btnPrint");
			this.btnPrint.Name = "btnPrint";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.btnOk);
			this.panelControl1.Controls.Add(this.btnCancel);
			this.panelControl1.Controls.Add(this.btnPrintPreview);
			this.panelControl1.Controls.Add(this.btnPrint);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.xtraTabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PageSetupForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.format.ResumeLayout(false);
			this.paper.ResumeLayout(false);
			this.tabPageResources.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected internal DevExpress.XtraScheduler.Design.PageSetupFormatTabControl tabFormatControl;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected internal DevExpress.XtraEditors.SimpleButton btnPrintPreview;
		protected DevExpress.XtraTab.XtraTabControl xtraTabControl;
		protected DevExpress.XtraTab.XtraTabPage paper;
		protected DevExpress.XtraEditors.PanelControl panelControl1;
		protected DevExpress.XtraTab.XtraTabPage format;
		protected internal DevExpress.XtraEditors.SimpleButton btnPrint;
		protected internal DevExpress.XtraScheduler.Design.PageSetupPaperTabControl tabPaperControl;
		protected DevExpress.XtraTab.XtraTabPage tabPageResources;
		protected internal DevExpress.XtraScheduler.Design.PageSetupResourcesTabControl tabResources;
		System.ComponentModel.IContainer components = null;
	}
}
