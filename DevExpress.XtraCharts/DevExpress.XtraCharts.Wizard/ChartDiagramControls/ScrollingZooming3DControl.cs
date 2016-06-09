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
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class ScrollingZooming3DControl : ChartUserControl {
		Diagram3D diagram;
		public ScrollingZooming3DControl() {
			InitializeComponent();
		}
		void txtHScroll_EditValueChanged(object sender, EventArgs e) {
			diagram.HorizontalScrollPercent = Convert.ToDouble(txtHScroll.EditValue);
		}
		private void txtVScroll_EditValueChanged(object sender, EventArgs e) {
			diagram.VerticalScrollPercent = Convert.ToDouble(txtVScroll.EditValue);
		}
		void txtZoom_EditValueChanged(object sender, EventArgs e) {
			diagram.ZoomPercent = Convert.ToInt32(txtZoom.EditValue);
		}
		public void Initialize(Diagram3D diagram) {
			this.diagram = diagram;
			txtHScroll.EditValue = diagram.HorizontalScrollPercent;
			txtVScroll.EditValue = diagram.VerticalScrollPercent;
			txtZoom.EditValue = diagram.ZoomPercent;
			if (diagram.RuntimeScrolling)
				scrollingOptions.Initialize(diagram.ScrollingOptions);
			else {
				gcScrollingOptions.Visible = false;
				pnlSeparator1.Visible = false;
			}
			if (diagram.RuntimeZooming)
				zoomingOptions.Initialize(diagram.ZoomingOptions);
			else {
				gcZoomingOptions.Visible = false;
				pnlSeparator2.Visible = false;
			}
		}
		public void UpdateScrollingAndZooming() {
			txtHScroll.EditValue = diagram.HorizontalScrollPercent;
			txtVScroll.EditValue = diagram.VerticalScrollPercent;
			txtZoom.EditValue = diagram.ZoomPercent;
		}
	}
}
