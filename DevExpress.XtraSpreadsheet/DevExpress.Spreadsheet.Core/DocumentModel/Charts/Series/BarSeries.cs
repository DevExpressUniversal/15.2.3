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
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Shapes
	public enum BarShape {
		Auto,
		Box,
		Cone,
		ConeToMax,
		Cylinder,
		Pyramid,
		PyramidToMax
	}
	#endregion
	public class BarSeries : SeriesWithPictureOptions, ISupportsInvertIfNegative {
		#region Fields
		bool invertIfNegative = false;
		BarShape shape = BarShape.Auto;
		#endregion
		public BarSeries(IChartView view)
			: base(view) {
		}
		#region Properties
		#region InvertIfNegative
		public bool InvertIfNegative {
			get { return invertIfNegative; }
			set {
				if(invertIfNegative == value)
					return;
				SetInvertIfNegative(value);
			}
		}
		void SetInvertIfNegative(bool value) {
			InvertIfNegativePropertyChangedHistoryItem historyItem = new InvertIfNegativePropertyChangedHistoryItem(DocumentModel, this, invertIfNegative, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetInvertIfNegativeCore(bool value) {
			this.invertIfNegative = value;
			Parent.Invalidate();
		}
		#endregion
		#region Shape
		public BarShape Shape {
			get { return shape; }
			set {
				if(shape == value)
					return;
				SetShape(value);
			}
		}
		void SetShape(BarShape value) {
			BarSeriesShapePropertyChangedHistoryItem historyItem = new BarSeriesShapePropertyChangedHistoryItem(DocumentModel, this, shape, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetShapeCore(BarShape value) {
			this.shape = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region ISeries members
		public override ChartSeriesType SeriesType { get { return ChartSeriesType.Bar; } }
		public override ChartType ChartType { 
			get {
				bool is3D = View.Is3DView;
				if (!is3D || Shape == BarShape.Auto)
					return View.ChartType;
				Bar3DChartView bar3DView = View as Bar3DChartView;
				Debug.Assert(bar3DView != null);
				return bar3DView.GetChartType(Shape);
			} 
		}
		public override ISeries CloneTo(IChartView view) {
			BarSeries result = new BarSeries(view);
			result.CopyFrom(this);
			return result;
		}
		public override bool IsCompatible(IChartView view) {
			if(view == null)
				return false;
			return view.ViewType == ChartViewType.Bar || view.ViewType == ChartViewType.Bar3D;
		}
		public override void Visit(ISeriesVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ISupportsCopyFrom<BarSeries> Members
		#endregion
		public override void CopyFrom(ISeries value) {
			base.CopyFrom(value);
			ISupportsInvertIfNegative serieWithInvertIfNegative = value as ISupportsInvertIfNegative;
			if (serieWithInvertIfNegative != null) {
				CopyFromCore(serieWithInvertIfNegative);
				BarSeries series = value as BarSeries;
				if (series != null)
					CopyFromCore(series);
			}
		}
		void CopyFromCore(BarSeries value) {
			Shape = value.Shape;
		}
		void CopyFromCore(ISupportsInvertIfNegative value) {
			InvertIfNegative = value.InvertIfNegative;
		}
		protected override bool CanRemoveOnResetToStyle(DataPoint dataPoint) {
			return dataPoint.InvertIfNegative == InvertIfNegative;
		}
		protected override bool IsCompatibleLabelPosition(DataLabelPosition position) {
			if (position == DataLabelPosition.Default)
				return true;
			BarChartViewBase barView = View as BarChartViewBase;
			if (barView == null)
				return false;
			bool standardOrClustered = barView.Grouping == BarChartGrouping.Standard || barView.Grouping == BarChartGrouping.Clustered;
			if (barView.ViewType == ChartViewType.Bar3D)
				return standardOrClustered ? position == DataLabelPosition.OutsideEnd : position == DataLabelPosition.Center;
			if (position == DataLabelPosition.Center || position == DataLabelPosition.InsideBase || position == DataLabelPosition.InsideEnd)
				return true;
			return standardOrClustered && position == DataLabelPosition.OutsideEnd;
		}
	}
}
