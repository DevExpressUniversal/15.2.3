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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class ScrollingZooming2DControl : ChartUserControl {
		XYDiagram2D diagram;
		public ScrollingZooming2DControl() {
			InitializeComponent();
		}
		void chEnableAxixXScrolling_CheckedChanged(object sender, EventArgs e) {
			diagram.EnableAxisXScrolling = chEnableAxixXScrolling.Checked;
			UpdateControls();
		}
		void chEnableAxixXZooming_CheckedChanged(object sender, EventArgs e) {
			diagram.EnableAxisXZooming = chEnableAxixXZooming.Checked;
			UpdateControls();
		}
		void chEnableAxixYScrolling_CheckedChanged(object sender, EventArgs e) {
			diagram.EnableAxisYScrolling = chEnableAxixYScrolling.Checked;
			UpdateControls();
		}
		void chEnableAxixYZooming_CheckedChanged(object sender, EventArgs e) {
			diagram.EnableAxisYZooming = chEnableAxixYZooming.Checked;
			UpdateControls();
		}
		public void UpdateControls() {
			gcScrollingOptions.Enabled = diagram.IsScrollingEnabled;
			gcZoomingOptions.Enabled = diagram.IsZoomingEnabled;
		}
		public void Initialize(XYDiagram2D diagram) {
			this.diagram = diagram;
			chEnableAxixXScrolling.Checked = diagram.EnableAxisXScrolling;
			chEnableAxixXZooming.Checked = diagram.EnableAxisXZooming;
			chEnableAxixYScrolling.Checked = diagram.EnableAxisYScrolling;
			chEnableAxixYZooming.Checked = diagram.EnableAxisYZooming;
			scrollingOptions.Initialize(diagram.ScrollingOptions);
			zoomingOptions.Initialize(diagram.ZoomingOptions);
			UpdateControls();
		}
	}
}
