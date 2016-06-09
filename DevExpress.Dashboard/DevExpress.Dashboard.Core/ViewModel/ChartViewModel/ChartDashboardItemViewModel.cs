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
namespace DevExpress.DashboardCommon.ViewModel {
	public class ChartDashboardItemViewModel : ChartDashboardItemBaseViewModel {
		readonly IList<ChartPaneViewModel> panes;
		ChartLegendViewModel legend;
		bool rotated;
		public IList<ChartPaneViewModel> Panes { get { return panes; } }
		public ChartLegendViewModel Legend {
			get { return legend; }
			set { legend = value; }
		}
		public bool Rotated { 
			get { return rotated; } 
			set { rotated = value; }
		}
		public ChartAxisViewModel AxisX { get; set; }
		public ChartDashboardItemViewModel()
			: base() {
			legend = new ChartLegendViewModel();
			panes = new List<ChartPaneViewModel>();
			AxisX = new ChartAxisViewModel();
		}
		public ChartDashboardItemViewModel(ChartDashboardItem dashboardItem, IList<ChartPaneViewModel> panes) : base(dashboardItem) {
			this.panes = panes;
			legend = new ChartLegendViewModel(dashboardItem.Legend);
			rotated = dashboardItem.Rotated;
		}
		public ChartDashboardItemViewModel(ScatterChartDashboardItem dashboardItem, IList<ChartPaneViewModel> panes)
			: base(dashboardItem) {
			this.panes = panes;
		}
	}
	public class ScatterChartDashboardItemViewModel : ChartDashboardItemViewModel {
		public string AxisXDataMember { get; set; }
		public bool AxisXPercentValues { get; set; }
		public ScatterChartDashboardItemViewModel(ScatterChartDashboardItem dashboardItem, ChartPaneViewModel pane)
			: base(dashboardItem, new[] { pane }) {
		}
	}
}
