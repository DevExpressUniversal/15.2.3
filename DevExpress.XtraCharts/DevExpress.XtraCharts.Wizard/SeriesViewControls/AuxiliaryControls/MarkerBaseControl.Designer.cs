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

namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	partial class MarkerBaseControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkerBaseControl));
			this.borderControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.MarkerBorderControl();
			this.polygonFillStyleControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.PolygonFillStyleSeriesViewControl();
			this.spnSize = new DevExpress.XtraEditors.SpinEdit();
			this.sepBorderColor = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblSize = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlStarPoints = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnStarPoints = new DevExpress.XtraEditors.SpinEdit();
			this.lblStarPoints = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepStarPoints = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.lblKind = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.sepFillStyle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbKind = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.pnlKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpStyle = new DevExpress.XtraEditors.GroupControl();
			this.sepKind = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.pnlSize = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.grpFillStyle = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.spnSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepBorderColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlStarPoints)).BeginInit();
			this.pnlStarPoints.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnStarPoints.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepStarPoints)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sepFillStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlKind)).BeginInit();
			this.pnlKind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpStyle)).BeginInit();
			this.grpStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepKind)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).BeginInit();
			this.pnlSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grpFillStyle)).BeginInit();
			this.grpFillStyle.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.borderControl, "borderControl");
			this.borderControl.Name = "borderControl";
			resources.ApplyResources(this.polygonFillStyleControl, "polygonFillStyleControl");
			this.polygonFillStyleControl.Name = "polygonFillStyleControl";
			resources.ApplyResources(this.spnSize, "spnSize");
			this.spnSize.Name = "spnSize";
			this.spnSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnSize.Properties.IsFloatValue = false;
			this.spnSize.Properties.Mask.EditMask = resources.GetString("spnSize.Properties.Mask.EditMask");
			this.spnSize.Properties.MaxValue = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.spnSize.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spnSize.Properties.ValidateOnEnterKey = true;
			this.spnSize.EditValueChanged += new System.EventHandler(this.spnSize_EditValueChanged);
			this.sepBorderColor.BackColor = System.Drawing.Color.Transparent;
			this.sepBorderColor.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepBorderColor, "sepBorderColor");
			this.sepBorderColor.Name = "sepBorderColor";
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.Name = "lblSize";
			resources.ApplyResources(this.pnlStarPoints, "pnlStarPoints");
			this.pnlStarPoints.BackColor = System.Drawing.Color.Transparent;
			this.pnlStarPoints.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlStarPoints.Controls.Add(this.spnStarPoints);
			this.pnlStarPoints.Controls.Add(this.lblStarPoints);
			this.pnlStarPoints.Name = "pnlStarPoints";
			resources.ApplyResources(this.spnStarPoints, "spnStarPoints");
			this.spnStarPoints.Name = "spnStarPoints";
			this.spnStarPoints.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnStarPoints.Properties.Mask.EditMask = resources.GetString("spnStarPoints.Properties.Mask.EditMask");
			this.spnStarPoints.Properties.MaxValue = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.spnStarPoints.Properties.MinValue = new decimal(new int[] {
			3,
			0,
			0,
			0});
			this.spnStarPoints.Properties.ValidateOnEnterKey = true;
			this.spnStarPoints.EditValueChanged += new System.EventHandler(this.spnStarPoints_EditValueChanged);
			resources.ApplyResources(this.lblStarPoints, "lblStarPoints");
			this.lblStarPoints.Name = "lblStarPoints";
			this.sepStarPoints.BackColor = System.Drawing.Color.Transparent;
			this.sepStarPoints.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepStarPoints, "sepStarPoints");
			this.sepStarPoints.Name = "sepStarPoints";
			resources.ApplyResources(this.lblKind, "lblKind");
			this.lblKind.Name = "lblKind";
			this.sepFillStyle.BackColor = System.Drawing.Color.Transparent;
			this.sepFillStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepFillStyle, "sepFillStyle");
			this.sepFillStyle.Name = "sepFillStyle";
			resources.ApplyResources(this.cbKind, "cbKind");
			this.cbKind.Name = "cbKind";
			this.cbKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbKind.Properties.Buttons"))))});
			this.cbKind.Properties.DropDownRows = 10;
			this.cbKind.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items"), ((object)(resources.GetObject("cbKind.Properties.Items1"))), ((int)(resources.GetObject("cbKind.Properties.Items2")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items3"), ((object)(resources.GetObject("cbKind.Properties.Items4"))), ((int)(resources.GetObject("cbKind.Properties.Items5")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items6"), ((object)(resources.GetObject("cbKind.Properties.Items7"))), ((int)(resources.GetObject("cbKind.Properties.Items8")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items9"), ((object)(resources.GetObject("cbKind.Properties.Items10"))), ((int)(resources.GetObject("cbKind.Properties.Items11")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items12"), ((object)(resources.GetObject("cbKind.Properties.Items13"))), ((int)(resources.GetObject("cbKind.Properties.Items14")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items15"), ((object)(resources.GetObject("cbKind.Properties.Items16"))), ((int)(resources.GetObject("cbKind.Properties.Items17")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items18"), ((object)(resources.GetObject("cbKind.Properties.Items19"))), ((int)(resources.GetObject("cbKind.Properties.Items20")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items21"), ((object)(resources.GetObject("cbKind.Properties.Items22"))), ((int)(resources.GetObject("cbKind.Properties.Items23")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items24"), ((object)(resources.GetObject("cbKind.Properties.Items25"))), ((int)(resources.GetObject("cbKind.Properties.Items26")))),
			new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("cbKind.Properties.Items27"), ((object)(resources.GetObject("cbKind.Properties.Items28"))), ((int)(resources.GetObject("cbKind.Properties.Items29"))))});
			this.cbKind.Properties.SmallImages = "";
			this.cbKind.EditValueChanged += new System.EventHandler(this.cbKind_EditValueChanged);
			resources.ApplyResources(this.pnlKind, "pnlKind");
			this.pnlKind.BackColor = System.Drawing.Color.Transparent;
			this.pnlKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlKind.Controls.Add(this.cbKind);
			this.pnlKind.Controls.Add(this.lblKind);
			this.pnlKind.Name = "pnlKind";
			resources.ApplyResources(this.grpStyle, "grpStyle");
			this.grpStyle.Controls.Add(this.borderControl);
			this.grpStyle.Controls.Add(this.sepBorderColor);
			this.grpStyle.Controls.Add(this.pnlStarPoints);
			this.grpStyle.Controls.Add(this.sepStarPoints);
			this.grpStyle.Controls.Add(this.pnlKind);
			this.grpStyle.Controls.Add(this.sepKind);
			this.grpStyle.Controls.Add(this.pnlSize);
			this.grpStyle.Name = "grpStyle";
			this.sepKind.BackColor = System.Drawing.Color.Transparent;
			this.sepKind.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepKind, "sepKind");
			this.sepKind.Name = "sepKind";
			resources.ApplyResources(this.pnlSize, "pnlSize");
			this.pnlSize.BackColor = System.Drawing.Color.Transparent;
			this.pnlSize.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSize.Controls.Add(this.spnSize);
			this.pnlSize.Controls.Add(this.lblSize);
			this.pnlSize.Name = "pnlSize";
			resources.ApplyResources(this.grpFillStyle, "grpFillStyle");
			this.grpFillStyle.Controls.Add(this.polygonFillStyleControl);
			this.grpFillStyle.Name = "grpFillStyle";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.grpFillStyle);
			this.Controls.Add(this.sepFillStyle);
			this.Controls.Add(this.grpStyle);
			this.Name = "MarkerBaseControl";
			((System.ComponentModel.ISupportInitialize)(this.spnSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepBorderColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlStarPoints)).EndInit();
			this.pnlStarPoints.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnStarPoints.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepStarPoints)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sepFillStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlKind)).EndInit();
			this.pnlKind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grpStyle)).EndInit();
			this.grpStyle.ResumeLayout(false);
			this.grpStyle.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepKind)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSize)).EndInit();
			this.pnlSize.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grpFillStyle)).EndInit();
			this.grpFillStyle.ResumeLayout(false);
			this.grpFillStyle.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private MarkerBorderControl borderControl;
		private PolygonFillStyleSeriesViewControl polygonFillStyleControl;
		private DevExpress.XtraEditors.SpinEdit spnSize;
		private ChartPanelControl sepBorderColor;
		private ChartLabelControl lblSize;
		private ChartPanelControl pnlStarPoints;
		private DevExpress.XtraEditors.SpinEdit spnStarPoints;
		private ChartLabelControl lblStarPoints;
		private ChartPanelControl sepStarPoints;
		private ChartLabelControl lblKind;
		private ChartPanelControl sepFillStyle;
		private DevExpress.XtraEditors.ImageComboBoxEdit cbKind;
		private ChartPanelControl pnlKind;
		private DevExpress.XtraEditors.GroupControl grpStyle;
		private ChartPanelControl sepKind;
		private ChartPanelControl pnlSize;
		protected DevExpress.XtraEditors.GroupControl grpFillStyle;
	}
}
