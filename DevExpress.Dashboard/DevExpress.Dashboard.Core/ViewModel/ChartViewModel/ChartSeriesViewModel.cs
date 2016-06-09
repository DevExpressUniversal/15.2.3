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

using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public enum ChartSeriesViewModelType { 
		Bar,
		StackedBar,
		FullStackedBar,
		Point,
		Line,
		StackedLine,
		FullStackedLine,
		StepLine,
		Spline,
		Area,
		StackedArea,
		FullStackedArea,
		StepArea,
		SplineArea,
		StackedSplineArea,
		FullStackedSplineArea,
		SideBySideRangeBar,
		RangeArea,
		Weighted,
		HighLowClose,
		CandleStick,
		Stock,
		Pie,
		Donut
	}
	public class ChartSeriesTemplateViewModel {
		ChartSeriesViewModelType seriesType;
		string name;
		string[] dataMembers;
		IList<string> measureCaptions;
		bool showPointMarkers;
		bool plotOnSecondaryAxis;
		bool ignoreEmptyPoints;
		PointLabelViewModel pointLabel;
		public bool OnlyPercentValues { get; set; }
		public ChartSeriesViewModelType SeriesType {
			get { return seriesType; }
			set { seriesType = value; }
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string[] DataMembers {
			get { return dataMembers; }
			set { dataMembers = value; }
		}
		public string ColorMeasureID { get; set; }
		public IList<string> MeasureCaptions {
			get { return measureCaptions; }
			set { measureCaptions = value; }
		}
		public bool ShowPointMarkers {
			get { return showPointMarkers; }
			set { showPointMarkers = value; }
		}
		public bool PlotOnSecondaryAxis {
			get { return plotOnSecondaryAxis; }
			set { plotOnSecondaryAxis = value; }
		}
		public bool IgnoreEmptyPoints {
			get { return ignoreEmptyPoints; }
			set { ignoreEmptyPoints = value; }
		}
		public PointLabelViewModel PointLabel {
			get { return pointLabel; }
			set { pointLabel = value; }
		}
	}
}
