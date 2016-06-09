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
namespace DevExpress.XtraCharts.Designer
{
	public partial class ChartDataControl 
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
		#region Component Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartDataControl));
			this.cbChooseDataSource = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.pnlChooseDS = new System.Windows.Forms.Panel();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.lblArrow = new DevExpress.XtraEditors.LabelControl();
			this.dataMemberPicker = new DevExpress.XtraCharts.Designer.DraggableDataMemberPicker();
			this.chartPanelControl1 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl3 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlDataMembers = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.chartPanelControl4 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbChooseDataSource.Properties)).BeginInit();
			this.pnlChooseDS.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataMembers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.cbChooseDataSource, "cbChooseDataSource");
			this.cbChooseDataSource.Name = "cbChooseDataSource";
			this.cbChooseDataSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbChooseDataSource.Properties.Buttons"))))});
			this.cbChooseDataSource.SelectedIndexChanged += new System.EventHandler(this.OnChooseDataSourceSelectedIndexChanged);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			this.pnlChooseDS.Controls.Add(this.cbChooseDataSource);
			this.pnlChooseDS.Controls.Add(this.labelControl1);
			resources.ApplyResources(this.pnlChooseDS, "pnlChooseDS");
			this.pnlChooseDS.Name = "pnlChooseDS";
			this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			this.lblArrow.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("lblArrow.Appearance.Image")));
			this.lblArrow.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.lblArrow, "lblArrow");
			this.lblArrow.Name = "lblArrow";
			resources.ApplyResources(this.dataMemberPicker, "dataMemberPicker");
			this.dataMemberPicker.Name = "dataMemberPicker";
			this.chartPanelControl1.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl1, "chartPanelControl1");
			this.chartPanelControl1.Name = "chartPanelControl1";
			this.chartPanelControl3.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl3, "chartPanelControl3");
			this.chartPanelControl3.Name = "chartPanelControl3";
			this.chartPanelControl2.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl2, "chartPanelControl2");
			this.chartPanelControl2.Name = "chartPanelControl2";
			resources.ApplyResources(this.pnlDataMembers, "pnlDataMembers");
			this.pnlDataMembers.BackColor = System.Drawing.Color.Transparent;
			this.pnlDataMembers.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlDataMembers.Name = "pnlDataMembers";
			this.chartPanelControl4.BackColor = System.Drawing.Color.Transparent;
			this.chartPanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.chartPanelControl4, "chartPanelControl4");
			this.chartPanelControl4.Name = "chartPanelControl4";
			this.Controls.Add(this.dataMemberPicker);
			this.Controls.Add(this.chartPanelControl4);
			this.Controls.Add(this.labelControl3);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.chartPanelControl2);
			this.Controls.Add(this.lblArrow);
			this.Controls.Add(this.chartPanelControl1);
			this.Controls.Add(this.pnlChooseDS);
			this.Controls.Add(this.chartPanelControl3);
			this.Controls.Add(this.pnlDataMembers);
			this.Name = "ChartDataControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.cbChooseDataSource.Properties)).EndInit();
			this.pnlChooseDS.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlDataMembers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chartPanelControl4)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion        
		private XtraEditors.ComboBoxEdit cbChooseDataSource;
		private XtraEditors.LabelControl labelControl1;
		private System.Windows.Forms.Panel pnlChooseDS;
		private DevExpress.XtraCharts.Designer.DraggableDataMemberPicker dataMemberPicker;
		private Wizard.ChartPanelControl chartPanelControl1;
		private XtraEditors.LabelControl labelControl2;
		private Wizard.ChartPanelControl chartPanelControl3;
		private Wizard.ChartPanelControl chartPanelControl2;
		private Wizard.ChartPanelControl pnlDataMembers;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.LabelControl lblArrow;
		private Wizard.ChartPanelControl chartPanelControl4;
	}
}
