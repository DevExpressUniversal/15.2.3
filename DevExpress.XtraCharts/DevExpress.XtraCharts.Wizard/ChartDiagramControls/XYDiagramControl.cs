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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class XYDiagramControl : DiagramControlBase {
		XYDiagram2DController controller;
		XYDiagram2D XYDiagram { get { return (XYDiagram2D)base.Diagram; } }
		bool ShouldHideScrollingZommingTab { get { return (OriginalChart != null && OriginalChart.Container != null && OriginalChart.Container.ControlType == ChartContainerType.WebControl); } }
		public XYDiagramControl() {
			InitializeComponent();
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			InitializeControls();
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = DiagramPageTab.XYDiagramGeneral;
			tbElements.Tag = DiagramPageTab.XYDiagramElements;
			tbScrollingZooming.Tag = DiagramPageTab.XYDiagramScrollingZooming;
		}
		void InitializeControls() {
			controller = XYDiagram2DController.CreateInstance(XYDiagram);
			chRotated.Visible = controller.RotatedPropertySupported;
			sepRotated.Visible = controller.RotatedPropertySupported;
			chRotated.Checked = controller.Rotated;
			txtPaneDistance.Value = XYDiagram.PaneDistance;
			cbPaneLayoutDirection.SelectedIndex = (int)XYDiagram.PaneLayoutDirection;
			secondaryAxesX.Initialize(((IXYDiagram2D)XYDiagram).SecondaryAxesX);
			secondaryAxesY.Initialize(((IXYDiagram2D)XYDiagram).SecondaryAxesY);
			panesRedact.Initialize(XYDiagram.Panes);
			marginsControl.Initialize(XYDiagram.Margins);
			if(ShouldHideScrollingZommingTab)
				tbcPagesControl.TabPages.Remove(tbScrollingZooming);
			else
				scrollingZooming2DControl.Initialize(XYDiagram);
		}
		void chReverse_CheckedChanged(object sender, EventArgs e) {
			controller.Rotated = chRotated.Checked;
		}
		void txtPaneDistance_ValueChanged(object sender, EventArgs e) {
			XYDiagram.PaneDistance = (int)txtPaneDistance.Value;
		}
		void cbPaneLayoutDirection_SelectedIndexChanged(object sender, EventArgs e) {
			XYDiagram.PaneLayoutDirection = (PaneLayoutDirection)cbPaneLayoutDirection.SelectedIndex;
		}
	}
	public class XYDiagram2DController {
		public static XYDiagram2DController CreateInstance(XYDiagram2D diagram) {
			if(diagram is GanttDiagram)
				return new XYDiagram2DController(diagram);
			XYDiagram xyDiagram = diagram as XYDiagram;
			if(xyDiagram != null)
				return new XYDiagramController(xyDiagram);
			return new XYDiagram2DController(diagram);
		}
		XYDiagram2D diagram;
		public XYDiagram2D Diagram { get { return diagram; } }
		public virtual bool RotatedPropertySupported { get { return false; } }
		public virtual bool Rotated { get { return ((IXYDiagram)diagram).Rotated; } set { } }
		public XYDiagram2DController(XYDiagram2D diagram) {
			this.diagram = diagram;
		}
	}
	public class XYDiagramController : XYDiagram2DController {
		XYDiagram XYDiagram { get { return (XYDiagram)base.Diagram; } }
		public override bool RotatedPropertySupported { get { return true; } }
		public override bool Rotated { get { return XYDiagram.Rotated; } set { XYDiagram.Rotated = value; } }
		public XYDiagramController(XYDiagram diagram) : base(diagram) { }
	}
}
