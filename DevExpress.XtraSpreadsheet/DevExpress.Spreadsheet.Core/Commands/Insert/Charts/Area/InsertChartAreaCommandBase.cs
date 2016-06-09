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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertChartAreaCommandBase (abstract class)
	public abstract class InsertChartAreaCommandBase : InsertChartCommandBase {
		protected InsertChartAreaCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract ChartGrouping Grouping { get; }
		protected abstract bool Is3D { get; }
		protected override ChartLayoutModifier Preset { get { return ChartAreaPresets.Instance.DefaultModifier; } }
		protected override AxisCrossBetween ValueAxisCrossBetween { get { return AxisCrossBetween.Midpoint; } }
		protected override ChartViewType ViewType { get { return ChartViewType.Area; } }
		protected override void CreateAxes(Chart chart) {
			if (Is3D)
				CreateThreePrimaryAxes(chart);
			else
				CreateTwoPrimaryAxes(chart);
			ApplyPercentFormatOnValueAxis(chart, Grouping == ChartGrouping.PercentStacked);
		}
		protected internal override SeriesBase CreateSeriesCore(IChartView view) {
			return new AreaSeries(view);
		}
		protected internal override IChartView CreateChartView(IChart parent) {
			if (Is3D) {
				Area3DChartView view = new Area3DChartView(parent);
				view.Grouping = Grouping;
				return view;
			}
			else {
				AreaChartView view = new AreaChartView(parent);
				view.Grouping = Grouping;
				return view;
			}
		}
		protected internal override bool IsCompatibleView(IChartView chartView) {
			if (Is3D) {
				Area3DChartView view = chartView as Area3DChartView;
				return view != null && view.Grouping == this.Grouping;
			}
			else {
				AreaChartView view = chartView as AreaChartView;
				return view != null && view.Grouping == this.Grouping;
			}
		}
	}
	#endregion
}
