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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class RotateControl : ChartUserControl {
		Diagram3D diagram;
		ChartDesignControl designControl;
		bool initialized = false;		
		public RotateControl() {
			InitializeComponent();
		}
		public void Initialize(Diagram3D diagram, Chart chart) {
			this.diagram = diagram;
			try {
				designControl = (ChartDesignControl)CommonUtils.FindChartContainer(chart);
				InitializeControls();
			}
			catch {
				this.Enabled = false;
			}
		}
		void InitializeControls() {
			rgRotationType.Properties.Items[0].Value = RotationType.UseMouseStandard;
			rgRotationType.Properties.Items[1].Value = RotationType.UseMouseAdvanced;
			rgRotationType.Properties.Items[2].Value = RotationType.UseAngles;
			rgRotationType.EditValue = diagram.RotationType;
			if (diagram.RuntimeRotation && GestureHelper.IsGestureSupported) {
				chUseMouse.Checked = diagram.RotationOptions.UseMouse;
				chUseTouchDevice.Checked = diagram.RotationOptions.UseTouchDevice;
			}
			else
				groupRotationOptions.Visible = false;
			initialized = true;
			UpdateRotationType();
		}
		void UpdateRotationType() {
			if (!initialized)
				return;
			designControl.UseHandCursor = diagram.RotationType != RotationType.UseAngles;
			diagram.RotationType = (RotationType)rgRotationType.EditValue;			
			UpdateAngles();
		}
		void UpdateAngles() {
			grAngles.Enabled = diagram.RotationType == RotationType.UseAngles;
			if (diagram.RotationType == RotationType.UseAngles) {
				tbXAxisAngle.EditValue = diagram.RotationAngleX;
				tbYAxisAngle.EditValue = diagram.RotationAngleY;
				tbZAxisAngle.EditValue = diagram.RotationAngleZ;
			}
		}
		void rgRotationType_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateRotationType();
		}
		void cbRotationOrder_SelectedIndexChanged(object sender, EventArgs e) {
			diagram.RotationOrder = (RotationOrder)cbRotationOrder.SelectedIndex;
		}
		void tbXAxisAngle_EditValueChanged(object sender, EventArgs e) {
			diagram.RotationAngleX = Convert.ToInt32(tbXAxisAngle.EditValue);
		}
		void tbYAxisAngle_EditValueChanged(object sender, EventArgs e) {
			diagram.RotationAngleY = Convert.ToInt32(tbYAxisAngle.EditValue);
		}
		void tbZAxisAngle_EditValueChanged(object sender, EventArgs e) {
			diagram.RotationAngleZ = Convert.ToInt32(tbZAxisAngle.EditValue);
		}
		void chUseMouse_CheckedChanged(object sender, EventArgs e) {
			diagram.RotationOptions.UseMouse = chUseMouse.Checked;
		}
		void chUseTouchDevice_CheckedChanged(object sender, EventArgs e) {
			diagram.RotationOptions.UseTouchDevice = chUseTouchDevice.Checked;
		}
	}
}
