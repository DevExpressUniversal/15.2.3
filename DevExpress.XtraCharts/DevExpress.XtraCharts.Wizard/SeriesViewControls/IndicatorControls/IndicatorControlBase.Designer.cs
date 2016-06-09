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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls 
{
	partial class IndicatorControlBase
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndicatorControlBase));
			this.panelName = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.txtName = new DevExpress.XtraEditors.TextEdit();
			this.labelName = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.chkVisible = new DevExpress.XtraEditors.CheckEdit();
			this.chkShowInLegend = new DevExpress.XtraEditors.CheckEdit();
			this.chkCheckableInLegend = new DevExpress.XtraEditors.CheckEdit();
			this.chkCheckedInLegend = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelName)).BeginInit();
			this.panelName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.panelName, "panelName");
			this.panelName.BackColor = System.Drawing.Color.Transparent;
			this.panelName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelName.Controls.Add(this.txtName);
			this.panelName.Controls.Add(this.labelName);
			this.panelName.Name = "panelName";
			resources.ApplyResources(this.txtName, "txtName");
			this.txtName.Name = "txtName";
			this.txtName.EditValueChanged += new System.EventHandler(this.txtName_EditValueChanged);
			resources.ApplyResources(this.labelName, "labelName");
			this.labelName.Name = "labelName";
			resources.ApplyResources(this.chkVisible, "chkVisible");
			this.chkVisible.Name = "chkVisible";
			this.chkVisible.Properties.Caption = resources.GetString("chkVisible.Properties.Caption");
			this.chkVisible.CheckedChanged += new System.EventHandler(this.chkVisible_CheckedChanged);
			resources.ApplyResources(this.chkShowInLegend, "chkShowInLegend");
			this.chkShowInLegend.Name = "chkShowInLegend";
			this.chkShowInLegend.Properties.Caption = resources.GetString("chkShowInLegend.Properties.Caption");
			this.chkShowInLegend.CheckedChanged += new System.EventHandler(this.chkShowInLegend_CheckedChanged);
			resources.ApplyResources(this.chkCheckableInLegend, "chkCheckableInLegend");
			this.chkCheckableInLegend.Name = "chkCheckableInLegend";
			this.chkCheckableInLegend.Properties.Caption = resources.GetString("chkCheckableInLegend.Properties.Caption");
			this.chkCheckableInLegend.CheckedChanged += new System.EventHandler(this.chkCheckableInLegend_CheckedChanged);
			resources.ApplyResources(this.chkCheckedInLegend, "chkCheckedInLegend");
			this.chkCheckedInLegend.Name = "chkCheckedInLegend";
			this.chkCheckedInLegend.Properties.Caption = resources.GetString("chkCheckedInLegend.Properties.Caption");
			this.chkCheckedInLegend.CheckedChanged += new System.EventHandler(this.chkCheckedInLegend_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.chkVisible);
			this.Controls.Add(this.chkShowInLegend);
			this.Controls.Add(this.chkCheckableInLegend);
			this.Controls.Add(this.chkCheckedInLegend);
			this.Controls.Add(this.panelName);
			this.Name = "IndicatorControlBase";
			((System.ComponentModel.ISupportInitialize)(this.panelName)).EndInit();
			this.panelName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkShowInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckableInLegend.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkCheckedInLegend.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected ChartPanelControl panelName;
		protected DevExpress.XtraEditors.TextEdit txtName;
		protected ChartLabelControl labelName;
		protected DevExpress.XtraEditors.CheckEdit chkVisible;
		protected DevExpress.XtraEditors.CheckEdit chkShowInLegend;
		protected DevExpress.XtraEditors.CheckEdit chkCheckableInLegend;
		protected DevExpress.XtraEditors.CheckEdit chkCheckedInLegend;
	}
}
