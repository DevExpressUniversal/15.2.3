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

namespace DevExpress.XtraSpreadsheet.Model {
	public class LineSeries : SeriesWithMarkerAndSmooth {
		public LineSeries(IChartView view)
			: base(view) {
		}
		#region ISeries
		public override ChartSeriesType SeriesType { get { return ChartSeriesType.Line; } }
		public override ChartType ChartType { 
			get {
				ChartType viewChartType = View.ChartType;
				if (View.ViewType != ChartViewType.Line)
					return viewChartType;
				LineChartView lineView = View as LineChartView;
				return lineView.GetActualChartStyle(Marker.Symbol != MarkerStyle.None);
			} 
		}
		public override ISeries CloneTo(IChartView view) {
			LineSeries result = new LineSeries(view);
			result.CopyFrom(this);
			return result;
		}
		public override bool IsCompatible(IChartView view) {
			if (view == null)
				return false;
			return view.ViewType == ChartViewType.Line3D || IsCompatibleCore(view);
		}
		bool IsCompatibleCore(IChartView view) {
			ChartViewType viewType = view.ViewType;
			return
				viewType == ChartViewType.Line ||
				viewType == ChartViewType.Stock;
		}
		public override void Visit(ISeriesVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		protected override bool IsLinesOnly { get { return View.ViewType != ChartViewType.Line3D; } }
		public void SetMarkerSymbol(bool showMarker) {
			Marker.Symbol = showMarker ? MarkerStyle.Auto : MarkerStyle.None;
		}
		protected override bool IsCompatibleLabelPosition(DataLabelPosition position) {
			if (position == DataLabelPosition.Default)
				return true;
			if (View.ViewType == ChartViewType.Line3D)
				return position == DataLabelPosition.Right;
			if (IsCompatibleCore(View))
				return
					position == DataLabelPosition.Left ||
					position == DataLabelPosition.Top ||
					position == DataLabelPosition.Right ||
					position == DataLabelPosition.Bottom ||
					position == DataLabelPosition.Center;
			return false;
		}
	}	
}
