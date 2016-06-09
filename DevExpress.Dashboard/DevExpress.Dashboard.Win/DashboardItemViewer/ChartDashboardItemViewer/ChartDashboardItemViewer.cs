#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraCharts;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public abstract class AxesChartDashboardItemViewer : ChartDashboardItemViewerBase {
		protected override void InitializeControl() {
			base.InitializeControl();
			ChartControl.CrosshairOptions.ShowGroupHeaders = false;
		}
		protected override DashboardChartControlViewerBase CreateControlViewer() {
			return new DashboardChartControlViewer(ChartControl);
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(ChartControl);
			XYDiagram diagram = ChartControl.Diagram as XYDiagram;
			if(diagram != null && diagram.EnableAxisXScrolling) {
				DashboardChartControlViewer viewControl = (DashboardChartControlViewer)ControlViewer;
				state.SpecificState = new Dictionary<string, object>();
				state.SpecificState.Add("ChartAxisXMinValue", viewControl.GetAxisPointUniqueValue(diagram.AxisX.VisualRange.MinValue));
				state.SpecificState.Add("ChartAxisXMaxValue", viewControl.GetAxisPointUniqueValue(diagram.AxisX.VisualRange.MaxValue));
			}
		}
		protected override DashboardChartControl CreateChartControl() {
			return new DashboardChartControl();
		}
	}
	[DXToolboxItem(false)]
	public class ChartDashboardItemViewer : AxesChartDashboardItemViewer {
		protected override DashboardChartControlViewerBase CreateControlViewer() {
			return new DashboardChartControlViewer(ChartControl);
		}
	}
	[DXToolboxItem(false)]
	public class ScatterChartDashboardItemViewer : AxesChartDashboardItemViewer {
		protected override DashboardChartControlViewerBase CreateControlViewer() {
			return new DashboardScatterChartControlViewer(ChartControl);
		}
	}
}
