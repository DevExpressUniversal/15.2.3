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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class FillStylesControl : ChartUserControl {
		public FillStylesControl() {
			InitializeComponent();
		}
		Control GetActualControl(FillStyleBase fillStyle, bool supportCenterGradientMode) {
			if (fillStyle is RectangleFillStyle) {
				rectangleFillStyleControl.Initialize((RectangleFillStyle)fillStyle);
				return rectangleFillStyleControl;
			}
			if (fillStyle is RectangleFillStyle3D) {
				rectangleFillStyle3DControl.Initialize((RectangleFillStyle3D)fillStyle);
				return rectangleFillStyle3DControl;
			}
			if (fillStyle is PolygonFillStyle) {
				polygonFillStyleControl.Initialize((PolygonFillStyle)fillStyle, supportCenterGradientMode);
				return polygonFillStyleControl;
			}
			if (fillStyle is PolygonFillStyle3D) {
				polygonFillStyle3DControl.Initialize((PolygonFillStyle3D)fillStyle);
				return polygonFillStyle3DControl;
			}
			return null;
		}
		public void Initialize(FillStyleBase fillStyle) {
			Initialize(fillStyle, true);
		}
		public void Initialize(FillStyleBase fillStyle, bool supportCenterGradientMode) {
			Control actualControl = GetActualControl(fillStyle, supportCenterGradientMode);
			if (actualControl == null)
				return;
			Hide();
			try {
				Controls.Clear();
				Controls.Add(actualControl);
			}
			finally {
				Show();
			}
		}
	}
}
