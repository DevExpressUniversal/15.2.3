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
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class GridLinesControl : ChartUserControl {
		GridLines lines;
		AxisBase axis;
		public GridLinesControl() {
			InitializeComponent();
		}
		public void Initialize(GridLines lines, AxisBase axis) {
			this.lines = lines;
			this.axis = axis;
			chVisible.Checked = lines.Visible;
			ceColor.EditValue = lines.Color;
			ceMinorColor.EditValue = lines.MinorColor;
			lineStyleControl.Initialize(lines.LineStyle);
			minorLineStyleControl.Initialize(lines.MinorLineStyle);
			chMinorVisible.Checked = lines.MinorVisible;
			InitializeMinorCount();
			UpdateControls();
		}
		public void InitializeMinorCount() {
			if (lines != null) {
				if (axis != null)
					spnMinorCount.EditValue = axis.MinorCount;
				else
					ChartDebug.Fail("The axis can't be null.");
			}
		}
		void UpdateControls() {
			grLines.Enabled = lines.Visible;
			pnlMinorControls.Enabled = lines.MinorVisible;
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			lines.Visible = chVisible.Checked;
			UpdateControls();
		}
		void ceColor_EditValueChanged(object sender, EventArgs e) {
			lines.Color = (Color)ceColor.EditValue;
		}
		void ceMinorColor_EditValueChanged(object sender, EventArgs e) {
			lines.MinorColor = (Color)ceMinorColor.EditValue;
		}
		void chMinorVisible_CheckedChanged(object sender, EventArgs e) {
			lines.MinorVisible = chMinorVisible.Checked;
			UpdateControls();
		}
		void spnMinorCount_EditValueChanged(object sender, EventArgs e) {
			if (axis != null)
				axis.MinorCount = Convert.ToInt32(spnMinorCount.EditValue);
		}
	}
}
