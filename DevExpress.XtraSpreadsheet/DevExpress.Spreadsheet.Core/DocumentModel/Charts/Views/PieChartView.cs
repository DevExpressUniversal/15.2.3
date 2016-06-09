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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PieChartView
	public class PieChartView : ChartViewWithSlice {
		#region Static Members
		internal static bool HasExplodedFirstSeries(SeriesCollection seriesCollection) {
			if (seriesCollection.Count == 0)
				return false;
			PieSeries firstSeries = seriesCollection[0] as PieSeries;
			return firstSeries != null && firstSeries.Explosion > 0;
		}
		#endregion
		public PieChartView(IChart parent)
			: base(parent) {
		}
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Pie; } }
		public override ChartType ChartType { get { return HasExplodedFirstSeries(Series) ? ChartType.PieExploded : ChartType.Pie; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.Empty; } }
		public override bool IsSingleSeriesView { get { return true; } }
		public override IChartView CloneTo(IChart parent) {
			PieChartView result = new PieChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.BestFit; }
		}
		public override ISeries CreateSeriesInstance() {
			return PieSeries.Create(this, Exploded);
		}
		#endregion
	}
	#endregion
	#region Pie3DChartView
	public class Pie3DChartView : ChartViewWithVaryColors {
		public Pie3DChartView(IChart parent)
			: base(parent) {
		}
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Pie3D; } }
		public override ChartType ChartType { get { return PieChartView.HasExplodedFirstSeries(Series) ? ChartType.Pie3DExploded : ChartType.Pie3D; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.Empty; } }
		public override bool Is3DView { get { return true; } }
		public override bool IsSingleSeriesView { get { return true; } }
		public override IChartView CloneTo(IChart parent) {
			Pie3DChartView result = new Pie3DChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.BestFit; }
		}
		public override ISeries CreateSeriesInstance() {
			return PieSeries.Create(this, Exploded);
		}
		#endregion
		protected internal bool Exploded { get; set; }
	}
	#endregion
}
