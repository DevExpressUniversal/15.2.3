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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class XYDiagram3DControl : DiagramControlBase {
		XYDiagram3D XYDiagram { get { return (XYDiagram3D)base.Diagram; } }
		public XYDiagram3DControl() {
			InitializeComponent();
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			InitializeControls();
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = DiagramPageTab.XYDiagram3DGeneral;
			tbRotate.Tag = DiagramPageTab.XYDiagram3DRotate;
			tbScrollingZooming.Tag = DiagramPageTab.XYDiagram3DScrollingZooming;
			tbBackground.Tag = DiagramPageTab.XYDiagram3DBackground;
		}
		public override void DesignControl_MouseMove(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Middle)
				scrollingZooming3DControl.UpdateScrollingAndZooming();
		}
		public override void DesignControl_MouseWheel(object sender, MouseEventArgs e) {
			scrollingZooming3DControl.UpdateScrollingAndZooming();
		}
		void InitializeControls() {
			txtIndent.EditValue = XYDiagram.SeriesIndentFixed;
			txtDistanceFixed.EditValue = XYDiagram.SeriesDistanceFixed;
			txtDistance.EditValue = XYDiagram.SeriesDistance;
			txtPerspectiveAngle.EditValue = XYDiagram.PerspectiveAngle;
			txtPlaneDepth.EditValue = XYDiagram.PlaneDepthFixed;
			chUsePerspective.Checked = XYDiagram.PerspectiveEnabled;
			backgroundControl.Initialize(XYDiagram, OriginalChart);
			rotateControl.Initialize(XYDiagram, Chart);
			scrollingZooming3DControl.Initialize(XYDiagram);
		}
		private void txtIndent_EditValueChanged(object sender, EventArgs e) {
			XYDiagram.SeriesIndentFixed = Convert.ToInt32(txtIndent.EditValue);
		}
		private void txtDistanceFixed_EditValueChanged(object sender, EventArgs e) {
			XYDiagram.SeriesDistanceFixed = Convert.ToInt32(txtDistanceFixed.EditValue);
		}
		private void txtDistance_EditValueChanged(object sender, EventArgs e) {
			XYDiagram.SeriesDistance = Convert.ToDouble(txtDistance.EditValue);
		}
		private void txtPerspectiveAngle_EditValueChanged(object sender, EventArgs e) {
			XYDiagram.PerspectiveAngle = Convert.ToInt32(txtPerspectiveAngle.EditValue);
		}
		private void chUsePerspective_CheckedChanged(object sender, EventArgs e) {
			XYDiagram.PerspectiveEnabled = chUsePerspective.Checked;
			txtPerspectiveAngle.Enabled = chUsePerspective.Checked;
		}
		private void txtPlaneDepth_EditValueChanged(object sender, EventArgs e) {
			XYDiagram.PlaneDepthFixed = Convert.ToInt32(txtPlaneDepth.EditValue);
		}
	}
}
