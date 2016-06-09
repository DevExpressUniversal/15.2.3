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

namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	partial class PointSeriesLabelOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PointSeriesLabelOptionsControl));
			this.pnlAngle = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.spnAngle = new DevExpress.XtraEditors.SpinEdit();
			this.lblAngle = new DevExpress.XtraCharts.Wizard.ChartLabelControl();
			this.pnlBubbleView = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.sepBubbleView = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.bubbleSeriesLabelOptionsControl = new DevExpress.XtraCharts.Wizard.SeriesLabelsControls.BubbleSeriesLabelOptionsControl();
			this.pnlPosition = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblPosition = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlAngle)).BeginInit();
			this.pnlAngle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnAngle.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBubbleView)).BeginInit();
			this.pnlBubbleView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sepBubbleView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).BeginInit();
			this.pnlPosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlAngle, "pnlAngle");
			this.pnlAngle.BackColor = System.Drawing.Color.Transparent;
			this.pnlAngle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAngle.Controls.Add(this.spnAngle);
			this.pnlAngle.Controls.Add(this.lblAngle);
			this.pnlAngle.Name = "pnlAngle";
			resources.ApplyResources(this.spnAngle, "spnAngle");
			this.spnAngle.Name = "spnAngle";
			this.spnAngle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnAngle.Properties.IsFloatValue = false;
			this.spnAngle.Properties.Mask.EditMask = resources.GetString("spnAngle.Properties.Mask.EditMask");
			this.spnAngle.Properties.MaxValue = new decimal(new int[] {
			360,
			0,
			0,
			0});
			this.spnAngle.Properties.MinValue = new decimal(new int[] {
			360,
			0,
			0,
			-2147483648});
			this.spnAngle.EditValueChanged += new System.EventHandler(this.spnAngle_EditValueChanged);
			resources.ApplyResources(this.lblAngle, "lblAngle");
			this.lblAngle.Name = "lblAngle";
			resources.ApplyResources(this.pnlBubbleView, "pnlBubbleView");
			this.pnlBubbleView.BackColor = System.Drawing.Color.Transparent;
			this.pnlBubbleView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBubbleView.Controls.Add(this.sepBubbleView);
			this.pnlBubbleView.Name = "pnlBubbleView";
			this.sepBubbleView.BackColor = System.Drawing.Color.Transparent;
			this.sepBubbleView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.sepBubbleView, "sepBubbleView");
			this.sepBubbleView.Name = "sepBubbleView";
			resources.ApplyResources(this.bubbleSeriesLabelOptionsControl, "bubbleSeriesLabelOptionsControl");
			this.bubbleSeriesLabelOptionsControl.Name = "bubbleSeriesLabelOptionsControl";
			resources.ApplyResources(this.pnlPosition, "pnlPosition");
			this.pnlPosition.BackColor = System.Drawing.Color.Transparent;
			this.pnlPosition.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPosition.Controls.Add(this.cbPosition);
			this.pnlPosition.Controls.Add(this.lblPosition);
			this.pnlPosition.Name = "pnlPosition";
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.lblPosition, "lblPosition");
			this.lblPosition.Name = "lblPosition";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.bubbleSeriesLabelOptionsControl);
			this.Controls.Add(this.pnlPosition);
			this.Controls.Add(this.pnlBubbleView);
			this.Controls.Add(this.pnlAngle);
			this.Name = "PointSeriesLabelOptionsControl";
			((System.ComponentModel.ISupportInitialize)(this.pnlAngle)).EndInit();
			this.pnlAngle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.spnAngle.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBubbleView)).EndInit();
			this.pnlBubbleView.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sepBubbleView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPosition)).EndInit();
			this.pnlPosition.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private ChartPanelControl pnlAngle;
		private DevExpress.XtraEditors.SpinEdit spnAngle;
		private ChartLabelControl lblAngle;
		private ChartPanelControl pnlBubbleView;
		private ChartPanelControl sepBubbleView;
		private BubbleSeriesLabelOptionsControl bubbleSeriesLabelOptionsControl;
		private ChartPanelControl pnlPosition;
		private XtraEditors.ComboBoxEdit cbPosition;
		private XtraEditors.LabelControl lblPosition;
	}
}
