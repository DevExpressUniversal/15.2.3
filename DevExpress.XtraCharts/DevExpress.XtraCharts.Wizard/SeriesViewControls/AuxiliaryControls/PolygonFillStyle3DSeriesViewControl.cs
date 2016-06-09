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
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class PolygonFillStyle3DSeriesViewControl : ChartUserControl {
		PolygonFillStyle3D fillStyle;
		Color Color2 {
			get {
				if (fillStyle.FillMode == FillMode3D.Gradient)
					return ((PolygonGradientFillOptions)fillStyle.Options).Color2;
				else
					return Color.Empty;
			}
			set {
				if (fillStyle.FillMode == FillMode3D.Gradient)
					((FillOptionsColor2Base)fillStyle.Options).Color2 = value;
			}
		}
		PolygonGradientMode GradientMode {
			get {
				if (fillStyle.FillMode == FillMode3D.Gradient)
					return ((PolygonGradientFillOptions)fillStyle.Options).GradientMode;
				return default(PolygonGradientMode);
			}
			set {
				if (fillStyle.FillMode == FillMode3D.Gradient)
					((PolygonGradientFillOptions)fillStyle.Options).GradientMode = value;
			}
		}
		public PolygonFillStyle3DSeriesViewControl() {
			InitializeComponent();
		}		
		void cbFillMode_SelectedIndexChanged(object sender, EventArgs e) {
			fillStyle.FillMode = (FillMode3D)cbFillMode.SelectedIndex;
			UpdateControls();
		}
		void colorEdit_EditValueChanged(object sender, EventArgs e) {
			Color2 = (Color)colorEdit.EditValue;
			UpdateControls();
		}
		void cbGradientMode_SelectedIndexChanged(object sender, EventArgs e) {
			GradientMode = (PolygonGradientMode)cbGradientMode.SelectedIndex;
			UpdateControls();
		}
		void UpdateControls() {
			colorEdit.Enabled = this.fillStyle.FillMode != FillMode3D.Empty && this.fillStyle.FillMode != FillMode3D.Solid;
			cbGradientMode.Enabled = this.fillStyle.FillMode == FillMode3D.Gradient;
			cbFillMode.SelectedIndex = (int)this.fillStyle.FillMode;
			if(this.fillStyle.FillMode == FillMode3D.Gradient) {
				PolygonGradientFillOptions options = (PolygonGradientFillOptions)this.fillStyle.Options;
				colorEdit.EditValue = options.Color2;
				cbGradientMode.SelectedIndex = (int)options.GradientMode;
			}
		}
		public void Initialize(PolygonFillStyle3D fillStyle) {
			this.fillStyle = fillStyle;
			UpdateControls();
		}
	}
}
