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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisLabelLayoutControl : ChartUserControl {
		AxisLabel label;
		AxisBase axis;
		Chart chart;
		public AxisLabelLayoutControl() {
			InitializeComponent();
		}
		public void Initialize(AxisLabel label, AxisBase axis, Chart chart) {
			this.label = label;
			this.axis = axis;
			this.chart = chart;
			ceStaggered.Checked = label.Staggered;
			txtAngle.EditValue = label.Angle;
			AxisLabel3D label3D = label as AxisLabel3D;
			if (label3D == null)
				panelPosition.Visible = false;
			else
				cbPosition.SelectedIndex = (int)label3D.Position;
			RadarAxisXLabel radarLabel = label as RadarAxisXLabel;
			if (radarLabel == null)
				panelDirection.Visible = false;
			else {
				ceStaggered.Visible = false;
				panelAngle.Visible = false;
				cbDirection.SelectedIndex = (int)radarLabel.TextDirection;
			}
		}
		public void UpdateControls() {
			if (axis != null && chart != null)
				ceStaggered.Enabled = !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, axis, false);
		}
		void ceStaggered_CheckedChanged(object sender, EventArgs e) {
			label.Staggered = ceStaggered.Checked;
		}
		void txtAngle_EditValueChanged(object sender, EventArgs e) {
			label.Angle = Convert.ToInt32(txtAngle.EditValue);
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			AxisLabel3D label3D = label as AxisLabel3D;
			if (label3D != null)
				label3D.Position = (AxisLabel3DPosition)cbPosition.SelectedIndex;
		}
		void cbDirection_SelectedIndexChanged(object sender, EventArgs e){
			RadarAxisXLabel radarLabel = label as RadarAxisXLabel;
			if (radarLabel != null)
				radarLabel.TextDirection = (RadarAxisXLabelTextDirection)cbDirection.SelectedIndex;
		}
	}
}
