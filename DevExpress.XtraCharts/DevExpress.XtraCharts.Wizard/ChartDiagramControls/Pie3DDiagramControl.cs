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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class Pie3DDiagramControl : DiagramControlBase {
		SimpleDiagram3D SimpleDiagram { get { return (SimpleDiagram3D)base.Diagram; } }
		public Pie3DDiagramControl() {
			InitializeComponent();
		}
		void txtDimension_EditValueChanged(object sender, EventArgs e) {
			SimpleDiagram.Dimension = Convert.ToInt32(txtDimension.EditValue);
		}
		void cbDirection_SelectedIndexChanged(object sender, EventArgs e) {
			SimpleDiagram.LayoutDirection = (LayoutDirection)cbDirection.SelectedIndex;
		}
		void txtPerspectiveAngle_EditValueChanged(object sender, EventArgs e) {
			SimpleDiagram.PerspectiveAngle = Convert.ToInt32(txtPerspectiveAngle.EditValue);
		}
		void chUsePerspective_CheckedChanged(object sender, EventArgs e) {
			SimpleDiagram.PerspectiveEnabled = chUsePerspective.Checked;
			txtPerspectiveAngle.Enabled = chUsePerspective.Checked;
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			txtPerspectiveAngle.EditValue = SimpleDiagram.PerspectiveAngle;
			chUsePerspective.Checked = SimpleDiagram.PerspectiveEnabled;
			txtDimension.EditValue = SimpleDiagram.Dimension;
			cbDirection.SelectedIndex = (int)SimpleDiagram.LayoutDirection;
			if (Chart.AutoLayout) {
				txtDimension.Enabled = false;
				cbDirection.Enabled = false;
			}
			scrollingZooming3DControl.Initialize(SimpleDiagram);
			rotateControl.Initialize(SimpleDiagram, Chart);
			grLayout.Enabled = Chart == null ||
				!PivotGridDataSourceUtils.HasDataSource(Chart.DataContainer.PivotGridDataSourceOptions) || !Chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled;
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = DiagramPageTab.SimpleDiagram3DGeneral;
			tbRotation.Tag = DiagramPageTab.SimpleDiagram3DRotate;
			tbScrollingZooming.Tag = DiagramPageTab.SimpleDiagram3DScrollingZooming;
		}
		public override void DesignControl_MouseMove(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Middle)
				scrollingZooming3DControl.UpdateScrollingAndZooming();
		}
		public override void DesignControl_MouseWheel(object sender, MouseEventArgs e) {
			scrollingZooming3DControl.UpdateScrollingAndZooming();
		}
	}
}
