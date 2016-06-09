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
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisAppearanceControl : ChartUserControl {
		Axis2D axis;
		public AxisAppearanceControl() {
			InitializeComponent();
		}
		public void Initialize(Axis2D axis) {
			this.axis = axis;
			this.ceColor.EditValue = axis.Color;
			this.chEnableInterlace.Checked = axis.Interlaced;
			this.ceInterlaceColor.EditValue = axis.InterlacedColor;
			this.interlaceFillStyle.Initialize(axis.InterlacedFillStyle);
			this.txtThickness.EditValue = axis.Thickness;
			UpdateControls();
		}
		public void UpdateControls() {
			this.pnlControls.Enabled = this.axis.Interlaced;
			this.interlaceFillStyle.Enabled = this.axis.Interlaced;
		}
		private void ceColor_EditValueChanged(object sender, EventArgs e) {
			this.axis.Color = (Color)this.ceColor.EditValue;
		}
		private void ceInterlaceColor_EditValueChanged(object sender, EventArgs e) {
			this.axis.InterlacedColor = (Color)this.ceInterlaceColor.EditValue;
		}
		private void chEnableInterlace_CheckedChanged(object sender, EventArgs e) {
			this.axis.Interlaced = this.chEnableInterlace.Checked;
			UpdateControls();
		}
		private void txtThickness_EditValueChanged(object sender, EventArgs e) {
			this.axis.Thickness = Convert.ToInt32(this.txtThickness.EditValue);
		}
	}
}
