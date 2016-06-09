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

using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisGeneralTabsControl : ChartUserControl {
		Diagram diagram;
		public AxisGeneralTabsControl() {
			InitializeComponent();
		}
		public bool Initialize(AxisBase axis, Chart chart) {
			return Initialize(axis, chart, null);
		}
		public bool Initialize(AxisBase axis, Chart chart, Action0 method) {
			this.diagram = chart.Diagram;
			bool generalPageVisible = generalControl.Initialize(this, axis, chart, method);
			pageGeneral.PageVisible = generalPageVisible;
			return UpdateRangePages(axis) || generalPageVisible;
		}
		public bool UpdateRangePages(AxisBase axis) {
			bool rangeVisual = visualRangeControl.Initialize(axis, axis.VisualRange, this);
			pageVisualRange.PageVisible = rangeVisual && (diagram is XYDiagram);
			pageWholeRange.PageVisible = rangeVisual;
			if (pageWholeRange.PageVisible)
				wholeRangeControl.Initialize(axis, axis.WholeRange, this);
			return rangeVisual;
		}
		public void UpdateRanges() {
			if (pageVisualRange.PageVisible)
				visualRangeControl.UpdateControls();
			if (pageWholeRange.PageVisible)
				wholeRangeControl.UpdateControls();
		}
	}
}
