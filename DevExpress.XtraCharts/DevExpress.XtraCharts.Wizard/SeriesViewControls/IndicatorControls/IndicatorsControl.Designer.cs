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
	partial class IndicatorsControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndicatorsControl));
			this.indicatorsSelectionControl = new DevExpress.XtraCharts.Wizard.SeriesViewControls.IndicatorsSelectionControl();
			this.panelIndicator = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.panelType = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.labelIndicatorType = new DevExpress.XtraEditors.LabelControl();
			this.labelIndicatorTypeLabel = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelIndicator)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelType)).BeginInit();
			this.panelType.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.indicatorsSelectionControl, "indicatorsSelectionControl");
			this.indicatorsSelectionControl.Name = "indicatorsSelectionControl";
			this.indicatorsSelectionControl.SelectedElementChanged += new DevExpress.XtraCharts.Wizard.SelectedElementChangedEventHandler(this.indicatorsSelectionControl_SelectedElementChanged);
			resources.ApplyResources(this.panelIndicator, "panelIndicator");
			this.panelIndicator.BackColor = System.Drawing.Color.Transparent;
			this.panelIndicator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelIndicator.Name = "panelIndicator";
			resources.ApplyResources(this.panelType, "panelType");
			this.panelType.BackColor = System.Drawing.Color.Transparent;
			this.panelType.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelType.Controls.Add(this.labelIndicatorType);
			this.panelType.Controls.Add(this.labelIndicatorTypeLabel);
			this.panelType.Name = "panelType";
			resources.ApplyResources(this.labelIndicatorType, "labelIndicatorType");
			this.labelIndicatorType.Name = "labelIndicatorType";
			resources.ApplyResources(this.labelIndicatorTypeLabel, "labelIndicatorTypeLabel");
			this.labelIndicatorTypeLabel.Name = "labelIndicatorTypeLabel";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelIndicator);
			this.Controls.Add(this.panelType);
			this.Controls.Add(this.indicatorsSelectionControl);
			this.Name = "IndicatorsControl";
			((System.ComponentModel.ISupportInitialize)(this.panelIndicator)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelType)).EndInit();
			this.panelType.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
	   }
		#endregion
		private IndicatorsSelectionControl indicatorsSelectionControl;
		private ChartPanelControl panelIndicator;
		private ChartPanelControl panelType;
		private DevExpress.XtraEditors.LabelControl labelIndicatorTypeLabel;
		private DevExpress.XtraEditors.LabelControl labelIndicatorType;
	}
}
