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
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class XYDiagramSeriesViewLinkedElementsControl : DevExpress.XtraCharts.Wizard.ChartUserControl {
		XYDiagram2DSeriesViewBaseController controller;
		XYDiagram2D diagram;
		IXYDiagram2D Diagram { get { return diagram; } }
		public XYDiagramSeriesViewLinkedElementsControl() {
			InitializeComponent();
		}
		XYDiagram2D GetDiagram(XYDiagram2DSeriesViewBase view, Chart chart) {
			XYDiagram2D diagram = chart.Diagram as XYDiagram2D;
			if (diagram != null && diagram.GetType().Equals(view.DiagramType))
				return diagram;
			return null;
		}
		void FillControls() {
			if(diagram == null)
				return;
			foreach (Axis2D axis in this.diagram.GetAllAxesX())
				cbAxisX.Properties.Items.Add(axis.Name);
			foreach (Axis2D axis in this.diagram.GetAllAxesY())
				cbAxisY.Properties.Items.Add(axis.Name);
			cbPane.Properties.Items.Add(diagram.DefaultPane.Name);
			foreach(XYDiagramPane pane in diagram.Panes)
				cbPane.Properties.Items.Add(pane.Name);
		}
		void UpdateControls() {
			this.cbAxisX.Enabled = diagram != null;
			this.cbAxisY.Enabled = diagram != null;
			this.cbPane.Enabled = diagram != null;
			IXYSeriesView2D view2D = controller.View as IXYSeriesView2D;
			if(diagram != null && view2D != null) {
				cbAxisX.SelectedIndex = view2D.AxisX == Diagram.AxisX ? 0 : Diagram.SecondaryAxesX.IndexOf(view2D.AxisX) + 1;
				cbAxisY.SelectedIndex = view2D.AxisY == Diagram.AxisY ? 0 : Diagram.SecondaryAxesY.IndexOf(view2D.AxisY) + 1;
				cbPane.SelectedIndex = view2D.Pane == diagram.DefaultPane ? 0 : diagram.Panes.IndexOf(view2D.Pane) + 1;
			}
		}
		public void Initialize(XYDiagram2DSeriesViewBaseController controller, Chart chart) {
			this.controller = controller;
			this.diagram = GetDiagram(controller.View, chart);
			FillControls();
			UpdateControls();
		}
		private void cbAxisX_SelectedIndexChanged(object sender, EventArgs e) {
			Axis2D axisX = cbAxisX.SelectedIndex == 0 ? Diagram.AxisX : (Axis2D)Diagram.SecondaryAxesX[cbAxisX.SelectedIndex - 1];
			controller.SetAxisX(axisX);
		}
		private void cbAxisY_SelectedIndexChanged(object sender, EventArgs e) {
			Axis2D axisY = cbAxisY.SelectedIndex == 0 ? Diagram.AxisY : (Axis2D)Diagram.SecondaryAxesY[cbAxisY.SelectedIndex - 1];
			controller.SetAxisY(axisY);
		}
		private void cbPane_SelectedIndexChanged(object sender, EventArgs e) {
			XYDiagramPaneBase pane = cbPane.SelectedIndex == 0 ? (XYDiagramPaneBase)diagram.DefaultPane : (XYDiagramPaneBase)diagram.Panes[cbPane.SelectedIndex - 1];
			controller.SetPane(pane);
		}
	}
}
