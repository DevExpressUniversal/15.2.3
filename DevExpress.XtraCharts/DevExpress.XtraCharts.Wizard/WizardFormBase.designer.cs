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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Wizard;
namespace DevExpress.XtraCharts.Native {
	partial class WizardFormBase {
		private IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (DesignControl != null)
				DesignControl.Dispose();
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardFormBase));
			this.cbShowOnNextStart = new DevExpress.XtraEditors.CheckEdit();
			this.sbPreviousPage = new DevExpress.XtraEditors.SimpleButton();
			this.sbNextPage = new DevExpress.XtraEditors.SimpleButton();
			this.sbFinish = new DevExpress.XtraEditors.SimpleButton();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.pnlBase = new DevExpress.XtraEditors.PanelControl();
			this.grWizardPanel = new DevExpress.XtraCharts.Wizard.MainWizardControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.pnlTitle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblTitle = new DevExpress.XtraCharts.Wizard.AntialiasedLabel();
			this.peWizardImage = new System.Windows.Forms.PictureBox();
			this.peLogo = new System.Windows.Forms.PictureBox();
			this.hitTestTransparentPanelControl = new DevExpress.XtraCharts.Wizard.HitTestTransparentPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbShowOnNextStart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBase)).BeginInit();
			this.pnlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlTitle)).BeginInit();
			this.pnlTitle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.peWizardImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.peLogo)).BeginInit();
			this.hitTestTransparentPanelControl.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.cbShowOnNextStart, "cbShowOnNextStart");
			this.cbShowOnNextStart.Name = "cbShowOnNextStart";
			this.cbShowOnNextStart.Properties.AutoWidth = true;
			this.cbShowOnNextStart.Properties.Caption = resources.GetString("cbShowOnNextStart.Properties.Caption");
			this.cbShowOnNextStart.CheckedChanged += new System.EventHandler(this.ShowOnNextStart_CheckedChanged);
			resources.ApplyResources(this.sbPreviousPage, "sbPreviousPage");
			this.sbPreviousPage.Name = "sbPreviousPage";
			this.sbPreviousPage.Click += new System.EventHandler(this.sbPreviousPage_Click);
			resources.ApplyResources(this.sbNextPage, "sbNextPage");
			this.sbNextPage.Name = "sbNextPage";
			this.sbNextPage.Click += new System.EventHandler(this.sbNextPage_Click);
			resources.ApplyResources(this.sbFinish, "sbFinish");
			this.sbFinish.Name = "sbFinish";
			this.sbFinish.Click += new System.EventHandler(this.sbFinish_Click);
			resources.ApplyResources(this.sbCancel, "sbCancel");
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.sbCancel.Name = "sbCancel";
			this.sbCancel.Click += new System.EventHandler(this.sbCancel_Click);
			this.pnlBase.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBase.Controls.Add(this.grWizardPanel);
			this.pnlBase.Controls.Add(this.labelControl1);
			this.pnlBase.Controls.Add(this.pnlTitle);
			resources.ApplyResources(this.pnlBase, "pnlBase");
			this.pnlBase.Name = "pnlBase";
			this.grWizardPanel.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.grWizardPanel, "grWizardPanel");
			this.grWizardPanel.Name = "grWizardPanel";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			this.pnlTitle.BackColor = System.Drawing.Color.Transparent;
			this.pnlTitle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTitle.Controls.Add(this.lblTitle);
			this.pnlTitle.Controls.Add(this.peWizardImage);
			this.pnlTitle.Controls.Add(this.peLogo);
			resources.ApplyResources(this.pnlTitle, "pnlTitle");
			this.pnlTitle.Name = "pnlTitle";
			resources.ApplyResources(this.lblTitle, "lblTitle");
			this.lblTitle.Name = "lblTitle";
			resources.ApplyResources(this.peWizardImage, "peWizardImage");
			this.peWizardImage.Name = "peWizardImage";
			this.peWizardImage.TabStop = false;
			resources.ApplyResources(this.peLogo, "peLogo");
			this.peLogo.Name = "peLogo";
			this.peLogo.TabStop = false;
			this.hitTestTransparentPanelControl.BackColor = System.Drawing.Color.Transparent;
			this.hitTestTransparentPanelControl.Controls.Add(this.cbShowOnNextStart);
			this.hitTestTransparentPanelControl.Controls.Add(this.sbCancel);
			this.hitTestTransparentPanelControl.Controls.Add(this.sbPreviousPage);
			this.hitTestTransparentPanelControl.Controls.Add(this.sbFinish);
			this.hitTestTransparentPanelControl.Controls.Add(this.sbNextPage);
			resources.ApplyResources(this.hitTestTransparentPanelControl, "hitTestTransparentPanelControl");
			this.hitTestTransparentPanelControl.Name = "hitTestTransparentPanelControl";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlBase);
			this.Controls.Add(this.hitTestTransparentPanelControl);
			this.MinimizeBox = false;
			this.Name = "WizardFormBase";
			this.ShowInTaskbar = false;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WizardFormBase_FormClosed);
			this.ResizeEnd += new System.EventHandler(this.WizardFormBase_ResizeEnd);
			this.SizeChanged += new System.EventHandler(this.BaseWizardForm_SizeChanged);
			this.Move += new System.EventHandler(this.WizardFormBase_Move);
			((System.ComponentModel.ISupportInitialize)(this.cbShowOnNextStart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBase)).EndInit();
			this.pnlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlTitle)).EndInit();
			this.pnlTitle.ResumeLayout(false);
			this.pnlTitle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.peWizardImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.peLogo)).EndInit();
			this.hitTestTransparentPanelControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private ChartPanelControl pnlTitle;
		private SimpleButton sbPreviousPage;
		private SimpleButton sbNextPage;
		private SimpleButton sbFinish;
		private SimpleButton sbCancel;
		private MainWizardControl grWizardPanel;
		private CheckEdit cbShowOnNextStart;
		private PictureBox peLogo;
		private PictureBox peWizardImage;
		private AntialiasedLabel lblTitle;
		private PanelControl pnlBase;
		private LabelControl labelControl1;
		private HitTestTransparentPanelControl hitTestTransparentPanelControl;
	}
}
